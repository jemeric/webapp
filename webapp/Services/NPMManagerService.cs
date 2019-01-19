using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SemVer;
using webapp.Util.Dto;
using Nito.AsyncEx;
using System.Net.Http;

namespace webapp.Services
{
    public class NPMManagerService
    {
        const string npmCdnUrl = "https://cdn.jsdelivr.net/npm/";
        const string registryBaseUrl = "http://registry.npmjs.org/";
        private readonly IHostingEnvironment env;
        private readonly MemoryCache versionCache;
        private readonly AsyncLazy<JObject> packageJson;
        private readonly AsyncLazy<List<NPMExternal>> externals;
        private Dictionary<string, string> externalVersions = new Dictionary<string, string>();

        public NPMManagerService(IHostingEnvironment env)
        {
            this.env = env;
            // see "cached values" http://blog.stephencleary.com/2013/01/async-oop-3-properties.html
            packageJson = new AsyncLazy<JObject>(async () =>
            {
                return await Task.Run<JObject>(() => JObject.Parse(File.ReadAllText($"{env.ContentRootPath}/ClientApp/package.json")));
            });
            packageJson.Start();
            externals = new AsyncLazy<List<NPMExternal>>(async () =>
            {
                return await Task.Run<List<NPMExternal>>(() => {
                    return JsonConvert.DeserializeObject<List<NPMExternal>>(File.ReadAllText($"{env.ContentRootPath}/Assets/Json/NPMExternals.json"));
                });
            });
            externals.Start();

            // this should be built on a lazy refresh cache, for now just cache indefinitely or until it grows too large
            // see https://github.com/aspnet/Extensions/issues/769
            this.versionCache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }

        public async Task InitializePackageVersions()
        {
            // build external version list based on package.json <key -> version>
            JObject npmPackageObject = await packageJson;
            externalVersions = (await externals).Select(external => new { external.Module, Version = GetSemanticVersion(external.Module, npmPackageObject) })
                .ToDictionary(externalToVersion => externalToVersion.Module, externalToVersion => externalToVersion.Version);

            // TODO - update to use RX extensions
            // cache the package versions for each of our externals
            var versions = (await externals).Select(external => GetVersion(external.Module));
            await Task.WhenAll(versions);
        }

        public async Task<List<NPMExternal>> GetExternals()
        {
            return await externals;
        }

        private async Task<string> GetVersion(string package)
        {
            JObject npmPackageObj = await packageJson;
            return await GetVersion(package, GetSemanticVersion(package, npmPackageObj));
        }

        public Task<string> GetVersion(string package, string semanticVersionOrTag)
        {
            return versionCache.GetOrCreateAsync<string>($"{package}@{semanticVersionOrTag}", async (entry) =>
            {
                // TODO - expire cache - pending https://github.com/aspnet/Extensions/issues/769
                // must set size with size limit - https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.2#use-setsize-size-and-sizelimit-to-limit-cache-size
                entry.SetSize(1);
                JObject registryObj = await GetRegistry(package);
                return GetMaxVersion(semanticVersionOrTag, registryObj);
            });
        }

        private async Task<JObject> GetRegistry(string package)
        {
            using(HttpClient client = new HttpClient())
            {
                // TODO update to use caching + streaming
                String registryBody = await client.GetStringAsync($"{registryBaseUrl}{package}");
                return JObject.Parse(registryBody);
            }
        }

        public async Task<string> GetNPMModule(NPMExternal external, string productionAsset, string developmentAsset)
        {
            return await GetNPMModule(external.Module, externalVersions[external.Module], productionAsset, developmentAsset);
        }

        public async Task<string> GetNPMModule(string package, string semanticVersion, string productionAsset, string developmentAsset = null)
        {
            string version = await GetVersion(package, semanticVersion);
            return GetModuleUrl(package, version, productionAsset, developmentAsset);
        }

        private string GetModuleUrl(string package, string version, string productionAsset, string developmentAsset = null)
        {
            // TODO automatically get version from package.json
            string asset = env.IsProduction() ? 
                productionAsset : 
                developmentAsset ?? productionAsset;
            return $"{npmCdnUrl}{package}@{version}/{asset}";
        }
        
        public static string GetSemanticVersion(string package, JObject packageJson)
        {
            // https://github.com/webpack/webpack/issues/520#issuecomment-174011824
            // app built using webpack has no runtime dependencies and thus all frontend dependencies
            // should be listed as devDependencies
            return (from dependencies in packageJson["devDependencies"].Values<JProperty>()
                    where package.Equals(dependencies.Name)
                    select dependencies).Select(d => d.Value.Value<string>()).FirstOrDefault();
        }
        
        // see https://semver.npmjs.com/
        public static string GetMaxVersion(string semanticVersionOrTag, JObject registry)
        {
            string tag = (from tags in registry["dist-tags"].Values<JProperty>()
                          where semanticVersionOrTag.Equals(tags.Name)
                          select tags).Select(t => t.Value.Value<string>()).FirstOrDefault();
            if(tag != null)
            {
                return tag;
            }                     

           var versions = from dists in registry["time"].Values<JProperty>()
                           select dists.Name;

            return Range.MaxSatisfying(semanticVersionOrTag, versions.ToArray());
        }

    }
}

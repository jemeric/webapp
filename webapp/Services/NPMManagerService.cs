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
        const string npmCdn = "https://cdn.jsdelivr.net/npm/";
        const string registry = "http://registry.npmjs.org/";
        private readonly IHostingEnvironment env;
        private readonly MemoryCache versionCache;
        private readonly AsyncLazy<JObject> packageJson;

        public NPMManagerService(IHostingEnvironment env)
        {
            this.env = env;
            // see "cached values" http://blog.stephencleary.com/2013/01/async-oop-3-properties.html
            packageJson = new AsyncLazy<JObject>(async () =>
            {
                return await Task.Run<JObject>(() => JObject.Parse(File.ReadAllText($"{env.ContentRootPath}package.json")));
            });
            packageJson.Start();

            // this should be built on a lazy refresh cache, for now just cache indefinitely or until it grows too large
            // see https://github.com/aspnet/Extensions/issues/769
            this.versionCache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });

            // TODO - we need to wait for this (dependency injection?)
            InitializePackageVersions();
        }

        private void InitializePackageVersions()
        {
            // TODO - update to use RX extensions
            GetExternals().Select(external => new { external.Package, Version = GetVersion(external.Package) });
        }

        private Task<string> GetVersion(string package)
        {
            return versionCache.GetOrCreateAsync<string>(package, async (entry) =>
            {
                JObject npmPackageObj = await packageJson;
                JObject registryObj = await GetRegistry(package);
                return GetMaxVersion(GetSemanticVersion(package, npmPackageObj), registryObj);
            });
        }

        private async Task<JObject> GetRegistry(string package)
        {
            using(HttpClient client = new HttpClient())
            {
                // TODO update to use caching + streams
                String registryBody = await client.GetStringAsync($"{registry}{package}");
                return JObject.Parse(registryBody);
            }
        }

        private List<NPMExternal> GetExternals()
        {
            // TODO - update to use RX extensions
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (StreamReader sr = new StreamReader(assembly.GetManifestResourceStream("app.Assets.Json.NPMExternals.json")))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<List<NPMExternal>>(jr);
            }
        }

        public async Task<string> GetNPMModule(string package, string asset)
        {
            return await GetNPMModule(package, GetSemanticVersion(package, await packageJson), asset);
        }

        public async Task<string> GetNPMModule(string package, string semanticVersion, string asset)
        {

            // TODO - get registry + cache version if not already in version cache
            //lazyCache.GetOrAddAsync("test", () =>
            //{
            //    return Task.FromResult("test");
            //});
            ////lazyCache.GetOrAddAsync()
            ////var cacheEntryOptions = new MemoryCacheEntryOptions()
            ////    .
            ////_cache.Set("", "", MemoryCacheEntryOptions.)
            //_cache.GetOrCreateAsync<string>("test", e => {
            //    return Task.FromResult<string>("test");
            //});

            return ""; // IMemoryCache - PostEvictionDelegate ?
        }

        public string GetModuleUrl(string package, string productionAsset, string developmentAsset = null)
        {
            // TODO automatically get version from package.json
            string asset = env.IsProduction() ? 
                productionAsset : 
                developmentAsset ?? productionAsset;
            return String.Concat(npmCdn, package, "/", productionAsset);
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

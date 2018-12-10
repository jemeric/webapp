using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using SemVer;

namespace webapp.Services
{
    public class NPMManagerService
    {
        const string npmCdn = "https://cdn.jsdelivr.net/npm/";
        private readonly IHostingEnvironment env;

        public NPMManagerService(IHostingEnvironment env)
        {
            InitializePackageVersions();
            this.env = env;
        }

        private void InitializePackageVersions()
        {

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SemVer;

namespace webapp.Services
{
    public class NPMManagerService
    {
        const string npmCdn = "https://cdn.jsdelivr.net/npm/";

        public NPMManagerService()
        {
            InitializePackageVersions();
        }

        private void InitializePackageVersions()
        {

        }

        public string GetModuleUrl(string package, string productionAsset, string developmentAsset = null)
        {
            // TODO automatically get version from package.json
            bool isProduction = false; // TODO determine development or production based on environment
            string asset = isProduction ? 
                productionAsset : 
                developmentAsset ?? productionAsset;
            return String.Concat(npmCdn, package, "/", productionAsset);
        }

        // see https://semver.npmjs.com/
        public static string GetNPMVersion(string rangeOrTag, JObject registry)
        {
            string tag = (from tags in registry["dist-tags"].Values<JProperty>()
                          where rangeOrTag.Equals(tags.Name)
                          select tags).Select(t => t.Value.Value<string>()).FirstOrDefault();
            if(tag != null)
            {
                return tag;
            }                     

           var versions = from dists in registry["time"].Values<JProperty>()
                           select dists.Name;

            return Range.MaxSatisfying(rangeOrTag, versions.ToArray());
        }

    }
}

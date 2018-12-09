﻿using System;
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

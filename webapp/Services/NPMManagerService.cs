using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public static string GetVersion()
        {
            return null;
        }

    }
}

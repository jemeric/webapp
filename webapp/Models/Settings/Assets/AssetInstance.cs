using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.Settings.Assets
{
    public class AssetInstance
    {
        private readonly string address;
        private readonly AssetVersion[] assetVersions;

        public AssetInstance(string address, AssetVersion[] assetVersions)
        {
            this.address = address;
            this.assetVersions = assetVersions;
        }

        public string GetAddress()
        {
            return this.address;
        }

        public AssetVersion[] GetInstalledVersions()
        {
            return this.assetVersions;
        }

        public AssetVersion GetDownloadingVersion()
        {
            return null;
        }
    }
}

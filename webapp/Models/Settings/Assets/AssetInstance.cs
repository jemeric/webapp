using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.Settings.Assets
{
    public class AssetInstance
    {
        public string Address { get; }
        public AssetVersion[] InstalledVersions { get; }
        public AssetVersion DownloadingVersion { get; }

        public AssetInstance(string address, AssetVersion[] installedVersions, AssetVersion downloadingVersion)
        {
            this.Address = address;
            this.InstalledVersions = installedVersions;
            this.DownloadingVersion = downloadingVersion;
        }
    }
}

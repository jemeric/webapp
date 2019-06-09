using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.Settings.Assets;

namespace webapp.Services.GraphQL
{
    public class AssetInstanceResolver
    {

        public string GetAddress(AssetInstance instance)
        {
            return instance.Address;
        }

        public AssetVersion[] GetInstalledVersions(AssetInstance instance)
        {
            return instance.InstalledVersions;
        }

        public AssetVersion GetDownloadingVersion(AssetInstance instance)
        {
            return instance.DownloadingVersion;
        }

    }
}

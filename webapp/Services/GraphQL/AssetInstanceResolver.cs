using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.GraphQL;

namespace webapp.Services.GraphQL
{
    public class AssetInstanceResolver
    {

        public string GetAddress(AssetInstance instance)
        {
            return instance.GetAddress();
        }

        public string[] GetAvailableVersions(AssetInstance instance)
        {
            return instance.GetAvailableVersions();
        }

        public string GetDownloadingVersion(AssetInstance instance)
        {
            return instance.GetDownloadingVersion();
        }

        public string GetLatestVersion(AssetInstance instance)
        {
            return instance.GetLatestVersion();
        }

        public string GetPublishedVersion(AssetInstance instance)
        {
            return instance.GetPublishedVersion();
        }
    }
}

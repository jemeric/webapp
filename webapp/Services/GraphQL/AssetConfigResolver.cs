using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.GraphQL;

namespace webapp.Services.GraphQL
{
    public class AssetConfigResolver
    {
        public String GetLatestVersion(AssetConfig assetConfig)
        {
            return assetConfig.GetLatestVersion();
        }

        public String GetPublishedVersion(AssetConfig assetConfig)
        {
            return assetConfig.GetPublishedVersion();
        }

        public AssetInstance[] GetInstances(AssetConfig assetConfig)
        {
            return assetConfig.GetInstances();
        }
    }
}

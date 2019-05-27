using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.GraphQL
{
    public class AssetConfig
    {
        public string GetLatestVersion()
        {
            return "nothing";
        }

        public string GetPublishedVersion()
        {
            return "nothing";
        }

        public AssetInstance[] GetInstances()
        {
            return null;
        }
    }
}

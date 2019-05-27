using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.GraphQL
{
    public class AssetInstance
    {
        public string GetAddress()
        {
            return "nothing";
        }

        public string[] GetAvailableVersions()
        {
            return new string[] { "nothing" };
        }

        public string GetDownloadingVersion()
        {
            return null;
        }

        public string GetLatestVersion()
        {
            return null;
        }

        public string GetPublishedVersion()
        {
            return null;
        }
    }
}

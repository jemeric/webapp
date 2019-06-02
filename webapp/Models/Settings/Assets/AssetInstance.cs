using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.Settings.Assets
{
    public class AssetInstance
    {
        public string GetAddress()
        {
            return "nothing";
        }

        public AssetVersion[] GetAvailableVersions()
        {
            return null; // new string[] { "nothing" };
        }

        public AssetVersion GetDownloadingVersion()
        {
            return null;
        }
    }
}

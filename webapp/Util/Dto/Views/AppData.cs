using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Util.Dto.Views
{
    public class AppData
    {
        public string[] ExternalScripts { get; }
        public string AssetsVersion { get; }

        public AppData(string[] externalScripts, string assetsVersion)
        {
            ExternalScripts = externalScripts;
            AssetsVersion = assetsVersion;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.Settings.Authorization;

namespace webapp.Util.Dto.Views
{
    public class AppData
    {
        public string[] ExternalScripts { get; }
        public string AssetsVersion { get; }
        public AuthorizationContext AuthorizationContext { get; }

        public AppData(string[] externalScripts, string assetsVersion, AuthorizationContext authorizationContext)
        {
            ExternalScripts = externalScripts;
            AssetsVersion = assetsVersion;
            AuthorizationContext = authorizationContext;
        }

    }
}

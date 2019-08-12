using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Util.Dto.Views
{
    public class LoginData
    {
        public string AssetsVersion { get; }

        public LoginData(string assetsVersion)
        {
            AssetsVersion = assetsVersion;
        }
    }
}

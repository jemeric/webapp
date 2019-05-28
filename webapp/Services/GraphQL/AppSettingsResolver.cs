using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.GraphQL;

namespace webapp.Services.GraphQL
{
    public class AppSettingsResolver
    {

        public AssetConfig GetAssetConfig(AppSettings settings)
        {
            return new AssetConfig(); // nothing to do here yet
        }

    }
}

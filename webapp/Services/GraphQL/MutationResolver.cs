using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.GraphQL;

namespace webapp.Services.GraphQL
{
    public class MutationResolver
    {
        [GraphQLName("updateAssetsVersion")]
        public async Task<AssetConfig> UpdateAssetsVersion(string assetsVersion)
        {
            return await Task.Run(() => new AssetConfig());
        }

        [GraphQLName("publishVersion")]
        public AssetConfig GetPublishVersion(string assetsVersion)
        {
            return new AssetConfig();
        }

        [GraphQLName("removeVersion")]
        public AssetConfig GetRemoveVersion(string assetsVersion)
        {
            return new AssetConfig();
        }
    }
}

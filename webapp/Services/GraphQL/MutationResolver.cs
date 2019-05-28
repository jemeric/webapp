using HotChocolate;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.GraphQL;
using webapp.Util;

namespace webapp.Services.GraphQL
{
    public class MutationResolver
    {
        private readonly IDistributedCache cache;

        public MutationResolver(IDistributedCache cache)
        {
            this.cache = cache;
        }

        [GraphQLName("updateAssetsVersion")]
        public async Task<AssetConfig> UpdateAssetsVersion(string assetsVersion)
        {
            await cache.SetStringAsync(AppConstants.CacheKeys.lastUpdatedVersion, assetsVersion);
            // TODO - request service to download assets
            //return await cache.GetAsync(assetsVersion);
            return await Task.Run(() => new AssetConfig());
        }

        [GraphQLName("publishVersion")]
        public AssetConfig PublishVersion(string assetsVersion)
        {
            return new AssetConfig();
        }
    }
}

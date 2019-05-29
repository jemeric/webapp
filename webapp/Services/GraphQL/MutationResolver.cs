using HotChocolate;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.GraphQL;
using webapp.Services.Assets;
using webapp.Util;

namespace webapp.Services.GraphQL
{
    public class MutationResolver
    {
        private readonly IAssetsService assetsService;

        public MutationResolver(IAssetsService assetsService)
        {
            this.assetsService = assetsService;
        }

        [GraphQLName("updateAssetsVersion")]
        public async Task<AssetConfig> UpdateAssetsVersion(string assetsVersion)
        {
            await this.assetsService.UpdateVersion(assetsVersion);
            //await cache.SetStringAsync(AppConstants.CacheKeys.lastUpdatedVersion, assetsVersion);
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

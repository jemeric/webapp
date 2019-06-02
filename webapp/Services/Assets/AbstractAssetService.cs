using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.Settings.Assets;
using webapp.Util;
using webapp.Util.Extensions;

namespace webapp.Services.Assets
{
    public abstract class AbstractAssetService : IAssetsService
    {
        private readonly IDistributedCache distributedCache;

        public AbstractAssetService(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public async Task<AssetVersion> GetLastUpdatedVersion()
        {
            return await distributedCache.GetAsync<AssetVersion>(AppConstants.CacheKeys.lastUpdatedVersion);
        }
        public async Task<AssetVersion> GetPreviousPublishedVersion()
        {
            return await distributedCache.GetAsync<AssetVersion>(AppConstants.CacheKeys.previouslyPublishedVersion);
        }
        public async Task<AssetVersion> GetPublishedVersion()
        {
            return await distributedCache.GetAsync<AssetVersion>(AppConstants.CacheKeys.publishedVersion);
        }
        public abstract Task<AssetInstance[]> GetInstances();
        public abstract Task PublishVersion(string assetsVersion);
        public abstract Task UpdateVersion(string assetsVersion);
    }
}

using Microsoft.Extensions.Caching.Distributed;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.Settings.Assets;
using webapp.Util;
using webapp.Util.Dto.Configuration;
using webapp.Util.Extensions;

namespace webapp.Services.Assets
{
    public abstract class AbstractAssetService : IAssetsService
    {
        private readonly IDistributedCache distributedCache;
        private readonly AssetsConfiguration assetsConfiguration;
        private readonly AsyncLazy<AssetVersion> lastUpdatedVersion;
        private readonly AsyncLazy<AssetVersion> previouslyPublishedVersion;
        private readonly AsyncLazy<AssetVersion> publishedVersion;
        private readonly AsyncLazy<bool?> isCDNEnabled;

        public AbstractAssetService(IDistributedCache distributedCache, AssetsConfiguration assetsConfiguration)
        {
            this.distributedCache = distributedCache;
            this.assetsConfiguration = assetsConfiguration;
            this.lastUpdatedVersion = new AsyncLazy<AssetVersion>(() => GetVersion(AppConstants.CacheKeys.lastUpdatedVersion));
            this.previouslyPublishedVersion = new AsyncLazy<AssetVersion>(() => GetVersion(AppConstants.CacheKeys.previouslyPublishedVersion));
            this.publishedVersion = new AsyncLazy<AssetVersion>(() => GetVersion(AppConstants.CacheKeys.publishedVersion));
            this.isCDNEnabled = new AsyncLazy<bool?>(() => distributedCache.GetAsync<bool?>(AppConstants.CacheKeys.isCDNEnabled));
        }

        private async Task<AssetVersion> GetVersion(string cacheKey)
        {
            return await distributedCache.GetAsync<AssetVersion>(cacheKey);
        }

        public async Task<AssetVersion> GetLastUpdatedVersion()
        {
            return await lastUpdatedVersion;
        }

        public async Task<AssetVersion> GetPreviousPublishedVersion()
        {
            return await previouslyPublishedVersion;
        }

        public async Task<AssetVersion> GetPublishedVersion()
        {
            return await publishedVersion;
        }

        public string GetCDNHost()
        {
            return assetsConfiguration.CDNHost;
        }

        public async Task<bool> IsCDNEnabled()
        {
            var cdnEnabled = await isCDNEnabled;
            return cdnEnabled.GetValueOrDefault(assetsConfiguration.IsCDNEnabled);
        }

        public abstract Task<AssetInstance[]> GetInstances();
        public abstract Task PublishVersion(string assetsVersion);
        public abstract Task UpdateVersion(string assetsVersion);
        public abstract Task ToggleAssetCDN(bool enable);
    }
}

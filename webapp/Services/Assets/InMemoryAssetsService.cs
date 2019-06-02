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
    public class InMemoryAssetsService : AbstractAssetService
    {
        private readonly IDistributedCache distributedCache;
        private readonly SettingsService settingsService;

        public InMemoryAssetsService(IDistributedCache distributedCache, SettingsService settingsService) : base(distributedCache)
        {
            this.distributedCache = distributedCache;
            this.settingsService = settingsService;
        }

        public override Task<AssetInstance[]> GetInstances()
        {
            throw new NotImplementedException();
        }

        public override Task PublishVersion(string assetsVersion)
        {
            throw new NotImplementedException();
        }

        public override async Task UpdateVersion(string assetsVersion)
        {
            // TODO - download remote folder stream from DO/S3/GCP?
            // write into ClientApp/dist/versions
            // change last updated version
            AssetVersion updated = new AssetVersion(assetsVersion, settingsService.GetNow());
            // TODO - update using Mongo or something else for distributed version of this (with distributed cache just as a wrapper)
            await this.distributedCache.SetAsync<AssetVersion>(AppConstants.CacheKeys.lastUpdatedVersion, updated, new DistributedCacheEntryOptions());
            //this.distributedCache.Try
        }
    }
}

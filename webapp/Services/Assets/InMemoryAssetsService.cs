using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.Settings.Assets;
using webapp.Services.Storage;
using webapp.Util;
using webapp.Util.Dto.Configuration;
using webapp.Util.Extensions;

namespace webapp.Services.Assets
{
    public class InMemoryAssetsService : AbstractAssetService
    {
        private readonly IHostingEnvironment env;
        private readonly IDistributedCache distributedCache;
        private readonly SettingsService settingsService;
        private readonly IStorageService storageService;

        public InMemoryAssetsService(IHostingEnvironment env, IDistributedCache distributedCache, SettingsService settingsService, IStorageService storageService, 
            AssetsConfiguration assetsConfiguration) : base(distributedCache, assetsConfiguration)
        {
            this.env = env;
            this.distributedCache = distributedCache;
            this.settingsService = settingsService;
            this.storageService = storageService;
        }

        public override Task<AssetInstance[]> GetInstances()
        {
            throw new NotImplementedException();
        }

        public override Task PublishVersion(string assetsVersion)
        {
            throw new NotImplementedException();
        }

        public override async Task ToggleAssetCDN(bool enable)
        {
            await this.distributedCache.SetAsync<bool>(AppConstants.CacheKeys.isCDNEnabled, enable, new DistributedCacheEntryOptions());
        }

        public override async Task UpdateVersion(string assetsVersion)
        {
            // download remote folder stream from DO/S3/GCP?
            await storageService.Copy(assetsVersion, $"{env.ContentRootPath}/{AppConstants.webRoot}/{assetsVersion}");
            
            // write into ClientApp/dist/versions
            // change last updated version
            Models.Settings.AppClock appClock = await settingsService.GetClock();
            AssetVersion updated = new AssetVersion(assetsVersion, appClock.CurrentTime);
            // cleanup orphaned versions if they exist
            // TODO - update using Mongo or something else for distributed version of this (with distributed cache just as a wrapper)
            await this.distributedCache.SetAsync<AssetVersion>(AppConstants.CacheKeys.lastUpdatedVersion, updated, new DistributedCacheEntryOptions());
        }
    }
}

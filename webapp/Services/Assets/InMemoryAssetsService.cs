using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.IO;
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

        private string GetClientPath(string assetsVersion)
        {
            return $"{env.ContentRootPath}/{AppConstants.webRoot}/{assetsVersion}";
        }

        private string GetServerPath(string assetsVersion)
        {
            return $"{env.ContentRootPath}/{AppConstants.assetRoot}/server/{assetsVersion}";
        }

        private async Task<AssetVersion[]> GetLatestVersions()
        {
            AssetVersion[] versions = await Task.WhenAll(GetVersion(AppConstants.CacheKeys.lastUpdatedVersion),
                GetVersion(AppConstants.CacheKeys.previouslyPublishedVersion),
                GetVersion(AppConstants.CacheKeys.publishedVersion));
            return versions.Where(v => v != null).ToArray();
        }

        public override async Task UpdateVersion(string assetsVersion)
        {
            // TODO check if version exists in remote storage and error otherwise

            // check if folder versions already exist in local file system and ignore otherwise
            string clientAssetsVersionRoot = GetClientPath(assetsVersion);
            string serverAssetsVersionRoot = GetServerPath(assetsVersion);

            if(Directory.Exists(clientAssetsVersionRoot) && Directory.Exists(serverAssetsVersionRoot))
            {
                return;
            }

            // copy versioned folder from remote storage to local storage
            await storageService.Copy($"{assetsVersion}/client", clientAssetsVersionRoot);
            await storageService.Copy($"{assetsVersion}/server", serverAssetsVersionRoot);

            // change last updated version
            Models.Settings.AppClock appClock = await settingsService.GetClock();
            AssetVersion updated = new AssetVersion(assetsVersion, appClock.CurrentTime);
            // TODO - update using Mongo or something else for distributed version of this (with distributed cache just as a wrapper)
            await this.distributedCache.SetAsync<AssetVersion>(AppConstants.CacheKeys.lastUpdatedVersion, updated, new DistributedCacheEntryOptions());

            // cleanup orphaned versions if they exist
            AssetVersion[] versions = await GetLatestVersions();
            //Directory.GetDirectories()

        }
    }
}

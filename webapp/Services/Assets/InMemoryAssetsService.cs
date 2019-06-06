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
        private readonly IDistributedCache distributedCache;
        private readonly SettingsService settingsService;
        private readonly IStorageService storageService;

        public InMemoryAssetsService(IHostingEnvironment env, IDistributedCache distributedCache, SettingsService settingsService, IStorageService storageService, 
            AssetsConfiguration assetsConfiguration) : base(env, distributedCache, assetsConfiguration)
        {
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
            // check if folder versions already exist in local file system and ignore otherwise
            string clientAssetsVersionRoot = GetClientPath(assetsVersion);
            string serverAssetsVersionRoot = GetServerPath(assetsVersion);

            if(!Directory.Exists(clientAssetsVersionRoot) || !Directory.Exists(serverAssetsVersionRoot))
            {
                // check if version exists in remote storage and error otherwise
                if(!(await storageService.Exists(assetsVersion)))
                {
                    throw new Exception("Could not find asset version in remote storage");
                }

                // copy versioned folder from remote storage to local storage
                await storageService.Copy($"{assetsVersion}/client", clientAssetsVersionRoot);
                await storageService.Copy($"{assetsVersion}/server", serverAssetsVersionRoot);
            }

            // change last updated version
            Models.Settings.AppClock appClock = await settingsService.GetClock();
            AssetVersion updated = new AssetVersion(assetsVersion, appClock.CurrentTime);
            // TODO - update using Mongo or something else for distributed version of this (with distributed cache just as a wrapper)
            await SetLastUpdatedVersion(updated);

            // cleanup orphaned versions if they exist
            await CleanOrphanedVersions();
        }
    }
}

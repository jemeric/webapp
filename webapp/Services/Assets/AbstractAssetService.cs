using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Nito.AsyncEx;
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
    public abstract class AbstractAssetService : IAssetsService
    {
        private readonly IHostingEnvironment env;
        private readonly IDistributedCache distributedCache;
        private readonly AssetsConfiguration assetsConfiguration;
        private readonly SettingsService settingsService;
        private readonly IStorageService storageService;
        private AsyncLazy<AssetVersion> lastUpdatedVersion;
        private AsyncLazy<AssetVersion> previouslyPublishedVersion;
        private AsyncLazy<AssetVersion> publishedVersion;
        private readonly AsyncLazy<bool?> isCDNEnabled;

        public AbstractAssetService(IHostingEnvironment env, IDistributedCache distributedCache, AssetsConfiguration assetsConfiguration, SettingsService settingsService, 
            IStorageService storageService)
        {
            this.env = env;
            this.distributedCache = distributedCache;
            this.assetsConfiguration = assetsConfiguration;
            this.settingsService = settingsService;
            this.storageService = storageService;
            this.lastUpdatedVersion = GetLazyVersion(AppConstants.CacheKeys.lastUpdatedVersion);
            this.previouslyPublishedVersion = GetLazyVersion(AppConstants.CacheKeys.previouslyPublishedVersion);
            this.publishedVersion = GetLazyVersion(AppConstants.CacheKeys.publishedVersion);
            this.isCDNEnabled = new AsyncLazy<bool?>(() => distributedCache.GetAsync<bool?>(AppConstants.CacheKeys.isCDNEnabled));
        }

        private AsyncLazy<AssetVersion> GetLazyVersion(string cacheKey)
        {
            return new AsyncLazy<AssetVersion>(() => GetVersion(cacheKey));
        }

        private async Task<AssetVersion> GetVersion(string cacheKey)
        {
            return await distributedCache.GetAsync<AssetVersion>(cacheKey);
        }

        private async Task SetVersion(string cacheKey, AssetVersion version)
        {
           await this.distributedCache.SetAsync<AssetVersion>(cacheKey, version, new DistributedCacheEntryOptions());
        }

        protected async Task SetLastUpdatedVersion(AssetVersion version)
        {
            await SetVersion(AppConstants.CacheKeys.lastUpdatedVersion, version);
            this.lastUpdatedVersion = GetLazyVersion(AppConstants.CacheKeys.lastUpdatedVersion);
        }

        public async Task<AssetVersion> GetLastUpdatedVersion()
        {
            return await lastUpdatedVersion;
        }

        protected async Task SetPreviousPublishedVersion(AssetVersion version)
        {
            await SetVersion(AppConstants.CacheKeys.previouslyPublishedVersion, version);
            this.previouslyPublishedVersion = GetLazyVersion(AppConstants.CacheKeys.previouslyPublishedVersion);
        }

        public async Task<AssetVersion> GetPreviousPublishedVersion()
        {
            return await previouslyPublishedVersion;
        }

        protected async Task SetPublishedVersion(AssetVersion version)
        {
            // don't allow the publishing of a version that hasn't been installed on all instances
            AssetInstance[] instances = await GetInstances();
            bool isVersionInstalled = instances.All(i => {
                return i.InstalledVersions.Contains(version);
            });
            if(!isVersionInstalled)
            {
                throw new Exception("Cannot publish version that hasn't been installed to all instances");
            }

            await SetVersion(AppConstants.CacheKeys.publishedVersion, version);
            this.publishedVersion = GetLazyVersion(AppConstants.CacheKeys.publishedVersion);
        }

        public async Task<AssetVersion> GetPublishedVersion()
        {
            return await publishedVersion;
        }

        protected async Task<AssetVersion[]> GetUsableVersions()
        {
            AssetVersion[] versions = await Task.WhenAll(GetLastUpdatedVersion(),
                GetPreviousPublishedVersion(),
                GetPublishedVersion());
            return versions.Where(v => v != null).ToArray();
        }
        
        protected async Task<AssetVersion> GetDownloadingVersion()
        {
            AssetVersion[] installedVersions = await GetUsableVersions();
            AssetVersion lastUpdatedVersion = await GetLastUpdatedVersion();
            if(!installedVersions.Contains(lastUpdatedVersion))
            {
                return lastUpdatedVersion;
            }
            return null; // not currently downloading a version since it already exists
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

        protected string GetClientPath(string assetsVersion = null)
        {
            string root = $"{env.ContentRootPath}/{AppConstants.webRoot}";
            if (assetsVersion == null)
            {
                return root;
            }
            return $"{root}/{assetsVersion}";
        }

        protected string GetServerPath(string assetsVersion = null)
        {
            string root = $"{env.ContentRootPath}/{AppConstants.assetRoot}/server";
            if (assetsVersion == null)
            {
                return root;
            }
            return $"{root}/{assetsVersion}";
        }

        protected void DeleteInstalledVersion(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        protected async Task CleanOrphanedVersions()
        {
            string[] installedVersions = (await GetUsableVersions()).Select(v => v.Version).ToArray();

            string[] installedClientVersions = Directory.GetDirectories(GetClientPath());
            string[] installedServerVersions = Directory.GetDirectories(GetServerPath());
            string[] removeVersions = installedClientVersions
                .Concat(installedServerVersions)
                .Select(d => new DirectoryInfo(d).Name)
                .Where(d => !installedVersions.Contains(d) && d != AppConstants.Assets.defaultAssetVersion)
                .Distinct()
                .ToArray();

            foreach (var version in removeVersions)
            {
                DeleteInstalledVersion(GetClientPath(version));
                DeleteInstalledVersion(GetServerPath(version));
            }
        }

        public async Task PublishVersion(string assetsVersion)
        {
            AssetVersion previouslyPublishedVersion = await GetPublishedVersion();
            Models.Settings.AppClock appClock = await settingsService.GetClock();
            await SetPublishedVersion(new AssetVersion(assetsVersion, appClock.CurrentTime));
            if(previouslyPublishedVersion != null)
            {
                await SetPreviousPublishedVersion(previouslyPublishedVersion);
            }
        }

        public async Task ToggleAssetCDN(bool enable)
        {
            await this.distributedCache.SetAsync<bool>(AppConstants.CacheKeys.isCDNEnabled, enable, new DistributedCacheEntryOptions());
        }

        // install version locally
        protected async Task InstallVersion(string assetsVersion)
        {
            // check if folder versions already exist in local file system and ignore otherwise
            string clientAssetsVersionRoot = GetClientPath(assetsVersion);
            string serverAssetsVersionRoot = GetServerPath(assetsVersion);

            if (!Directory.Exists(clientAssetsVersionRoot) || !Directory.Exists(serverAssetsVersionRoot))
            {
                // check if version exists in remote storage and error otherwise
                if (!(await storageService.Exists(assetsVersion)))
                {
                    throw new Exception("Could not find asset version in remote storage");
                }

                // TODO - make externals scripts file installable/invalidate cache and read from it (allows full frontend deployment)
                // copy versioned folder from remote storage to local storage
                await storageService.Copy($"{assetsVersion}/client", clientAssetsVersionRoot);
                await storageService.Copy($"{assetsVersion}/server", serverAssetsVersionRoot);
            }

            // change last updated version
            Models.Settings.AppClock appClock = await settingsService.GetClock();
            AssetVersion updated = new AssetVersion(assetsVersion, appClock.CurrentTime);
            await SetLastUpdatedVersion(updated);

            // cleanup orphaned versions if they exist
            await CleanOrphanedVersions();
        }

        public abstract Task<AssetInstance[]> GetInstances();
        public abstract Task UpdateVersion(string assetsVersion);
    }
}

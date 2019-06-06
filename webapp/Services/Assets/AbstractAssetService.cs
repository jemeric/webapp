using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IHostingEnvironment env;
        private readonly IDistributedCache distributedCache;
        private readonly AssetsConfiguration assetsConfiguration;
        private AsyncLazy<AssetVersion> lastUpdatedVersion;
        private readonly AsyncLazy<AssetVersion> previouslyPublishedVersion;
        private readonly AsyncLazy<AssetVersion> publishedVersion;
        private readonly AsyncLazy<bool?> isCDNEnabled;

        public AbstractAssetService(IHostingEnvironment env, IDistributedCache distributedCache, AssetsConfiguration assetsConfiguration)
        {
            this.env = env;
            this.distributedCache = distributedCache;
            this.assetsConfiguration = assetsConfiguration;
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
            await this.distributedCache.SetAsync<AssetVersion>(AppConstants.CacheKeys.lastUpdatedVersion, version, new DistributedCacheEntryOptions());
            this.lastUpdatedVersion = GetLazyVersion(AppConstants.CacheKeys.lastUpdatedVersion);
        }

        protected async Task SetLastUpdatedVersion(AssetVersion version)
        {
            await SetVersion(AppConstants.CacheKeys.lastUpdatedVersion, version);
        }

        public async Task<AssetVersion> GetLastUpdatedVersion()
        {
            return await lastUpdatedVersion;
        }

        protected async Task SetPreviousPublishedVersion(AssetVersion version)
        {
            await SetVersion(AppConstants.CacheKeys.previouslyPublishedVersion, version);
        }

        public async Task<AssetVersion> GetPreviousPublishedVersion()
        {
            return await previouslyPublishedVersion;
        }

        protected async Task SetPublishedVersion(AssetVersion version)
        {
            await SetVersion(AppConstants.CacheKeys.publishedVersion, version);
        }

        public async Task<AssetVersion> GetPublishedVersion()
        {
            return await publishedVersion;
        }

        protected async Task<AssetVersion[]> GetTrackedVersions()
        {
            AssetVersion[] versions = await Task.WhenAll(GetLastUpdatedVersion(),
                GetPreviousPublishedVersion(),
                GetPublishedVersion());
            return versions.Where(v => v != null).ToArray();
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
            string[] supportedVersions = (await GetTrackedVersions()).Select(v => v.Version).ToArray();

            string[] installedClientVersions = Directory.GetDirectories(GetClientPath());
            string[] installedServerVersions = Directory.GetDirectories(GetServerPath());
            string[] removeVersions = installedClientVersions
                .Concat(installedServerVersions)
                .Select(d => new DirectoryInfo(d).Name)
                .Where(d => !supportedVersions.Contains(d) && d != AppConstants.Assets.defaultAssetVersion)
                .Distinct()
                .ToArray();

            foreach (var version in removeVersions)
            {
                DeleteInstalledVersion(GetClientPath(version));
                DeleteInstalledVersion(GetServerPath(version));
            }
        }

        public abstract Task<AssetInstance[]> GetInstances();
        public abstract Task PublishVersion(string assetsVersion);
        public abstract Task UpdateVersion(string assetsVersion);
        public abstract Task ToggleAssetCDN(bool enable);
    }
}

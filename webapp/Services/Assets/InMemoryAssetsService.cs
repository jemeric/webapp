using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IHttpContextAccessor httpContextAccessor;

        public InMemoryAssetsService(IHostingEnvironment env, IDistributedCache distributedCache, SettingsService settingsService, IStorageService storageService, 
            AppConfig appConfiguration, IHttpContextAccessor httpContextAccessor) : base(env, distributedCache, appConfiguration, settingsService, storageService)
        {
            this.distributedCache = distributedCache;
            this.settingsService = settingsService;
            this.storageService = storageService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public override async Task<AssetInstance[]> GetInstances()
        {
            // in a non-distributed system we only have one instance (the current one)
            AssetVersion[] versions = await GetUsableVersions();
            string host = this.httpContextAccessor.HttpContext.Request.Host.Host;
            AssetVersion downloadingVersion = await GetDownloadingVersion();
            AssetInstance instance = new AssetInstance(host, versions, downloadingVersion);
            return new AssetInstance[] { instance };
        }

        public override async Task UpdateVersion(string assetsVersion)
        {
            // TODO - update using Mongo or something else for distributed version of this (with distributed cache just as a wrapper)

            // in this case there's only one instance so just need to install the new version to the local machine
            await InstallVersion(assetsVersion);
        }
    }
}

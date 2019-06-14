using HotChocolate;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.Settings;
using webapp.Models.Settings.Assets;
using webapp.Services.Assets;
using webapp.Util;

namespace webapp.Services.GraphQL
{
    public class MutationResolver
    {
        private readonly IAssetsService assetsService;
        private readonly SettingsService settingsService;

        public MutationResolver(IAssetsService assetsService, SettingsService settingsService)
        {
            this.assetsService = assetsService;
            this.settingsService = settingsService;
        }

        [GraphQLName("changeClockOffset")]
        public async Task<AppClock> ChangeClockOffset(TimeSpan offset)
        {
            return await settingsService.SetClockOffset((long)offset.TotalMilliseconds);
        }

        [GraphQLName("updateAssetsVersion")]
        public async Task<AssetConfig> UpdateAssetsVersion(string assetsVersion)
        {
            await this.assetsService.UpdateVersion(assetsVersion);
            //await cache.SetStringAsync(AppConstants.CacheKeys.lastUpdatedVersion, assetsVersion);
            // TODO - request service to download assets
            //return await cache.GetAsync(assetsVersion);
            return new AssetConfig();
        }

        [GraphQLName("publishVersion")]
        public async Task<AssetConfig> PublishVersion(string assetsVersion)
        {
            await this.assetsService.PublishVersion(assetsVersion);
            return new AssetConfig();
        }

        [GraphQLName("toggleAssetCDN")]
        public async Task<AssetConfig> ToggleAssetCDN(bool enable)
        {
            await this.assetsService.ToggleAssetCDN(enable);
            return new AssetConfig();
        }
    }
}

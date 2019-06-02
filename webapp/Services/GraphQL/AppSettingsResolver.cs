using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.Settings;
using webapp.Models.Settings.Assets;

namespace webapp.Services.GraphQL
{
    public class AppSettingsResolver
    {
        private readonly SettingsService settingsService;

        public AppSettingsResolver(SettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public async Task<AppClock> GetClock()
        {
            return await settingsService.GetClock();
        }

        public AppClock GetCurrentTime(AppSettings settings)
        {
            return new AppClock(0); // add lazy cache to request scoped (DI?)
        }

        public AssetConfig GetAssetConfig(AppSettings settings)
        {
            return new AssetConfig(); // nothing to do here yet
        }

    }
}

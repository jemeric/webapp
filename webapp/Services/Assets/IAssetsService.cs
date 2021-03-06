﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.Settings.Assets;

namespace webapp.Services.Assets
{
    public interface IAssetsService
    {
        Task<AssetVersion> GetLastUpdatedVersion();
        Task<AssetVersion> GetPublishedVersion();
        Task<AssetVersion> GetPreviousPublishedVersion();
        Task<string> GetCurrentVersion();
        Task<AssetInstance[]> GetInstances();
        Task UpdateVersion(string assetsVersion);
        Task PublishVersion(string assetsVersion);
        string GetCDNHost();
        Task<bool> IsCDNEnabled();
        Task ToggleAssetCDN(bool enable);
    }
}

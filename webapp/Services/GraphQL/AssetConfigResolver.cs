using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.GraphQL;
using webapp.Services.Assets;

namespace webapp.Services.GraphQL
{
    public class AssetConfigResolver
    {
        private readonly IAssetsService assetService;

        public AssetConfigResolver(IAssetsService assetService) {
            this.assetService = assetService;
        }

        public async Task<AssetVersion> GetLatestVersion(AssetConfig assetConfig)
        {
            return await this.assetService.GetLastUpdatedVersion();
        }

        public async Task<AssetVersion> GetPublishedVersion(AssetConfig assetConfig)
        {
            return await this.assetService.GetPublishedVersion();
        }

        public async Task<AssetInstance[]> GetInstances(AssetConfig assetConfig)
        {
            return await this.assetService.GetInstances();
        }
    }
}

using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.GraphQL;
using webapp.Util;
using webapp.Util.Extensions;

namespace webapp.Services.Assets
{
    public class InMemoryAssetsService : AbstractAssetService
    {
        private readonly IDistributedCache distributedCahce;

        public InMemoryAssetsService(IDistributedCache distributedCahce) : base(distributedCahce)
        {
            this.distributedCahce = distributedCahce;
        }

        public override Task<AssetInstance[]> GetInstances()
        {
            throw new NotImplementedException();
        }

        public override Task PublishVersion(string assetsVersion)
        {
            throw new NotImplementedException();
        }

        public override async Task UpdateVersion(string assetsVersion)
        {
            // TODO - download remote folder stream from DO/S3/GCP?
            // write into ClientApp/dist/versions
            // change last updated version
            AssetVersion updated = new AssetVersion("test123", new DateTime()); // TODO - create central Clock for DateTime
            await this.distributedCahce.SetAsync<AssetVersion>(AppConstants.CacheKeys.lastUpdatedVersion, updated, new DistributedCacheEntryOptions());
        }
    }
}

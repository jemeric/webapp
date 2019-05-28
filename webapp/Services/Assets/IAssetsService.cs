using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.GraphQL;

namespace webapp.Services.Assets
{
    public interface IAssetsService
    {
        Task<AssetVersion> GetLastUpdatedVersion();
        Task<AssetVersion> GetPublishedVersion();
        Task<AssetVersion> GetPreviousPublishedVersion();
        Task<AssetInstance[]> GetInstances();
        Task UpdateVersion(string assetsVersion);
        Task PublishVersion(string assetsVersion);
    }
}

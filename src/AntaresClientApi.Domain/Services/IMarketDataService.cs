using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Domain.Entities;

namespace AntaresClientApi.Domain.Services
{
    public interface IMarketDataService
    {
        Task<IReadOnlyList<Asset>> GetAssetsByTenant(string tenantId);
        Task<IReadOnlyList<AssetPair>> GetAssetPairsByTenant(string tenantId);

        Task<Asset> GetDefaultBaseAsset(string tenantId);
    }
}

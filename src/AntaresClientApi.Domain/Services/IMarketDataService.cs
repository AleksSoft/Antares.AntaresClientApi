using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Domain.Entities;
using OrderBooks.MyNoSql.OrderBookData;
using OrderBooks.MyNoSql.PriceData;

namespace AntaresClientApi.Domain.Services
{
    public interface IMarketDataService
    {
        Task<IReadOnlyList<Asset>> GetAssetsByTenant(string tenantId);
        Task<IReadOnlyList<AssetPair>> GetAssetPairsByTenant(string tenantId);

        Task<Asset> GetDefaultBaseAsset(string tenantId);
        IReadOnlyList<PriceEntity> GetPrices(string tenantId);
        OrderBookEntity OrderBook(string tenantId, string assetPairId);
    }
}

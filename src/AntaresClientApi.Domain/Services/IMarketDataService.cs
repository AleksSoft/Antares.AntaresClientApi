using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AntaresClientApi.Database.CandleData.Models;
using Assets.Domain.Entities;
using OrderBooks.MyNoSql.OrderBookData;
using OrderBooks.MyNoSql.PriceData;

namespace AntaresClientApi.Domain.Services
{
    public interface IMarketDataService
    {
        Task<IReadOnlyList<Asset>> GetAssetsByTenant(string tenantId);
        Task<IReadOnlyList<AssetPair>> GetAssetPairsByTenant(string tenantId);
        Task<AssetPair> GetAssetPairByTenantAndId(string tenantId, string assetPairId);

        Task<Asset> GetDefaultBaseAsset(string tenantId);
        IReadOnlyList<PriceEntity> GetPrices(string tenantId);
        OrderBookEntity OrderBook(string tenantId, string assetPairId);
        Task<IReadOnlyList<CandleEntity>> GetCandles(string symbol, DateTime fromDate, DateTime toDate,
            AntaresClientApi.Database.CandleData.Models.CandleType interval);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntaresClientApi.Database.CandleData.Models;
using AntaresClientApi.Database.Context;
using Assets.Domain.Entities;
using Assets.Domain.MyNoSql;
using Microsoft.EntityFrameworkCore;
using MyNoSqlServer.Abstractions;
using OrderBooks.MyNoSql.OrderBookData;
using OrderBooks.MyNoSql.PriceData;

namespace AntaresClientApi.Domain.Services
{
    public class MarketDataService : IMarketDataService
    {
        private IMyNoSqlServerDataReader<AssetsEntity> _assetsReader;
        private IMyNoSqlServerDataReader<AssetPairsEntity> _assetPairsReader;
        private readonly IMyNoSqlServerDataReader<OrderBookEntity> _orderBookDataReader;
        private readonly IMyNoSqlServerDataReader<PriceEntity> _priceDataReader;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MarketDataService(
            IMyNoSqlServerDataReader<AssetsEntity> assetsReader,
            IMyNoSqlServerDataReader<AssetPairsEntity> assetPairsReader,
            IMyNoSqlServerDataReader<OrderBookEntity> orderBookDataReader,
            IMyNoSqlServerDataReader<PriceEntity> priceDataReader,
            IDbConnectionFactory dbConnectionFactory)
        {
            _assetsReader = assetsReader;
            _assetPairsReader = assetPairsReader;
            _orderBookDataReader = orderBookDataReader;
            _priceDataReader = priceDataReader;
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IReadOnlyList<Asset>> GetAssetsByTenant(string tenantId)
        {
            var assets = _assetsReader.Get(AssetsEntity.GetPartitionKey(tenantId), AssetsEntity.GetRowKey());

            if (assets?.Assets == null)
            {
                return new List<Asset>();
            }

            return assets.Assets;
        }

        public async Task<Asset> GetAssetByTenantAndId(string tenantId, long assetId)
        {
            var assets = await GetAssetsByTenant(tenantId);
            var asset = assets.FirstOrDefault(a => a.Id == assetId);
            return asset;
        }

        public async Task<IReadOnlyList<AssetPair>> GetAssetPairsByTenant(string tenantId)
        {
            var pairs = _assetPairsReader.Get(AssetPairsEntity.GetPartitionKey(tenantId), AssetPairsEntity.GetRowKey());

            if (pairs?.AssetPairs == null)
            {
                return new List<AssetPair>();
            }

            return pairs.AssetPairs;
        }

        public async Task<AssetPair> GetAssetPairByTenantAndId(string tenantId, string assetPairId)
        {
            var pairs = await GetAssetPairsByTenant(tenantId);
            return pairs.FirstOrDefault(p => p.Symbol == assetPairId);
        }

        public async Task<Asset> GetDefaultBaseAsset(string tenantId)
        {
            //todo: add to asset service - is default base asset flag for assets of default asset property in asset list

            var assets = await GetAssetsByTenant(tenantId);

            var usd = assets.FirstOrDefault(a => a.Symbol == "USD");
            if (usd != null)
                return usd;

            return assets.FirstOrDefault(a => !a.IsDisabled);
        }

        public IReadOnlyList<PriceEntity> GetPrices(string tenantId)
        {
            var prices = _priceDataReader.Get(PriceEntity.GeneratePartitionKey(tenantId));
            return prices;
        }

        public OrderBookEntity OrderBook(string tenantId, string assetPairId)
        {
            var book = _orderBookDataReader.Get(OrderBookEntity.GeneratePartitionKey(tenantId), OrderBookEntity.GenerateRowKey(assetPairId));
            return book;
        }

        public async Task<IReadOnlyList<CandleEntity>> GetCandles(string symbol,
            DateTime fromDate,
            DateTime toDate,
            AntaresClientApi.Database.CandleData.Models.CandleType interval)
        {
            using (var ctx = _dbConnectionFactory.CreateCandleDataContext())
            {
                var data = await ctx.Candles
                    .Where(c => c.AssetPairId == symbol && c.Type == interval)
                    .Where(c => c.Time >= fromDate && c.Time <= toDate)
                    .ToListAsync();

                return data;
            }
        }
    }
}

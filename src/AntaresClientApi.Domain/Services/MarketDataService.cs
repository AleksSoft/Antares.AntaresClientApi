using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Domain.Entities;
using Assets.Domain.MyNoSql;
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

        public MarketDataService(IMyNoSqlServerDataReader<AssetsEntity> assetsReader, 
            IMyNoSqlServerDataReader<AssetPairsEntity> assetPairsReader,
            IMyNoSqlServerDataReader<OrderBookEntity> orderBookDataReader,
            IMyNoSqlServerDataReader<PriceEntity> priceDataReader)
        {
            _assetsReader = assetsReader;
            _assetPairsReader = assetPairsReader;
            _orderBookDataReader = orderBookDataReader;
            _priceDataReader = priceDataReader;
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

        public async Task<IReadOnlyList<AssetPair>> GetAssetPairsByTenant(string tenantId)
        {
            var pairs = _assetPairsReader.Get(AssetPairsEntity.GetPartitionKey(tenantId), AssetPairsEntity.GetRowKey());

            if (pairs?.AssetPairs == null)
            {
                return new List<AssetPair>();
            }

            return pairs.AssetPairs;
        }

        public async Task<Asset> GetDefaultBaseAsset(string tenantId)
        {
            //todo: add to asset service - is defaukt base asset flag for assets of defaukt asset property in asset list

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
    }
}

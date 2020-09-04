using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assets.Domain.Entities;
using Assets.Domain.MyNoSql;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Services
{
    public class MarketDataService : IMarketDataService
    {
        private IMyNoSqlServerDataReader<AssetsEntity> _assetsReader;
        private IMyNoSqlServerDataReader<AssetPairsEntity> _assetPairsReader;

        public MarketDataService(IMyNoSqlServerDataReader<AssetsEntity> assetsReader, IMyNoSqlServerDataReader<AssetPairsEntity> assetPairsReader)
        {
            _assetsReader = assetsReader;
            _assetPairsReader = assetPairsReader;
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
    }
}

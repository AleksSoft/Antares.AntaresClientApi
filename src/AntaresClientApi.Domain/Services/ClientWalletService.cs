using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntaresClientApi.Database.Context;
using AntaresClientApi.Database.MeData.Models;
using AntaresClientApi.Domain.Models;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Models.Wallet;
using AntaresClientApi.Domain.Services.Extention;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Services
{
    public class ClientWalletService : IClientWalletService
    {
        private readonly IMyNoSqlServerDataReader<ClientWalletEntity> _walletsReader;
        private readonly IMyNoSqlServerDataWriter<ClientWalletEntity> _walletsWriter;

        private readonly IMyNoSqlServerDataReader<ClientWalletIndexByIdEntity> _walletsByIdReader;
        private readonly IMyNoSqlServerDataWriter<ClientWalletIndexByIdEntity> _walletsByIdWriter;
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMarketDataService _marketDataService;


        private readonly ILogger<ClientWalletService> _logger;

        public ClientWalletService(
            IMyNoSqlServerDataReader<ClientWalletEntity> walletsReader,
            ILogger<ClientWalletService> logger,
            IMyNoSqlServerDataWriter<ClientWalletEntity> walletsWriter,
            IMyNoSqlServerDataReader<ClientWalletIndexByIdEntity> walletsByIdReader,
            IMyNoSqlServerDataWriter<ClientWalletIndexByIdEntity> walletsByIdWriter,
            IDbConnectionFactory dbConnectionFactory,
            IMarketDataService marketDataService
            )
        {
            _walletsReader = walletsReader;
            _logger = logger;
            _walletsWriter = walletsWriter;
            _walletsByIdReader = walletsByIdReader;
            _walletsByIdWriter = walletsByIdWriter;
            _dbConnectionFactory = dbConnectionFactory;
            _marketDataService = marketDataService;
        }

        public async Task<ClientWalletEntity> RegisterOrGetDefaultWallets(ClientIdentity client)
        {
            var existWallet = await _walletsWriter.TryGetAsync(ClientWalletEntity.GetPartitionKey(client.TenantId), ClientWalletEntity.GetRowKey(client.ClientId));
            if (existWallet != null)
            {
                return existWallet;
            }

            bool result;
            do
            {
                var walletId = GenerateWalletId();

                var entity = ClientWalletEntity.Generate(client.TenantId, client.ClientId);
                entity.WalletId = walletId;
                entity.Type = TradingWalletType.Trading;
                entity.Client = client;

                var indexById = ClientWalletIndexByIdEntity.Generate(entity.Client.TenantId, walletId, entity.Client.ClientId);
                result = await _walletsByIdWriter.TryInsertAsync(indexById);

                if (result)
                {
                    await _walletsWriter.InsertOrReplaceAsync(entity);
                    
                    return entity;
                }

                _logger.LogInformation("Cannot insert new wallet with id={WalletId}. ClientId={clientId}, TenantId={TenamtId}", entity.WalletId, entity.Client.TenantId, entity.Client.ClientId);

            } while (true);
        }

        public async Task<IReadOnlyList<IAssetBalance>> GetClientBalances(string tenantId, long clientId)
        {
            var wallet = await RegisterOrGetDefaultWallets(new ClientIdentity()
            {
                TenantId = tenantId,
                ClientId = clientId
            });

            using (var ctx = _dbConnectionFactory.CreateMeWriterDataContext())
            {
                var balance = await ctx.Balances
                    .Where(w =>
                        w.BrokerId == wallet.Client.TenantId &&
                        w.AccountId == wallet.Client.ClientId &&
                        w.WalletId == wallet.WalletId)
                    .ToListAsync();

                return balance;
            }
        }

        public async Task<IReadOnlyList<IClientOrder>> GetClientOrdersAsync(string tenantId, long clientId, string assetId)
        {
            var wallet = await RegisterOrGetDefaultWallets(new ClientIdentity()
            {
                TenantId = tenantId,
                ClientId = clientId
            });

            if (wallet == null)
                return new List<IClientOrder>();

            using (var ctx = _dbConnectionFactory.CreateMeWriterDataContext())
            {
                var orders = await ctx.Orders
                    .Where(o => o.BrokerId == tenantId && o.WalletId == wallet.WalletId &&
                                (o.Status == OrderStatus.Placed || o.Status == OrderStatus.PartiallyMatched))
                    .ToListAsync();

                return orders;
            }
        }

        public async Task<IReadOnlyList<IClientTrade>> GetClientTradesAsync(string tenantId,
            long clientId,
            string assetPairId,
            DateTime? fromTime,
            DateTime? toTime,
            string side,
            int skip,
            int take)
        {

            var wallet = await RegisterOrGetDefaultWallets(new ClientIdentity()
            {
                TenantId = tenantId,
                ClientId = clientId
            });

            if (wallet == null)
                return new List<IClientTrade>();

            using (var ctx = _dbConnectionFactory.CreateMeWriterDataContext())
            {
                var trades = ctx.Trades.Where(a => a.BrokerId == tenantId && a.WalletId == wallet.WalletId);

                if (!string.IsNullOrEmpty(assetPairId))
                {
                    var pairs = await _marketDataService.GetAssetPairsByTenant(tenantId);
                    var assets = await _marketDataService.GetAssetsByTenant(tenantId);
                    var pair = pairs.FirstOrDefault(p => p.Symbol == assetPairId);
                    
                    if (pair == null)
                    {
                        return new List<IClientTrade>();
                    }

                    var baseAsset = assets.FirstOrDefault(a => a.Id == pair.BaseAssetId);
                    var quoteAsset = assets.FirstOrDefault(a => a.Id == pair.QuotingAssetId);

                    if (baseAsset == null || quoteAsset==null)
                    {
                        return new List<IClientTrade>();
                    }

                    trades = trades.Where(t => t.BaseAssetId == baseAsset.Symbol && t.QuotingAssetId == quoteAsset.Symbol);
                }

                if (side == "Buy")
                {
                    trades = trades.Where(t => !t.BaseVolume.StartsWith("-"));
                }

                if (side == "Sell")
                {
                    trades = trades.Where(t => t.BaseVolume.StartsWith("-"));
                }

                if (fromTime.HasValue)
                {
                    trades = trades.Where(t => t.Timestamp >= fromTime.Value);
                }

                if (toTime.HasValue)
                {
                    trades = trades.Where(t => t.Timestamp <= toTime.Value);
                }

                trades = trades.OrderByDescending(t => t.Timestamp);

                if (skip > 0)
                {
                    trades = trades.Skip(skip);
                }

                if (take > 0)
                {
                    trades = trades.Take(take);
                }

                var tradeList = await trades.ToListAsync();


                return tradeList;
            }
        }

        public async Task<long> GetWalletIdAsync(string tenantId, long clientId)
        {
            var wallet = await RegisterOrGetDefaultWallets(new ClientIdentity()
            {
                TenantId = tenantId,
                ClientId = clientId
            });

            return wallet.WalletId;
        }

        private long GenerateWalletId()
        {
            var id = (long) (DateTime.UtcNow - DateTime.Parse("2020-01-01")).TotalSeconds;
            return id;
        }
    }
}

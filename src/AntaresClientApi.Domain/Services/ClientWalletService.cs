using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntaresClientApi.Database.Context;
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


        private readonly ILogger<ClientWalletService> _logger;

        public ClientWalletService(IMyNoSqlServerDataReader<ClientWalletEntity> walletsReader,
            ILogger<ClientWalletService> logger,
            IMyNoSqlServerDataWriter<ClientWalletEntity> walletsWriter,
            IMyNoSqlServerDataReader<ClientWalletIndexByIdEntity> walletsByIdReader,
            IMyNoSqlServerDataWriter<ClientWalletIndexByIdEntity> walletsByIdWriter,
            IDbConnectionFactory dbConnectionFactory)
        {
            _walletsReader = walletsReader;
            _logger = logger;
            _walletsWriter = walletsWriter;
            _walletsByIdReader = walletsByIdReader;
            _walletsByIdWriter = walletsByIdWriter;
            _dbConnectionFactory = dbConnectionFactory;
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
                try
                {
                    var balance = await ctx.Balances
                        .Where(w =>
                            w.BrokerId == wallet.Client.TenantId &&
                            w.AccountId == wallet.Client.ClientId &&
                            w.WalletId == wallet.WalletId)
                        .ToListAsync();

                    return balance;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        private long GenerateWalletId()
        {
            var id = (long) (DateTime.UtcNow - DateTime.Parse("2020-01-01")).TotalSeconds;
            return id;
        }
    }
}

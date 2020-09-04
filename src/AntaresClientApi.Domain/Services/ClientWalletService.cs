using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Services.Extention;
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

        
        private readonly ILogger<ClientWalletService> _logger;

        public ClientWalletService(IMyNoSqlServerDataReader<ClientWalletEntity> walletsReader,
            ILogger<ClientWalletService> logger,
            IMyNoSqlServerDataWriter<ClientWalletEntity> walletsWriter,
            IMyNoSqlServerDataReader<ClientWalletIndexByIdEntity> walletsByIdReader,
            IMyNoSqlServerDataWriter<ClientWalletIndexByIdEntity> walletsByIdWriter)
        {
            _walletsReader = walletsReader;
            _logger = logger;
            _walletsWriter = walletsWriter;
            _walletsByIdReader = walletsByIdReader;
            _walletsByIdWriter = walletsByIdWriter;
        }

        public async Task RegisterDefaultWallets(ClientIdentity client)
        {
            var existWallet = await _walletsWriter.TryGetAsync(ClientWalletEntity.GetPartitionKey(client.TenantId), ClientWalletEntity.GetRowKey(client.ClientId));
            if (existWallet != null)
            {
                return;
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
                }


                result = await _walletsWriter.TryInsertAsync(entity);

            } while (!result);
        }

        public async Task<IReadOnlyList<AssetBalance>> GetClientBalances(string tenantId, long clientId)
        {
            return null;
        }

        private long GenerateWalletId()
        {
            var id = (long) (DateTime.UtcNow - DateTime.Parse("2020-01-01")).TotalSeconds;
            return id;
        }
    }
}

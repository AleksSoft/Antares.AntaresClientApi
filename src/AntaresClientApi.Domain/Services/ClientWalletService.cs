using System;
using System.Threading.Tasks;
using AntaresClientApi.Domain.Models;
using AntaresClientApi.Domain.Models.MyNoSql;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Services
{
    public class ClientWalletService : IClientWalletService
    {
        private readonly IMyNoSqlServerDataReader<ClientWalletEntity> _walletsReader;
        private readonly ILogger<ClientWalletService> _logger;

        public ClientWalletService(IMyNoSqlServerDataReader<ClientWalletEntity> walletsReader,
            ILogger<ClientWalletService> logger)
        {
            _walletsReader = walletsReader;
            _logger = logger;
        }

        public Task RegisterDefaultWallets(ClientIdentity client)
        {
            //todo: implement Wallet creations!

            var wallet = _walletsReader.Get(ClientWalletEntity.GetPk(), ClientWalletEntity.GetRk(client.TenantId, client.ClientId));

            if (wallet == null)
            {
                _logger.LogError("Cannot found pre-created wallet for TenantId: {TenantId}, ClientId: {ClientId}. Please add data to MyNoSQL", client.TenantId, client.ClientId);
                throw new Exception("Cannot create wallets. Do not found pre-create records");
            }

            return Task.CompletedTask;
        }
    }
}

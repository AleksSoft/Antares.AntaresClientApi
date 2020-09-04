using System;
using System.Threading;
using AntaresClientApi.Domain.Models.MyNoSql;
using Assets.Domain.MyNoSql;
using Autofac;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;

namespace AntaresClientApi.Lifetime
{ 
    public class LifetimeManager : IStartable, IDisposable
    {
        private readonly ILogger<LifetimeManager> _logger;
        private readonly MyNoSqlTcpClient _client;
        private readonly IMyNoSqlServerDataReader<SessionEntity> _sessionReader;
        private readonly IMyNoSqlServerDataReader<RegistrationTokenEntity> _registrationTokenReader;
        private readonly IMyNoSqlServerDataReader<ClientWalletEntity> _clientWalletReader;
        private readonly IMyNoSqlServerDataReader<AssetsEntity> _assetsReader;
        private readonly IMyNoSqlServerDataReader<AssetPairsEntity> _assetPairsReader;
        private readonly IMyNoSqlServerDataReader<PersonalDataEntity> _personalDataReader;
        private readonly IMyNoSqlServerDataReader<AuthDataEntity> _authDataReader;
        private readonly IMyNoSqlServerDataReader<AuthDataIndexByIdEntity> _authIndexNyIdDataReader;

        public LifetimeManager(
            ILogger<LifetimeManager> logger,
            MyNoSqlTcpClient client,
            IMyNoSqlServerDataReader<SessionEntity> sessionReader,
            IMyNoSqlServerDataReader<RegistrationTokenEntity> registrationTokenReader,
            IMyNoSqlServerDataReader<ClientWalletEntity> clientWalletReader,
            IMyNoSqlServerDataReader<AssetsEntity> assetsReader,
            IMyNoSqlServerDataReader<AssetPairsEntity> assetPairsReader,
            IMyNoSqlServerDataReader<PersonalDataEntity> personalDataReader,
            IMyNoSqlServerDataReader<AuthDataEntity> authDataReader,
            IMyNoSqlServerDataReader<AuthDataIndexByIdEntity> authIndexNyIdDataReader)
        {
            _logger = logger;
            _client = client;
            _sessionReader = sessionReader;
            _registrationTokenReader = registrationTokenReader;
            _clientWalletReader = clientWalletReader;
            _assetsReader = assetsReader;
            _assetPairsReader = assetPairsReader;
            _personalDataReader = personalDataReader;
            _authDataReader = authDataReader;
            _authIndexNyIdDataReader = authIndexNyIdDataReader;
        }

        public void Start()
        {
            _logger.LogInformation("LifetimeManager starting...");
            _client.Start();

            _logger.LogInformation("LifetimeManager sleep 2 second...");
            Thread.Sleep(2000);
            
            _logger.LogInformation("sessionReader - count: {Count}", _sessionReader.Count());
            _logger.LogInformation("registrationTokenReader - count: {Count}", _registrationTokenReader.Count());
            _logger.LogInformation("clientWalletReader - count: {Count}", _clientWalletReader.Count());
            _logger.LogInformation("assetsReader - count: {Count}", _assetsReader.Count());
            _logger.LogInformation("assetPairsReader - count: {Count}", _assetPairsReader.Count());
            _logger.LogInformation("personalDataReader - count: {Count}", _personalDataReader.Count());
            _logger.LogInformation("authDataReader - count: {Count}", _authDataReader.Count());
            _logger.LogInformation("authIndexNyIdDataReader - count: {Count}", _authIndexNyIdDataReader.Count());


            _logger.LogInformation("LifetimeManager started");
        }

        public void Dispose()
        {
            if (_client.Connected)
            {
                _client.Stop();
            }
        }
    }
}

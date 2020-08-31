using System;
using System.Threading;
using AntaresClientApi.Domain.Models.MyNoSql;
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

        public LifetimeManager(
            ILogger<LifetimeManager> logger,
            MyNoSqlTcpClient client,
            IMyNoSqlServerDataReader<SessionEntity> sessionReader,
            IMyNoSqlServerDataReader<RegistrationTokenEntity> registrationTokenReader)
        {
            _logger = logger;
            _client = client;
            _sessionReader = sessionReader;
            _registrationTokenReader = registrationTokenReader;
        }

        public void Start()
        {
            _logger.LogInformation("LifetimeManager starting...");
            _client.Start();

            _logger.LogInformation("LifetimeManager sleep 2 second...");
            Thread.Sleep(2000);
            
            _logger.LogInformation("sessionReader - count: {Count}", _sessionReader.Count());
            _logger.LogInformation("registrationTokenReader - count: {Count}", _registrationTokenReader.Count());

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

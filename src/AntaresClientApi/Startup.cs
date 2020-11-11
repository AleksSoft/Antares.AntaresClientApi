using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AntaresClientApi.Common.Configuration;
using AntaresClientApi.Database;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Services;
using AntaresClientApi.Domain.Services.Mock;
using AntaresClientApi.GrpcServices;
using AntaresClientApi.GrpcServices.Authentication;
using AntaresClientApi.Lifetime;
using Assets.Client;
using Assets.Domain.MyNoSql;
using Autofac;
using Common;
using Grpc.AspNetCore.Server;
using MatchingEngine.Client;
using MatchingEngine.Client.Contracts.Incoming;
using MatchingEngine.Client.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Newtonsoft.Json;
using OrderBooks.MyNoSql.OrderBookData;
using OrderBooks.MyNoSql.PriceData;
using Prometheus;
using Swisschain.Sdk.Server.Common;

namespace AntaresClientApi
{
    public sealed class Startup : SwisschainStartup<AppConfig>
    {
        private MyNoSqlTcpClient _noSqlClient;

        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        protected override void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            base.RegisterEndpoints(endpoints);

            endpoints.MapGrpcService<MonitoringService>();
            endpoints.MapGrpcService<GrpcApiService>();
        }

        protected override void ConfigureExt(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMetricServer();
        }

        protected override void ConfigureContainerExt(ContainerBuilder builder)
        {
            builder
                .RegisterInstance(Config.SessionConfig)
                .AsSelf()
                .SingleInstance();

            RegisterServices(builder);

            builder.RegisterType<LifetimeManager>()
                .As<IStartable>()
                .SingleInstance()
                .AutoActivate();

            MyNoSql(builder);
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<SessionService>()
                .As<ISessionService>()
                .As<IRegistrationTokenService>()
                .SingleInstance();

            builder.RegisterType<SmsVerificationMock>()
                .As<ISmsVerification>()
                .SingleInstance();

            builder.RegisterType<AuthServiceMock>()
                .As<IAuthService>()
                .SingleInstance();

            builder.RegisterType<EmailVerificationMock>()
                .As<IEmailVerification>()
                .SingleInstance();

            builder.RegisterType<PersonalDataMock>()
                .As<IPersonalData>()
                .SingleInstance();

            builder.RegisterType<ClientAccountManager>()
                .As<IClientAccountManager>()
                .SingleInstance();

            builder.RegisterType<ClientWalletService>()
                .As<IClientWalletService>()
                .SingleInstance();

            builder.RegisterType<MarketDataService>()
                .As<IMarketDataService>()
                .SingleInstance();

            builder.RegisterType<CashInOutProcessor>()
                .As<ICashInOutProcessor>()
                .SingleInstance();

            builder.RegisterMatchingEngineClient(Config.MatchingEngine);

            builder.RegisterModule(new PostgresModule(Config.Db.MeWriterConnectionString, Config.Db.CandleConnectionString));

        }

        private void MyNoSql(ContainerBuilder builder)
        {
            _noSqlClient = new MyNoSqlTcpClient(
                () => Config.MyNoSqlServer.ReaderServiceUrl,
                $"{ApplicationInformation.AppName}-{Environment.MachineName}");

            builder.Register(ctx => _noSqlClient)
                .AsSelf()
                .SingleInstance();


            RegisterNoSqlReaderAndWriter<SessionEntity>(builder, MyNoSqlServerTables.SessionsTableName);
            RegisterNoSqlReaderAndWriter<RegistrationTokenEntity>(builder, MyNoSqlServerTables.RegistrationTokenTableName);

            RegisterNoSqlReaderAndWriter<ClientWalletEntity>(builder, MyNoSqlServerTables.ClientWalletTableName);
            RegisterNoSqlReaderAndWriter<ClientWalletIndexByIdEntity>(builder, MyNoSqlServerTables.ClientWalletIndexedByIdTableName);

            RegisterNoSqlReaderAndWriter<ClientProfileEntity>(builder, MyNoSqlServerTables.ClientProfileTableName);

            #region Mock
            RegisterNoSqlReaderAndWriter<PersonalDataEntity>(builder, MyNoSqlServerTables.PersonalDataTableName);
            RegisterNoSqlReaderAndWriter<AuthDataEntity>(builder, MyNoSqlServerTables.AuthDataTableName);
            RegisterNoSqlReaderAndWriter<AuthDataIndexByIdEntity>(builder, MyNoSqlServerTables.AuthDataIndexByIdTableName);



            #endregion

            RegisterNoSqlReader<AssetsEntity>(builder, SetupMyNoSqlAssetService.AssetsTableName);
            RegisterNoSqlReader<AssetPairsEntity>(builder, SetupMyNoSqlAssetService.AssetPairsTableName);

            RegisterNoSqlReader<OrderBookEntity>(builder, OrderBookEntity.OrderBookTableName);
            RegisterNoSqlReader<PriceEntity>(builder, PriceEntity.PriceTableName);
        }


        private void RegisterNoSqlReaderAndWriter<TEntity>(ContainerBuilder builder, string table) where TEntity : IMyNoSqlDbEntity, new()
        {
            RegisterNoSqlReader<TEntity>(builder, table);

            builder.Register(ctx =>
                {
                    return new MyNoSqlServer.DataWriter.MyNoSqlServerDataWriter<TEntity>(() => Config.MyNoSqlServer.WriterServiceUrl,
                        table);
                })
                .As<IMyNoSqlServerDataWriter<TEntity>>()
                .SingleInstance();
        }

        private void RegisterNoSqlReader<TEntity>(ContainerBuilder builder, string table) where TEntity : IMyNoSqlDbEntity, new()
        {
            builder
                .Register(ctx => new MyNoSqlReadRepository<TEntity>(_noSqlClient, table))
                .As<IMyNoSqlServerDataReader<TEntity>>()
                .SingleInstance();
        }

        protected override void ConfigureGrpcServiceOptions(GrpcServiceOptions options)
        {
            base.ConfigureGrpcServiceOptions(options);
            options.Interceptors.Add<AuthenticationInterceptor>();
        }
    }

    
}

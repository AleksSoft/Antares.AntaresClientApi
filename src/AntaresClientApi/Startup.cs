using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AntaresClientApi.Common.Configuration;
using AntaresClientApi.Domain.Models.MyNoSql;
using AntaresClientApi.Domain.Services;
using AntaresClientApi.Domain.Services.Mock;
using AntaresClientApi.GrpcServices;
using AntaresClientApi.GrpcServices.Authentication;
using AntaresClientApi.Lifetime;
using Autofac;
using Common;
using Grpc.AspNetCore.Server;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Newtonsoft.Json;
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

        private static void RegisterServices(ContainerBuilder builder)
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
        }


        private void RegisterNoSqlReaderAndWriter<TEntity>(ContainerBuilder builder, string table) where TEntity : IMyNoSqlDbEntity, new()
        {
            builder
                .Register(ctx => new MyNoSqlReadRepository<TEntity>(_noSqlClient, table))
                .As<IMyNoSqlServerDataReader<TEntity>>()
                .SingleInstance();

            builder.Register(ctx =>
                {
                    return new MyNoSqlServer.DataWriter.MyNoSqlServerDataWriter<TEntity>(() => Config.MyNoSqlServer.WriterServiceUrl,
                        table);
                })
                .As<IMyNoSqlServerDataWriter<TEntity>>()
                .SingleInstance();
        }

        protected override void ConfigureGrpcServiceOptions(GrpcServiceOptions options)
        {
            base.ConfigureGrpcServiceOptions(options);
            options.Interceptors.Add<AuthenticationInterceptor>();
        }
    }

    
}

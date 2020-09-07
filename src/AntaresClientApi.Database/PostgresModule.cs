using AntaresClientApi.Database.Context;
using Autofac;

namespace AntaresClientApi.Database
{
    public class PostgresModule : Module
    {
        private readonly string _meWriterConnectionString;
        private readonly string _candleConnectionString;

        public PostgresModule(string meWriterConnectionString, string candleConnectionString)
        {
            _meWriterConnectionString = meWriterConnectionString;
            _candleConnectionString = candleConnectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new DbConnectionFactory(_meWriterConnectionString, _candleConnectionString))
                .AsSelf()
                .As<IDbConnectionFactory>()
                .SingleInstance();
        }
    }
}

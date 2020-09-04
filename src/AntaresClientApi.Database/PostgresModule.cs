using AntaresClientApi.Database.Context;
using Autofac;

namespace AntaresClientApi.Database
{
    public class PostgresModule : Module
    {
        private readonly string _meWriterConnectionString;

        public PostgresModule(string meWriterConnectionString)
        {
            _meWriterConnectionString = meWriterConnectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DbConnectionFactory>()
                .AsSelf()
                .As<IDbConnectionFactory>()
                .WithParameter(TypedParameter.From(_meWriterConnectionString))
                .SingleInstance();
        }
    }
}

using AntaresClientApi.Database.CandleData;
using AntaresClientApi.Database.MeData;
using Microsoft.EntityFrameworkCore;

namespace AntaresClientApi.Database.Context
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _meWriterConnectionString;
        private readonly string _candleConnectionString;

        public DbConnectionFactory(string meWriterConnectionString, string candleConnectionString)
        {
            _meWriterConnectionString = meWriterConnectionString;
            _candleConnectionString = candleConnectionString;
        }

        public void EnsureMigration()
        {
            //using (var context = CreateMeWriterDataContext())
            //{
            //    context.Database.Migrate();
            //}
        }

        public MeWriterDataContext CreateMeWriterDataContext()
        {
            return new MeWriterDataContext(_meWriterConnectionString);
        }

        public CandleDataContext CreateCandleDataContext()
        {
            return new CandleDataContext(_candleConnectionString);
        }
    }

    public interface IDbConnectionFactory
    {
        MeWriterDataContext CreateMeWriterDataContext();
        CandleDataContext CreateCandleDataContext();
    }
}

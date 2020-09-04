using AntaresClientApi.Database.MeData;
using Microsoft.EntityFrameworkCore;

namespace AntaresClientApi.Database.Context
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _meWriterConnectionString;

        public DbConnectionFactory(string meWriterConnectionString)
        {
            _meWriterConnectionString = meWriterConnectionString;
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
    }

    public interface IDbConnectionFactory
    {
        MeWriterDataContext CreateMeWriterDataContext();
    }
}

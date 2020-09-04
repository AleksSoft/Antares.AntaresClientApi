using Microsoft.EntityFrameworkCore;

namespace AntaresClientApi.Database.Context
{
    public class ConnectionFactory
    {
        private readonly string _meWriterConnectionString;

        public ConnectionFactory(string meWriterConnectionString)
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

        internal MeWriterDataContext CreateMeWriterDataContext()
        {
            return new MeWriterDataContext(_meWriterConnectionString);
        }
    }
}

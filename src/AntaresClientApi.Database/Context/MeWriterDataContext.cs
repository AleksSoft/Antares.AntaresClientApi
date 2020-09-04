using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AntaresClientApi.Database.Context
{
    public class MeWriterDataContext: DbContext
    {
        private string _connectionString;

        public MeWriterDataContext()
        {
        }

        public MeWriterDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (_connectionString == null)
            //{
            //    System.Console.Write("Enter connection string: ");
            //    _connectionString = System.Console.ReadLine();
            //}

            //optionsBuilder.UseNpgsql(_connectionString,
            //    o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schema));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema(Schema);
        }
    }
}

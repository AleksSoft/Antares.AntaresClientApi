using AntaresClientApi.Database.MeData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AntaresClientApi.Database.MeData
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

        public DbSet<BalanceDbEntity> Balances { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            
            //if (_connectionString == null)
            //{
            //    System.Console.Write("Enter connection string: ");
            //    _connectionString = System.Console.ReadLine();
            //}

            optionsBuilder.UseNpgsql(_connectionString,
                o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema(Schema);
        }
    }
}

using AntaresClientApi.Database.MeData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace AntaresClientApi.Database.MeData
{
    public class MeWriterDataContext: DbContext
    {
        public static ILoggerFactory LoggerFactory { get; set; }

        private string _connectionString;

        public MeWriterDataContext()
        {
        }

        public MeWriterDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<BalanceDbEntity> Balances { get; set; }

        public DbSet<OrderDbEntity> Orders { get; set; }

        public DbSet<TradeDbEntity> Trades { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            
            //if (_connectionString == null)
            //{
            //    System.Console.Write("Enter connection string: ");
            //    _connectionString = System.Console.ReadLine();
            //}

            optionsBuilder
                //.UseLoggerFactory(LoggerFactory)
                //.EnableSensitiveDataLogging()
                .UseNpgsql(_connectionString,
                o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema(Schema);
        }
    }
}

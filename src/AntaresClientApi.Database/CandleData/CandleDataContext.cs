using AntaresClientApi.Database.CandleData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AntaresClientApi.Database.CandleData
{
    public class CandleDataContext: DbContext
    {
        private const string Schema = "candles";

        private readonly string _connectionString;

        public CandleDataContext()
        {
        }

        public CandleDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<CandleEntity> Candles { get; set; }

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
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<CandleEntity>()
                .HasKey(entity => new { entity.AssetPairId, entity.Type, entity.Time });

            modelBuilder.Entity<CandleEntity>()
                .Property(o => o.Type)
                .HasConversion(new EnumToNumberConverter<CandleType, short>());
        }
    }
}

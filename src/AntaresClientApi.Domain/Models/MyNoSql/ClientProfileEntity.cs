using System;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Models.MyNoSql
{
    public class ClientProfileEntity : IMyNoSqlDbEntity
    {
        public string TenantId { get; set; }

        public long ClientId { get; set; }

        public string BaseAssetId { get; set; }

        public static string GeneratePartitionKey(string tenantId) => tenantId;

        public static string GenerateRowKey(long clientId) => clientId.ToString();

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public DateTime? Expires { get; set; }
    }
}

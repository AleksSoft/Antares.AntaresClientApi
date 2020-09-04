using System;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Models.MyNoSql
{
    public class AuthDataIndexByIdEntity : IMyNoSqlDbEntity
    {
        public string TenantId { get; set; }
        public long ClientId { get; set; }
        public string Email { get; set; }

        public static string GeneratePartitionKey(string tenantId) => tenantId;

        public static string GenerateRowKey(long clientId) => clientId.ToString();

        public static AuthDataIndexByIdEntity Generate(string tenantId, long clientId, string email)
        {
            var entity = new AuthDataIndexByIdEntity()
            {
                PartitionKey = GeneratePartitionKey(tenantId),
                RowKey = GenerateRowKey(clientId),
                TenantId = tenantId,
                ClientId = clientId,
                Email = email
            };
            return entity;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public DateTime? Expires { get; set; }
    }
}

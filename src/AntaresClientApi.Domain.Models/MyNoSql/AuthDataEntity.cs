using System;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Models.MyNoSql
{
    public class AuthDataEntity: IMyNoSqlDbEntity
    {
        public string TenantId { get; set; }
        public long ClientId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Hint { get; set; }
        public string PinHash { get; set; }

        public static string GeneratePartitionKey() => "auth-mock";

        public static string GenerateRowKey(string tenantId, string email) => $"{tenantId}:::{email}";

        public static AuthDataEntity Generate(string tenantId, string email)
        {
            var entity = new AuthDataEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(tenantId, email),
                TenantId = tenantId,
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

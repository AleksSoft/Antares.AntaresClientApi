using System;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Models.MyNoSql
{
    public class PersonalDataEntity : IMyNoSqlDbEntity
    {
        public PersonalData Data { get; set; }

        public static string GeneratePartitionKey() => "DataById";
        public static string GenerateRowKey(string tenantId, long clientId) => $"{tenantId}:::{clientId}";

        public static PersonalDataEntity Generate(string tenantId, long clientId)
        {
            var entity = new PersonalDataEntity()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(tenantId, clientId),
                Data = new PersonalData(tenantId, clientId)
            };

            return entity;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public DateTime? Expires { get; set; }
    }
}

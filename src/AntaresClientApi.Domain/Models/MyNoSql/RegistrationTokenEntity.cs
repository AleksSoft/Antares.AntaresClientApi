using System;
using AntaresClientApi.Domain.Tools;
using Common;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Models.MyNoSql
{
    public class RegistrationTokenEntity : IMyNoSqlDbEntity
    {
        public const int SmsVerificationLimitDefault = 5;
        public static string GetPk() => "RegistrationToken";

        public string Id { get; set; }
        public int SmsVerificationLimit { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string EmailHash { get; set; }
        public string PhoneHash { get; set; }
        public bool EmailVerified { get; set; }
        public bool PhoneVerified { get; set; }

        public string LastCodeHash { get; set; }


        public bool RegistrationDone { get; set; }
        public string ClientId { get; set; }
        public string TenantId { get; set; }


        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public DateTime? Expires { get; set; }

        public static (RegistrationTokenEntity, string) Generate(int expirationInMins)
        {
            string token = $"{Guid.NewGuid():N}{Guid.NewGuid():N}{Guid.NewGuid():N}";
            var id = token.ToSha256().ToBase64();

            return (new RegistrationTokenEntity()
            {
                PartitionKey = GetPk(),
                RowKey = id,
                Id = id,
                SmsVerificationLimit = SmsVerificationLimitDefault,
                ExpirationDate = DateTime.UtcNow.AddMinutes(expirationInMins)
            }, token);
        }
    }
}

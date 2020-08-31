using System;
using System.Security.Principal;
using AntaresClientApi.Domain.Tools;
using Common;
using MyNoSqlServer.Abstractions;
using Lykke.Common.Extensions;

namespace AntaresClientApi.Domain.Models.MyNoSql
{
    public class SessionEntity : IMyNoSqlDbEntity
    {
        public const int SmsVerificationLimitDefault = 5;
        public const int PinVerificationLimitDefault = 5;

        public string Id { get; set; }
        public string ClientId { get; set; }
        public string TenantId { get; set; }
        public bool Verified { get; set; }
        public bool Sms { get; set; }
        public bool Pin { get; set; }
        public string PublicKey { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string LastCodeHash { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public DateTime? Expires { get; set; }

        public int SmsVerificationLimit { get; set; }
        public int PinVerificationLimit { get; set; }
        
        public static string GetPk() => "Session";

        public static (SessionEntity, string) Generate(int expirationInMins)
        {
            string token = $"{Guid.NewGuid():N}{Guid.NewGuid():N}{Guid.NewGuid():N}";
            var id = token.ToSha256().ToBase64();

            return (new SessionEntity
            {
                PartitionKey = GetPk(),
                RowKey = id,
                Id = id,
                ExpirationDate = DateTime.UtcNow.AddMinutes(expirationInMins),
                SmsVerificationLimit = SmsVerificationLimitDefault,
                PinVerificationLimit = PinVerificationLimitDefault
            }, token);
        }

        public void ResetSmsVerificationLimit()
        {
            SmsVerificationLimit = SmsVerificationLimitDefault;
        }

        public void ResetPinVerificationLimit()
        {
            PinVerificationLimit = PinVerificationLimitDefault;
        }
    }
}

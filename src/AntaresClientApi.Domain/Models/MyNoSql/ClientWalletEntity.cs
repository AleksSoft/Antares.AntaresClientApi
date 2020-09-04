using System;
using System.Collections.Generic;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Models.MyNoSql
{
    public class ClientWalletEntity: IMyNoSqlDbEntity
    {
        public long WalletId { get; set; }

        public TradingWalletType Type { get; set; }

        public ClientIdentity Client { get; set; }

        public static string GetPartitionKey(string tenantId) => tenantId;
        public static string GetRowKey(long clientId) => clientId.ToString();

        public static ClientWalletEntity Generate(string tenantId, long clientId)
        {
            var entity = new ClientWalletEntity()
            {
                PartitionKey = GetPartitionKey(tenantId),
                RowKey = GetRowKey(clientId)
            };

            return entity;
        }


        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public DateTime? Expires { get; set; }
    }


    public class ClientWalletIndexByIdEntity : IMyNoSqlDbEntity
    {
        public long WalletId { get; set; }

        public string TenantId { get; set; }

        public long ClientId { get; set; }

        public static string GetPartitionKey() => "wallets-index-by-id";
        public static string GetRowKey(long walletId) => walletId.ToString();

        public static ClientWalletIndexByIdEntity Generate(string tenantId, long walletId, long clientId)
        {
            var entity = new ClientWalletIndexByIdEntity()
            {
                PartitionKey = GetPartitionKey(),
                RowKey = GetRowKey(walletId),
                TenantId = tenantId,
                ClientId = clientId,
                WalletId = walletId
            };

            return entity;
        }


        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public DateTime? Expires { get; set; }
    }
}

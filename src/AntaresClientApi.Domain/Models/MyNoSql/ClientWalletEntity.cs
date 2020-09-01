using System;
using System.Collections.Generic;
using MyNoSqlServer.Abstractions;

namespace AntaresClientApi.Domain.Models.MyNoSql
{
    public class ClientWalletEntity: IMyNoSqlDbEntity
    {
        public List<TradingWallet> WalletList { get; set; }

        public ClientIdentity Client { get; set; }

        public static string GetPk() => "TradingWallet-ClientIndex";
        public static string GetRk(string tenantId, string clientId) => $"{tenantId}:::{clientId}";

        public static ClientWalletEntity Generate(string tenantId, string clientId)
        {
            var entity = new ClientWalletEntity()
            {
                PartitionKey = GetPk(),
                RowKey = GetRk(tenantId, clientId),
                WalletList = new List<TradingWallet>()
            };

            return entity;
        }


        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string TimeStamp { get; set; }
        public DateTime? Expires { get; set; }
    }
}

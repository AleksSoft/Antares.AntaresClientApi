namespace AntaresClientApi.Domain.Models.MyNoSql
{
    public class TradingWallet
    {
        public long WalletId { get; set; }

        public TradingWalletType Type { get; set; }

        public ClientIdentity Client { get; set; }
    }
}

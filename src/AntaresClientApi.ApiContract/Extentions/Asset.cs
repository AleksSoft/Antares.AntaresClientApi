namespace Swisschain.Lykke.AntaresWalletApi.ApiContract
{
    public partial class Asset
    {
        public Asset(
            string id,
            string name,
            string symbol,
            string displayId,
            int accuracy,
            bool kycNeeded,
            string categoryId,
            bool cardDeposit,
            bool swiftDeposit,
            bool blockchainDeposit,
            bool swiftWithdrawal,
            bool forwardWithdrawa,
            bool crosschainWithdrawal,
            bool isTrusted,
            bool canBeBase,
            string iconUrl)
        {
            Accuracy = accuracy;
            KycNeeded = kycNeeded;
            CardDeposit = cardDeposit;
            SwiftDeposit = swiftDeposit;
            BlockchainDeposit = blockchainDeposit;
            SwiftWithdrawal = swiftWithdrawal;
            ForwardWithdrawal = forwardWithdrawa;
            CrosschainWithdrawal = crosschainWithdrawal;
            IsTrusted = isTrusted;
            CanBeBase = canBeBase;
            Id = id;
            Name = name;
            Symbol = symbol;
            DisplayId = displayId;
            CategoryId = categoryId;
            IconUrl = iconUrl;
        }
    }
}

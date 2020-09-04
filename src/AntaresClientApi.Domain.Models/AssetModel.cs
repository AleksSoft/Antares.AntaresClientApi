namespace AntaresClientApi.Domain.Models
{
    public class AssetModel
    {
        public string TenantId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string DisplayId { get; set; }
        public int Accuracy { get; set; }
        public bool KycNeeded { get; set; }
        public string CategoryId { get; set; }
        public bool CardDeposit { get; set; }
        public bool SwiftDeposit { get; set; }
        public bool BlockchainDeposit { get; set; }
        public bool SwiftWithdrawal { get; set; }
        public bool CanBeBase { get; set; }
        public string IconUrl { get; set; }

    }
}

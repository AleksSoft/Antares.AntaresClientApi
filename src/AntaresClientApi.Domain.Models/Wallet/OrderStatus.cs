namespace AntaresClientApi.Domain.Models.Wallet
{
    public enum OrderStatus
    {
        Unknown = 0,

        Placed = 1,

        PartiallyMatched = 2,

        Matched = 3,

        Pending = 4,

        Cancelled = 5,

        Replaced = 6,

        Rejected = 7,

        Executed = 8
    }
}

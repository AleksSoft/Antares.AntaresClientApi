using System;

namespace AntaresClientApi.Domain.Models.Wallet
{
    public interface IClientOrder
    {
        long Id { get; }

        string BrokerId { get; }

        string ExternalId { get; }

        long AccountId { get; }

        long WalletId { get; }

        string AssetPairId { get; }

        OrderType OrderType { get; }

        OrderSide Side { get; }

        string Volume { get; }

        string RemainingVolume { get; }

        string Price { get; }

        OrderStatus Status { get; }

        string RejectReason { get; }

        DateTime StatusDate { get; }

        DateTime CreatedAt { get; }

        DateTime RegisteredAt { get; }

        DateTime? LastMatchTime { get; }

        string LowerLimitPrice { get; }

        string LowerPrice { get; }

        string UpperLimitPrice { get; }

        string UpperPrice { get; }

        OrderTimeInForce TimeInForce { get; }

        string ExpiryTime { get; }
    }
}

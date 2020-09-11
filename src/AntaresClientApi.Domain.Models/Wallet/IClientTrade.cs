using System;

namespace AntaresClientApi.Domain.Models.Wallet
{
    public interface IClientTrade
    {
        long Id { get; }

        string BrokerId { get; }

        string ExternalOrderId { get; }

        long AccountId { get; }

        long WalletId { get; }

        long OrderId { get; }

        long OrderHistoryId { get; }

        string TradeId { get; }

        string BaseAssetId { get; }

        string BaseVolume { get; }

        string Price { get; }

        string OppositeOrderId { get; }

        string OppositeExternalOrderId { get; }

        string OppositeWalletId { get; }

        string QuotingAssetId { get; }

        string QuotingVolume { get; }

        string AbsoluteSpread { get; }

        string RelativeSpread { get; }

        TradeRole Role { get; }

        DateTime Timestamp { get; }

    }
}

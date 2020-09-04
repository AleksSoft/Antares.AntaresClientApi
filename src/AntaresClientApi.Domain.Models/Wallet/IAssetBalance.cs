using System;
using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualBasic;

namespace AntaresClientApi.Domain.Models.Wallet
{
    public interface IAssetBalance
    {
        string AssetId { get; }

        decimal Balance { get; }

        decimal Reserve { get; }

        decimal Available { get; }

        DateTime Timestamp { get; }
    }
}

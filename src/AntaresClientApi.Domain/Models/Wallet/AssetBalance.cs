using System;
using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualBasic;

namespace AntaresClientApi.Domain.Models.Wallet
{
    public class AssetBalance
    {
        public string AssetId { get; set; }

        public decimal Balance { get; set; }

        public decimal Reserve { get; set; }

        public decimal Available() => Balance - Reserve;

        public Timestamp Timestamp { get; set; }
    }
}

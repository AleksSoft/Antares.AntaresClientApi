using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AntaresClientApi.Domain.Models.Wallet;

namespace AntaresClientApi.Database.MeData.Models
{
    [Table("trades")]
    public class TradeDbEntity: IClientTrade
    {
        [Key]
        [Column("id", TypeName = "bigint")]
        public long Id { get; set; }

        [Required]
        [Column("broker_id", TypeName = "varchar(255)")]
        public string BrokerId { get; set; }

        [Required]
        [Column("external_order_id", TypeName = "varchar(255)")]
        public string ExternalOrderId { get; set; }

        [Required]
        [Column("account_id", TypeName = "bigint")]
        public long AccountId { get; set; }

        [Required]
        [Column("wallet_id", TypeName = "bigint")]
        public long WalletId { get; set; }

        [Required]
        [Column("order_id", TypeName = "bigint")]
        public long OrderId { get; set; }

        [Required]
        [Column("order_history_id", TypeName = "bigint")]
        public long OrderHistoryId { get; set; }

        [Required]
        [Column("trade_id", TypeName = "varchar(255)")]
        public string TradeId { get; set; }

        [Required]
        [Column("base_asset_id", TypeName = "varchar(255)")]
        public string BaseAssetId { get; set; }

        [Required]
        [Column("base_volume", TypeName = "varchar(255)")]
        public string BaseVolume { get; set; }

        [Required]
        [Column("price", TypeName = "varchar(255)")]
        public string Price { get; set; }

        [Required]
        [Column("opposite_order_id", TypeName = "varchar(255)")]
        public string OppositeOrderId { get; set; }

        [Required]
        [Column("opposite_external_order_id", TypeName = "varchar(255)")]
        public string OppositeExternalOrderId { get; set; }

        [Required]
        [Column("opposite_wallet_id", TypeName = "varchar(255)")]
        public string OppositeWalletId { get; set; }

        [Required]
        [Column("quoting_asset_id", TypeName = "varchar(255)")]
        public string QuotingAssetId { get; set; }

        [Required]
        [Column("quoting_volume", TypeName = "varchar(255)")]
        public string QuotingVolume { get; set; }

        [Required]
        [Column("absolute_spread", TypeName = "varchar(255)")]
        public string AbsoluteSpread { get; set; }

        [Required]
        [Column("relative_spread", TypeName = "varchar(255)")]
        public string RelativeSpread { get; set; }

        [Required]
        [Column("role", TypeName = "int2")]
        public TradeRole Role { get; set; }

        [Required]
        [Column("timestamp", TypeName = "timestamp")]
        public DateTime Timestamp { get; set; }
    }
}

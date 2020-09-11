using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AntaresClientApi.Domain.Models.Wallet;

namespace AntaresClientApi.Database.MeData.Models
{
    [Table("orders")]
    public class OrderDbEntity : IClientOrder
    {
        [Key]
        [Column("id", TypeName = "bigint")]
        public long Id { get; set; }

        [Required]
        [Column("broker_id", TypeName = "varchar(255)")]
        public string BrokerId { get; set; }

        [Required]
        [Column("external_id", TypeName = "varchar(255)")]
        public string ExternalId { get; set; }

        [Required]
        [Column("account_id", TypeName = "bigint")]
        public long AccountId { get; set; }

        [Required]
        [Column("wallet_id", TypeName = "bigint")]
        public long WalletId { get; set; }

        [Required]
        [Column("asset_pair_id", TypeName = "varchar(255)")]
        public string AssetPairId { get; set; }

        [Required]
        [Column("order_type", TypeName = "int2")]
        public OrderType OrderType { get; set; }

        [Required]
        [Column("side", TypeName = "int2")]
        public OrderSide Side { get; set; }

        [Required]
        [Column("volume", TypeName = "varchar(255)")]
        public string Volume { get; set; }

        [Column("remaining_volume", TypeName = "varchar(255)")]
        public string RemainingVolume { get; set; }

        [Column("price", TypeName = "varchar(255)")]
        public string Price { get; set; }

        [Required]
        [Column("status", TypeName = "int2")]
        public OrderStatus Status { get; set; }

        [Column("reject_reason", TypeName = "varchar(255)")]
        public string RejectReason { get; set; }

        [Required]
        [Column("status_date", TypeName = "timestamp")]
        public DateTime StatusDate { get; set; }

        [Required]
        [Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("registered_at", TypeName = "timestamp")]
        public DateTime RegisteredAt { get; set; }

        [Column("last_match_time", TypeName = "timestamp")]
        public DateTime? LastMatchTime { get; set; }

        [Column("lower_limit_price", TypeName = "varchar(255)")]
        public string LowerLimitPrice { get; set; }

        [Column("lower_price", TypeName = "varchar(255)")]
        public string LowerPrice { get; set; }

        [Column("upper_limit_price", TypeName = "varchar(255)")]
        public string UpperLimitPrice { get; set; }

        [Column("upper_price", TypeName = "varchar(255)")]
        public string UpperPrice { get; set; }

        [Required]
        [Column("time_in_force", TypeName = "int2")]
        public OrderTimeInForce TimeInForce { get; set; }

        [Column("expiry_time", TypeName = "varchar(255)")]
        public string ExpiryTime { get; set; }
    }
}

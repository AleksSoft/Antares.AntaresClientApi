using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AntaresClientApi.Database.CandleData.Models
{
    [Table("candles")]
    public class CandleEntity
    {
        [Column("asset_pair_id", TypeName = "varchar(36)")]
        public string AssetPairId { get; set; }

        [Column("time")]
        public DateTime Time { get; set; }

        [Column("type")]
        public CandleType Type { get; set; }

        [Column("open")]
        public double Open { get; set; }

        [Column("close")]
        public double Close { get; set; }

        [Column("high")]
        public double High { get; set; }

        [Column("low")]
        public double Low { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 自由市場訂單交易頁面表
    /// </summary>
    [Table("player_market_order_tradepage")]
    public class PlayerMarketOrderTradepage
    {
        /// <summary>
        /// 交易頁面ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long tradepage_id { get; set; }

        /// <summary>
        /// 訂單ID（外鍵參考 player_market_order_info.order_id）
        /// </summary>
        [Required]
        public long order_id { get; set; }

        /// <summary>
        /// 交易狀態（pending/completed/cancelled）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string trade_status { get; set; } = "pending";

        /// <summary>
        /// 交易開始時間
        /// </summary>
        public DateTime trade_start_time { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 交易完成時間
        /// </summary>
        public DateTime? trade_complete_time { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? updated_at { get; set; }

        // 導航屬性
        /// <summary>
        /// 訂單
        /// </summary>
        [ForeignKey("order_id")]
        public virtual PlayerMarketOrderInfo Order { get; set; } = null!;
    }
}
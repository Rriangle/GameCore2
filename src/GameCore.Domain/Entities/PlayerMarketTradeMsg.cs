using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 自由市場交易訊息表
    /// </summary>
    [Table("player_market_trade_msg")]
    public class PlayerMarketTradeMsg
    {
        /// <summary>
        /// 訊息ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long msg_id { get; set; }

        /// <summary>
        /// 訂單ID（外鍵參考 player_market_order_info.order_id）
        /// </summary>
        [Required]
        public long order_id { get; set; }

        /// <summary>
        /// 發送者ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int sender_id { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        [Required]
        public string message_content { get; set; } = string.Empty;

        /// <summary>
        /// 訊息類型（text/image/system）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string message_type { get; set; } = "text";

        /// <summary>
        /// 發送時間
        /// </summary>
        public DateTime sent_at { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 訂單
        /// </summary>
        [ForeignKey("order_id")]
        public virtual PlayerMarketOrderInfo Order { get; set; } = null!;

        /// <summary>
        /// 發送者
        /// </summary>
        [ForeignKey("sender_id")]
        public virtual User Sender { get; set; } = null!;
    }
}
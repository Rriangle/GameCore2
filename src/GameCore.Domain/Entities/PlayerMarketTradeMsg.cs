using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 自由市場交易訊息實體 - 交易頁面的買賣雙方對話記錄
/// 對應資料表: PlayerMarketTradeMsg
/// </summary>
[Table("PlayerMarketTradeMsg")]
public class PlayerMarketTradeMsg
{
    /// <summary>
    /// 交易訊息編號 (主鍵)
    /// </summary>
    [Key]
    [Column("trade_msg_id")]
    public int TradeMsgId { get; set; }

    /// <summary>
    /// 交易頁面編號 (外鍵至 PlayerMarketOrderTradepage)
    /// </summary>
    [Required]
    [Column("p_order_tradepage_id")]
    public int POrderTradepageId { get; set; }

    /// <summary>
    /// 訊息發送者角色 (seller/buyer)
    /// </summary>
    [Required]
    [Column("msg_from")]
    [StringLength(20)]
    public string MsgFrom { get; set; } = string.Empty;

    /// <summary>
    /// 訊息發送者用戶編號 (外鍵至 Users)
    /// </summary>
    [Required]
    [Column("sender_user_id")]
    public int SenderUserId { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    [Required]
    [Column("message_text")]
    [StringLength(1000)]
    public string MessageText { get; set; } = string.Empty;

    /// <summary>
    /// 訊息類型 (text/system/image)
    /// </summary>
    [Required]
    [Column("message_type")]
    [StringLength(20)]
    public string MessageType { get; set; } = "text";

    /// <summary>
    /// 訊息狀態 (sent/read/deleted)
    /// </summary>
    [Required]
    [Column("message_status")]
    [StringLength(20)]
    public string MessageStatus { get; set; } = "sent";

    /// <summary>
    /// 傳訊時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 訊息讀取時間
    /// </summary>
    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 是否已刪除
    /// </summary>
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 附件資料 (圖片等，二進位格式)
    /// </summary>
    [Column("attachment_data")]
    public byte[]? AttachmentData { get; set; }

    /// <summary>
    /// 附件檔名
    /// </summary>
    [Column("attachment_filename")]
    [StringLength(255)]
    public string? AttachmentFilename { get; set; }

    /// <summary>
    /// 導航屬性 - 交易頁面
    /// </summary>
    [ForeignKey("POrderTradepageId")]
    public virtual PlayerMarketOrderTradepage TradePage { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 訊息發送者
    /// </summary>
    [ForeignKey("SenderUserId")]
    public virtual User Sender { get; set; } = null!;
}
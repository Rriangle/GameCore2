using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 自由市場交易頁面實體 - 買賣雙方確認交易的頁面
/// 對應資料表: PlayerMarketOrderTradepage (POTP)
/// </summary>
[Table("PlayerMarketOrderTradepage")]
public class PlayerMarketOrderTradepage
{
    /// <summary>
    /// 交易頁面編號 (主鍵)
    /// </summary>
    [Key]
    [Column("p_order_tradepage_id")]
    public int POrderTradepageId { get; set; }

    /// <summary>
    /// 自由市場訂單編號 (外鍵至 PlayerMarketOrderInfo)
    /// </summary>
    [Required]
    [Column("p_order_id")]
    public int POrderId { get; set; }

    /// <summary>
    /// 自由市場商品編號 (外鍵至 PlayerMarketProductInfo)
    /// </summary>
    [Required]
    [Column("p_product_id")]
    public int PProductId { get; set; }

    /// <summary>
    /// 平台抽成金額
    /// </summary>
    [Required]
    [Column("p_order_platform_fee", TypeName = "decimal(18,2)")]
    public decimal POrderPlatformFee { get; set; }

    /// <summary>
    /// 賣家移交時間 (賣家確認已轉移道具給買家的時間)
    /// </summary>
    [Column("seller_transferred_at")]
    public DateTime? SellerTransferredAt { get; set; }

    /// <summary>
    /// 買家接收時間 (買家確認已收到道具的時間)
    /// </summary>
    [Column("buyer_received_at")]
    public DateTime? BuyerReceivedAt { get; set; }

    /// <summary>
    /// 交易完成時間 (雙方都確認後自動設定)
    /// </summary>
    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 交易頁面狀態 (active/completed/disputed/cancelled)
    /// </summary>
    [Required]
    [Column("status")]
    [StringLength(50)]
    public string Status { get; set; } = "active";

    /// <summary>
    /// 交易頁面創建時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 交易頁面更新時間
    /// </summary>
    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 賣家確認說明
    /// </summary>
    [Column("seller_notes")]
    [StringLength(500)]
    public string? SellerNotes { get; set; }

    /// <summary>
    /// 買家確認說明
    /// </summary>
    [Column("buyer_notes")]
    [StringLength(500)]
    public string? BuyerNotes { get; set; }

    /// <summary>
    /// 導航屬性 - 訂單
    /// </summary>
    [ForeignKey("POrderId")]
    public virtual PlayerMarketOrderInfo Order { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 商品
    /// </summary>
    [ForeignKey("PProductId")]
    public virtual PlayerMarketProductInfo Product { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 交易對話記錄
    /// </summary>
    public virtual ICollection<PlayerMarketTradeMsg> Messages { get; set; } = new List<PlayerMarketTradeMsg>();
}
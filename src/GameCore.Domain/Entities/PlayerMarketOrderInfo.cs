using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 自由市場訂單實體 - C2C交易的訂單資訊
/// 對應資料表: PlayerMarketOrderInfo (PO)
/// </summary>
[Table("PlayerMarketOrderInfo")]
public class PlayerMarketOrderInfo
{
    /// <summary>
    /// 自由市場訂單編號 (主鍵)
    /// </summary>
    [Key]
    [Column("p_order_id")]
    public int POrderId { get; set; }

    /// <summary>
    /// 自由市場商品編號 (外鍵至 PlayerMarketProductInfo)
    /// </summary>
    [Required]
    [Column("p_product_id")]
    public int PProductId { get; set; }

    /// <summary>
    /// 賣家編號 (外鍵至 Users)
    /// </summary>
    [Required]
    [Column("seller_id")]
    public int SellerId { get; set; }

    /// <summary>
    /// 買家編號 (外鍵至 Users)
    /// </summary>
    [Required]
    [Column("buyer_id")]
    public int BuyerId { get; set; }

    /// <summary>
    /// 自由市場訂單狀態 (pending/confirmed/completed/cancelled)
    /// </summary>
    [Required]
    [Column("p_order_status")]
    [StringLength(50)]
    public string POrderStatus { get; set; } = "pending";

    /// <summary>
    /// 自由市場付款狀態 (pending/paid/failed)
    /// </summary>
    [Required]
    [Column("p_payment_status")]
    [StringLength(50)]
    public string PPaymentStatus { get; set; } = "pending";

    /// <summary>
    /// 自由市場訂單數量
    /// </summary>
    [Required]
    [Column("p_order_quantity")]
    public int POrderQuantity { get; set; }

    /// <summary>
    /// 自由市場訂單單價
    /// </summary>
    [Required]
    [Column("p_order_unit_price", TypeName = "decimal(18,2)")]
    public decimal POrderUnitPrice { get; set; }

    /// <summary>
    /// 自由市場訂單總額
    /// </summary>
    [Required]
    [Column("p_order_total_amount", TypeName = "decimal(18,2)")]
    public decimal POrderTotalAmount { get; set; }

    /// <summary>
    /// 自由市場訂單創建時間
    /// </summary>
    [Required]
    [Column("p_order_created_at")]
    public DateTime POrderCreatedAt { get; set; }

    /// <summary>
    /// 自由市場訂單更新時間
    /// </summary>
    [Required]
    [Column("p_order_updated_at")]
    public DateTime POrderUpdatedAt { get; set; }

    /// <summary>
    /// 付款完成時間
    /// </summary>
    [Column("payment_completed_at")]
    public DateTime? PaymentCompletedAt { get; set; }

    /// <summary>
    /// 訂單完成時間
    /// </summary>
    [Column("order_completed_at")]
    public DateTime? OrderCompletedAt { get; set; }

    /// <summary>
    /// 導航屬性 - 商品
    /// </summary>
    [ForeignKey("PProductId")]
    public virtual PlayerMarketProductInfo Product { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 賣家
    /// </summary>
    [ForeignKey("SellerId")]
    public virtual User Seller { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 買家
    /// </summary>
    [ForeignKey("BuyerId")]
    public virtual User Buyer { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 交易頁面
    /// </summary>
    public virtual PlayerMarketOrderTradepage? TradePage { get; set; }
}
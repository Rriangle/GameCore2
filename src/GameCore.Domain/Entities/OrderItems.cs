using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 訂單詳細實體 - 訂單中的商品明細
/// 對應資料表: OrderItems (OD)
/// </summary>
[Table("OrderItems")]
public class OrderItems
{
    /// <summary>
    /// 訂單詳細編號 (主鍵)
    /// </summary>
    [Key]
    [Column("item_id")]
    public int ItemId { get; set; }

    /// <summary>
    /// 訂單編號 (外鍵至 OrderInfo)
    /// </summary>
    [Required]
    [Column("order_id")]
    public int OrderId { get; set; }

    /// <summary>
    /// 商品編號 (外鍵至 ProductInfo)
    /// </summary>
    [Required]
    [Column("product_id")]
    public int ProductId { get; set; }

    /// <summary>
    /// 實際物品編號 (1,2,3...)
    /// </summary>
    [Required]
    [Column("line_no")]
    public int LineNo { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    [Required]
    [Column("unit_price", TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 下單數量
    /// </summary>
    [Required]
    [Column("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// 小計 (單價 × 數量)
    /// </summary>
    [Required]
    [Column("subtotal", TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    /// <summary>
    /// 導航屬性 - 所屬訂單
    /// </summary>
    [ForeignKey("OrderId")]
    public virtual OrderInfo Order { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 訂購的商品
    /// </summary>
    [ForeignKey("ProductId")]
    public virtual ProductInfo Product { get; set; } = null!;
}
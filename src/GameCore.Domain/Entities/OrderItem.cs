using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 訂單詳細表
/// 對應資料庫 OrderItems 表
/// </summary>
[Table("OrderItems")]
public class OrderItem
{
    /// <summary>
    /// 訂單詳細ID (主鍵)
    /// </summary>
    [Key]
    public int ItemID { get; set; }

    /// <summary>
    /// 訂單ID (指向訂單)
    /// </summary>
    public int OrderID { get; set; }

    /// <summary>
    /// 商品ID (指向商品)
    /// </summary>
    public int ProductID { get; set; }

    /// <summary>
    /// 實際物品編號 1.2.3...
    /// </summary>
    public int LineNo { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 下單數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 小計
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    /// <summary>
    /// 總價 (別名，用於向後兼容)
    /// </summary>
    [NotMapped]
    public decimal TotalPrice => Subtotal;

    // 導航屬性
    /// <summary>
    /// 所屬訂單
    /// </summary>
    public virtual OrderInfo OrderInfo { get; set; } = null!;

    /// <summary>
    /// 商品資訊
    /// </summary>
    public virtual ProductInfo ProductInfo { get; set; } = null!;
} 
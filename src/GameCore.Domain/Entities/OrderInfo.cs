using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 訂單資訊表
/// 對應資料庫 OrderInfo 表
/// </summary>
[Table("OrderInfo")]
public class OrderInfo
{
    /// <summary>
    /// 訂單ID (主鍵)
    /// </summary>
    [Key]
    public int OrderID { get; set; }

    /// <summary>
    /// 下訂會員ID
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// 下單日期
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 訂單狀態 (未付款-為出貨-已出貨-已完成)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string OrderStatus { get; set; } = "Created";

    /// <summary>
    /// 訂單狀態 (別名，用於向後兼容)
    /// </summary>
    [NotMapped]
    public string Status => OrderStatus;

    /// <summary>
    /// 付款狀態 (下單-待付款-已付款)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PaymentStatus { get; set; } = "Placed";

    /// <summary>
    /// 訂單總額
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal OrderTotal { get; set; }

    /// <summary>
    /// 訂單總額 (別名，用於向後兼容)
    /// </summary>
    [NotMapped]
    public decimal TotalAmount => OrderTotal;

    /// <summary>
    /// 付款時間
    /// </summary>
    public DateTime? PaymentAt { get; set; }

    /// <summary>
    /// 出貨時間
    /// </summary>
    public DateTime? ShippedAt { get; set; }

    /// <summary>
    /// 完成時間
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 創建時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 導航屬性
    /// <summary>
    /// 下訂會員
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// 訂單項目
    /// </summary>
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
} 
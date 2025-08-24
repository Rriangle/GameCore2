using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 訂單資訊實體 - 官方商城訂單主檔
/// 對應資料表: OrderInfo (O)
/// </summary>
[Table("OrderInfo")]
public class OrderInfo
{
    /// <summary>
    /// 訂單編號 (主鍵)
    /// </summary>
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    /// <summary>
    /// 下訂會員編號 (外鍵至 Users)
    /// </summary>
    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// 下單日期
    /// </summary>
    [Required]
    [Column("order_date")]
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 訂單狀態 (Created/ToShip/Shipped/Completed/Cancelled)
    /// </summary>
    [Required]
    [Column("order_status")]
    [StringLength(50)]
    public string OrderStatus { get; set; } = "Created";

    /// <summary>
    /// 付款狀態 (Placed/Pending/Paid)
    /// </summary>
    [Required]
    [Column("payment_status")]
    [StringLength(50)]
    public string PaymentStatus { get; set; } = "Placed";

    /// <summary>
    /// 訂單總額
    /// </summary>
    [Required]
    [Column("order_total", TypeName = "decimal(18,2)")]
    public decimal OrderTotal { get; set; }

    /// <summary>
    /// 付款時間
    /// </summary>
    [Column("payment_at")]
    public DateTime? PaymentAt { get; set; }

    /// <summary>
    /// 出貨時間
    /// </summary>
    [Column("shipped_at")]
    public DateTime? ShippedAt { get; set; }

    /// <summary>
    /// 完成時間
    /// </summary>
    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 導航屬性 - 下訂的會員
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 此訂單的商品明細
    /// </summary>
    public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
}
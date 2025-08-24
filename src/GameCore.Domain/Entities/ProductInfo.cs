using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 商品資訊實體 - 官方商城商品主檔
/// 對應資料表: ProductInfo (PI)
/// </summary>
[Table("ProductInfo")]
public class ProductInfo
{
    /// <summary>
    /// 商品編號 (主鍵)
    /// </summary>
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    [Required]
    [Column("product_name")]
    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品類型 (遊戲/其他)
    /// </summary>
    [Required]
    [Column("product_type")]
    [StringLength(50)]
    public string ProductType { get; set; } = string.Empty;

    /// <summary>
    /// 售價
    /// </summary>
    [Required]
    [Column("price", TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// 使用幣別
    /// </summary>
    [Required]
    [Column("currency_code")]
    [StringLength(10)]
    public string CurrencyCode { get; set; } = "TWD";

    /// <summary>
    /// 庫存數量
    /// </summary>
    [Required]
    [Column("Shipment_Quantity")]
    public int ShipmentQuantity { get; set; }

    /// <summary>
    /// 創建者
    /// </summary>
    [Column("product_created_by")]
    [StringLength(100)]
    public string? ProductCreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Column("product_created_at")]
    public DateTime? ProductCreatedAt { get; set; }

    /// <summary>
    /// 最後修改者
    /// </summary>
    [Column("product_updated_by")]
    [StringLength(100)]
    public string? ProductUpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    [Column("product_updated_at")]
    public DateTime? ProductUpdatedAt { get; set; }

    /// <summary>
    /// 會員編號 (可能用於關聯創建者)
    /// </summary>
    [Column("user_id")]
    public int? UserId { get; set; }

    /// <summary>
    /// 導航屬性 - 創建此商品的會員
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User? Creator { get; set; }

    /// <summary>
    /// 導航屬性 - 遊戲商品詳細資訊 (一對一)
    /// </summary>
    public virtual GameProductDetails? GameProductDetails { get; set; }

    /// <summary>
    /// 導航屬性 - 其他商品詳細資訊 (一對一)
    /// </summary>
    public virtual OtherProductDetails? OtherProductDetails { get; set; }

    /// <summary>
    /// 導航屬性 - 此商品的訂單明細
    /// </summary>
    public virtual ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();

    /// <summary>
    /// 導航屬性 - 此商品的修改日誌
    /// </summary>
    public virtual ICollection<ProductInfoAuditLog> AuditLogs { get; set; } = new List<ProductInfoAuditLog>();

    /// <summary>
    /// 導航屬性 - 此商品的排行榜記錄
    /// </summary>
    public virtual ICollection<OfficialStoreRanking> Rankings { get; set; } = new List<OfficialStoreRanking>();
}
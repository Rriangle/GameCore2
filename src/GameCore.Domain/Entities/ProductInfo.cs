using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 商品資訊表
/// 對應資料庫 ProductInfo 表
/// </summary>
[Table("ProductInfo")]
public class ProductInfo
{
    /// <summary>
    /// 商品ID (主鍵)
    /// </summary>
    [Key]
    public int ProductID { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    [Required]
    [StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品名稱 (別名，用於向後兼容)
    /// </summary>
    [NotMapped]
    public string Product_Name => ProductName;

    /// <summary>
    /// 商品類型
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ProductType { get; set; } = string.Empty;

    /// <summary>
    /// 商品類型 (別名，用於向後兼容)
    /// </summary>
    [NotMapped]
    public string Category => ProductType;

    /// <summary>
    /// 商品描述
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// 售價
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// 使用幣別
    /// </summary>
    [StringLength(10)]
    public string? CurrencyCode { get; set; }

    /// <summary>
    /// 庫存數量
    /// </summary>
    public int ShipmentQuantity { get; set; }

    /// <summary>
    /// 庫存數量 (別名，用於向後兼容)
    /// </summary>
    [NotMapped]
    public int Stock_Quantity => ShipmentQuantity;

    /// <summary>
    /// 創建者
    /// </summary>
    [StringLength(100)]
    public string? ProductCreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime ProductCreatedAt { get; set; }

    /// <summary>
    /// 建立時間 (別名，用於向後兼容)
    /// </summary>
    [NotMapped]
    public DateTime Created_At => ProductCreatedAt;

    /// <summary>
    /// 最後修改者
    /// </summary>
    [StringLength(100)]
    public string? ProductUpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime ProductUpdatedAt { get; set; }

    /// <summary>
    /// 更新時間 (別名，用於向後兼容)
    /// </summary>
    [NotMapped]
    public DateTime Updated_At => ProductUpdatedAt;

    /// <summary>
    /// 會員ID
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// 供應商ID
    /// </summary>
    public int? SupplierID { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool Is_Active { get; set; } = true;

    // 導航屬性
    /// <summary>
    /// 所屬會員
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// 遊戲商品詳細資料
    /// </summary>
    public virtual GameProductDetails? GameProductDetails { get; set; }

    /// <summary>
    /// 其他商品詳細資料
    /// </summary>
    public virtual OtherProductDetails? OtherProductDetails { get; set; }

    /// <summary>
    /// 訂單項目
    /// </summary>
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    /// <summary>
    /// 官方商城排行榜
    /// </summary>
    public virtual ICollection<OfficialStoreRanking> OfficialStoreRankings { get; set; } = new List<OfficialStoreRanking>();

    /// <summary>
    /// 商品修改日誌
    /// </summary>
    public virtual ICollection<ProductInfoAuditLog> AuditLogs { get; set; } = new List<ProductInfoAuditLog>();
} 
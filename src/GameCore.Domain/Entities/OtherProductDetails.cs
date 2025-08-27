using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 非遊戲主檔商品資訊實體 - 非遊戲類商品的詳細資訊
/// 對應資料表: OtherProductDetails (OPD)
/// </summary>
[Table("OtherProductDetails")]
public class OtherProductDetails
{
    /// <summary>
    /// 商品編號 (主鍵，外鍵至 ProductInfo)
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
    /// 商品描述
    /// </summary>
    [Column("product_description")]
    [StringLength(1000)]
    public string? ProductDescription { get; set; }

    /// <summary>
    /// 廠商編號 (外鍵至 Supplier)
    /// </summary>
    [Required]
    [Column("supplier_id")]
    public int SupplierId { get; set; }

    /// <summary>
    /// 遊戲平台編號
    /// </summary>
    [Column("platform_id")]
    public int? PlatformId { get; set; }

    /// <summary>
    /// 數位序號兌換碼
    /// </summary>
    [Column("digital_code")]
    [StringLength(100)]
    public string? DigitalCode { get; set; }

    /// <summary>
    /// 尺寸
    /// </summary>
    [Column("size")]
    [StringLength(50)]
    public string? Size { get; set; }

    /// <summary>
    /// 顏色
    /// </summary>
    [Column("color")]
    [StringLength(50)]
    public string? Color { get; set; }

    /// <summary>
    /// 重量
    /// </summary>
    [Column("weight")]
    [StringLength(50)]
    public string? Weight { get; set; }

    /// <summary>
    /// 尺寸規格
    /// </summary>
    [Column("dimensions")]
    [StringLength(100)]
    public string? Dimensions { get; set; }

    /// <summary>
    /// 材質
    /// </summary>
    [Column("material")]
    [StringLength(100)]
    public string? Material { get; set; }

    /// <summary>
    /// 庫存數量
    /// </summary>
    [Column("stock_quantity")]
    [StringLength(50)]
    public string? StockQuantity { get; set; }

    /// <summary>
    /// 導航屬性 - 商品主檔 (一對一)
    /// </summary>
    [ForeignKey("ProductId")]
    public virtual ProductInfo ProductInfo { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 供應商
    /// </summary>
    [ForeignKey("SupplierId")]
    public virtual Supplier Supplier { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 非遊戲主檔商品資訊表
/// 對應資料庫 OtherProductDetails 表
/// </summary>
[Table("OtherProductDetails")]
public class OtherProductDetails
{
    /// <summary>
    /// 商品ID (主鍵，關聯 ProductInfo)
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
    /// 商品描述
    /// </summary>
    public string? ProductDescription { get; set; }

    /// <summary>
    /// 廠商ID
    /// </summary>
    public int SupplierID { get; set; }

    /// <summary>
    /// 遊戲平台ID
    /// </summary>
    public int PlatformID { get; set; }

    /// <summary>
    /// 數位序號兌換碼
    /// </summary>
    [StringLength(100)]
    public string? DigitalCode { get; set; }

    /// <summary>
    /// 尺寸
    /// </summary>
    [StringLength(50)]
    public string? Size { get; set; }

    /// <summary>
    /// 顏色
    /// </summary>
    [StringLength(50)]
    public string? Color { get; set; }

    /// <summary>
    /// 重量
    /// </summary>
    [StringLength(50)]
    public string? Weight { get; set; }

    /// <summary>
    /// 尺寸規格
    /// </summary>
    [StringLength(100)]
    public string? Dimensions { get; set; }

    /// <summary>
    /// 材質
    /// </summary>
    [StringLength(100)]
    public string? Material { get; set; }

    /// <summary>
    /// 庫存數量
    /// </summary>
    [StringLength(50)]
    public string? StockQuantity { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的商品資訊
    /// </summary>
    public virtual ProductInfo ProductInfo { get; set; } = null!;

    /// <summary>
    /// 關聯的供應商
    /// </summary>
    public virtual Supplier Supplier { get; set; } = null!;
} 
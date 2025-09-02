using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 遊戲主檔商品資訊表
/// 對應資料庫 GameProductDetails 表
/// </summary>
[Table("GameProductDetails")]
public class GameProductDetails
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
    /// 遊戲ID
    /// </summary>
    public int GameID { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    [StringLength(200)]
    public string? GameName { get; set; }

    /// <summary>
    /// 下載連結
    /// </summary>
    [StringLength(500)]
    public string? DownloadLink { get; set; }

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
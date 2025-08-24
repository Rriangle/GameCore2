using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 遊戲主檔商品資訊實體 - 遊戲類商品的詳細資訊
/// 對應資料表: GameProductDetails (GPD)
/// </summary>
[Table("GameProductDetails")]
public class GameProductDetails
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
    /// 遊戲編號
    /// </summary>
    [Column("game_id")]
    public int? GameId { get; set; }

    /// <summary>
    /// 遊戲名稱
    /// </summary>
    [Column("game_name")]
    [StringLength(200)]
    public string? GameName { get; set; }

    /// <summary>
    /// 下載連結
    /// </summary>
    [Column("download_link")]
    [StringLength(500)]
    public string? DownloadLink { get; set; }

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
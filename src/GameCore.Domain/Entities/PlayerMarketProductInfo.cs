using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 自由市場商品資訊表
/// 對應資料庫 PlayerMarketProductInfo 表
/// </summary>
[Table("PlayerMarketProductInfo")]
public class PlayerMarketProductInfo
{
    /// <summary>
    /// 自由市場商品ID (主鍵)
    /// </summary>
    [Key]
    public int PProductID { get; set; }

    /// <summary>
    /// 商品類型
    /// </summary>
    [Required]
    [StringLength(50)]
    public string PProductType { get; set; } = string.Empty;

    /// <summary>
    /// 商品標題 (噱頭標語)
    /// </summary>
    [Required]
    [StringLength(200)]
    public string PProductTitle { get; set; } = string.Empty;

    /// <summary>
    /// 商品名稱
    /// </summary>
    [Required]
    [StringLength(200)]
    public string PProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品描述
    /// </summary>
    public string? PProductDescription { get; set; }

    /// <summary>
    /// 關聯的商品ID
    /// </summary>
    public int? ProductID { get; set; }

    /// <summary>
    /// 賣家ID
    /// </summary>
    public int SellerID { get; set; }

    /// <summary>
    /// 商品狀態
    /// </summary>
    [Required]
    [StringLength(20)]
    public string PStatus { get; set; } = "Draft";

    /// <summary>
    /// 售價
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    /// <summary>
    /// 商品圖片ID
    /// </summary>
    [StringLength(100)]
    public string? PProductImgID { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的官方商品
    /// </summary>
    public virtual ProductInfo? ProductInfo { get; set; }

    /// <summary>
    /// 賣家
    /// </summary>
    public virtual User Seller { get; set; } = null!;

    /// <summary>
    /// 商品圖片
    /// </summary>
    public virtual ICollection<PlayerMarketProductImg> ProductImages { get; set; } = new List<PlayerMarketProductImg>();

    /// <summary>
    /// 訂單 (賣家)
    /// </summary>
    public virtual ICollection<PlayerMarketOrderInfo> OrdersAsSeller { get; set; } = new List<PlayerMarketOrderInfo>();

    /// <summary>
    /// 市場排行榜
    /// </summary>
    public virtual ICollection<PlayerMarketRanking> MarketRankings { get; set; } = new List<PlayerMarketRanking>();
} 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 自由市場商品資訊實體 - C2C交易的商品主檔
/// 對應資料表: PlayerMarketProductInfo (PMP)
/// </summary>
[Table("PlayerMarketProductInfo")]
public class PlayerMarketProductInfo
{
    /// <summary>
    /// 自由市場商品編號 (主鍵)
    /// </summary>
    [Key]
    [Column("p_product_id")]
    public int PProductId { get; set; }

    /// <summary>
    /// 自由市場商品類型
    /// </summary>
    [Required]
    [Column("p_product_type")]
    [StringLength(100)]
    public string PProductType { get; set; } = string.Empty;

    /// <summary>
    /// 商品標題 (噱頭標語)
    /// </summary>
    [Required]
    [Column("p_product_title")]
    [StringLength(200)]
    public string PProductTitle { get; set; } = string.Empty;

    /// <summary>
    /// 自由市場商品名稱
    /// </summary>
    [Required]
    [Column("p_product_name")]
    [StringLength(200)]
    public string PProductName { get; set; } = string.Empty;

    /// <summary>
    /// 自由市場商品描述
    /// </summary>
    [Column("p_product_description")]
    [StringLength(2000)]
    public string? PProductDescription { get; set; }

    /// <summary>
    /// 自由市場商品價格
    /// </summary>
    [Required]
    [Column("p_product_price", TypeName = "decimal(18,2)")]
    public decimal PProductPrice { get; set; }

    /// <summary>
    /// 自由市場商品數量
    /// </summary>
    [Required]
    [Column("p_product_quantity")]
    public int PProductQuantity { get; set; }

    /// <summary>
    /// 自由市場商品狀態 (available/sold/hidden)
    /// </summary>
    [Required]
    [Column("p_status")]
    [StringLength(50)]
    public string PStatus { get; set; } = "available";

    /// <summary>
    /// 賣家編號 (外鍵至 Users)
    /// </summary>
    [Required]
    [Column("seller_id")]
    public int SellerId { get; set; }

    /// <summary>
    /// 關聯官方商品編號 (可選，外鍵至 ProductInfo)
    /// </summary>
    [Column("product_id")]
    public int? ProductId { get; set; }

    /// <summary>
    /// 自由市場商品圖片編號
    /// </summary>
    [Column("p_product_img_id")]
    [StringLength(100)]
    public string? PProductImgId { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    [Required]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 導航屬性 - 賣家
    /// </summary>
    [ForeignKey("SellerId")]
    public virtual User Seller { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 關聯的官方商品 (可選)
    /// </summary>
    [ForeignKey("ProductId")]
    public virtual ProductInfo? Product { get; set; }

    /// <summary>
    /// 導航屬性 - 商品圖片
    /// </summary>
    public virtual ICollection<PlayerMarketProductImgs> Images { get; set; } = new List<PlayerMarketProductImgs>();

    /// <summary>
    /// 導航屬性 - 此商品的訂單
    /// </summary>
    public virtual ICollection<PlayerMarketOrderInfo> Orders { get; set; } = new List<PlayerMarketOrderInfo>();

    /// <summary>
    /// 導航屬性 - 此商品的排行榜記錄
    /// </summary>
    public virtual ICollection<PlayerMarketRanking> Rankings { get; set; } = new List<PlayerMarketRanking>();
}
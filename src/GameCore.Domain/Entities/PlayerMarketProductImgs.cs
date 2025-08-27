using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 自由市場商品圖片實體 - 存儲C2C商品的圖片
/// 對應資料表: PlayerMarketProductImgs
/// </summary>
[Table("PlayerMarketProductImgs")]
public class PlayerMarketProductImgs
{
    /// <summary>
    /// 自由市場商品圖片編號 (主鍵)
    /// </summary>
    [Key]
    [Column("p_product_img_id")]
    public int PProductImgId { get; set; }

    /// <summary>
    /// 自由市場商品編號 (外鍵至 PlayerMarketProductInfo)
    /// </summary>
    [Required]
    [Column("p_product_id")]
    public int PProductId { get; set; }

    /// <summary>
    /// 自由市場商品圖片網址 (二進位存放)
    /// </summary>
    [Required]
    [Column("p_product_img_url")]
    public byte[] PProductImgUrl { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// 圖片說明文字
    /// </summary>
    [Column("img_description")]
    [StringLength(200)]
    public string? ImgDescription { get; set; }

    /// <summary>
    /// 圖片順序
    /// </summary>
    [Column("img_order")]
    public int ImgOrder { get; set; } = 0;

    /// <summary>
    /// 是否為主圖片
    /// </summary>
    [Column("is_primary")]
    public bool IsPrimary { get; set; } = false;

    /// <summary>
    /// 上傳時間
    /// </summary>
    [Required]
    [Column("uploaded_at")]
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// 導航屬性 - 所屬商品
    /// </summary>
    [ForeignKey("PProductId")]
    public virtual PlayerMarketProductInfo Product { get; set; } = null!;
}
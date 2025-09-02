using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 自由市場商品圖片表
/// 對應資料庫 PlayerMarketProductImgs 表
/// </summary>
[Table("PlayerMarketProductImgs")]
public class PlayerMarketProductImg
{
    /// <summary>
    /// 自由市場商品圖片ID (主鍵)
    /// </summary>
    [Key]
    public int PProductImgID { get; set; }

    /// <summary>
    /// 指向自由市場商品
    /// </summary>
    public int PProductID { get; set; }

    /// <summary>
    /// 商品圖片網址 (二進位儲存)
    /// </summary>
    public byte[]? PProductImgURL { get; set; }

    // 導航屬性
    /// <summary>
    /// 所屬商品
    /// </summary>
    public virtual PlayerMarketProductInfo Product { get; set; } = null!;
} 
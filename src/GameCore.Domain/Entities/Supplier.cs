using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 供應商實體 - 管理官方商城的商品供應商資訊
/// 對應資料表: Supplier (S)
/// </summary>
[Table("Supplier")]
public class Supplier
{
    /// <summary>
    /// 廠商編號 (主鍵)
    /// </summary>
    [Key]
    [Column("supplier_id")]
    public int SupplierId { get; set; }

    /// <summary>
    /// 廠商名稱
    /// </summary>
    [Required]
    [Column("supplier_name")]
    [StringLength(100)]
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>
    /// 導航屬性 - 此供應商的遊戲商品
    /// </summary>
    public virtual ICollection<GameProductDetails> GameProducts { get; set; } = new List<GameProductDetails>();

    /// <summary>
    /// 導航屬性 - 此供應商的其他商品
    /// </summary>
    public virtual ICollection<OtherProductDetails> OtherProducts { get; set; } = new List<OtherProductDetails>();
}
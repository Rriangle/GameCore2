using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 供應商資料表
/// 對應資料庫 Supplier 表
/// </summary>
[Table("Supplier")]
public class Supplier
{
    /// <summary>
    /// 供應商ID (主鍵)
    /// </summary>
    [Key]
    public int SupplierID { get; set; }

    /// <summary>
    /// 供應商名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    public string SupplierName { get; set; } = string.Empty;

    // 導航屬性
    /// <summary>
    /// 供應商的商品
    /// </summary>
    public virtual ICollection<ProductInfo> Products { get; set; } = new List<ProductInfo>();
} 
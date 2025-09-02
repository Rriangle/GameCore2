using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 商品修改日誌表
/// 對應資料庫 ProductInfoAuditLog 表
/// </summary>
[Table("ProductInfoAuditLog")]
public class ProductInfoAuditLog
{
    /// <summary>
    /// 日誌ID (主鍵)
    /// </summary>
    [Key]
    public long LogID { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public int ProductID { get; set; }

    /// <summary>
    /// 動作類型
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// 修改欄位名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// 舊值
    /// </summary>
    [StringLength(500)]
    public string? OldValue { get; set; }

    /// <summary>
    /// 新值
    /// </summary>
    [StringLength(500)]
    public string? NewValue { get; set; }

    /// <summary>
    /// 操作人ID (管理員)
    /// </summary>
    public int ManagerID { get; set; }

    /// <summary>
    /// 修改時間
    /// </summary>
    public DateTime ChangedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的商品
    /// </summary>
    public virtual ProductInfo ProductInfo { get; set; } = null!;

    /// <summary>
    /// 操作的管理員
    /// </summary>
    public virtual ManagerData Manager { get; set; } = null!;
} 
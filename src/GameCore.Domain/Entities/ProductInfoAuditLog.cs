using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 商品修改日誌實體 - 追蹤商品資訊的異動記錄
/// 對應資料表: ProductInfoAuditLog (PIAL)
/// </summary>
[Table("ProductInfoAuditLog")]
public class ProductInfoAuditLog
{
    /// <summary>
    /// 日誌編號 (主鍵)
    /// </summary>
    [Key]
    [Column("log_id")]
    public long LogId { get; set; }

    /// <summary>
    /// 商品編號 (外鍵至 ProductInfo)
    /// </summary>
    [Required]
    [Column("product_id")]
    public int ProductId { get; set; }

    /// <summary>
    /// 動作類型 (CREATE/UPDATE/DELETE)
    /// </summary>
    [Required]
    [Column("action_type")]
    [StringLength(50)]
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// 修改欄位名稱
    /// </summary>
    [Required]
    [Column("field_name")]
    [StringLength(100)]
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// 舊值
    /// </summary>
    [Column("old_value")]
    [StringLength(500)]
    public string? OldValue { get; set; }

    /// <summary>
    /// 新值
    /// </summary>
    [Column("new_value")]
    [StringLength(500)]
    public string? NewValue { get; set; }

    /// <summary>
    /// 操作管理者編號 (外鍵至 ManagerData)
    /// </summary>
    [Required]
    [Column("Manager_Id")]
    public int ManagerId { get; set; }

    /// <summary>
    /// 修改時間
    /// </summary>
    [Required]
    [Column("changed_at")]
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// 導航屬性 - 被修改的商品
    /// </summary>
    [ForeignKey("ProductId")]
    public virtual ProductInfo Product { get; set; } = null!;

    // 注意：ManagerData 實體還未實作，故暫時不加入導航屬性
    // 在後續的管理員模組實作時會加入
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 管理者角色指派實體 - 管理者與角色的多對多關聯
/// 對應資料表: ManagerRole
/// </summary>
[Table("ManagerRole")]
public class ManagerRole
{
    /// <summary>
    /// 管理者編號 (複合主鍵之一，外鍵至 ManagerData)
    /// </summary>
    [Key]
    [Column("Manager_Id", Order = 0)]
    public int ManagerId { get; set; }

    /// <summary>
    /// 角色編號 (複合主鍵之一，外鍵至 ManagerRolePermission)
    /// </summary>
    [Key]
    [Column("ManagerRole_Id", Order = 1)]
    public int ManagerRoleId { get; set; }

    /// <summary>
    /// 角色名稱
    /// </summary>
    [Required]
    [Column("ManagerRole")]
    [StringLength(100)]
    public string ManagerRoleName { get; set; } = string.Empty;

    /// <summary>
    /// 指派時間
    /// </summary>
    [Column("AssignedAt")]
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 指派者編號
    /// </summary>
    [Column("AssignedBy")]
    public int? AssignedBy { get; set; }

    /// <summary>
    /// 角色狀態 (active/inactive/suspended)
    /// </summary>
    [Column("RoleStatus")]
    [StringLength(20)]
    public string RoleStatus { get; set; } = "active";

    /// <summary>
    /// 角色生效時間
    /// </summary>
    [Column("EffectiveFrom")]
    public DateTime? EffectiveFrom { get; set; }

    /// <summary>
    /// 角色失效時間
    /// </summary>
    [Column("EffectiveTo")]
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// 是否為主要角色
    /// </summary>
    [Column("IsPrimaryRole")]
    public bool IsPrimaryRole { get; set; } = false;

    /// <summary>
    /// 角色優先級 (數值越高優先級越高)
    /// </summary>
    [Column("Priority")]
    public int Priority { get; set; } = 0;

    /// <summary>
    /// 指派原因
    /// </summary>
    [Column("AssignReason")]
    [StringLength(500)]
    public string? AssignReason { get; set; }

    /// <summary>
    /// 最後使用時間
    /// </summary>
    [Column("LastUsedAt")]
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// 使用次數
    /// </summary>
    [Column("UsageCount")]
    public int UsageCount { get; set; } = 0;

    /// <summary>
    /// 撤銷時間
    /// </summary>
    [Column("RevokedAt")]
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// 撤銷者編號
    /// </summary>
    [Column("RevokedBy")]
    public int? RevokedBy { get; set; }

    /// <summary>
    /// 撤銷原因
    /// </summary>
    [Column("RevokeReason")]
    [StringLength(500)]
    public string? RevokeReason { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column("Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 自動撤銷 (到期自動撤銷)
    /// </summary>
    [Column("AutoRevoke")]
    public bool AutoRevoke { get; set; } = false;

    /// <summary>
    /// 創建時間
    /// </summary>
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 導航屬性 - 管理者
    /// </summary>
    [ForeignKey("ManagerId")]
    public virtual ManagerData Manager { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 角色權限
    /// </summary>
    [ForeignKey("ManagerRoleId")]
    public virtual ManagerRolePermission RolePermission { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 指派者
    /// </summary>
    [ForeignKey("AssignedBy")]
    public virtual ManagerData? AssignedByManager { get; set; }

    /// <summary>
    /// 導航屬性 - 撤銷者
    /// </summary>
    [ForeignKey("RevokedBy")]
    public virtual ManagerData? RevokedByManager { get; set; }
}
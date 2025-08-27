using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 角色權限定義實體
/// 對應資料表: ManagerRolePermission
/// </summary>
[Table("ManagerRolePermission")]
public class ManagerRolePermission
{
    /// <summary>
    /// 管理者角色編號 (主鍵)
    /// </summary>
    [Key]
    [Column("ManagerRole_Id")]
    public int ManagerRoleId { get; set; }

    /// <summary>
    /// 顯示名稱 舉例:商城管理員
    /// </summary>
    [Required]
    [Column("role_name")]
    [StringLength(100)]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 管理者權限管理
    /// </summary>
    [Required]
    [Column("AdministratorPrivilegesManagement")]
    public bool AdministratorPrivilegesManagement { get; set; } = false;

    /// <summary>
    /// 使用者狀態管理
    /// </summary>
    [Required]
    [Column("UserStatusManagement")]
    public bool UserStatusManagement { get; set; } = false;

    /// <summary>
    /// 商城權限管理
    /// </summary>
    [Required]
    [Column("ShoppingPermissionManagement")]
    public bool ShoppingPermissionManagement { get; set; } = false;

    /// <summary>
    /// 論壇權限管理
    /// </summary>
    [Required]
    [Column("MessagePermissionManagement")]
    public bool MessagePermissionManagement { get; set; } = false;

    /// <summary>
    /// 寵物權限管理
    /// </summary>
    [Required]
    [Column("Pet_Rights_Management")]
    public bool PetRightsManagement { get; set; } = false;

    /// <summary>
    /// 客服權限管理
    /// </summary>
    [Required]
    [Column("customer_service")]
    public bool CustomerService { get; set; } = false;

    /// <summary>
    /// 角色描述
    /// </summary>
    [Column("RoleDescription")]
    [StringLength(500)]
    public string? RoleDescription { get; set; }

    /// <summary>
    /// 是否為系統內建角色
    /// </summary>
    [Column("IsSystemRole")]
    public bool IsSystemRole { get; set; } = false;

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Column("IsActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 排序順序
    /// </summary>
    [Column("SortOrder")]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 角色等級 (數值越高權限越大)
    /// </summary>
    [Column("RoleLevel")]
    public int RoleLevel { get; set; } = 1;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 建立者
    /// </summary>
    [Column("CreatedBy")]
    public int? CreatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    [Column("UpdatedBy")]
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column("Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 角色顏色 (用於UI顯示)
    /// </summary>
    [Column("RoleColor")]
    [StringLength(7)]
    public string? RoleColor { get; set; }

    /// <summary>
    /// 角色圖標
    /// </summary>
    [Column("RoleIcon")]
    [StringLength(100)]
    public string? RoleIcon { get; set; }

    /// <summary>
    /// 導航屬性 - 角色指派關聯
    /// </summary>
    public virtual ICollection<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();
}
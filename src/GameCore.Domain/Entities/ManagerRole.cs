using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 管理者角色指派表
/// 對應資料庫 ManagerRole 表
/// </summary>
[Table("ManagerRole")]
public class ManagerRole
{
    /// <summary>
    /// 管理者ID
    /// </summary>
    public int ManagerID { get; set; }

    /// <summary>
    /// 角色編號
    /// </summary>
    public int ManagerRoleID { get; set; }

    /// <summary>
    /// 角色名稱
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ManagerRoleName { get; set; } = string.Empty;

    // 複合主鍵
    [Key]
    public int ID { get; set; }

    // 導航屬性
    /// <summary>
    /// 管理者
    /// </summary>
    public virtual ManagerData Manager { get; set; } = null!;

    /// <summary>
    /// 角色權限
    /// </summary>
    public virtual ManagerRolePermission RolePermission { get; set; } = null!;
} 
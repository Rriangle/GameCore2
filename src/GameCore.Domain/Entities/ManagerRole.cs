using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 管理者 ↔ 角色的指派表（關聯表；多對多）
/// 一個管理者可有多個角色；一個角色可被多位管理者指派
/// </summary>
[Table("ManagerRole")]
public class ManagerRole
{
    /// <summary>
    /// 管理者編號 (外鍵到 ManagerData)
    /// </summary>
    [Required]
    public int Manager_Id { get; set; }

    /// <summary>
    /// 角色編號 (外鍵到 ManagerRolePermission)
    /// </summary>
    [Required]
    public int ManagerRole_Id { get; set; }

    /// <summary>
    /// 角色名稱
    /// </summary>
    [StringLength(100)]
    public string ManagerRole { get; set; } = string.Empty;

    /// <summary>
    /// 指派時間
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 指派者 ID
    /// </summary>
    public int? AssignedBy { get; set; }

    // 導航屬性
    [ForeignKey("Manager_Id")]
    public virtual ManagerData Manager { get; set; } = null!;

    [ForeignKey("ManagerRole_Id")]
    public virtual ManagerRolePermission RolePermission { get; set; } = null!;
}
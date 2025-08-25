using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 角色權限定義表
/// </summary>
[Table("ManagerRolePermission")]
public class ManagerRolePermission
{
    /// <summary>
    /// 管理者角色編號 (主鍵)
    /// </summary>
    [Key]
    public int ManagerRole_Id { get; set; }

    /// <summary>
    /// 顯示名稱 (舉例: 商城管理員)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string role_name { get; set; } = string.Empty;

    /// <summary>
    /// 管理者權限管理
    /// </summary>
    public bool AdministratorPrivilegesManagement { get; set; } = false;

    /// <summary>
    /// 使用者狀態管理
    /// </summary>
    public bool UserStatusManagement { get; set; } = false;

    /// <summary>
    /// 商城權限管理
    /// </summary>
    public bool ShoppingPermissionManagement { get; set; } = false;

    /// <summary>
    /// 論壇權限管理
    /// </summary>
    public bool MessagePermissionManagement { get; set; } = false;

    /// <summary>
    /// 寵物權限管理
    /// </summary>
    public bool Pet_Rights_Management { get; set; } = false;

    /// <summary>
    /// 客服權限管理
    /// </summary>
    public bool customer_service { get; set; } = false;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 導航屬性
    public virtual ICollection<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();
}
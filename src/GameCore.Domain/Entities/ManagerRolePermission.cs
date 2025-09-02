using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 角色權限定義表
/// 對應資料庫 ManagerRolePermission 表
/// </summary>
[Table("ManagerRolePermission")]
public class ManagerRolePermission
{
    /// <summary>
    /// 管理者角色編號 (主鍵)
    /// </summary>
    [Key]
    public int ManagerRoleID { get; set; }

    /// <summary>
    /// 顯示名稱 (舉例:商城管理員)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 管理者權限管理
    /// </summary>
    public bool AdministratorPrivilegesManagement { get; set; }

    /// <summary>
    /// 使用者狀態管理
    /// </summary>
    public bool UserStatusManagement { get; set; }

    /// <summary>
    /// 商城權限管理
    /// </summary>
    public bool ShoppingPermissionManagement { get; set; }

    /// <summary>
    /// 論壇權限管理
    /// </summary>
    public bool MessagePermissionManagement { get; set; }

    /// <summary>
    /// 寵物權限管理
    /// </summary>
    public bool PetRightsManagement { get; set; }

    /// <summary>
    /// 客服權限管理
    /// </summary>
    public bool CustomerService { get; set; }

    // 導航屬性
    /// <summary>
    /// 擁有此角色的管理者
    /// </summary>
    public virtual ICollection<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();
} 
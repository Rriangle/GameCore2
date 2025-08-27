using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 管理者角色權限實體
    /// </summary>
    [Table("ManagerRolePermission")]
    public class ManagerRolePermission
    {
        [Key]
        public int ManagerRole_Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string role_name { get; set; } = string.Empty;
        
        public bool AdministratorPrivilegesManagement { get; set; } = false;
        public bool UserStatusManagement { get; set; } = false;
        public bool ShoppingPermissionManagement { get; set; } = false;
        public bool MessagePermissionManagement { get; set; } = false;
        public bool Pet_Rights_Management { get; set; } = false;
        public bool customer_service { get; set; } = false;
        
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime? updated_at { get; set; }

        // Navigation properties
        public virtual ICollection<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();
    }
}
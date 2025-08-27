using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 管理者角色指派實體
    /// </summary>
    [Table("ManagerRole")]
    public class ManagerRole
    {
        [Required]
        public int Manager_Id { get; set; }
        
        [Required]
        public int ManagerRole_Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ManagerRole { get; set; } = string.Empty;
        
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ManagerData Manager { get; set; } = null!;
        public virtual ManagerRolePermission RolePermission { get; set; } = null!;
    }
}
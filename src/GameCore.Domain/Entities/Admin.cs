using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 管理員登入追蹤實體
    /// </summary>
    [Table("Admins")]
    public class Admin
    {
        [Key]
        public int admin_id { get; set; }
        
        [Required]
        public int manager_id { get; set; }
        
        public DateTime? last_login { get; set; }
        
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ManagerData Manager { get; set; } = null!;
    }
}
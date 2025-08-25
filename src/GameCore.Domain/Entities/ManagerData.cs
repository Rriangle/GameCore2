using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 管理者資料實體
    /// </summary>
    [Table("ManagerData")]
    public class ManagerData
    {
        [Key]
        public int Manager_Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Manager_Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Manager_Account { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string Manager_Password { get; set; } = string.Empty;
        
        [Required]
        public DateTime Administrator_registration_date { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<ManagerRole> Roles { get; set; } = new List<ManagerRole>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
        public virtual ICollection<Admin> AdminLogins { get; set; } = new List<Admin>();
        public virtual ICollection<Mute> Mutes { get; set; } = new List<Mute>();
        public virtual ICollection<Style> Styles { get; set; } = new List<Style>();
    }
}
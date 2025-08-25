using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 群組實體
    /// </summary>
    [Table("Groups")]
    public class Group
    {
        [Key]
        public int group_id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string group_name { get; set; } = string.Empty;
        
        [Required]
        public int created_by { get; set; }
        
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public virtual ICollection<GroupChat> ChatMessages { get; set; } = new List<GroupChat>();
        public virtual ICollection<GroupBlock> Blocks { get; set; } = new List<GroupBlock>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
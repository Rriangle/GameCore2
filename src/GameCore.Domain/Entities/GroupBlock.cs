using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 群組封鎖實體
    /// </summary>
    [Table("Group_Block")]
    public class GroupBlock
    {
        [Key]
        public int block_id { get; set; }
        
        [Required]
        public int group_id { get; set; }
        
        [Required]
        public int user_id { get; set; }
        
        [Required]
        public int blocked_by { get; set; }
        
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Group Group { get; set; } = null!;
        public virtual User BlockedUser { get; set; } = null!;
        public virtual User BlockedByUser { get; set; } = null!;
    }
}
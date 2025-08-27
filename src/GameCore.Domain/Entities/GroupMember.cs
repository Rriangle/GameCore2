using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 群組成員實體
    /// </summary>
    [Table("Group_Member")]
    public class GroupMember
    {
        [Required]
        public int group_id { get; set; }
        
        [Required]
        public int user_id { get; set; }
        
        [Required]
        public DateTime joined_at { get; set; } = DateTime.UtcNow;
        
        [Required]
        public bool is_admin { get; set; } = false;

        // Navigation properties
        public virtual Group Group { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
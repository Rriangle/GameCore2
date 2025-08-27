using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 禁言選項實體
    /// </summary>
    [Table("Mutes")]
    public class Mute
    {
        [Key]
        public int mute_id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string mute_name { get; set; } = string.Empty;
        
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        [Required]
        public bool is_active { get; set; } = true;
        
        [Required]
        public int manager_id { get; set; }

        // Navigation properties
        public virtual ManagerData Manager { get; set; } = null!;
    }
}
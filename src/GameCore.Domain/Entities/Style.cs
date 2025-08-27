using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 樣式池實體
    /// </summary>
    [Table("Styles")]
    public class Style
    {
        [Key]
        public int style_id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string style_name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string effect_desc { get; set; } = string.Empty;
        
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int manager_id { get; set; }

        // Navigation properties
        public virtual ManagerData Manager { get; set; } = null!;
    }
}
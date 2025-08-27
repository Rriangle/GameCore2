using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 群組聊天訊息實體
    /// </summary>
    [Table("Group_Chat")]
    public class GroupChat
    {
        [Key]
        public int group_chat_id { get; set; }
        
        [Required]
        public int group_id { get; set; }
        
        [Required]
        public int sender_id { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string group_chat_content { get; set; } = string.Empty;
        
        [Required]
        public DateTime sent_at { get; set; } = DateTime.UtcNow;
        
        [Required]
        public bool is_sent { get; set; } = true;

        // Navigation properties
        public virtual Group Group { get; set; } = null!;
        public virtual User Sender { get; set; } = null!;
    }
}
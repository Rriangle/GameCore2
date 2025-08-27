using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 聊天訊息實體
    /// </summary>
    [Table("Chat_Message")]
    public class ChatMessage
    {
        [Key]
        public int message_id { get; set; }
        
        public int? manager_id { get; set; }
        
        [Required]
        public int sender_id { get; set; }
        
        public int? receiver_id { get; set; }
        
        [Required]
        [StringLength(2000)]
        public string chat_content { get; set; } = string.Empty;
        
        [Required]
        public DateTime sent_at { get; set; } = DateTime.UtcNow;
        
        [Required]
        public bool is_read { get; set; } = false;
        
        [Required]
        public bool is_sent { get; set; } = true;

        // Navigation properties
        public virtual ManagerData? Manager { get; set; }
        public virtual User Sender { get; set; } = null!;
        public virtual User? Receiver { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("Group_Chat")]
public class GroupChat
{
    [Key]
    public int GroupChatID { get; set; }
    public int GroupID { get; set; }
    public int SenderID { get; set; }
    public string GroupChatContent { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsSent { get; set; } = true;
} 
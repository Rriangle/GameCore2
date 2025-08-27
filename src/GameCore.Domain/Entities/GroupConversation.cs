using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("GroupConversation")]
public class GroupConversation
{
    [Key]
    public int ConversationId { get; set; }

    [Required]
    public int GroupId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    [StringLength(20)]
    public string? MessageType { get; set; } = "text"; // text, image, file, etc.

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
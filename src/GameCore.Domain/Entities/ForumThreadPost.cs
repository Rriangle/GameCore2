using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("ForumThreadPost")]
public class ForumThreadPost
{
    [Key]
    public int ThreadPostId { get; set; }

    [Required]
    public int ThreadId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "normal"; // normal, hidden, archived, deleted

    [Required]
    public int LikeCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("ThreadId")]
    public virtual ForumThread Thread { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
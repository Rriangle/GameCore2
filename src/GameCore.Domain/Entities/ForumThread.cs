using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("ForumThread")]
public class ForumThread
{
    [Key]
    public int ThreadId { get; set; }

    [Required]
    public int PostId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "normal"; // normal, hidden, archived, deleted

    [Required]
    public int ViewCount { get; set; } = 0;

    [Required]
    public int LikeCount { get; set; } = 0;

    [Required]
    public int ReplyCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("PostId")]
    public virtual ForumPost Post { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<ForumThreadPost> ThreadPosts { get; set; } = new List<ForumThreadPost>();
}
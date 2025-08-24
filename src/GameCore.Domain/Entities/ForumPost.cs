using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("ForumPost")]
public class ForumPost
{
    [Key]
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
    public string Status { get; set; } = "draft"; // draft, published, hidden

    [Required]
    public int ViewCount { get; set; } = 0;

    [Required]
    public int LikeCount { get; set; } = 0;

    [Required]
    public int ReplyCount { get; set; } = 0;

    public DateTime? PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<ForumThread> Threads { get; set; } = new List<ForumThread>();
}
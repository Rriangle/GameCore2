using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("ForumBookmark")]
public class ForumBookmark
{
    [Key]
    public int BookmarkId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(20)]
    public string TargetType { get; set; } = string.Empty; // "post", "thread", "thread_post"

    [Required]
    public int TargetId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("Group")]
public class Group
{
    [Key]
    public int GroupId { get; set; }

    [Required]
    public int OwnerId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(200)]
    public string? AvatarUrl { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("OwnerId")]
    public virtual User Owner { get; set; } = null!;

    public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();

    public virtual ICollection<GroupConversation> Conversations { get; set; } = new List<GroupConversation>();
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("GroupMember")]
public class GroupMember
{
    [Key]
    public int MemberId { get; set; }

    [Required]
    public int GroupId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = "member"; // owner, admin, member

    [Required]
    public bool IsActive { get; set; } = true;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LeftAt { get; set; }

    // Navigation properties
    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
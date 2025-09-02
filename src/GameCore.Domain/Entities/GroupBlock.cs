using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("Group_Block")]
public class GroupBlock
{
    [Key]
    public int BlockID { get; set; }
    public int GroupID { get; set; }
    public int UserID { get; set; }
    public int BlockedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 
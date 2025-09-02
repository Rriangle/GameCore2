using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("Group_Member")]
public class GroupMember
{
    [Key]
    [Column(Order = 0)]
    public int GroupID { get; set; }
    
    [Key]
    [Column(Order = 1)]
    public int UserID { get; set; }
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsAdmin { get; set; } = false;

    // 導航屬性
    [ForeignKey("GroupID")]
    public virtual Group Group { get; set; } = null!;
    
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
} 
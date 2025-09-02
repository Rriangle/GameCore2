using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Work")]
public class UserProfileWork
{
    [Key]
    public int WorkID { get; set; }
    public int UserID { get; set; }
    public string? Company { get; set; }
    public string? Position { get; set; }
    public string? Industry { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 
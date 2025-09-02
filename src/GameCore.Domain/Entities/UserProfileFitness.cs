using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Fitness")]
public class UserProfileFitness
{
    [Key]
    public int FitnessID { get; set; }
    public int UserID { get; set; }
    public string FitnessActivity { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Frequency { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Education")]
public class UserProfileEducation
{
    [Key]
    public int EducationID { get; set; }
    public int UserID { get; set; }
    public string? School { get; set; }
    public string? Degree { get; set; }
    public string? FieldOfStudy { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 
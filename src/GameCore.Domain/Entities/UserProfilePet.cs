using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Pet")]
public class UserProfilePet
{
    [Key]
    public int PetID { get; set; }
    public int UserID { get; set; }
    public string PetName { get; set; } = string.Empty;
    public string? Species { get; set; }
    public string? Breed { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 
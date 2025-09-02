using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Location")]
public class UserProfileLocation
{
    [Key]
    public int LocationID { get; set; }
    public int UserID { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 
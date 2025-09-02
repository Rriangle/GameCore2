using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Book")]
public class UserProfileBook
{
    [Key]
    public int BookID { get; set; }
    public int UserID { get; set; }
    public string BookName { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Genre { get; set; }
    public int? PublicationYear { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_Profile_Document")]
public class UserProfileDocument
{
    [Key]
    public int DocumentID { get; set; }
    public int UserID { get; set; }
    public string DocumentUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? FileType { get; set; }
    public string? FileSize { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 
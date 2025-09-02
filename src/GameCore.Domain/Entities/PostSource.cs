using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("post_sources")]
public class PostSource
{
    [Key]
    public long ID { get; set; }
    public long PostID { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 
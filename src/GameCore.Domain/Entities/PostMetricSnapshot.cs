using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("post_metric_snapshot")]
public class PostMetricSnapshot
{
    [Key]
    public long PostID { get; set; }
    public int GameID { get; set; }
    public DateTime Date { get; set; }
    public decimal IndexValue { get; set; }
    public string DetailsJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 
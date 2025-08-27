using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 外部ID對應表實體 - 把內部遊戲對應到各來源的外部鍵
/// 對應資料表: game_source_map
/// </summary>
[Table("game_source_map")]
public class GameSourceMap
{
    /// <summary>
    /// 對應編號 (主鍵)
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 內部遊戲編號 (外鍵至 Game)
    /// </summary>
    [Required]
    [Column("game_id")]
    public int GameId { get; set; }

    /// <summary>
    /// 外部來源編號 (外鍵至 MetricSource)
    /// </summary>
    [Required]
    [Column("source_id")]
    public int SourceId { get; set; }

    /// <summary>
    /// 外部ID (Steam appid / 巴哈 slug等)
    /// </summary>
    [Required]
    [Column("external_key")]
    [StringLength(200)]
    public string ExternalKey { get; set; } = string.Empty;

    /// <summary>
    /// 外部名稱 (在該平台的顯示名稱)
    /// </summary>
    [Column("external_name")]
    [StringLength(200)]
    public string? ExternalName { get; set; }

    /// <summary>
    /// 外部URL
    /// </summary>
    [Column("external_url")]
    [StringLength(500)]
    public string? ExternalUrl { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 最後更新時間
    /// </summary>
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column("note")]
    [StringLength(500)]
    public string? Note { get; set; }

    /// <summary>
    /// 導航屬性 - 內部遊戲
    /// </summary>
    [ForeignKey("GameId")]
    public virtual Game Game { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 外部數據來源
    /// </summary>
    [ForeignKey("SourceId")]
    public virtual MetricSource Source { get; set; } = null!;
}
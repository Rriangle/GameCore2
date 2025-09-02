using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 每日熱度指數表
/// 對應資料庫 popularity_index_daily 表
/// </summary>
[Table("popularity_index_daily")]
public class PopularityIndexDaily
{
    /// <summary>
    /// 流水號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ID { get; set; }

    /// <summary>
    /// 遊戲ID
    /// </summary>
    public int GameID { get; set; }

    /// <summary>
    /// 日期
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 熱度指數 (加權計算)
    /// </summary>
    [Column(TypeName = "decimal(18,4)")]
    public decimal IndexValue { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的遊戲
    /// </summary>
    public virtual Game Game { get; set; } = null!;
} 
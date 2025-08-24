using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 小冒險遊戲紀錄表 - 小遊戲系統 (將在後續階段完整實現)
/// </summary>
[Table("MiniGame")]
public class MiniGame
{
    /// <summary>
    /// 遊戲執行記錄編號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    [Column("PlayID")]
    public int PlayID { get; set; }

    /// <summary>
    /// 玩家會員編號 (外鍵參考 Users.User_ID)
    /// </summary>
    [Required]
    [Column("UserID")]
    public int UserID { get; set; }

    /// <summary>
    /// 遊戲開始時間
    /// </summary>
    [Required]
    [Column("StartTime")]
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    // 導航屬性
    /// <summary>
    /// 玩家
    /// </summary>
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 簽到記錄表 - 每日簽到系統 (將在後續階段完整實現)
/// </summary>
[Table("UserSignInStats")]
public class UserSignInStats
{
    /// <summary>
    /// 簽到記錄編號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    [Column("LogID")]
    public int LogID { get; set; }

    /// <summary>
    /// 簽到時間
    /// </summary>
    [Required]
    [Column("SignTime")]
    public DateTime SignTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 會員編號 (外鍵參考 Users.User_ID)
    /// </summary>
    [Required]
    [Column("UserID")]
    public int UserID { get; set; }

    // 導航屬性
    /// <summary>
    /// 簽到會員
    /// </summary>
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
}
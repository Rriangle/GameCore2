using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 簽到記錄實體 - 每日簽到系統
/// 對應資料表: UserSignInStats
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
    /// 簽到時間（預設 UTC 當下時間）
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

    /// <summary>
    /// 此次簽到會員點數增減數量
    /// </summary>
    [Required]
    [Column("PointsChanged")]
    public int PointsChanged { get; set; } = 0;

    /// <summary>
    /// 此次簽到寵物獲得經驗值
    /// </summary>
    [Required]
    [Column("ExpGained")]
    public int ExpGained { get; set; } = 0;

    /// <summary>
    /// 點數變動時間
    /// </summary>
    [Required]
    [Column("PointsChangedTime")]
    public DateTime PointsChangedTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 寵物經驗值獲得時間
    /// </summary>
    [Required]
    [Column("ExpGainedTime")]
    public DateTime ExpGainedTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 簽到類型 (daily/weekend/streak/monthly)
    /// </summary>
    [Column("SignInType")]
    [StringLength(20)]
    public string SignInType { get; set; } = "daily";

    /// <summary>
    /// 連續簽到天數（簽到當時的連續天數）
    /// </summary>
    [Column("StreakDays")]
    public int StreakDays { get; set; } = 0;

    /// <summary>
    /// 是否為補簽
    /// </summary>
    [Column("IsMakeUp")]
    public bool IsMakeUp { get; set; } = false;

    /// <summary>
    /// 簽到獎勵說明
    /// </summary>
    [Column("RewardDescription")]
    [StringLength(200)]
    public string? RewardDescription { get; set; }

    /// <summary>
    /// 是否為額外獎勵 (7天連續/全月獎勵)
    /// </summary>
    [Column("IsBonusReward")]
    public bool IsBonusReward { get; set; } = false;

    /// <summary>
    /// 額外獎勵類型 (streak_7/monthly_perfect)
    /// </summary>
    [Column("BonusType")]
    [StringLength(30)]
    public string? BonusType { get; set; }

    /// <summary>
    /// 簽到來源 (web/mobile/auto)
    /// </summary>
    [Column("SignInSource")]
    [StringLength(20)]
    public string SignInSource { get; set; } = "web";

    /// <summary>
    /// IP位址
    /// </summary>
    [Column("IpAddress")]
    [StringLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// 用戶代理
    /// </summary>
    [Column("UserAgent")]
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// 簽到時的台北時間日期 (用於判定唯一性)
    /// </summary>
    [Column("SignInDate")]
    public DateOnly SignInDate { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    [Column("Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 導航屬性 - 簽到會員
    /// </summary>
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
}
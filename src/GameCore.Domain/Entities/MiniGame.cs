using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 小冒險遊戲紀錄實體 - 小遊戲系統
/// 對應資料表: MiniGame
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
    /// 出戰寵物編號 (外鍵參考 Pet.PetID)
    /// </summary>
    [Required]
    [Column("PetID")]
    public int PetID { get; set; }

    /// <summary>
    /// 遊戲關卡等級
    /// </summary>
    [Required]
    [Column("Level")]
    public int Level { get; set; } = 0;

    /// <summary>
    /// 需面對的怪物數量
    /// </summary>
    [Required]
    [Column("MonsterCount")]
    public int MonsterCount { get; set; } = 0;

    /// <summary>
    /// 怪物移動速度倍率
    /// </summary>
    [Required]
    [Column("SpeedMultiplier", TypeName = "decimal(5,2)")]
    public decimal SpeedMultiplier { get; set; } = 1.00m;

    /// <summary>
    /// 遊戲結果: Win(贏)/Lose(輸)/Abort(中退)
    /// </summary>
    [Required]
    [Column("Result")]
    [StringLength(10)]
    public string Result { get; set; } = "Unknown";

    /// <summary>
    /// 寵物本次獲得經驗值
    /// </summary>
    [Required]
    [Column("ExpGained")]
    public int ExpGained { get; set; } = 0;

    /// <summary>
    /// 寵物獲得經驗值時間
    /// </summary>
    [Column("ExpGainedTime")]
    public DateTime? ExpGainedTime { get; set; }

    /// <summary>
    /// 本次會員點數增減
    /// </summary>
    [Required]
    [Column("PointsChanged")]
    public int PointsChanged { get; set; } = 0;

    /// <summary>
    /// 本次會員點數變動時間
    /// </summary>
    [Column("PointsChangedTime")]
    public DateTime? PointsChangedTime { get; set; }

    /// <summary>
    /// 寵物飢餓值變化量
    /// </summary>
    [Required]
    [Column("HungerDelta")]
    public int HungerDelta { get; set; } = 0;

    /// <summary>
    /// 寵物心情值變化量
    /// </summary>
    [Required]
    [Column("MoodDelta")]
    public int MoodDelta { get; set; } = 0;

    /// <summary>
    /// 寵物體力值變化量
    /// </summary>
    [Required]
    [Column("StaminaDelta")]
    public int StaminaDelta { get; set; } = 0;

    /// <summary>
    /// 寵物清潔值變化量
    /// </summary>
    [Required]
    [Column("CleanlinessDelta")]
    public int CleanlinessDelta { get; set; } = 0;

    /// <summary>
    /// 遊戲開始時間
    /// </summary>
    [Required]
    [Column("StartTime")]
    public DateTime StartTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 遊戲結束時間，若中退則為null
    /// </summary>
    [Column("EndTime")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 是否中途放棄 (0=否,1=是)，預設為0(否)
    /// </summary>
    [Required]
    [Column("Aborted")]
    public bool Aborted { get; set; } = false;

    /// <summary>
    /// 遊戲總時長 (秒)
    /// </summary>
    [Column("DurationSeconds")]
    public int? DurationSeconds { get; set; }

    /// <summary>
    /// 擊敗的怪物數量
    /// </summary>
    [Column("MonstersDefeated")]
    public int MonstersDefeated { get; set; } = 0;

    /// <summary>
    /// 遊戲難度 (easy/normal/hard)
    /// </summary>
    [Column("Difficulty")]
    [StringLength(20)]
    public string Difficulty { get; set; } = "normal";

    /// <summary>
    /// 遊戲模式 (adventure/survival/boss)
    /// </summary>
    [Column("GameMode")]
    [StringLength(20)]
    public string GameMode { get; set; } = "adventure";

    /// <summary>
    /// 最高連擊數
    /// </summary>
    [Column("MaxCombo")]
    public int MaxCombo { get; set; } = 0;

    /// <summary>
    /// 獲得的成就 (JSON格式)
    /// </summary>
    [Column("AchievementsEarned")]
    public string? AchievementsEarned { get; set; }

    /// <summary>
    /// 遊戲評分 (1-5星)
    /// </summary>
    [Column("Rating")]
    public int? Rating { get; set; }

    /// <summary>
    /// 遊戲備註
    /// </summary>
    [Column("Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

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
    /// 客戶端版本
    /// </summary>
    [Column("ClientVersion")]
    [StringLength(50)]
    public string? ClientVersion { get; set; }

    /// <summary>
    /// 是否為練習模式
    /// </summary>
    [Column("IsPracticeMode")]
    public bool IsPracticeMode { get; set; } = false;

    /// <summary>
    /// 健康度變化量
    /// </summary>
    [Column("HealthDelta")]
    public int HealthDelta { get; set; } = 0;

    /// <summary>
    /// 是否觸發寵物升級
    /// </summary>
    [Column("TriggeredLevelUp")]
    public bool TriggeredLevelUp { get; set; } = false;

    /// <summary>
    /// 導航屬性 - 玩家
    /// </summary>
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 出戰寵物
    /// </summary>
    [ForeignKey("PetID")]
    public virtual Pet Pet { get; set; } = null!;
}
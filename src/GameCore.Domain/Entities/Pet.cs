using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 寵物狀態實體 - 虛擬寵物(史萊姆)系統
/// 對應資料表: Pet
/// </summary>
[Table("Pet")]
public class Pet
{
    /// <summary>
    /// 寵物編號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    [Column("PetID")]
    public int PetID { get; set; }

    /// <summary>
    /// 寵物主人會員編號 (外鍵參考 Users.User_ID)
    /// </summary>
    [Required]
    [Column("UserID")]
    public int UserID { get; set; }

    /// <summary>
    /// 寵物名稱，若未提供則預設為"小可愛"
    /// </summary>
    [Required]
    [Column("PetName")]
    [StringLength(50)]
    public string PetName { get; set; } = "小可愛";

    /// <summary>
    /// 寵物當前等級
    /// </summary>
    [Required]
    [Column("Level")]
    public int Level { get; set; } = 0;

    /// <summary>
    /// 寵物最後一次升級時間
    /// </summary>
    [Required]
    [Column("LevelUpTime")]
    public DateTime LevelUpTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 寵物累計總經驗值
    /// </summary>
    [Required]
    [Column("Experience")]
    public int Experience { get; set; } = 0;

    /// <summary>
    /// 飢餓值（0–100）
    /// </summary>
    [Required]
    [Column("Hunger")]
    public int Hunger { get; set; } = 0;

    /// <summary>
    /// 心情值（0–100）
    /// </summary>
    [Required]
    [Column("Mood")]
    public int Mood { get; set; } = 0;

    /// <summary>
    /// 體力值（0–100）
    /// </summary>
    [Required]
    [Column("Stamina")]
    public int Stamina { get; set; } = 0;

    /// <summary>
    /// 清潔值（0–100）
    /// </summary>
    [Required]
    [Column("Cleanliness")]
    public int Cleanliness { get; set; } = 0;

    /// <summary>
    /// 健康度（0–100）
    /// </summary>
    [Required]
    [Column("Health")]
    public int Health { get; set; } = 0;

    /// <summary>
    /// 膚色十六進位色碼
    /// </summary>
    [Required]
    [Column("SkinColor")]
    [StringLength(50)]
    public string SkinColor { get; set; } = "#ADD8E6";

    /// <summary>
    /// 最後一次膚色更換時間
    /// </summary>
    [Required]
    [Column("ColorChangedTime")]
    public DateTime ColorChangedTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 背景色
    /// </summary>
    [Required]
    [Column("BackgroundColor")]
    [StringLength(50)]
    public string BackgroundColor { get; set; } = "粉藍";

    /// <summary>
    /// 最後一次背景色更換時間
    /// </summary>
    [Required]
    [Column("BackgroundColorChangedTime")]
    public DateTime BackgroundColorChangedTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 最近一次幫寵物換色所花費之會員點數
    /// </summary>
    [Required]
    [Column("PointsChanged")]
    public int PointsChanged { get; set; } = 0;

    /// <summary>
    /// 幫寵物換色所花費之會員點數變動時間
    /// </summary>
    [Required]
    [Column("PointsChangedTime")]
    public DateTime PointsChangedTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 寵物創建時間
    /// </summary>
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 最後更新時間
    /// </summary>
    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 最後互動時間
    /// </summary>
    [Column("LastInteractionAt")]
    public DateTime? LastInteractionAt { get; set; }

    /// <summary>
    /// 最後喂食時間
    /// </summary>
    [Column("LastFeedTime")]
    public DateTime? LastFeedTime { get; set; }

    /// <summary>
    /// 最後洗澡時間
    /// </summary>
    [Column("LastBatheTime")]
    public DateTime? LastBatheTime { get; set; }

    /// <summary>
    /// 最後玩耍時間
    /// </summary>
    [Column("LastPlayTime")]
    public DateTime? LastPlayTime { get; set; }

    /// <summary>
    /// 最後休息時間
    /// </summary>
    [Column("LastRestTime")]
    public DateTime? LastRestTime { get; set; }

    /// <summary>
    /// 最後衰減計算時間
    /// </summary>
    [Column("LastDecayTime")]
    public DateTime? LastDecayTime { get; set; }

    /// <summary>
    /// 總互動次數
    /// </summary>
    [Column("TotalInteractions")]
    public int TotalInteractions { get; set; } = 0;

    /// <summary>
    /// 今日互動次數
    /// </summary>
    [Column("TodayInteractions")]
    public int TodayInteractions { get; set; } = 0;

    /// <summary>
    /// 寵物狀態 (active/sick/sleeping/dead)
    /// </summary>
    [Column("Status")]
    [StringLength(20)]
    public string Status { get; set; } = "active";

    /// <summary>
    /// 是否可以進行冒險
    /// </summary>
    [Column("CanAdventure")]
    public bool CanAdventure { get; set; } = true;

    /// <summary>
    /// 寵物外觀配置 (JSON格式)
    /// </summary>
    [Column("AppearanceConfig")]
    public string? AppearanceConfig { get; set; }

    /// <summary>
    /// 寵物備註
    /// </summary>
    [Column("Notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// 導航屬性 - 寵物主人
    /// </summary>
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 相關小遊戲記錄
    /// </summary>
    public virtual ICollection<MiniGame> MiniGames { get; set; } = new List<MiniGame>();
}
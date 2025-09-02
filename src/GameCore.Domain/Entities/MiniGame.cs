using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 小冒險遊戲紀錄實體
    /// </summary>
    [Table("MiniGame")]
    public class MiniGame
    {
        /// <summary>
        /// 遊戲執行記錄 ID，自動遞增
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayID { get; set; }

        /// <summary>
        /// 玩家會員 ID，外鍵參考 Users.UserID
        /// </summary>
        [Required]
        public int UserID { get; set; }

        /// <summary>
        /// 出戰寵物 ID，外鍵參考 Pet.PetID
        /// </summary>
        [Required]
        public int PetID { get; set; }

        /// <summary>
        /// 遊戲關卡等級
        /// </summary>
        [Required]
        public int Level { get; set; } = 0;

        /// <summary>
        /// 需面對的怪物數量
        /// </summary>
        [Required]
        public int MonsterCount { get; set; } = 0;

        /// <summary>
        /// 怪物移動速度倍率
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal SpeedMultiplier { get; set; } = 1.00m;

        /// <summary>
        /// 遊戲結果: Win(贏)/Lose(輸)/Abort(中退)
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Result { get; set; } = "Unknown";

        /// <summary>
        /// 寵物本次獲得經驗值
        /// </summary>
        [Required]
        public int ExpGained { get; set; } = 0;

        /// <summary>
        /// 寵物獲得經驗值時間
        /// </summary>
        public DateTime? ExpGainedTime { get; set; }

        /// <summary>
        /// 本次會員點數增減
        /// </summary>
        [Required]
        public int PointsChanged { get; set; } = 0;

        /// <summary>
        /// 本次會員點數變動時間
        /// </summary>
        public DateTime? PointsChangedTime { get; set; }

        /// <summary>
        /// 寵物飢餓值變化量
        /// </summary>
        [Required]
        public int HungerDelta { get; set; } = 0;

        /// <summary>
        /// 寵物心情值變化量
        /// </summary>
        [Required]
        public int MoodDelta { get; set; } = 0;

        /// <summary>
        /// 寵物體力值變化量
        /// </summary>
        [Required]
        public int StaminaDelta { get; set; } = 0;

        /// <summary>
        /// 寵物清潔值變化量
        /// </summary>
        [Required]
        public int CleanlinessDelta { get; set; } = 0;

        /// <summary>
        /// 遊戲開始時間
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 遊戲結束時間，若中退則為null
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否中途放棄 (0=否,1=是)，預設為0(否)
        /// </summary>
        [Required]
        public bool Aborted { get; set; } = false;

        // 導航屬性
        /// <summary>
        /// 關聯的用戶
        /// </summary>
        [ForeignKey("UserID")]
        public virtual User? User { get; set; }

        /// <summary>
        /// 關聯的寵物
        /// </summary>
        [ForeignKey("PetID")]
        public virtual Pet? Pet { get; set; }
    }
} 
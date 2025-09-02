using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 寵物狀態實體
    /// </summary>
    [Table("Pet")]
    public class Pet
    {
        /// <summary>
        /// 寵物 ID，自動遞增
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PetID { get; set; }

        /// <summary>
        /// 寵物主人會員 ID，外鍵參考 Users.UserID
        /// </summary>
        [Required]
        public int UserID { get; set; }

        /// <summary>
        /// 寵物名稱，若未提供則預設
        /// </summary>
        [Required]
        [StringLength(50)]
        public string PetName { get; set; } = "小可愛";

        /// <summary>
        /// 寵物當前等級
        /// </summary>
        [Required]
        public int Level { get; set; } = 0;

        /// <summary>
        /// 寵物最後一次升級時間
        /// </summary>
        [Required]
        public DateTime LevelUpTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 寵物累計總經驗值
        /// </summary>
        [Required]
        public int Experience { get; set; } = 0;

        /// <summary>
        /// 飢餓值（0–100）
        /// </summary>
        [Required]
        public int Hunger { get; set; } = 0;

        /// <summary>
        /// 心情值（0–100）
        /// </summary>
        [Required]
        public int Mood { get; set; } = 0;

        /// <summary>
        /// 體力值（0–100）
        /// </summary>
        [Required]
        public int Stamina { get; set; } = 0;

        /// <summary>
        /// 清潔值（0–100）
        /// </summary>
        [Required]
        public int Cleanliness { get; set; } = 0;

        /// <summary>
        /// 健康度（0–100）
        /// </summary>
        [Required]
        public int Health { get; set; } = 0;

        /// <summary>
        /// 膚色十六進位
        /// </summary>
        [Required]
        [StringLength(50)]
        public string SkinColor { get; set; } = "#ADD8E6";

        /// <summary>
        /// 最後一次膚色更換時間
        /// </summary>
        [Required]
        public DateTime ColorChangedTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 背景色
        /// </summary>
        [Required]
        [StringLength(50)]
        public string BackgroundColor { get; set; } = "粉藍";

        /// <summary>
        /// 最後一次被景色更換時間
        /// </summary>
        [Required]
        public DateTime BackgroundColorChangedTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 最近一次幫寵物換色所花費之會員點數
        /// </summary>
        [Required]
        public int PointsChanged { get; set; } = 0;

        /// <summary>
        /// 幫寵物換色所花費之會員點數變動時間
        /// </summary>
        [Required]
        public DateTime PointsChangedTime { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 關聯的用戶
        /// </summary>
        [ForeignKey("UserID")]
        public virtual User? User { get; set; }

        /// <summary>
        /// 關聯的小遊戲記錄
        /// </summary>
        public virtual ICollection<MiniGame>? MiniGames { get; set; }
    }
} 
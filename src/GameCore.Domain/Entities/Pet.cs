using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 寵物表
    /// </summary>
    [Table("pets")]
    public class Pet
    {
        /// <summary>
        /// 寵物ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pet_id { get; set; }

        /// <summary>
        /// 主人ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int owner_id { get; set; }

        /// <summary>
        /// 寵物名稱
        /// </summary>
        [Required]
        [StringLength(50)]
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// 寵物類型（cat/dog/bird/fish等）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string type { get; set; } = "cat";

        /// <summary>
        /// 寵物等級
        /// </summary>
        public int level { get; set; } = 1;

        /// <summary>
        /// 經驗值
        /// </summary>
        public int experience { get; set; } = 0;

        /// <summary>
        /// 健康值（0-100）
        /// </summary>
        public int health { get; set; } = 100;

        /// <summary>
        /// 飢餓值（0-100）
        /// </summary>
        public int hunger { get; set; } = 100;

        /// <summary>
        /// 心情值（0-100）
        /// </summary>
        public int mood { get; set; } = 100;

        /// <summary>
        /// 能量值（0-100）
        /// </summary>
        public int energy { get; set; } = 100;

        /// <summary>
        /// 寵物狀態（active/sleeping/sick/dead）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "active";

        /// <summary>
        /// 最後餵食時間
        /// </summary>
        public DateTime? last_fed_at { get; set; }

        /// <summary>
        /// 最後互動時間
        /// </summary>
        public DateTime? last_interacted_at { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime? updated_at { get; set; }

        // 導航屬性
        /// <summary>
        /// 主人
        /// </summary>
        [ForeignKey("owner_id")]
        public virtual User Owner { get; set; } = null!;
    }
}
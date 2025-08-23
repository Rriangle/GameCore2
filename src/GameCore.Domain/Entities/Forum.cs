using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 論壇表
    /// </summary>
    [Table("forums")]
    public class Forum
    {
        /// <summary>
        /// 論壇ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int forum_id { get; set; }

        /// <summary>
        /// 論壇名稱
        /// </summary>
        [Required]
        [StringLength(100)]
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// 論壇描述
        /// </summary>
        [StringLength(500)]
        public string? description { get; set; }

        /// <summary>
        /// 論壇類型（general/game-specific）
        /// </summary>
        [Required]
        [StringLength(50)]
        public string type { get; set; } = "general";

        /// <summary>
        /// 遊戲ID（外鍵參考 games.game_id，遊戲專用論壇）
        /// </summary>
        public int? game_id { get; set; }

        /// <summary>
        /// 論壇狀態（active/inactive/archived）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "active";

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
        /// 遊戲（遊戲專用論壇）
        /// </summary>
        [ForeignKey("game_id")]
        public virtual Game? Game { get; set; }

        /// <summary>
        /// 貼文
        /// </summary>
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
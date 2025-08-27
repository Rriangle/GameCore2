using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 熱度指數每日表
    /// </summary>
    [Table("popularity_index_dailies")]
    public class PopularityIndexDaily
    {
        /// <summary>
        /// 記錄ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long record_id { get; set; }

        /// <summary>
        /// 遊戲ID（外鍵參考 games.game_id）
        /// </summary>
        [Required]
        public int game_id { get; set; }

        /// <summary>
        /// 記錄日期
        /// </summary>
        [Required]
        public DateTime record_date { get; set; }

        /// <summary>
        /// 熱度指數值
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal popularity_index { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 遊戲
        /// </summary>
        [ForeignKey("game_id")]
        public virtual Game Game { get; set; } = null!;
    }
}
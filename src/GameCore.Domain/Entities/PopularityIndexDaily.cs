using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 每日熱度指數（計算結果）
    /// </summary>
    [Table("popularity_index_daily")]
    public class PopularityIndexDaily
    {
        /// <summary>
        /// 流水號
        /// </summary>
        [Key]
        public long id { get; set; }
        
        /// <summary>
        /// 遊戲ID
        /// </summary>
        [Required]
        public int game_id { get; set; }
        
        /// <summary>
        /// 日期
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateTime date { get; set; }
        
        /// <summary>
        /// 熱度指數（加權計算）
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal index_value { get; set; }
        
        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 導航屬性：遊戲
        /// </summary>
        public virtual Game Game { get; set; } = null!;
    }
}
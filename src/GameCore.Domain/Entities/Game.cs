using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 遊戲主檔：列出平台所有遊戲
    /// </summary>
    [Table("games")]
    public class Game
    {
        /// <summary>
        /// 遊戲ID（主鍵）
        /// </summary>
        [Key]
        public int game_id { get; set; }
        
        /// <summary>
        /// 遊戲名稱
        /// </summary>
        [Required]
        [StringLength(200)]
        public string name { get; set; } = string.Empty;
        
        /// <summary>
        /// 類型（FPS/MOBA等）
        /// </summary>
        [StringLength(100)]
        public string genre { get; set; } = string.Empty;
        
        /// <summary>
        /// 建立時間
        /// </summary>
        [Required]
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// 導航屬性：遊戲指標來源對應
        /// </summary>
        public virtual ICollection<GameSourceMap> GameSourceMaps { get; set; } = new List<GameSourceMap>();
        
        /// <summary>
        /// 導航屬性：每日指標數據
        /// </summary>
        public virtual ICollection<GameMetricDaily> GameMetricDailies { get; set; } = new List<GameMetricDaily>();
        
        /// <summary>
        /// 導航屬性：每日熱度指數
        /// </summary>
        public virtual ICollection<PopularityIndexDaily> PopularityIndexDailies { get; set; } = new List<PopularityIndexDaily>();
        
        /// <summary>
        /// 導航屬性：排行榜快照
        /// </summary>
        public virtual ICollection<LeaderboardSnapshot> LeaderboardSnapshots { get; set; } = new List<LeaderboardSnapshot>();
        
        /// <summary>
        /// 導航屬性：洞察貼文
        /// </summary>
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        
        /// <summary>
        /// 導航屬性：論壇
        /// </summary>
        public virtual Forum? Forum { get; set; }
    }
}
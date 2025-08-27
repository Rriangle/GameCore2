using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 外部ID對應表：把內部遊戲對應到各來源的外部鍵
    /// </summary>
    [Table("game_source_map")]
    public class GameSourceMap
    {
        /// <summary>
        /// 對應ID
        /// </summary>
        [Key]
        public int id { get; set; }
        
        /// <summary>
        /// 內部遊戲ID
        /// </summary>
        [Required]
        public int game_id { get; set; }
        
        /// <summary>
        /// 外部來源ID
        /// </summary>
        [Required]
        public int source_id { get; set; }
        
        /// <summary>
        /// 外部ID（Steam appid / 巴哈 slug）
        /// </summary>
        [Required]
        [StringLength(200)]
        public string external_key { get; set; } = string.Empty;
        
        /// <summary>
        /// 導航屬性：遊戲
        /// </summary>
        public virtual Game Game { get; set; } = null!;
        
        /// <summary>
        /// 導航屬性：指標來源
        /// </summary>
        public virtual MetricSource Source { get; set; } = null!;
    }
}
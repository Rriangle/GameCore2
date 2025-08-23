using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 遊戲商品詳情表
    /// </summary>
    [Table("game_product_details")]
    public class GameProductDetails
    {
        /// <summary>
        /// 詳情ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int detail_id { get; set; }

        /// <summary>
        /// 遊戲ID（外鍵參考 games.game_id）
        /// </summary>
        [Required]
        public int game_id { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        [Required]
        [StringLength(200)]
        public string product_name { get; set; } = string.Empty;

        /// <summary>
        /// 商品描述
        /// </summary>
        [StringLength(1000)]
        public string? description { get; set; }

        /// <summary>
        /// 商品價格
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal? price { get; set; }

        /// <summary>
        /// 商品狀態（available/sold-out/discontinued）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "available";

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
        /// 遊戲
        /// </summary>
        [ForeignKey("game_id")]
        public virtual Game Game { get; set; } = null!;
    }
}
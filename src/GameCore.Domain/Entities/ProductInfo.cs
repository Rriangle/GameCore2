using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 商品資訊表
    /// </summary>
    [Table("product_info")]
    public class ProductInfo
    {
        /// <summary>
        /// 商品ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int product_id { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        [Required]
        [StringLength(200)]
        public string product_name { get; set; } = string.Empty;

        /// <summary>
        /// 商品描述
        /// </summary>
        [StringLength(2000)]
        public string? product_description { get; set; }

        /// <summary>
        /// 商品類型（game/accessory/merchandise等）
        /// </summary>
        [Required]
        [StringLength(100)]
        public string product_type { get; set; } = string.Empty;

        /// <summary>
        /// 商品分類
        /// </summary>
        [StringLength(100)]
        public string? category { get; set; }

        /// <summary>
        /// 商品標籤（JSON格式）
        /// </summary>
        public string? tags { get; set; }

        /// <summary>
        /// 商品狀態（active/inactive/discontinued）
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
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 其他商品詳情表
    /// </summary>
    [Table("other_product_details")]
    public class OtherProductDetails
    {
        /// <summary>
        /// 詳情ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int detail_id { get; set; }

        /// <summary>
        /// 商品ID（外鍵參考 product_info.product_id）
        /// </summary>
        [Required]
        public int product_id { get; set; }

        /// <summary>
        /// 供應商ID（外鍵參考 suppliers.supplier_id）
        /// </summary>
        [Required]
        public int supplier_id { get; set; }

        /// <summary>
        /// 商品價格
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal price { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        public int stock_quantity { get; set; } = 0;

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
        /// 商品
        /// </summary>
        [ForeignKey("product_id")]
        public virtual ProductInfo Product { get; set; } = null!;

        /// <summary>
        /// 供應商
        /// </summary>
        [ForeignKey("supplier_id")]
        public virtual Supplier Supplier { get; set; } = null!;
    }
}
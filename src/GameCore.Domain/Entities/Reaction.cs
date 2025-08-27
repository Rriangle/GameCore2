using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 反應表
    /// </summary>
    [Table("reactions")]
    public class Reaction
    {
        /// <summary>
        /// 反應ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long reaction_id { get; set; }

        /// <summary>
        /// 用戶ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int user_id { get; set; }

        /// <summary>
        /// 反應類型（like/love/laugh/wow/sad/angry）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string reaction_type { get; set; } = "like";

        /// <summary>
        /// 目標類型（post/thread/comment）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string target_type { get; set; } = string.Empty;

        /// <summary>
        /// 目標ID
        /// </summary>
        [Required]
        public long target_id { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime created_at { get; set; } = DateTime.UtcNow;

        // 導航屬性
        /// <summary>
        /// 用戶
        /// </summary>
        [ForeignKey("user_id")]
        public virtual User User { get; set; } = null!;
    }
}
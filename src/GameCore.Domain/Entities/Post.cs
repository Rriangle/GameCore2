using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    /// <summary>
    /// 貼文表
    /// </summary>
    [Table("posts")]
    public class Post
    {
        /// <summary>
        /// 貼文ID（主鍵，自動遞增）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long post_id { get; set; }

        /// <summary>
        /// 論壇ID（外鍵參考 forums.forum_id）
        /// </summary>
        [Required]
        public int forum_id { get; set; }

        /// <summary>
        /// 作者ID（外鍵參考 users.user_id）
        /// </summary>
        [Required]
        public int author_id { get; set; }

        /// <summary>
        /// 父貼文ID（外鍵參考 posts.post_id，用於回覆）
        /// </summary>
        public long? parent_post_id { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        [StringLength(200)]
        public string? title { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        [Required]
        public string content { get; set; } = string.Empty;

        /// <summary>
        /// 貼文類型（thread/reply）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string type { get; set; } = "thread";

        /// <summary>
        /// 貼文狀態（active/hidden/deleted）
        /// </summary>
        [Required]
        [StringLength(20)]
        public string status { get; set; } = "active";

        /// <summary>
        /// 點讚數
        /// </summary>
        public int like_count { get; set; } = 0;

        /// <summary>
        /// 點讚用戶ID列表（JSON格式）
        /// </summary>
        public string? liked_user_ids { get; set; }

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
        /// 論壇
        /// </summary>
        [ForeignKey("forum_id")]
        public virtual Forum Forum { get; set; } = null!;

        /// <summary>
        /// 作者
        /// </summary>
        [ForeignKey("author_id")]
        public virtual User Author { get; set; } = null!;

        /// <summary>
        /// 父貼文
        /// </summary>
        [ForeignKey("parent_post_id")]
        public virtual Post? ParentPost { get; set; }

        /// <summary>
        /// 子貼文
        /// </summary>
        public virtual ICollection<Post> ChildPosts { get; set; } = new List<Post>();
    }
}
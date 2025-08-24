using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 通知接收者實體
/// 對應資料表: Notification_Recipients
/// </summary>
[Table("Notification_Recipients")]
public class NotificationRecipient
{
    /// <summary>
    /// 接收紀錄編號 (主鍵)
    /// </summary>
    [Key]
    [Column("recipient_id")]
    public int RecipientId { get; set; }

    /// <summary>
    /// 通知編號 (外鍵至 Notification)
    /// </summary>
    [Required]
    [Column("notification_id")]
    public int NotificationId { get; set; }

    /// <summary>
    /// 使用者編號 (外鍵至 User)
    /// </summary>
    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// 是否已讀
    /// </summary>
    [Required]
    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// 已讀時間
    /// </summary>
    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 是否已刪除 (軟刪除)
    /// </summary>
    [Column("is_deleted")]
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 刪除時間
    /// </summary>
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// 是否已收藏
    /// </summary>
    [Column("is_starred")]
    public bool IsStarred { get; set; } = false;

    /// <summary>
    /// 收藏時間
    /// </summary>
    [Column("starred_at")]
    public DateTime? StarredAt { get; set; }

    /// <summary>
    /// 接收時間 (實際投遞時間)
    /// </summary>
    [Column("received_at")]
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 最後查看時間
    /// </summary>
    [Column("last_viewed_at")]
    public DateTime? LastViewedAt { get; set; }

    /// <summary>
    /// 查看次數
    /// </summary>
    [Column("view_count")]
    public int ViewCount { get; set; } = 0;

    /// <summary>
    /// 接收方式 (inbox/email/sms/push)
    /// </summary>
    [Column("delivery_method")]
    [StringLength(20)]
    public string DeliveryMethod { get; set; } = "inbox";

    /// <summary>
    /// 投遞狀態 (pending/delivered/failed)
    /// </summary>
    [Column("delivery_status")]
    [StringLength(20)]
    public string DeliveryStatus { get; set; } = "delivered";

    /// <summary>
    /// 投遞錯誤訊息
    /// </summary>
    [Column("delivery_error")]
    [StringLength(500)]
    public string? DeliveryError { get; set; }

    /// <summary>
    /// 導航屬性 - 通知
    /// </summary>
    [ForeignKey("NotificationId")]
    public virtual Notification Notification { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 接收者
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
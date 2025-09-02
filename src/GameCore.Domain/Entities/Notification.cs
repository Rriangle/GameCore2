using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 通知表
/// 對應資料庫 Notifications 表
/// </summary>
[Table("Notifications")]
public class Notification
{
    /// <summary>
    /// 通知ID (主鍵)
    /// </summary>
    [Key]
    public int NotificationId { get; set; }

    /// <summary>
    /// 來源ID
    /// </summary>
    public int SourceId { get; set; }

    /// <summary>
    /// 動作ID
    /// </summary>
    public int ActionId { get; set; }

    /// <summary>
    /// 發送者ID
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// 發送者管理員ID (可為空)
    /// </summary>
    public int? SenderManagerId { get; set; }

    /// <summary>
    /// 通知標題
    /// </summary>
    [StringLength(200)]
    public string? NotificationTitle { get; set; }

    /// <summary>
    /// 通知內容
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string NotificationMessage { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 群組ID (可為空)
    /// </summary>
    public int? GroupId { get; set; }

    // 導航屬性
    /// <summary>
    /// 通知來源
    /// </summary>
    public virtual NotificationSource Source { get; set; } = null!;

    /// <summary>
    /// 通知動作
    /// </summary>
    public virtual NotificationAction Action { get; set; } = null!;

    /// <summary>
    /// 發送者
    /// </summary>
    public virtual User Sender { get; set; } = null!;

    /// <summary>
    /// 群組
    /// </summary>
    public virtual Group? Group { get; set; }

    /// <summary>
    /// 通知接收者
    /// </summary>
    public virtual ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
} 
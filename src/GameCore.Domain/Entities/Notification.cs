using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 通知主表實體
/// 對應資料表: Notifications
/// </summary>
[Table("Notifications")]
public class Notification
{
    /// <summary>
    /// 通知編號 (主鍵)
    /// </summary>
    [Key]
    [Column("notification_id")]
    public int NotificationId { get; set; }

    /// <summary>
    /// 來源類型編號 (外鍵至 NotificationSource)
    /// </summary>
    [Required]
    [Column("source_id")]
    public int SourceId { get; set; }

    /// <summary>
    /// 行為類型編號 (外鍵至 NotificationAction)
    /// </summary>
    [Required]
    [Column("action_id")]
    public int ActionId { get; set; }

    /// <summary>
    /// 發送者編號 (外鍵至 User)
    /// </summary>
    [Required]
    [Column("sender_id")]
    public int SenderId { get; set; }

    /// <summary>
    /// 發送者管理員編號 (外鍵至 ManagerRole，可為空)
    /// </summary>
    [Column("sender_manager_id")]
    public int? SenderManagerId { get; set; }

    /// <summary>
    /// 通知標題
    /// </summary>
    [Column("notification_title")]
    [StringLength(200)]
    public string? NotificationTitle { get; set; }

    /// <summary>
    /// 通知內容
    /// </summary>
    [Column("notification_message")]
    public string? NotificationMessage { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 群組編號 (外鍵至 Group，若為群組相關)
    /// </summary>
    [Column("group_id")]
    public int? GroupId { get; set; }

    /// <summary>
    /// 通知類型 (info/warning/error/success)
    /// </summary>
    [Column("notification_type")]
    [StringLength(20)]
    public string NotificationType { get; set; } = "info";

    /// <summary>
    /// 通知優先級 (1-5，數值越高優先級越高)
    /// </summary>
    [Column("priority")]
    public int Priority { get; set; } = 3;

    /// <summary>
    /// 過期時間
    /// </summary>
    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// 相關資源類型 (post/thread/product等)
    /// </summary>
    [Column("related_type")]
    [StringLength(50)]
    public string? RelatedType { get; set; }

    /// <summary>
    /// 相關資源編號
    /// </summary>
    [Column("related_id")]
    public long? RelatedId { get; set; }

    /// <summary>
    /// 附加資料 (JSON格式)
    /// </summary>
    [Column("extra_data")]
    public string? ExtraData { get; set; }

    /// <summary>
    /// 是否為系統通知
    /// </summary>
    [Column("is_system")]
    public bool IsSystem { get; set; } = false;

    /// <summary>
    /// 導航屬性 - 通知來源
    /// </summary>
    [ForeignKey("SourceId")]
    public virtual NotificationSource Source { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 通知行為
    /// </summary>
    [ForeignKey("ActionId")]
    public virtual NotificationAction Action { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 發送者
    /// </summary>
    [ForeignKey("SenderId")]
    public virtual User Sender { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 相關群組
    /// </summary>
    [ForeignKey("GroupId")]
    public virtual Group? Group { get; set; }

    /// <summary>
    /// 導航屬性 - 通知接收者列表
    /// </summary>
    public virtual ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
}
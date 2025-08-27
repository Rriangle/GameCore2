using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 群組聊天實體
/// 對應資料表: Group_Chat
/// </summary>
[Table("Group_Chat")]
public class GroupChat
{
    /// <summary>
    /// 群組聊天編號 (主鍵)
    /// </summary>
    [Key]
    [Column("group_chat_id")]
    public int GroupChatId { get; set; }

    /// <summary>
    /// 群組編號 (外鍵至 Group)
    /// </summary>
    [Required]
    [Column("group_id")]
    public int GroupId { get; set; }

    /// <summary>
    /// 發送者編號 (外鍵至 User)
    /// </summary>
    [Required]
    [Column("sender_id")]
    public int SenderId { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    [Required]
    [Column("group_chat_content")]
    public string GroupChatContent { get; set; } = string.Empty;

    /// <summary>
    /// 發送時間
    /// </summary>
    [Required]
    [Column("sent_at")]
    public DateTime SentAt { get; set; }

    /// <summary>
    /// 是否寄送
    /// </summary>
    [Required]
    [Column("is_sent")]
    public bool IsSent { get; set; } = true;

    /// <summary>
    /// 訊息類型 (text/image/file/system/notice)
    /// </summary>
    [Column("message_type")]
    [StringLength(20)]
    public string MessageType { get; set; } = "text";

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
    /// 刪除者編號
    /// </summary>
    [Column("deleted_by")]
    public int? DeletedBy { get; set; }

    /// <summary>
    /// 是否已編輯
    /// </summary>
    [Column("is_edited")]
    public bool IsEdited { get; set; } = false;

    /// <summary>
    /// 編輯時間
    /// </summary>
    [Column("edited_at")]
    public DateTime? EditedAt { get; set; }

    /// <summary>
    /// 原始內容 (編輯前)
    /// </summary>
    [Column("original_content")]
    public string? OriginalContent { get; set; }

    /// <summary>
    /// 附件資料 (JSON格式)
    /// </summary>
    [Column("attachments")]
    public string? Attachments { get; set; }

    /// <summary>
    /// 回覆的訊息編號
    /// </summary>
    [Column("reply_to_message_id")]
    public int? ReplyToMessageId { get; set; }

    /// <summary>
    /// 提及的用戶 (JSON格式)
    /// </summary>
    [Column("mentioned_users")]
    public string? MentionedUsers { get; set; }

    /// <summary>
    /// 是否為置頂訊息
    /// </summary>
    [Column("is_pinned")]
    public bool IsPinned { get; set; } = false;

    /// <summary>
    /// 置頂時間
    /// </summary>
    [Column("pinned_at")]
    public DateTime? PinnedAt { get; set; }

    /// <summary>
    /// 置頂者編號
    /// </summary>
    [Column("pinned_by")]
    public int? PinnedBy { get; set; }

    /// <summary>
    /// IP位址
    /// </summary>
    [Column("ip_address")]
    [StringLength(45)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// 用戶代理
    /// </summary>
    [Column("user_agent")]
    [StringLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// 讀取狀態 (JSON格式，記錄哪些用戶已讀)
    /// </summary>
    [Column("read_status")]
    public string? ReadStatus { get; set; }

    /// <summary>
    /// 導航屬性 - 群組
    /// </summary>
    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 發送者
    /// </summary>
    [ForeignKey("SenderId")]
    public virtual User Sender { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 回覆的訊息
    /// </summary>
    [ForeignKey("ReplyToMessageId")]
    public virtual GroupChat? ReplyToMessage { get; set; }

    /// <summary>
    /// 導航屬性 - 回覆此訊息的訊息列表
    /// </summary>
    public virtual ICollection<GroupChat> Replies { get; set; } = new List<GroupChat>();

    /// <summary>
    /// 導航屬性 - 置頂者
    /// </summary>
    [ForeignKey("PinnedBy")]
    public virtual User? PinnedByUser { get; set; }

    /// <summary>
    /// 導航屬性 - 刪除者
    /// </summary>
    [ForeignKey("DeletedBy")]
    public virtual User? DeletedByUser { get; set; }
}
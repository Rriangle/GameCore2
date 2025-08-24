using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 聊天訊息實體 - 私訊系統
/// 對應資料表: Chat_Message
/// </summary>
[Table("Chat_Message")]
public class ChatMessage
{
    /// <summary>
    /// 訊息編號 (主鍵)
    /// </summary>
    [Key]
    [Column("message_id")]
    public int MessageId { get; set; }

    /// <summary>
    /// 管理員編號 (外鍵至 ManagerRole，客服用，可為空)
    /// </summary>
    [Column("manager_id")]
    public int? ManagerId { get; set; }

    /// <summary>
    /// 發送者編號 (外鍵至 User)
    /// </summary>
    [Required]
    [Column("sender_id")]
    public int SenderId { get; set; }

    /// <summary>
    /// 接收者編號 (外鍵至 User，可為空如果是群發)
    /// </summary>
    [Column("receiver_id")]
    public int? ReceiverId { get; set; }

    /// <summary>
    /// 訊息內容
    /// </summary>
    [Required]
    [Column("chat_content")]
    public string ChatContent { get; set; } = string.Empty;

    /// <summary>
    /// 發送時間
    /// </summary>
    [Required]
    [Column("sent_at")]
    public DateTime SentAt { get; set; }

    /// <summary>
    /// 是否已讀
    /// </summary>
    [Required]
    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// 是否寄送
    /// </summary>
    [Required]
    [Column("is_sent")]
    public bool IsSent { get; set; } = true;

    /// <summary>
    /// 已讀時間
    /// </summary>
    [Column("read_at")]
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// 訊息類型 (text/image/file/system)
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
    /// 導航屬性 - 發送者
    /// </summary>
    [ForeignKey("SenderId")]
    public virtual User Sender { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 接收者
    /// </summary>
    [ForeignKey("ReceiverId")]
    public virtual User? Receiver { get; set; }

    /// <summary>
    /// 導航屬性 - 回覆的訊息
    /// </summary>
    [ForeignKey("ReplyToMessageId")]
    public virtual ChatMessage? ReplyToMessage { get; set; }

    /// <summary>
    /// 導航屬性 - 回覆此訊息的訊息列表
    /// </summary>
    public virtual ICollection<ChatMessage> Replies { get; set; } = new List<ChatMessage>();
}
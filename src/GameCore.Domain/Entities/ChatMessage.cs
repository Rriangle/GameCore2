using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 聊天訊息表
/// 對應資料庫 Chat_Message 表
/// </summary>
[Table("Chat_Message")]
public class ChatMessage
{
    /// <summary>
    /// 訊息ID (主鍵)
    /// </summary>
    [Key]
    public int MessageId { get; set; }

    /// <summary>
    /// 管理員ID (可為空，用於客服)
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// 發送者ID
    /// </summary>
    public int SenderId { get; set; }

    /// <summary>
    /// 接收者ID (可為空)
    /// </summary>
    public int? ReceiverId { get; set; }

    /// <summary>
    /// 聊天內容
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string ChatContent { get; set; } = string.Empty;

    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 是否已讀
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// 是否已發送
    /// </summary>
    public bool IsSent { get; set; } = true;

    // 導航屬性
    /// <summary>
    /// 發送者
    /// </summary>
    public virtual User Sender { get; set; } = null!;

    /// <summary>
    /// 接收者
    /// </summary>
    public virtual User? Receiver { get; set; }
} 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 群組封鎖實體
/// 對應資料表: Group_Block
/// </summary>
[Table("Group_Block")]
public class GroupBlock
{
    /// <summary>
    /// 封鎖編號 (主鍵)
    /// </summary>
    [Key]
    [Column("block_id")]
    public int BlockId { get; set; }

    /// <summary>
    /// 群組編號 (外鍵至 Group)
    /// </summary>
    [Required]
    [Column("group_id")]
    public int GroupId { get; set; }

    /// <summary>
    /// 被封鎖者編號 (外鍵至 User)
    /// </summary>
    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    /// <summary>
    /// 封鎖者編號 (外鍵至 User)
    /// </summary>
    [Required]
    [Column("blocked_by")]
    public int BlockedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 封鎖原因
    /// </summary>
    [Column("block_reason")]
    [StringLength(500)]
    public string? BlockReason { get; set; }

    /// <summary>
    /// 封鎖類型 (temporary/permanent/warning)
    /// </summary>
    [Column("block_type")]
    [StringLength(20)]
    public string BlockType { get; set; } = "temporary";

    /// <summary>
    /// 封鎖至時間 (永久封鎖則為null)
    /// </summary>
    [Column("blocked_until")]
    public DateTime? BlockedUntil { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 解除封鎖時間
    /// </summary>
    [Column("unblocked_at")]
    public DateTime? UnblockedAt { get; set; }

    /// <summary>
    /// 解除封鎖者編號
    /// </summary>
    [Column("unblocked_by")]
    public int? UnblockedBy { get; set; }

    /// <summary>
    /// 解除封鎖原因
    /// </summary>
    [Column("unblock_reason")]
    [StringLength(500)]
    public string? UnblockReason { get; set; }

    /// <summary>
    /// 封鎖等級 (1-5，數值越高越嚴重)
    /// </summary>
    [Column("block_level")]
    public int BlockLevel { get; set; } = 1;

    /// <summary>
    /// 是否可申訴
    /// </summary>
    [Column("can_appeal")]
    public bool CanAppeal { get; set; } = true;

    /// <summary>
    /// 申訴次數
    /// </summary>
    [Column("appeal_count")]
    public int AppealCount { get; set; } = 0;

    /// <summary>
    /// 最後申訴時間
    /// </summary>
    [Column("last_appeal_at")]
    public DateTime? LastAppealAt { get; set; }

    /// <summary>
    /// 相關訊息編號 (導致封鎖的訊息)
    /// </summary>
    [Column("related_message_id")]
    public int? RelatedMessageId { get; set; }

    /// <summary>
    /// 附加資料 (JSON格式)
    /// </summary>
    [Column("extra_data")]
    public string? ExtraData { get; set; }

    /// <summary>
    /// 警告次數
    /// </summary>
    [Column("warning_count")]
    public int WarningCount { get; set; } = 0;

    /// <summary>
    /// 管理員備註
    /// </summary>
    [Column("admin_note")]
    [StringLength(1000)]
    public string? AdminNote { get; set; }

    /// <summary>
    /// 導航屬性 - 群組
    /// </summary>
    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 被封鎖者
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 封鎖者
    /// </summary>
    [ForeignKey("BlockedBy")]
    public virtual User BlockedByUser { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 解除封鎖者
    /// </summary>
    [ForeignKey("UnblockedBy")]
    public virtual User? UnblockedByUser { get; set; }

    /// <summary>
    /// 導航屬性 - 相關訊息
    /// </summary>
    [ForeignKey("RelatedMessageId")]
    public virtual GroupChat? RelatedMessage { get; set; }
}
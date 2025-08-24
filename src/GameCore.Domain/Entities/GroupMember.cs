using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 群組成員實體
/// 對應資料表: Group_Member
/// </summary>
[Table("Group_Member")]
public class GroupMember
{
    /// <summary>
    /// 群組編號 (複合主鍵之一，外鍵至 Group)
    /// </summary>
    [Key]
    [Column("group_id", Order = 0)]
    public int GroupId { get; set; }

    /// <summary>
    /// 使用者編號 (複合主鍵之一，外鍵至 User)
    /// </summary>
    [Key]
    [Column("user_id", Order = 1)]
    public int UserId { get; set; }

    /// <summary>
    /// 加入時間
    /// </summary>
    [Column("joined_at")]
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 是否為管理員
    /// </summary>
    [Required]
    [Column("is_admin")]
    public bool IsAdmin { get; set; } = false;

    /// <summary>
    /// 成員狀態 (active/inactive/pending/banned)
    /// </summary>
    [Column("member_status")]
    [StringLength(20)]
    public string MemberStatus { get; set; } = "active";

    /// <summary>
    /// 成員角色 (owner/admin/moderator/member)
    /// </summary>
    [Column("member_role")]
    [StringLength(20)]
    public string MemberRole { get; set; } = "member";

    /// <summary>
    /// 邀請者編號
    /// </summary>
    [Column("invited_by")]
    public int? InvitedBy { get; set; }

    /// <summary>
    /// 邀請時間
    /// </summary>
    [Column("invited_at")]
    public DateTime? InvitedAt { get; set; }

    /// <summary>
    /// 最後活動時間
    /// </summary>
    [Column("last_activity_at")]
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// 訊息數量
    /// </summary>
    [Column("message_count")]
    public int MessageCount { get; set; } = 0;

    /// <summary>
    /// 今日訊息數量
    /// </summary>
    [Column("today_message_count")]
    public int TodayMessageCount { get; set; } = 0;

    /// <summary>
    /// 是否靜音群組
    /// </summary>
    [Column("is_muted")]
    public bool IsMuted { get; set; } = false;

    /// <summary>
    /// 靜音至時間
    /// </summary>
    [Column("muted_until")]
    public DateTime? MutedUntil { get; set; }

    /// <summary>
    /// 個人群組備註
    /// </summary>
    [Column("personal_note")]
    [StringLength(500)]
    public string? PersonalNote { get; set; }

    /// <summary>
    /// 群組暱稱
    /// </summary>
    [Column("group_nickname")]
    [StringLength(50)]
    public string? GroupNickname { get; set; }

    /// <summary>
    /// 權限設定 (JSON格式)
    /// </summary>
    [Column("permissions")]
    public string? Permissions { get; set; }

    /// <summary>
    /// 離開時間
    /// </summary>
    [Column("left_at")]
    public DateTime? LeftAt { get; set; }

    /// <summary>
    /// 離開原因
    /// </summary>
    [Column("leave_reason")]
    [StringLength(200)]
    public string? LeaveReason { get; set; }

    /// <summary>
    /// 導航屬性 - 群組
    /// </summary>
    [ForeignKey("GroupId")]
    public virtual Group Group { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 使用者
    /// </summary>
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 邀請者
    /// </summary>
    [ForeignKey("InvitedBy")]
    public virtual User? Inviter { get; set; }
}
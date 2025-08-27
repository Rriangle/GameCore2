using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 群組實體
/// 對應資料表: Groups
/// </summary>
[Table("Groups")]
public class Group
{
    /// <summary>
    /// 群組編號 (主鍵)
    /// </summary>
    [Key]
    [Column("group_id")]
    public int GroupId { get; set; }

    /// <summary>
    /// 群組名稱
    /// </summary>
    [Required]
    [Column("group_name")]
    [StringLength(100)]
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// 建立者編號 (外鍵至 User)
    /// </summary>
    [Required]
    [Column("created_by")]
    public int CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 群組描述
    /// </summary>
    [Column("group_description")]
    [StringLength(1000)]
    public string? GroupDescription { get; set; }

    /// <summary>
    /// 群組圖標
    /// </summary>
    [Column("group_icon")]
    [StringLength(500)]
    public string? GroupIcon { get; set; }

    /// <summary>
    /// 群組類型 (public/private/secret)
    /// </summary>
    [Column("group_type")]
    [StringLength(20)]
    public string GroupType { get; set; } = "public";

    /// <summary>
    /// 群組狀態 (active/inactive/archived)
    /// </summary>
    [Column("group_status")]
    [StringLength(20)]
    public string GroupStatus { get; set; } = "active";

    /// <summary>
    /// 最大成員數
    /// </summary>
    [Column("max_members")]
    public int MaxMembers { get; set; } = 100;

    /// <summary>
    /// 當前成員數
    /// </summary>
    [Column("current_members")]
    public int CurrentMembers { get; set; } = 1;

    /// <summary>
    /// 是否需要審核加入
    /// </summary>
    [Column("require_approval")]
    public bool RequireApproval { get; set; } = false;

    /// <summary>
    /// 是否允許邀請
    /// </summary>
    [Column("allow_invite")]
    public bool AllowInvite { get; set; } = true;

    /// <summary>
    /// 群組標籤 (JSON格式)
    /// </summary>
    [Column("tags")]
    public string? Tags { get; set; }

    /// <summary>
    /// 群組規則
    /// </summary>
    [Column("rules")]
    public string? Rules { get; set; }

    /// <summary>
    /// 最後活動時間
    /// </summary>
    [Column("last_activity_at")]
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// 總訊息數
    /// </summary>
    [Column("total_messages")]
    public int TotalMessages { get; set; } = 0;

    /// <summary>
    /// 今日訊息數
    /// </summary>
    [Column("today_messages")]
    public int TodayMessages { get; set; } = 0;

    /// <summary>
    /// 更新時間
    /// </summary>
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 導航屬性 - 建立者
    /// </summary>
    [ForeignKey("CreatedBy")]
    public virtual User Creator { get; set; } = null!;

    /// <summary>
    /// 導航屬性 - 群組成員列表
    /// </summary>
    public virtual ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();

    /// <summary>
    /// 導航屬性 - 群組聊天訊息列表
    /// </summary>
    public virtual ICollection<GroupChat> GroupChats { get; set; } = new List<GroupChat>();

    /// <summary>
    /// 導航屬性 - 群組封鎖列表
    /// </summary>
    public virtual ICollection<GroupBlock> GroupBlocks { get; set; } = new List<GroupBlock>();

    /// <summary>
    /// 導航屬性 - 相關通知列表
    /// </summary>
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
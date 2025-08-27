using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 後台管理員實體 - 用於後台登入追蹤
/// 對應資料表: Admins
/// </summary>
[Table("Admins")]
public class Admin
{
    /// <summary>
    /// 管理員編號 (主鍵，外鍵至 ManagerRole.Manager_Id)
    /// </summary>
    [Key]
    [Column("manager_id")]
    public int ManagerId { get; set; }

    /// <summary>
    /// 上次登入時間，用於後台登入追蹤
    /// </summary>
    [Column("last_login")]
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// 登入次數
    /// </summary>
    [Column("LoginCount")]
    public int LoginCount { get; set; } = 0;

    /// <summary>
    /// 首次登入時間
    /// </summary>
    [Column("FirstLoginAt")]
    public DateTime? FirstLoginAt { get; set; }

    /// <summary>
    /// 上次登入IP
    /// </summary>
    [Column("LastLoginIp")]
    [StringLength(45)]
    public string? LastLoginIp { get; set; }

    /// <summary>
    /// 上次登入用戶代理
    /// </summary>
    [Column("LastUserAgent")]
    [StringLength(500)]
    public string? LastUserAgent { get; set; }

    /// <summary>
    /// 登入失敗次數
    /// </summary>
    [Column("FailedLoginCount")]
    public int FailedLoginCount { get; set; } = 0;

    /// <summary>
    /// 最後登入失敗時間
    /// </summary>
    [Column("LastFailedLoginAt")]
    public DateTime? LastFailedLoginAt { get; set; }

    /// <summary>
    /// 是否被鎖定
    /// </summary>
    [Column("IsLocked")]
    public bool IsLocked { get; set; } = false;

    /// <summary>
    /// 鎖定時間
    /// </summary>
    [Column("LockedAt")]
    public DateTime? LockedAt { get; set; }

    /// <summary>
    /// 鎖定到期時間
    /// </summary>
    [Column("LockedUntil")]
    public DateTime? LockedUntil { get; set; }

    /// <summary>
    /// 鎖定原因
    /// </summary>
    [Column("LockReason")]
    [StringLength(500)]
    public string? LockReason { get; set; }

    /// <summary>
    /// 會話Token (用於單點登入控制)
    /// </summary>
    [Column("SessionToken")]
    [StringLength(500)]
    public string? SessionToken { get; set; }

    /// <summary>
    /// 會話開始時間
    /// </summary>
    [Column("SessionStartAt")]
    public DateTime? SessionStartAt { get; set; }

    /// <summary>
    /// 會話到期時間
    /// </summary>
    [Column("SessionExpiresAt")]
    public DateTime? SessionExpiresAt { get; set; }

    /// <summary>
    /// 最後活動時間
    /// </summary>
    [Column("LastActivityAt")]
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// 活動頁面路徑
    /// </summary>
    [Column("LastPagePath")]
    [StringLength(500)]
    public string? LastPagePath { get; set; }

    /// <summary>
    /// 瀏覽器指紋
    /// </summary>
    [Column("BrowserFingerprint")]
    [StringLength(500)]
    public string? BrowserFingerprint { get; set; }

    /// <summary>
    /// 時區偏移 (分鐘)
    /// </summary>
    [Column("TimezoneOffset")]
    public int TimezoneOffset { get; set; } = 480; // 預設 UTC+8

    /// <summary>
    /// 偏好語言
    /// </summary>
    [Column("PreferredLanguage")]
    [StringLength(10)]
    public string PreferredLanguage { get; set; } = "zh-TW";

    /// <summary>
    /// 偏好主題
    /// </summary>
    [Column("PreferredTheme")]
    [StringLength(20)]
    public string PreferredTheme { get; set; } = "light";

    /// <summary>
    /// 創建時間
    /// </summary>
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    [Column("UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 導航屬性 - 管理者資料
    /// </summary>
    [ForeignKey("ManagerId")]
    public virtual ManagerData Manager { get; set; } = null!;
}
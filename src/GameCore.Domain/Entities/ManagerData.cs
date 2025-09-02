using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 管理者資料表
/// 對應資料庫 ManagerData 表
/// </summary>
[Table("ManagerData")]
public class ManagerData
{
    /// <summary>
    /// 管理者編號 (主鍵)
    /// </summary>
    [Key]
    public int ManagerID { get; set; }

    /// <summary>
    /// 管理者姓名
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ManagerName { get; set; } = string.Empty;

    /// <summary>
    /// 管理者帳號 (建議唯一)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ManagerAccount { get; set; } = string.Empty;

    /// <summary>
    /// 管理者密碼 (實務請存雜湊)
    /// </summary>
    [Required]
    [StringLength(255)]
    public string ManagerPassword { get; set; } = string.Empty;

    /// <summary>
    /// 管理者註冊時間
    /// </summary>
    public DateTime AdministratorRegistrationDate { get; set; }

    // 導航屬性
    /// <summary>
    /// 管理者的角色
    /// </summary>
    public virtual ICollection<ManagerRole> ManagerRoles { get; set; } = new List<ManagerRole>();

    /// <summary>
    /// 管理者的後台登入記錄
    /// </summary>
    public virtual ICollection<Admin> AdminLogins { get; set; } = new List<Admin>();

    /// <summary>
    /// 管理者建立的禁言項目
    /// </summary>
    public virtual ICollection<Mute> CreatedMutes { get; set; } = new List<Mute>();

    /// <summary>
    /// 管理者建立的樣式
    /// </summary>
    public virtual ICollection<Style> CreatedStyles { get; set; } = new List<Style>();

    /// <summary>
    /// 管理者發送的通知
    /// </summary>
    public virtual ICollection<Notification> SentNotifications { get; set; } = new List<Notification>();

    /// <summary>
    /// 管理者發送的聊天訊息
    /// </summary>
    public virtual ICollection<ChatMessage> SentChatMessages { get; set; } = new List<ChatMessage>();

    /// <summary>
    /// 管理者操作的商品審計日誌
    /// </summary>
    public virtual ICollection<ProductInfoAuditLog> ProductAuditLogs { get; set; } = new List<ProductInfoAuditLog>();
} 
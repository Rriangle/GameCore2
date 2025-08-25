using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者基本資料表
/// </summary>
[Table("Users")]
public class User
{
    /// <summary>
    /// 使用者編號 (主鍵)
    /// </summary>
    [Key]
    public int User_ID { get; set; }

    /// <summary>
    /// 使用者姓名 (必填，唯一)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string User_name { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號 (必填，唯一)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string User_Account { get; set; } = string.Empty;

    /// <summary>
    /// 使用者密碼 (必填，雜湊儲存)
    /// </summary>
    [Required]
    [StringLength(255)]
    public string User_Password { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件 (用於聯繫)
    /// </summary>
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 最後登入時間
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 是否驗證 Email
    /// </summary>
    public bool IsEmailVerified { get; set; } = false;

    // 導航屬性
    public virtual UserIntroduce? UserIntroduce { get; set; }
    public virtual UserRights? UserRights { get; set; }
    public virtual UserWallet? UserWallet { get; set; }
    public virtual MemberSalesProfile? MemberSalesProfile { get; set; }
    public virtual UserSalesInformation? UserSalesInformation { get; set; }
}

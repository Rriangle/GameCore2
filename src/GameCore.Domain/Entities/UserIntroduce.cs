using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者介紹資料表
/// 對應資料庫 User_Introduce 表
/// </summary>
[Table("User_Introduce")]
public class UserIntroduce
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵參考 Users.User_ID)
    /// </summary>
    [Key]
    public int User_ID { get; set; }

    /// <summary>
    /// 使用者暱稱 (必填，唯一)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string User_NickName { get; set; } = string.Empty;

    /// <summary>
    /// 性別 (必填)
    /// </summary>
    [Required]
    [StringLength(1)]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// 身分證字號 (必填，唯一)
    /// </summary>
    [Required]
    [StringLength(10)]
    public string IdNumber { get; set; } = string.Empty;

    /// <summary>
    /// 聯繫電話 (必填，唯一)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Cellphone { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件 (必填，唯一)
    /// </summary>
    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 地址 (必填)
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 出生年月日 (必填)
    /// </summary>
    [Required]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// 創建帳號日期 (必填)
    /// </summary>
    [Required]
    public DateTime Create_Account { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 頭像圖片 (可選，二進位資料)
    /// </summary>
    public byte[]? User_Picture { get; set; }

    /// <summary>
    /// 使用者自介 (可選，最多200字)
    /// </summary>
    [StringLength(200)]
    public string? User_Introduce { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的使用者資料
    /// </summary>
    [ForeignKey("User_ID")]
    public virtual User User { get; set; } = null!;
} 
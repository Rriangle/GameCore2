using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者介紹表 - 詳細個人資料
/// </summary>
[Table("User_Introduce")]
public class UserIntroduce
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵到 Users)
    /// </summary>
    [Key]
    [Column("User_ID")]
    public int User_ID { get; set; }

    /// <summary>
    /// 使用者暱稱 (必填，唯一)
    /// </summary>
    [Required]
    [Column("User_NickName")]
    [StringLength(100)]
    public string User_NickName { get; set; } = string.Empty;

    /// <summary>
    /// 性別
    /// </summary>
    [Required]
    [Column("Gender")]
    [StringLength(1)]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// 身分證字號 (必填，唯一)
    /// </summary>
    [Required]
    [Column("IdNumber")]
    [StringLength(20)]
    public string IdNumber { get; set; } = string.Empty;

    /// <summary>
    /// 聯繫電話 (必填，唯一)
    /// </summary>
    [Required]
    [Column("Cellphone")]
    [StringLength(20)]
    public string Cellphone { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件 (必填，唯一)
    /// </summary>
    [Required]
    [Column("Email")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 地址 (必填)
    /// </summary>
    [Required]
    [Column("Address")]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 出生年月日 (必填)
    /// </summary>
    [Required]
    [Column("DateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// 創建帳號日期 (必填)
    /// </summary>
    [Required]
    [Column("Create_Account")]
    public DateTime Create_Account { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 頭像圖片 (可為空，MAX)
    /// </summary>
    [Column("User_Picture")]
    public byte[]? User_Picture { get; set; }

    /// <summary>
    /// 使用者自介 (最多200字，可為空)
    /// </summary>
    [Column("User_Introduce")]
    [StringLength(200)]
    public string? User_Introduce { get; set; }

    // 導航屬性
    /// <summary>
    /// 所屬使用者
    /// </summary>
    [ForeignKey("User_ID")]
    public virtual User User { get; set; } = null!;
}
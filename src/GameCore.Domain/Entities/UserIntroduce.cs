using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者介紹資料表 - 存放用戶詳細個人資訊
/// 對應資料庫表格: User_Introduce
/// </summary>
[Table("User_Introduce")]
public class UserIntroduce
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵到 Users)
    /// </summary>
    [Key]
    public int User_ID { get; set; }

    /// <summary>
    /// 使用者暱稱 (唯一，不可為空)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string User_NickName { get; set; } = string.Empty;

    /// <summary>
    /// 性別 (M/F)
    /// </summary>
    [Required]
    [StringLength(1)]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// 身分證字號 (唯一，不可為空)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string IdNumber { get; set; } = string.Empty;

    /// <summary>
    /// 聯繫電話 (唯一，不可為空)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Cellphone { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件 (唯一，不可為空)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 出生年月日
    /// </summary>
    [Required]
    [Column(TypeName = "date")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// 創建帳號日期
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2")]
    public DateTime Create_Account { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 頭像圖片 (二進制數據)
    /// </summary>
    [Column(TypeName = "varbinary(max)")]
    public byte[]? User_Picture { get; set; }

    /// <summary>
    /// 使用者自介 (最多200字)
    /// </summary>
    [StringLength(200)]
    public string? User_Introduce { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯到使用者主檔
    /// </summary>
    [ForeignKey("User_ID")]
    public virtual User User { get; set; } = null!;
}
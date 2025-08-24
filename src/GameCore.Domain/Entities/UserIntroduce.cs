using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者介紹資料表 - 儲存使用者詳細個人資料
/// 對應資料庫表：User_Introduce
/// </summary>
[Table("User_Introduce")]
public class UserIntroduce
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵參考 Users.User_ID)
    /// </summary>
    [Key]
    public int UserId { get; set; }

    /// <summary>
    /// 使用者暱稱 (唯一，不可為空)
    /// </summary>
    [Required]
    [Column("User_NickName")]
    [StringLength(100)]
    public string UserNickName { get; set; } = string.Empty;

    /// <summary>
    /// 性別 (CHAR 型別)
    /// </summary>
    [Required]
    [Column(TypeName = "CHAR(1)")]
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
    /// 地址 (不可為空)
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 出生年月日 (DATE 型別，不可為空)
    /// </summary>
    [Required]
    [Column(TypeName = "DATE")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// 創建帳號日期 (DATETIME2 型別，不可為空)
    /// </summary>
    [Required]
    [Column("Create_Account", TypeName = "DATETIME2")]
    public DateTime CreateAccount { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 頭像圖片 (VARBINARY(MAX)，可為空)
    /// 注意：MAX 在 EF Core 中使用 byte[] 型別
    /// </summary>
    [Column("User_Picture")]
    public byte[]? UserPicture { get; set; }

    /// <summary>
    /// 使用者自介 (最大200字元，可為空)
    /// </summary>
    [StringLength(200)]
    [Column("User_Introduce")]
    public string? UserIntroduceText { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的使用者基本資料
    /// </summary>
    public virtual User User { get; set; } = null!;
}
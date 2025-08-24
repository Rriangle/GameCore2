using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者基本資料表 - 存放用戶登入帳號和基本資訊
/// 對應資料庫表格: Users
/// </summary>
[Table("Users")]
public class User
{
    /// <summary>
    /// 使用者編號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    public int User_ID { get; set; }

    /// <summary>
    /// 使用者姓名 (唯一，不可為空)
    /// </summary>
    [Required]
    [StringLength(100)]
    public string User_name { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號 (唯一，不可為空)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string User_Account { get; set; } = string.Empty;

    /// <summary>
    /// 使用者密碼 (雜湊後儲存)
    /// </summary>
    [Required]
    [StringLength(255)]
    public string User_Password { get; set; } = string.Empty;

    // 導航屬性
    /// <summary>
    /// 用戶詳細介紹資料 (一對一關係)
    /// </summary>
    public virtual UserIntroduce? UserIntroduce { get; set; }

    /// <summary>
    /// 用戶權限設定 (一對一關係)
    /// </summary>
    public virtual UserRights? UserRights { get; set; }

    /// <summary>
    /// 用戶錢包 (一對一關係)
    /// </summary>
    public virtual UserWallet? Wallet { get; set; }
}

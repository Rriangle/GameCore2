using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者基本資料表 - 儲存使用者核心登入資訊
/// 對應資料庫表：Users
/// </summary>
[Table("Users")]
public class User
{
    /// <summary>
    /// 使用者編號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    [Column("User_ID")]
    public int UserId { get; set; }

    /// <summary>
    /// 使用者姓名 (NVARCHAR 型別，不可為空，唯一)
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("User_name")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號 (NVARCHAR 型別，不可為空，唯一)
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("User_Account")]
    public string UserAccount { get; set; } = string.Empty;

    /// <summary>
    /// 使用者密碼 (NVARCHAR 型別，不可為空)
    /// 實際儲存雜湊後的密碼
    /// </summary>
    [Required]
    [StringLength(255)]
    [Column("User_Password")]
    public string UserPassword { get; set; } = string.Empty;

    // 導航屬性
    /// <summary>
    /// 一對一關聯：使用者詳細介紹資料
    /// </summary>
    public virtual UserIntroduce? UserIntroduce { get; set; }

    /// <summary>
    /// 一對一關聯：使用者權限設定
    /// </summary>
    public virtual UserRights? UserRights { get; set; }

    /// <summary>
    /// 一對一關聯：使用者錢包
    /// </summary>
    public virtual UserWallet? Wallet { get; set; }

    /// <summary>
    /// 一對一關聯：會員銷售資料 (可選，需申請開通)
    /// </summary>
    public virtual MemberSalesProfile? MemberSalesProfile { get; set; }

    /// <summary>
    /// 一對一關聯：使用者銷售資訊 (可選，需申請開通)
    /// </summary>
    public virtual UserSalesInformation? UserSalesInformation { get; set; }
}

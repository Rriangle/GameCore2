using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者基本資料表 - 主要登入資訊
/// </summary>
[Table("Users")]
public class User
{
    /// <summary>
    /// 使用者編號 (主鍵，自動遞增)
    /// </summary>
    [Key]
    [Column("User_ID")]
    public int User_ID { get; set; }

    /// <summary>
    /// 使用者姓名 (必填，唯一)
    /// </summary>
    [Required]
    [Column("User_name")]
    [StringLength(100)]
    public string User_name { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號 (必填，唯一)
    /// </summary>
    [Required]
    [Column("User_Account")]
    [StringLength(100)]
    public string User_Account { get; set; } = string.Empty;

    /// <summary>
    /// 使用者密碼 (雜湊儲存)
    /// </summary>
    [Required]
    [Column("User_Password")]
    [StringLength(255)]
    public string User_Password { get; set; } = string.Empty;

    // 導航屬性
    /// <summary>
    /// 使用者詳細介紹資料
    /// </summary>
    public virtual UserIntroduce? UserIntroduce { get; set; }

    /// <summary>
    /// 使用者權限設定
    /// </summary>
    public virtual UserRights? UserRights { get; set; }

    /// <summary>
    /// 使用者錢包
    /// </summary>
    public virtual UserWallet? UserWallet { get; set; }

    /// <summary>
    /// 銷售檔案（如果有開通銷售功能）
    /// </summary>
    public virtual MemberSalesProfile? MemberSalesProfile { get; set; }

    /// <summary>
    /// 銷售錢包資訊
    /// </summary>
    public virtual UserSalesInformation? UserSalesInformation { get; set; }

    /// <summary>
    /// 寵物
    /// </summary>
    public virtual Pet? Pet { get; set; }

    /// <summary>
    /// 簽到記錄
    /// </summary>
    public virtual ICollection<UserSignInStats> SignInStats { get; set; } = new List<UserSignInStats>();

    /// <summary>
    /// 小遊戲記錄
    /// </summary>
    public virtual ICollection<MiniGame> MiniGames { get; set; } = new List<MiniGame>();
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者錢包資料表 - 存放用戶點數和優惠券
/// 對應資料庫表格: User_wallet
/// </summary>
[Table("User_wallet")]
public class UserWallet
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵到 Users)
    /// </summary>
    [Key]
    public int User_Id { get; set; }

    /// <summary>
    /// 使用者點數餘額
    /// </summary>
    [Required]
    public int User_Point { get; set; } = 0;

    /// <summary>
    /// 優惠券編號 (可為空)
    /// </summary>
    [StringLength(50)]
    public string? Coupon_Number { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯到使用者主檔
    /// </summary>
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
}

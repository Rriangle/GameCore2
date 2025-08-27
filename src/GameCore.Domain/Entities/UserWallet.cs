using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者錢包表 - 管理用戶點數和優惠券
/// </summary>
[Table("User_wallet")]
public class UserWallet
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵到 Users)
    /// </summary>
    [Key]
    [Column("User_Id")]
    public int User_Id { get; set; }

    /// <summary>
    /// 使用者點數
    /// </summary>
    [Column("User_Point")]
    public int User_Point { get; set; } = 0;

    /// <summary>
    /// 優惠券編號
    /// </summary>
    [Column("Coupon_Number")]
    [StringLength(50)]
    public string? Coupon_Number { get; set; }

    // 導航屬性
    /// <summary>
    /// 所屬使用者
    /// </summary>
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
}

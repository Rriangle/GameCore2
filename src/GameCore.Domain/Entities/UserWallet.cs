using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者錢包表
/// 對應資料庫 User_wallet 表
/// </summary>
[Table("User_wallet")]
public class UserWallet
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵參考 Users.User_ID)
    /// </summary>
    [Key]
    public int User_Id { get; set; }

    /// <summary>
    /// 使用者點數餘額
    /// </summary>
    public int User_Point { get; set; } = 0;

    /// <summary>
    /// 使用者餘額 (與 User_Point 相同)
    /// </summary>
    public int User_Balance { get; set; } = 0;

    /// <summary>
    /// 優惠券編號 (可選)
    /// </summary>
    [StringLength(50)]
    public string? Coupon_Number { get; set; }

    /// <summary>
    /// 最後更新時間
    /// </summary>
    public DateTime Last_Updated { get; set; } = DateTime.UtcNow;

    // 導航屬性
    /// <summary>
    /// 關聯的使用者資料
    /// </summary>
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者錢包資料表 - 儲存使用者的點數和優惠券資訊
/// 對應資料庫表：User_wallet
/// </summary>
[Table("User_wallet")]
public class UserWallet
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵參考 Users.User_ID)
    /// </summary>
    [Key]
    [Column("User_Id")]
    public int UserId { get; set; }

    /// <summary>
    /// 使用者點數 (INT 型別)
    /// 注意：規格中欄位名稱為 User_Point，但註釋寫「使用者編號」應為筆誤，實際應為使用者點數
    /// </summary>
    [Column("User_Point")]
    public int UserPoint { get; set; } = 0;

    /// <summary>
    /// 優惠券編號 (VARCHAR 型別，可為空)
    /// </summary>
    [StringLength(50)]
    [Column("Coupon_Number")]
    public string? CouponNumber { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的使用者基本資料
    /// </summary>
    public virtual User User { get; set; } = null!;
}

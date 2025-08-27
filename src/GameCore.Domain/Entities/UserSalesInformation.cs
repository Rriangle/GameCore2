using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者銷售資訊資料表 - 儲存使用者的銷售錢包資訊
/// 對應資料庫表：User_Sales_Information
/// </summary>
[Table("User_Sales_Information")]
public class UserSalesInformation
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵參考 Users.User_ID)
    /// </summary>
    [Key]
    [Column("User_Id")]
    public int UserId { get; set; }

    /// <summary>
    /// 使用者銷售錢包 (INT 型別)
    /// 儲存使用者透過銷售獲得的收入金額
    /// </summary>
    public int UserSalesWallet { get; set; } = 0;

    // 導航屬性
    /// <summary>
    /// 關聯的使用者基本資料
    /// </summary>
    public virtual User User { get; set; } = null!;
}
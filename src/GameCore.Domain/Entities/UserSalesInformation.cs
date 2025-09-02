using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者銷售資訊表
/// 對應資料庫 User_Sales_Information 表
/// </summary>
[Table("User_Sales_Information")]
public class UserSalesInformation
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵參考 Users.User_ID)
    /// </summary>
    [Key]
    public int User_Id { get; set; }

    /// <summary>
    /// 使用者銷售錢包餘額
    /// </summary>
    public int UserSales_Wallet { get; set; } = 0;

    /// <summary>
    /// 總訂單數
    /// </summary>
    public int Total_Orders { get; set; } = 0;

    /// <summary>
    /// 總收入
    /// </summary>
    public decimal Total_Revenue { get; set; } = 0;

    /// <summary>
    /// 最後訂單日期
    /// </summary>
    public DateTime? Last_Order_Date { get; set; }

    /// <summary>
    /// 客戶數量
    /// </summary>
    public int Customer_Count { get; set; } = 0;

    /// <summary>
    /// 創建時間
    /// </summary>
    public DateTime Created_At { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime Updated_At { get; set; } = DateTime.UtcNow;

    // 導航屬性
    /// <summary>
    /// 關聯的使用者資料
    /// </summary>
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
} 
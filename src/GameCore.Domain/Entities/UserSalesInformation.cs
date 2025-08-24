using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者銷售資訊表 - 銷售錢包
/// </summary>
[Table("User_Sales_Information")]
public class UserSalesInformation
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵到 Users)
    /// </summary>
    [Key]
    [Column("User_Id")]
    public int User_Id { get; set; }

    /// <summary>
    /// 使用者銷售錢包
    /// </summary>
    [Column("UserSales_Wallet")]
    public int UserSales_Wallet { get; set; } = 0;

    // 導航屬性
    /// <summary>
    /// 所屬使用者
    /// </summary>
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
}
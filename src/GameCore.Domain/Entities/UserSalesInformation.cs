using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者銷售資訊表
/// </summary>
[Table("User_Sales_Information")]
public class UserSalesInformation
{
    /// <summary>
    /// 使用者編號 (主鍵，外鍵到 Users)
    /// </summary>
    [Key]
    public int User_Id { get; set; }

    /// <summary>
    /// 使用者銷售錢包餘額
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UserSales_Wallet { get; set; } = 0.00m;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 導航屬性
    [ForeignKey("User_Id")]
    public virtual User User { get; set; } = null!;
}
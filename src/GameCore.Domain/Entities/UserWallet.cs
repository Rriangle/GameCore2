using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者錢包表
/// </summary>
[Table("User_wallet")]
public class UserWallet
{
    /// <summary>
    /// 錢包 ID (主鍵)
    /// </summary>
    [Key]
    public int WalletId { get; set; }

    /// <summary>
    /// 使用者編號 (外鍵)
    /// </summary>
    [Required]
    public int User_Id { get; set; }

    /// <summary>
    /// 使用者點數餘額
    /// </summary>
    [Required]
    [Column(TypeName = "int")]
    public int User_Point { get; set; } = 0; // 初始點數 0

    /// <summary>
    /// 優惠券編號
    /// </summary>
    [StringLength(100)]
    public string? Coupon_Number { get; set; }

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

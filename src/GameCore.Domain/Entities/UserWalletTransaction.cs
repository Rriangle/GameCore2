using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

/// <summary>
/// 使用者錢包交易記錄表
/// 對應資料庫 UserWalletTransaction 表
/// </summary>
[Table("UserWalletTransaction")]
public class UserWalletTransaction
{
    /// <summary>
    /// 交易記錄 ID (主鍵，自動遞增)
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TransactionID { get; set; }

    /// <summary>
    /// 使用者編號 (外鍵參考 Users.User_ID)
    /// </summary>
    [Required]
    public int UserID { get; set; }

    /// <summary>
    /// 交易類型 (Deposit/Withdraw/Transfer/Refund)
    /// </summary>
    [Required]
    [StringLength(20)]
    public string TransactionType { get; set; } = string.Empty;

    /// <summary>
    /// 交易金額
    /// </summary>
    [Required]
    public int Amount { get; set; }

    /// <summary>
    /// 交易前餘額
    /// </summary>
    [Required]
    public int BalanceBefore { get; set; }

    /// <summary>
    /// 交易後餘額
    /// </summary>
    [Required]
    public int BalanceAfter { get; set; }

    /// <summary>
    /// 交易描述
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 交易時間
    /// </summary>
    [Required]
    public DateTime TransactionTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 相關訂單ID (可選)
    /// </summary>
    public int? RelatedOrderID { get; set; }

    // 導航屬性
    /// <summary>
    /// 關聯的使用者資料
    /// </summary>
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;
} 
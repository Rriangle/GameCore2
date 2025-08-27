using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GameCore.Domain.Enums;

namespace GameCore.Domain.Entities;

[Table("WalletTransactions")]
public class WalletTransaction
{
    [Key]
    public int TransactionId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public TransactionType Type { get; set; }

    [Required]
    public TransactionStatus Status { get; set; } = TransactionStatus.Completed;

    // 導航屬性
    public virtual User User { get; set; } = null!;
}

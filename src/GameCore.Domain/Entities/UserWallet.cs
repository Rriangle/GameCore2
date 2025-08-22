using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities;

[Table("User_wallet")]
public class UserWallet
{
    [Key]
    public int WalletId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; } = 100.00m; // 初始點數 100

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // 導航屬性
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}

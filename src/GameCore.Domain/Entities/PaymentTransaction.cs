using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameCore.Domain.Entities
{
    [Table("PaymentTransactions")]
    public class PaymentTransaction
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TransactionId { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        
        [Required]
        public PaymentStatus Status { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string PaymentDetails { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string ErrorMessage { get; set; } = string.Empty;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ProcessedAt { get; set; }
        
        public DateTime? FailedAt { get; set; }
        
        // Navigation properties
        public virtual Order Order { get; set; } = null!;
    }
    
    public enum PaymentMethod
    {
        CreditCard = 0,
        DebitCard = 1,
        PayPal = 2,
        BankTransfer = 3,
        DigitalWallet = 4,
        Cryptocurrency = 5
    }
}
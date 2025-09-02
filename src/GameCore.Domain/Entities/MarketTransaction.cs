using System.ComponentModel.DataAnnotations;

namespace GameCore.Domain.Entities
{
    public class MarketTransaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int MarketItemId { get; set; }

        [Required]
        public int SellerId { get; set; }

        [Required]
        public int BuyerId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal TotalAmount { get; set; }

        [Required]
        [Range(0, 999999.99)]
        public decimal FeeAmount { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal SellerAmount { get; set; }

        [Required]
        public MarketTransactionStatus Status { get; set; } = MarketTransactionStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        // 導航屬性
        public virtual MarketItem MarketItem { get; set; } = null!;
        public virtual User Seller { get; set; } = null!;
        public virtual User Buyer { get; set; } = null!;
    }

    public enum MarketTransactionStatus
    {
        Pending = 1,
        Completed = 2,
        Cancelled = 3,
        Failed = 4
    }
}

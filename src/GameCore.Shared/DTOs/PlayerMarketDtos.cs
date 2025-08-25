using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs
{
    public class CreatePlayerMarketListingDto
    {
        [Required]
        public int SellerId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        [StringLength(100)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [Required]
        public bool IsNegotiable { get; set; } = false;
        
        public DateTime? ExpiresAt { get; set; }
    }
    
    public class UpdatePlayerMarketListingDto
    {
        [StringLength(200)]
        public string? Title { get; set; }
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal? Price { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int? Quantity { get; set; }
        
        [StringLength(100)]
        public string? ImageUrl { get; set; }
        
        public bool? IsNegotiable { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
    }
    
    public class PlayerMarketListingResponseDto
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsNegotiable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
    
    public class CreatePlayerMarketOrderDto
    {
        [Required]
        public int ListingId { get; set; }
        
        [Required]
        public int BuyerId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        [StringLength(500)]
        public string BuyerNotes { get; set; } = string.Empty;
    }
    
    public class PlayerMarketOrderResponseDto
    {
        public int Id { get; set; }
        public int ListingId { get; set; }
        public string ListingTitle { get; set; } = string.Empty;
        public int BuyerId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal SellerAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string BuyerNotes { get; set; } = string.Empty;
        public string SellerNotes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
    }
    
    public class PlayerMarketStatsDto
    {
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal PlatformFees { get; set; }
    }
}
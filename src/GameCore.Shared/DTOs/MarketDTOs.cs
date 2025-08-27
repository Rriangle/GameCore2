using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs
{
    public class MarketResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public static MarketResult SuccessResult(object data)
        {
            return new MarketResult
            {
                Success = true,
                Data = data
            };
        }

        public static MarketResult Failure(string message)
        {
            return new MarketResult
            {
                Success = false,
                Message = message
            };
        }
    }

    public class ListItemRequest
    {
        [Required]
        public int SellerId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }

    public class BuyItemRequest
    {
        [Required]
        public int BuyerId { get; set; }

        [Required]
        public int MarketItemId { get; set; }

        [Required]
        [Range(1, 999)]
        public int Quantity { get; set; }
    }

    public class MarketSearchRequest
    {
        public string? Keyword { get; set; }
        public int? ProductId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? SellerId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class MarketListResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<MarketItemDto>? Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public static MarketListResult SuccessResult(List<MarketItemDto> items, int totalCount, int page, int pageSize)
        {
            return new MarketListResult
            {
                Success = true,
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public static MarketListResult Failure(string message)
        {
            return new MarketListResult
            {
                Success = false,
                Message = message
            };
        }
    }

    public class MarketItemDto
    {
        public int MarketItemId { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class TransactionListResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<MarketTransactionDto>? Transactions { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public static TransactionListResult SuccessResult(List<MarketTransactionDto> transactions, int totalCount, int page, int pageSize)
        {
            return new TransactionListResult
            {
                Success = true,
                Transactions = transactions,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public static TransactionListResult Failure(string message)
        {
            return new TransactionListResult
            {
                Success = false,
                Message = message
            };
        }
    }

    public class MarketTransactionDto
    {
        public int TransactionId { get; set; }
        public int MarketItemId { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public int BuyerId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal SellerAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

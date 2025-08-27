using System.ComponentModel.DataAnnotations;

namespace GameCore.Shared.DTOs
{
    public class WalletResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal? Balance { get; set; }
        public string? Currency { get; set; } = "TWD";

        public static WalletResult SuccessResult(decimal balance)
        {
            return new WalletResult
            {
                Success = true,
                Balance = balance
            };
        }

        public static WalletResult Failure(string message)
        {
            return new WalletResult
            {
                Success = false,
                Message = message
            };
        }
    }

    public class WalletTransactionListResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public static WalletTransactionListResult SuccessResult(List<TransactionDto> transactions, int totalCount, int page, int pageSize)
        {
            return new WalletTransactionListResult
            {
                Success = true,
                Transactions = transactions,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public static WalletTransactionListResult Failure(string message)
        {
            return new WalletTransactionListResult
            {
                Success = false,
                Message = message
            };
        }
    }

    public class TransactionDto
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class TransferRequest
    {
        [Required]
        public int TargetUserId { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
    }
}

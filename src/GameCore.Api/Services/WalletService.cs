using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace GameCore.Api.Services;

/// <summary>
/// 錢包服務實現 - 優化版本
/// 增強安全性、輸入驗證、錯誤處理和性能
/// </summary>
public class WalletService : IWalletService
{
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly IUserSalesInformationRepository _userSalesInformationRepository;
    private readonly IMemberSalesProfileRepository _memberSalesProfileRepository;
    private readonly ILogger<WalletService> _logger;
    private readonly IMemoryCache _memoryCache;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;
    private const int CacheExpirationMinutes = 5;
    private const string WalletBalanceCacheKey = "WalletBalance_{0}";
    private const string SalesBalanceCacheKey = "SalesBalance_{0}";

    public WalletService(
        IUserWalletRepository userWalletRepository,
        IUserSalesInformationRepository userSalesInformationRepository,
        IMemberSalesProfileRepository memberSalesProfileRepository,
        ILogger<WalletService> logger,
        IMemoryCache memoryCache)
    {
        _userWalletRepository = userWalletRepository ?? throw new ArgumentNullException(nameof(userWalletRepository));
        _userSalesInformationRepository = userSalesInformationRepository ?? throw new ArgumentNullException(nameof(userSalesInformationRepository));
        _memberSalesProfileRepository = memberSalesProfileRepository ?? throw new ArgumentNullException(nameof(memberSalesProfileRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    /// <summary>
    /// 獲取用戶錢包餘額 - 優化版本（含快取）
    /// </summary>
    public async Task<int> GetUserWalletBalanceAsync(int userId)
    {
        _logger.LogInformation("開始獲取用戶錢包餘額，用戶ID: {UserId}", userId);

        try
        {
            // 輸入驗證
            if (userId <= 0)
            {
                _logger.LogWarning("無效的用戶ID: {UserId}", userId);
                return 0;
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(WalletBalanceCacheKey, userId);
            if (_memoryCache.TryGetValue(cacheKey, out int cachedBalance))
            {
                _logger.LogDebug("從快取獲取錢包餘額，用戶ID: {UserId}, 餘額: {Balance}", userId, cachedBalance);
                return cachedBalance;
            }

            // 從資料庫獲取
            var userWallet = await _userWalletRepository.GetByUserIdAsync(userId);
            if (userWallet == null)
            {
                _logger.LogWarning("用戶錢包不存在，用戶ID: {UserId}", userId);
                return 0;
            }

            var balance = userWallet.User_Point;

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, balance, cacheOptions);

            _logger.LogInformation("成功獲取用戶錢包餘額，用戶ID: {UserId}, 餘額: {Balance}", userId, balance);
            return balance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶錢包餘額時發生錯誤，用戶ID: {UserId}", userId);
            return 0;
        }
    }

    /// <summary>
    /// 獲取銷售錢包餘額 - 優化版本（含快取）
    /// </summary>
    public async Task<decimal> GetSalesWalletBalanceAsync(int userId)
    {
        _logger.LogInformation("開始獲取銷售錢包餘額，用戶ID: {UserId}", userId);

        try
        {
            // 輸入驗證
            if (userId <= 0)
            {
                _logger.LogWarning("無效的用戶ID: {UserId}", userId);
                return 0m;
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(SalesBalanceCacheKey, userId);
            if (_memoryCache.TryGetValue(cacheKey, out decimal cachedBalance))
            {
                _logger.LogDebug("從快取獲取銷售錢包餘額，用戶ID: {UserId}, 餘額: {Balance}", userId, cachedBalance);
                return cachedBalance;
            }

            // 從資料庫獲取
            var salesInfo = await _userSalesInformationRepository.GetByUserIdAsync(userId);
            if (salesInfo == null)
            {
                _logger.LogWarning("銷售錢包不存在，用戶ID: {UserId}", userId);
                return 0m;
            }

            var balance = salesInfo.UserSales_Wallet;

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, balance, cacheOptions);

            _logger.LogInformation("成功獲取銷售錢包餘額，用戶ID: {UserId}, 餘額: {Balance}", userId, balance);
            return balance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取銷售錢包餘額時發生錯誤，用戶ID: {UserId}", userId);
            return 0m;
        }
    }

    /// <summary>
    /// 獲取用戶完整錢包資訊 - 新增方法，提高性能
    /// </summary>
    public async Task<WalletSummaryDto> GetWalletSummaryAsync(int userId)
    {
        _logger.LogInformation("開始獲取用戶完整錢包資訊，用戶ID: {UserId}", userId);

        try
        {
            // 輸入驗證
            if (userId <= 0)
            {
                _logger.LogWarning("無效的用戶ID: {UserId}", userId);
                return CreateEmptyWalletSummary(userId);
            }

            // 並行獲取錢包資訊，提高性能
            var tasks = new[]
            {
                GetUserWalletBalanceAsync(userId),
                GetSalesWalletBalanceAsync(userId),
                GetSalesProfileStatusAsync(userId)
            };

            var results = await Task.WhenAll(tasks);
            var userBalance = results[0];
            var salesBalance = results[1];
            var salesProfileStatus = results[2];

            var summary = new WalletSummaryDto
            {
                UserId = userId,
                UserWalletBalance = userBalance,
                SalesWalletBalance = salesBalance,
                SalesProfileStatus = salesProfileStatus,
                LastUpdated = DateTime.UtcNow
            };

            _logger.LogInformation("成功獲取用戶完整錢包資訊，用戶ID: {UserId}", userId);
            return summary;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶完整錢包資訊時發生錯誤，用戶ID: {UserId}", userId);
            return CreateEmptyWalletSummary(userId);
        }
    }

    /// <summary>
    /// 獲取用戶錢包交易記錄 - 優化版本
    /// </summary>
    public async Task<PaginatedWalletTransactionsDto> GetUserWalletTransactionsAsync(int userId, int page, int pageSize)
    {
        _logger.LogInformation("開始獲取用戶錢包交易記錄，用戶ID: {UserId}, 頁碼: {Page}, 頁面大小: {PageSize}", userId, page, pageSize);

        try
        {
            // 輸入驗證
            var validationResult = ValidatePaginationParameters(page, pageSize);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("分頁參數驗證失敗: {UserId}, 錯誤: {Error}", userId, validationResult.ErrorMessage);
                return CreateEmptyPaginatedTransactions();
            }

            // 獲取交易記錄
            var transactions = await _userWalletRepository.GetTransactionsByUserIdAsync(userId, page, pageSize);
            if (transactions == null || !transactions.Any())
            {
                _logger.LogInformation("用戶錢包無交易記錄，用戶ID: {UserId}", userId);
                return CreateEmptyPaginatedTransactions();
            }

            // 獲取總記錄數（用於分頁）
            var totalCount = await _userWalletRepository.GetTransactionCountByUserIdAsync(userId);

            var transactionDtos = transactions.Select(MapToWalletTransactionDto).ToList();

            var result = new PaginatedWalletTransactionsDto
            {
                Transactions = transactionDtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = page < (int)Math.Ceiling((double)totalCount / pageSize),
                HasPreviousPage = page > 1
            };

            _logger.LogInformation("成功獲取用戶錢包交易記錄，用戶ID: {UserId}, 記錄數: {Count}", userId, transactionDtos.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶錢包交易記錄時發生錯誤，用戶ID: {UserId}", userId);
            return CreateEmptyPaginatedTransactions();
        }
    }

    /// <summary>
    /// 獲取銷售錢包交易記錄 - 優化版本
    /// </summary>
    public async Task<PaginatedSalesWalletTransactionsDto> GetSalesWalletTransactionsAsync(int userId, int page, int pageSize)
    {
        _logger.LogInformation("開始獲取銷售錢包交易記錄，用戶ID: {UserId}, 頁碼: {Page}, 頁面大小: {PageSize}", userId, page, pageSize);

        try
        {
            // 輸入驗證
            var validationResult = ValidatePaginationParameters(page, pageSize);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("分頁參數驗證失敗: {UserId}, 錯誤: {Error}", userId, validationResult.ErrorMessage);
                return CreateEmptyPaginatedSalesTransactions();
            }

            // 獲取交易記錄
            var transactions = await _userSalesInformationRepository.GetTransactionsByUserIdAsync(userId, page, pageSize);
            if (transactions == null || !transactions.Any())
            {
                _logger.LogInformation("銷售錢包無交易記錄，用戶ID: {UserId}", userId);
                return CreateEmptyPaginatedSalesTransactions();
            }

            // 獲取總記錄數（用於分頁）
            var totalCount = await _userSalesInformationRepository.GetTransactionCountByUserIdAsync(userId);

            var transactionDtos = transactions.Select(MapToSalesWalletTransactionDto).ToList();

            var result = new PaginatedSalesWalletTransactionsDto
            {
                Transactions = transactionDtos,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasNextPage = page < (int)Math.Ceiling((double)totalCount / pageSize),
                HasPreviousPage = page > 1
            };

            _logger.LogInformation("成功獲取銷售錢包交易記錄，用戶ID: {UserId}, 記錄數: {Count}", userId, transactionDtos.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取銷售錢包交易記錄時發生錯誤，用戶ID: {UserId}", userId);
            return CreateEmptyPaginatedSalesTransactions();
        }
    }

    /// <summary>
    /// 清除錢包快取 - 新增方法，用於資料更新後清除快取
    /// </summary>
    public void ClearWalletCache(int userId)
    {
        try
        {
            var userWalletCacheKey = string.Format(WalletBalanceCacheKey, userId);
            var salesWalletCacheKey = string.Format(SalesBalanceCacheKey, userId);

            _memoryCache.Remove(userWalletCacheKey);
            _memoryCache.Remove(salesWalletCacheKey);

            _logger.LogInformation("已清除用戶錢包快取，用戶ID: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除用戶錢包快取時發生錯誤，用戶ID: {UserId}", userId);
        }
    }

    #region 私有方法

    /// <summary>
    /// 驗證分頁參數 - 新增方法，提高可讀性
    /// </summary>
    private ValidationResult ValidatePaginationParameters(int page, int pageSize)
    {
        var result = new ValidationResult();

        if (page <= 0)
            result.AddError("頁碼必須大於0");

        if (pageSize <= 0)
            result.AddError("頁面大小必須大於0");

        if (pageSize > MaxPageSize)
            result.AddError($"頁面大小不能超過{MaxPageSize}");

        return result;
    }

    /// <summary>
    /// 獲取銷售檔案狀態 - 新增方法
    /// </summary>
    private async Task<string> GetSalesProfileStatusAsync(int userId)
    {
        try
        {
            var salesProfile = await _memberSalesProfileRepository.GetByUserIdAsync(userId);
            return salesProfile != null ? "已開通" : "未開通";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取銷售檔案狀態時發生錯誤，用戶ID: {UserId}", userId);
            return "未知";
        }
    }

    /// <summary>
    /// 映射到錢包交易DTO - 新增方法
    /// </summary>
    private WalletTransactionDto MapToWalletTransactionDto(dynamic transaction)
    {
        return new WalletTransactionDto
        {
            TransactionId = transaction.TransactionId,
            UserId = transaction.UserId,
            TransactionType = transaction.TransactionType,
            Amount = transaction.Amount,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt
        };
    }

    /// <summary>
    /// 映射到銷售錢包交易DTO - 新增方法
    /// </summary>
    private SalesWalletTransactionDto MapToSalesWalletTransactionDto(dynamic transaction)
    {
        return new SalesWalletTransactionDto
        {
            TransactionId = transaction.TransactionId,
            UserId = transaction.UserId,
            TransactionType = transaction.TransactionType,
            Amount = transaction.Amount,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt
        };
    }

    /// <summary>
    /// 建立空的錢包摘要 - 新增方法
    /// </summary>
    private WalletSummaryDto CreateEmptyWalletSummary(int userId)
    {
        return new WalletSummaryDto
        {
            UserId = userId,
            UserWalletBalance = 0,
            SalesWalletBalance = 0m,
            SalesProfileStatus = "未知",
            LastUpdated = DateTime.UtcNow
        };
    }

    /// <summary>
    /// 建立空的分頁交易記錄 - 新增方法
    /// </summary>
    private PaginatedWalletTransactionsDto CreateEmptyPaginatedTransactions()
    {
        return new PaginatedWalletTransactionsDto
        {
            Transactions = new List<WalletTransactionDto>(),
            Page = 1,
            PageSize = DefaultPageSize,
            TotalCount = 0,
            TotalPages = 0,
            HasNextPage = false,
            HasPreviousPage = false
        };
    }

    /// <summary>
    /// 建立空的分頁銷售交易記錄 - 新增方法
    /// </summary>
    private PaginatedSalesWalletTransactionsDto CreateEmptyPaginatedSalesTransactions()
    {
        return new PaginatedSalesWalletTransactionsDto
        {
            Transactions = new List<SalesWalletTransactionDto>(),
            Page = 1,
            PageSize = DefaultPageSize,
            TotalCount = 0,
            TotalPages = 0,
            HasNextPage = false,
            HasPreviousPage = false
        };
    }

    #endregion
}
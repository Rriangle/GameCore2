using GameCore.Domain.Entities;
using GameCore.Domain.Enums;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using GameCore.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Transactions;

namespace GameCore.Api.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUserWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WalletService> _logger;

        public WalletService(
            IUserWalletRepository walletRepository,
            IUserRepository userRepository,
            IMemoryCache cache,
            ILogger<WalletService> logger)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<WalletResult> GetBalanceAsync(int userId)
        {
            try
            {
                var cacheKey = $"wallet_balance_{userId}";

                // 嘗試從快取獲取
                if (_cache.TryGetValue(cacheKey, out decimal cachedBalance))
                {
                    return WalletResult.SuccessResult(cachedBalance);
                }

                var wallet = await _walletRepository.GetByUserIdAsync(userId);
                if (wallet == null)
                {
                    return WalletResult.Failure("錢包不存在");
                }

                // 快取餘額 5 分鐘
                _cache.Set(cacheKey, wallet.Balance, TimeSpan.FromMinutes(5));

                return WalletResult.SuccessResult(wallet.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取錢包餘額失敗: {UserId}", userId);
                return WalletResult.Failure("獲取餘額失敗");
            }
        }

        public async Task<WalletResult> AddFundsAsync(int userId, decimal amount, string description = "充值")
        {
            // 輸入驗證
            if (amount <= 0)
            {
                return WalletResult.Failure("充值金額必須大於 0");
            }

            if (amount > 10000)
            {
                return WalletResult.Failure("單次充值金額不能超過 10,000");
            }

            // 防止惡意輸入
            if (ContainsSqlInjection(description) || ContainsXss(description))
            {
                _logger.LogWarning("檢測到惡意輸入嘗試: {UserId}, {Description}", userId, description);
                return WalletResult.Failure("無效的輸入格式");
            }

            // 檢查用戶是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return WalletResult.Failure("用戶不存在");
            }

            if (!user.IsActive)
            {
                return WalletResult.Failure("用戶帳戶已被停用");
            }

            // 檢查充值頻率限制
            var cacheKey = $"deposit_limit_{userId}";
            var recentDeposits = await GetRecentDepositsAsync(cacheKey);
            if (recentDeposits >= 5)
            {
                _logger.LogWarning("充值頻率過高: {UserId}, 最近充值次數: {Count}", userId, recentDeposits);
                return WalletResult.Failure("充值頻率過高，請稍後再試");
            }

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);
                if (wallet == null)
                {
                    return WalletResult.Failure("錢包不存在");
                }

                // 檢查餘額上限
                if (wallet.Balance + amount > 100000)
                {
                    return WalletResult.Failure("錢包餘額不能超過 100,000");
                }

                // 創建交易記錄
                var transactionRecord = new WalletTransaction
                {
                    UserId = userId,
                    Amount = amount,
                    Type = TransactionType.Deposit,
                    Description = description,
                    CreatedAt = DateTime.UtcNow,
                    Status = GameCore.Domain.Enums.TransactionStatus.Completed
                };

                await _walletRepository.AddTransactionAsync(transactionRecord);

                // 更新錢包餘額
                wallet.Balance += amount;
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);

                await _walletRepository.SaveChangesAsync();
                transaction.Complete();

                // 清除快取
                var balanceCacheKey = $"wallet_balance_{userId}";
                _cache.Remove(balanceCacheKey);

                // 更新充值頻率記錄
                await IncrementRecentDepositsAsync(cacheKey);

                _logger.LogInformation("充值成功: {UserId}, 金額: {Amount}, 新餘額: {NewBalance}",
                    userId, amount, wallet.Balance);

                return WalletResult.SuccessResult(wallet.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "充值失敗: {UserId}, 金額: {Amount}", userId, amount);
                return WalletResult.Failure("充值失敗，請稍後再試");
            }
        }

        public async Task<WalletResult> DeductFundsAsync(int userId, decimal amount, string description = "扣款")
        {
            if (amount <= 0)
            {
                return WalletResult.Failure("扣款金額必須大於 0");
            }

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);
                if (wallet == null)
                {
                    return WalletResult.Failure("錢包不存在");
                }

                // 檢查餘額
                if (wallet.Balance < amount)
                {
                    return WalletResult.Failure("餘額不足");
                }

                // 創建交易記錄
                var transactionRecord = new WalletTransaction
                {
                    UserId = userId,
                    Amount = -amount,
                    Type = TransactionType.Withdrawal,
                    Description = description,
                    CreatedAt = DateTime.UtcNow,
                    Status = GameCore.Domain.Enums.TransactionStatus.Completed
                };

                await _walletRepository.AddTransactionAsync(transactionRecord);

                // 更新餘額
                wallet.Balance -= amount;
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);
                await _walletRepository.SaveChangesAsync();

                // 清除快取
                _cache.Remove($"wallet_balance_{userId}");

                transaction.Complete();

                _logger.LogInformation("用戶 {UserId} 扣款成功: {Amount}", userId, amount);

                return WalletResult.SuccessResult(wallet.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "扣款失敗: {UserId}, {Amount}", userId, amount);
                return WalletResult.Failure("扣款失敗，請稍後再試");
            }
        }

        public async Task<WalletTransactionListResult> GetTransactionHistoryAsync(int userId, int page = 1, int pageSize = 20)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var transactions = await _walletRepository.GetTransactionsByUserIdAsync(userId, page, pageSize);
                var totalCount = await _walletRepository.GetTransactionCountByUserIdAsync(userId);

                var transactionDtos = transactions.Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    UserId = t.UserId,
                    Amount = t.Amount,
                    Type = t.Type.ToString(),
                    Status = t.Status.ToString(),
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                }).ToList();

                return WalletTransactionListResult.SuccessResult(transactionDtos, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取交易歷史失敗: {UserId}", userId);
                return WalletTransactionListResult.Failure("獲取交易歷史失敗");
            }
        }

        public async Task<WalletResult> TransferFundsAsync(int fromUserId, int toUserId, decimal amount, string description = "轉帳")
        {
            // 輸入驗證
            if (amount <= 0)
            {
                return WalletResult.Failure("轉帳金額必須大於 0");
            }

            if (fromUserId == toUserId)
            {
                return WalletResult.Failure("不能轉帳給自己");
            }

            // 檢查用戶是否存在且啟用
            var fromUser = await _userRepository.GetByIdAsync(fromUserId);
            var toUser = await _userRepository.GetByIdAsync(toUserId);

            if (fromUser == null || toUser == null)
            {
                return WalletResult.Failure("用戶不存在");
            }

            if (!fromUser.IsActive || !toUser.IsActive)
            {
                return WalletResult.Failure("用戶帳戶已被停用");
            }

            // 檢查轉帳限制
            if (amount > 5000)
            {
                return WalletResult.Failure("單次轉帳金額不能超過 5,000");
            }

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                // 檢查轉出方錢包
                var fromWallet = await _walletRepository.GetByUserIdAsync(fromUserId);
                if (fromWallet == null)
                {
                    return WalletResult.Failure("轉出方錢包不存在");
                }

                if (fromWallet.Balance < amount)
                {
                    return WalletResult.Failure("餘額不足");
                }

                // 檢查轉入方錢包
                var toWallet = await _walletRepository.GetByUserIdAsync(toUserId);
                if (toWallet == null)
                {
                    return WalletResult.Failure("轉入方錢包不存在");
                }

                // 創建轉出記錄
                var fromTransaction = new WalletTransaction
                {
                    UserId = fromUserId,
                    Amount = -amount,
                    Type = TransactionType.Transfer,
                    Description = $"轉帳給用戶 {toUserId}: {description}",
                    CreatedAt = DateTime.UtcNow,
                    Status = GameCore.Domain.Enums.TransactionStatus.Completed
                };

                // 創建轉入記錄
                var toTransaction = new WalletTransaction
                {
                    UserId = toUserId,
                    Amount = amount,
                    Type = TransactionType.Transfer,
                    Description = $"收到用戶 {fromUserId} 轉帳: {description}",
                    CreatedAt = DateTime.UtcNow,
                    Status = GameCore.Domain.Enums.TransactionStatus.Completed
                };

                await _walletRepository.AddTransactionAsync(fromTransaction);
                await _walletRepository.AddTransactionAsync(toTransaction);

                // 更新餘額
                fromWallet.Balance -= amount;
                fromWallet.UpdatedAt = DateTime.UtcNow;
                toWallet.Balance += amount;
                toWallet.UpdatedAt = DateTime.UtcNow;

                await _walletRepository.UpdateAsync(fromWallet);
                await _walletRepository.UpdateAsync(toWallet);
                await _walletRepository.SaveChangesAsync();

                // 清除快取
                _cache.Remove($"wallet_balance_{fromUserId}");
                _cache.Remove($"wallet_balance_{toUserId}");

                transaction.Complete();

                _logger.LogInformation("轉帳成功: {FromUserId} -> {ToUserId}, {Amount}", fromUserId, toUserId, amount);

                return WalletResult.SuccessResult(fromWallet.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "轉帳失敗: {FromUserId} -> {ToUserId}, {Amount}", fromUserId, toUserId, amount);
                return WalletResult.Failure("轉帳失敗，請稍後再試");
            }
        }

        public async Task<WalletResult> ValidateWalletAsync(int userId)
        {
            try
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);
                if (wallet == null)
                {
                    return WalletResult.Failure("錢包不存在");
                }

                // 檢查餘額是否為負數
                if (wallet.Balance < 0)
                {
                    _logger.LogWarning("檢測到負餘額: 用戶 {UserId}, 餘額 {Balance}", userId, wallet.Balance);
                    return WalletResult.Failure("錢包餘額異常");
                }

                // 檢查最近交易記錄
                var recentTransactions = await _walletRepository.GetTransactionsByUserIdAsync(userId, 1, 10);
                var suspiciousTransactions = recentTransactions.Where(t =>
                    Math.Abs(t.Amount) > 10000 ||
                    t.CreatedAt < DateTime.UtcNow.AddDays(-30) && t.Amount < 0
                ).ToList();

                if (suspiciousTransactions.Any())
                {
                    _logger.LogWarning("檢測到可疑交易: 用戶 {UserId}, 可疑交易數 {Count}",
                        userId, suspiciousTransactions.Count);
                }

                return WalletResult.SuccessResult(wallet.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "錢包驗證失敗: {UserId}", userId);
                return WalletResult.Failure("錢包驗證失敗");
            }
        }

        public async Task<WalletResult> GetWalletStatisticsAsync(int userId)
        {
            try
            {
                var wallet = await _walletRepository.GetByUserIdAsync(userId);
                if (wallet == null)
                {
                    return WalletResult.Failure("錢包不存在");
                }

                var totalTransactions = await _walletRepository.GetTransactionCountByUserIdAsync(userId);

                var statistics = new
                {
                    CurrentBalance = wallet.Balance,
                    TotalTransactions = totalTransactions,
                    LastUpdated = wallet.UpdatedAt
                };

                return WalletResult.SuccessResult(wallet.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取錢包統計失敗: {UserId}", userId);
                return WalletResult.Failure("獲取統計失敗");
            }
        }

        // 新增安全驗證方法
        private bool ContainsSqlInjection(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            var sqlKeywords = new[] { "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER", "EXEC", "EXECUTE", "UNION", "OR", "AND" };
            var upperInput = input.ToUpperInvariant();

            return sqlKeywords.Any(keyword => upperInput.Contains(keyword));
        }

        private bool ContainsXss(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            var xssPatterns = new[] { "<script", "javascript:", "onload=", "onerror=", "onclick=" };
            var lowerInput = input.ToLowerInvariant();

            return xssPatterns.Any(pattern => lowerInput.Contains(pattern));
        }

        // 新增充值頻率限制方法
        private Task<int> GetRecentDepositsAsync(string cacheKey)
        {
            if (_cache.TryGetValue(cacheKey, out int count))
            {
                return Task.FromResult(count);
            }
            return Task.FromResult(0);
        }

        private Task IncrementRecentDepositsAsync(string cacheKey)
        {
            var currentCount = _cache.TryGetValue(cacheKey, out int count) ? count : 0;
            _cache.Set(cacheKey, currentCount + 1, TimeSpan.FromMinutes(10));
            return Task.CompletedTask;
        }

        public async Task<WalletResult> WithdrawFundsAsync(int userId, decimal amount, string description = "提現")
        {
            try
            {
                if (amount <= 0)
                {
                    return WalletResult.Failure("提現金額必須大於 0");
                }

                var wallet = await _walletRepository.GetByUserIdAsync(userId);
                if (wallet == null)
                {
                    return WalletResult.Failure("錢包不存在");
                }

                if (wallet.Balance < amount)
                {
                    return WalletResult.Failure("餘額不足");
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                wallet.Balance -= amount;
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);

                var walletTransaction = new WalletTransaction
                {
                    UserId = userId,
                    Amount = -amount,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                };
                await _walletRepository.AddTransactionAsync(walletTransaction);

                transaction.Complete();

                return WalletResult.SuccessResult(wallet.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "提現失敗: {UserId}, {Amount}", userId, amount);
                return WalletResult.Failure("提現失敗，請稍後再試");
            }
        }

        public async Task<WalletResult> TransferFundsAsync(int userId, TransferRequest request)
        {
            try
            {
                if (request.Amount <= 0)
                {
                    return WalletResult.Failure("轉帳金額必須大於 0");
                }

                var sourceWallet = await _walletRepository.GetByUserIdAsync(userId);
                var targetWallet = await _walletRepository.GetByUserIdAsync(request.TargetUserId);

                if (sourceWallet == null || targetWallet == null)
                {
                    return WalletResult.Failure("錢包不存在");
                }

                if (sourceWallet.Balance < request.Amount)
                {
                    return WalletResult.Failure("餘額不足");
                }

                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                sourceWallet.Balance -= request.Amount;
                targetWallet.Balance += request.Amount;

                sourceWallet.UpdatedAt = DateTime.UtcNow;
                targetWallet.UpdatedAt = DateTime.UtcNow;

                await _walletRepository.UpdateAsync(sourceWallet);
                await _walletRepository.UpdateAsync(targetWallet);

                var sourceTransaction = new WalletTransaction
                {
                    UserId = userId,
                    Amount = -request.Amount,
                    Description = $"轉帳給用戶 {request.TargetUserId}",
                    CreatedAt = DateTime.UtcNow
                };

                var targetTransaction = new WalletTransaction
                {
                    UserId = request.TargetUserId,
                    Amount = request.Amount,
                    Description = $"收到來自用戶 {userId} 的轉帳",
                    CreatedAt = DateTime.UtcNow
                };

                await _walletRepository.AddTransactionAsync(sourceTransaction);
                await _walletRepository.AddTransactionAsync(targetTransaction);

                transaction.Complete();

                return WalletResult.SuccessResult(sourceWallet.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "轉帳失敗: {UserId}, {TargetUserId}, {Amount}", userId, request.TargetUserId, request.Amount);
                return WalletResult.Failure("轉帳失敗，請稍後再試");
            }
        }
    }
}

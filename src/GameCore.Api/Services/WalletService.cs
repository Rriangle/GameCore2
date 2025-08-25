using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Services;

/// <summary>
/// 錢包服務實現
/// </summary>
public class WalletService : IWalletService
{
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly IUserSalesInformationRepository _userSalesInformationRepository;
    private readonly ILogger<WalletService> _logger;

    public WalletService(
        IUserWalletRepository userWalletRepository,
        IUserSalesInformationRepository userSalesInformationRepository,
        ILogger<WalletService> logger)
    {
        _userWalletRepository = userWalletRepository;
        _userSalesInformationRepository = userSalesInformationRepository;
        _logger = logger;
    }

    /// <summary>
    /// 獲取用戶錢包餘額
    /// </summary>
    public async Task<int> GetUserWalletBalanceAsync(int userId)
    {
        try
        {
            var userWallet = await _userWalletRepository.GetByUserIdAsync(userId);
            if (userWallet == null)
            {
                _logger.LogWarning("用戶錢包不存在，用戶ID: {UserId}", userId);
                return 0;
            }

            return userWallet.User_Point;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶錢包餘額時發生錯誤，用戶ID: {UserId}", userId);
            return 0;
        }
    }

    /// <summary>
    /// 獲取銷售錢包餘額
    /// </summary>
    public async Task<decimal> GetSalesWalletBalanceAsync(int userId)
    {
        try
        {
            var salesInfo = await _userSalesInformationRepository.GetByUserIdAsync(userId);
            if (salesInfo == null)
            {
                _logger.LogWarning("銷售錢包不存在，用戶ID: {UserId}", userId);
                return 0m;
            }

            return salesInfo.UserSales_Wallet;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取銷售錢包餘額時發生錯誤，用戶ID: {UserId}", userId);
            return 0m;
        }
    }

    /// <summary>
    /// 獲取用戶錢包交易記錄
    /// </summary>
    public async Task<List<WalletTransactionDto>> GetUserWalletTransactionsAsync(int userId, int page, int pageSize)
    {
        try
        {
            var transactions = await _userWalletRepository.GetTransactionsByUserIdAsync(userId, page, pageSize);
            if (transactions == null)
            {
                _logger.LogWarning("用戶錢包交易記錄不存在，用戶ID: {UserId}", userId);
                return new List<WalletTransactionDto>();
            }

            return transactions.Select(t => new WalletTransactionDto
            {
                TransactionId = t.TransactionId,
                UserId = t.UserId,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶錢包交易記錄時發生錯誤，用戶ID: {UserId}", userId);
            return new List<WalletTransactionDto>();
        }
    }

    /// <summary>
    /// 獲取銷售錢包交易記錄
    /// </summary>
    public async Task<List<SalesWalletTransactionDto>> GetSalesWalletTransactionsAsync(int userId, int page, int pageSize)
    {
        try
        {
            var transactions = await _userSalesInformationRepository.GetTransactionsByUserIdAsync(userId, page, pageSize);
            if (transactions == null)
            {
                _logger.LogWarning("銷售錢包交易記錄不存在，用戶ID: {UserId}", userId);
                return new List<SalesWalletTransactionDto>();
            }

            return transactions.Select(t => new SalesWalletTransactionDto
            {
                TransactionId = t.TransactionId,
                UserId = t.UserId,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                Description = t.Description,
                CreatedAt = t.CreatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取銷售錢包交易記錄時發生錯誤，用戶ID: {UserId}", userId);
            return new List<SalesWalletTransactionDto>();
        }
    }
}
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 錢包服務實作，處理錢包相關的業務邏輯
/// </summary>
public class WalletService : IWalletService
{
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<WalletService> _logger;

    /// <summary>
    /// 建構函式，注入相依服務
    /// </summary>
    /// <param name="userWalletRepository">用戶錢包倉儲</param>
    /// <param name="userRepository">用戶倉儲</param>
    /// <param name="logger">日誌記錄器</param>
    public WalletService(
        IUserWalletRepository userWalletRepository,
        IUserRepository userRepository,
        ILogger<WalletService> logger)
    {
        _userWalletRepository = userWalletRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// 取得用戶錢包餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>錢包餘額資訊</returns>
    public async Task<WalletBalanceDto?> GetWalletBalanceAsync(int userId)
    {
        try
        {
            _logger.LogInformation("正在查詢用戶 {UserId} 的錢包餘額", userId);

            var wallet = await _userWalletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                _logger.LogWarning("用戶 {UserId} 的錢包不存在", userId);
                return null;
            }

            var balance = new WalletBalanceDto
            {
                User_Id = wallet.User_Id,
                Balance = wallet.User_Point,
                Coupon_Number = wallet.Coupon_Number
            };

            _logger.LogInformation("用戶 {UserId} 的錢包餘額: {Balance} 點", userId, balance.Balance);
            return balance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢用戶 {UserId} 錢包餘額時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 取得點數交易明細 (分頁查詢)
    /// 根據規格，此方法需要從多個來源彙整交易記錄
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁交易明細</returns>
    public async Task<PagedPointTransactionsDto> GetPointTransactionsAsync(int userId, PointTransactionQueryDto query)
    {
        try
        {
            _logger.LogInformation("正在查詢用戶 {UserId} 的點數交易明細", userId);

            var transactions = new List<PointTransactionDto>();

            // TODO: 這裡需要實作多來源交易記錄彙整
            // 1. 從 UserSignInStats 查詢簽到記錄
            // 2. 從 MiniGame 查詢小遊戲記錄
            // 3. 從 Pet 查詢寵物換色記錄 (最近一次)
            // 4. 從 Notifications 查詢管理者調整記錄
            // 由於目前專注於核心功能，此處先返回空結果

            var totalCount = transactions.Count;
            var pagedTransactions = transactions
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var result = new PagedPointTransactionsDto
            {
                Transactions = pagedTransactions,
                TotalCount = totalCount,
                CurrentPage = query.Page,
                PageSize = query.PageSize
            };

            _logger.LogInformation("用戶 {UserId} 的點數交易明細查詢完成，總計 {Count} 筆", userId, totalCount);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢用戶 {UserId} 點數交易明細時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 增加用戶點數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="points">增加的點數</param>
    /// <param name="source">來源 (signin, minigame, pet_color, adjustment)</param>
    /// <param name="description">交易描述</param>
    /// <param name="sourceId">來源記錄編號</param>
    /// <returns>是否成功</returns>
    public async Task<bool> AddPointsAsync(int userId, int points, string source, string description, string? sourceId = null)
    {
        try
        {
            _logger.LogInformation("正在為用戶 {UserId} 增加 {Points} 點數，來源: {Source}", userId, points, source);

            if (points <= 0)
            {
                _logger.LogWarning("點數增加量 {Points} 必須大於 0", points);
                return false;
            }

            var success = await _userWalletRepository.AddPointsAsync(userId, points);
            if (success)
            {
                _logger.LogInformation("成功為用戶 {UserId} 增加 {Points} 點數，來源: {Source}，描述: {Description}", 
                    userId, points, source, description);
            }
            else
            {
                _logger.LogWarning("為用戶 {UserId} 增加點數失敗", userId);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "為用戶 {UserId} 增加點數時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 扣除用戶點數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="points">扣除的點數</param>
    /// <param name="source">來源 (pet_color, store_purchase, market_purchase)</param>
    /// <param name="description">交易描述</param>
    /// <param name="sourceId">來源記錄編號</param>
    /// <returns>是否成功 (餘額不足會返回 false)</returns>
    public async Task<bool> DeductPointsAsync(int userId, int points, string source, string description, string? sourceId = null)
    {
        try
        {
            _logger.LogInformation("正在為用戶 {UserId} 扣除 {Points} 點數，來源: {Source}", userId, points, source);

            if (points <= 0)
            {
                _logger.LogWarning("點數扣除量 {Points} 必須大於 0", points);
                return false;
            }

            // 檢查餘額是否足夠
            var hasEnough = await HasSufficientPointsAsync(userId, points);
            if (!hasEnough)
            {
                _logger.LogWarning("用戶 {UserId} 的點數餘額不足，無法扣除 {Points} 點數", userId, points);
                return false;
            }

            var success = await _userWalletRepository.DeductPointsAsync(userId, points);
            if (success)
            {
                _logger.LogInformation("成功為用戶 {UserId} 扣除 {Points} 點數，來源: {Source}，描述: {Description}", 
                    userId, points, source, description);
            }
            else
            {
                _logger.LogWarning("為用戶 {UserId} 扣除點數失敗", userId);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "為用戶 {UserId} 扣除點數時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 管理者調整用戶點數
    /// </summary>
    /// <param name="adminUserId">管理者編號</param>
    /// <param name="request">調整請求</param>
    /// <returns>是否成功</returns>
    public async Task<bool> AdminAdjustPointsAsync(int adminUserId, AdminAdjustPointsDto request)
    {
        try
        {
            _logger.LogInformation("管理者 {AdminUserId} 正在調整用戶 {UserId} 的點數: {Delta}，原因: {Reason}", 
                adminUserId, request.UserId, request.Delta, request.Reason);

            // 檢查目標用戶是否存在
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("目標用戶 {UserId} 不存在", request.UserId);
                return false;
            }

            // 如果是扣除點數，檢查餘額
            if (request.Delta < 0)
            {
                var hasEnough = await HasSufficientPointsAsync(request.UserId, Math.Abs(request.Delta));
                if (!hasEnough)
                {
                    _logger.LogWarning("用戶 {UserId} 的點數餘額不足，無法扣除 {Points} 點數", 
                        request.UserId, Math.Abs(request.Delta));
                    return false;
                }
            }

            bool success;
            if (request.Delta > 0)
            {
                success = await AddPointsAsync(request.UserId, request.Delta, "adjustment", 
                    $"管理者調整: {request.Reason}", adminUserId.ToString());
            }
            else
            {
                success = await DeductPointsAsync(request.UserId, Math.Abs(request.Delta), "adjustment", 
                    $"管理者調整: {request.Reason}", adminUserId.ToString());
            }

            if (success)
            {
                _logger.LogInformation("管理者 {AdminUserId} 成功調整用戶 {UserId} 的點數: {Delta}", 
                    adminUserId, request.UserId, request.Delta);

                // TODO: 發送通知給目標用戶
                // await _notificationService.SendPointsAdjustmentNotificationAsync(request.UserId, request.Delta, request.Reason);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "管理者 {AdminUserId} 調整用戶 {UserId} 點數時發生錯誤", 
                adminUserId, request.UserId);
            throw;
        }
    }

    /// <summary>
    /// 檢查用戶是否有足夠點數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="requiredPoints">需要的點數</param>
    /// <returns>是否有足夠點數</returns>
    public async Task<bool> HasSufficientPointsAsync(int userId, int requiredPoints)
    {
        try
        {
            _logger.LogInformation("正在檢查用戶 {UserId} 是否有足夠點數 {RequiredPoints}", userId, requiredPoints);

            var currentPoints = await _userWalletRepository.GetPointsAsync(userId);
            var hasEnough = currentPoints >= requiredPoints;

            _logger.LogInformation("用戶 {UserId} 目前點數: {CurrentPoints}，需要點數: {RequiredPoints}，足夠: {HasEnough}", 
                userId, currentPoints, requiredPoints, hasEnough);

            return hasEnough;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查用戶 {UserId} 點數是否足夠時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 取得銀行代號清單
    /// </summary>
    /// <returns>銀行代號清單</returns>
    public async Task<List<BankCodeDto>> GetBankCodesAsync()
    {
        try
        {
            _logger.LogInformation("正在取得銀行代號清單");

            // 台灣常見銀行代號清單
            var bankCodes = new List<BankCodeDto>
            {
                new() { Code = 004, Name = "台灣銀行", ShortName = "BOT" },
                new() { Code = 005, Name = "土地銀行", ShortName = "LBT" },
                new() { Code = 006, Name = "合作金庫銀行", ShortName = "CCB" },
                new() { Code = 007, Name = "第一銀行", ShortName = "FIRST" },
                new() { Code = 008, Name = "華南銀行", ShortName = "HNCB" },
                new() { Code = 009, Name = "彰化銀行", ShortName = "CHB" },
                new() { Code = 011, Name = "上海銀行", ShortName = "SCB" },
                new() { Code = 012, Name = "台北富邦銀行", ShortName = "FUBON" },
                new() { Code = 013, Name = "國泰世華銀行", ShortName = "CATHAY" },
                new() { Code = 017, Name = "兆豐銀行", ShortName = "MEGABANK" },
                new() { Code = 021, Name = "花旗銀行", ShortName = "CITI" },
                new() { Code = 050, Name = "台灣中小企銀", ShortName = "TSMEC" },
                new() { Code = 052, Name = "渣打銀行", ShortName = "SCB" },
                new() { Code = 103, Name = "台新銀行", ShortName = "TAISHIN" },
                new() { Code = 108, Name = "陽信銀行", ShortName = "SUNNY" },
                new() { Code = 147, Name = "三信銀行", ShortName = "ENTIE" },
                new() { Code = 700, Name = "中華郵政", ShortName = "POST" },
                new() { Code = 803, Name = "聯邦銀行", ShortName = "UBT" },
                new() { Code = 805, Name = "遠東銀行", ShortName = "FEIB" },
                new() { Code = 807, Name = "永豐銀行", ShortName = "SINOPAC" },
                new() { Code = 808, Name = "玉山銀行", ShortName = "ESUN" },
                new() { Code = 809, Name = "凱基銀行", ShortName = "KGI" },
                new() { Code = 812, Name = "台新銀行", ShortName = "TAISHIN" },
                new() { Code = 816, Name = "安泰銀行", ShortName = "ENTRUST" },
                new() { Code = 822, Name = "中國信託", ShortName = "CTBC" }
            };

            _logger.LogInformation("取得銀行代號清單完成，共 {Count} 家銀行", bankCodes.Count);
            return await Task.FromResult(bankCodes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得銀行代號清單時發生錯誤");
            throw;
        }
    }
}
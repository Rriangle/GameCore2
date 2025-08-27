using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 用戶銷售資訊倉儲實作，處理銷售錢包的資料庫操作
/// </summary>
public class UserSalesInformationRepository : IUserSalesInformationRepository
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<UserSalesInformationRepository> _logger;

    /// <summary>
    /// 建構函式，注入相依服務
    /// </summary>
    /// <param name="context">資料庫上下文</param>
    /// <param name="logger">日誌記錄器</param>
    public UserSalesInformationRepository(GameCoreDbContext context, ILogger<UserSalesInformationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 根據用戶編號取得銷售資訊
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>銷售資訊，不存在則返回 null</returns>
    public async Task<UserSalesInformation?> GetByUserIdAsync(int userId)
    {
        try
        {
            _logger.LogInformation("正在查詢用戶 {UserId} 的銷售資訊", userId);

            var salesInfo = await _context.UserSalesInformation
                .Include(si => si.User)
                .FirstOrDefaultAsync(si => si.User_Id == userId);

            if (salesInfo == null)
            {
                _logger.LogInformation("用戶 {UserId} 的銷售資訊不存在", userId);
            }

            return salesInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢用戶 {UserId} 銷售資訊時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 建立新銷售資訊
    /// </summary>
    /// <param name="salesInfo">銷售資訊</param>
    /// <returns>建立的銷售資訊</returns>
    public async Task<UserSalesInformation> CreateAsync(UserSalesInformation salesInfo)
    {
        try
        {
            _logger.LogInformation("正在建立用戶 {UserId} 的銷售資訊", salesInfo.User_Id);

            _context.UserSalesInformation.Add(salesInfo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("成功建立用戶 {UserId} 的銷售資訊，初始餘額: {Balance}", salesInfo.User_Id, salesInfo.UserSales_Wallet);
            return salesInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立用戶 {UserId} 銷售資訊時發生錯誤", salesInfo.User_Id);
            throw;
        }
    }

    /// <summary>
    /// 更新銷售資訊
    /// </summary>
    /// <param name="salesInfo">銷售資訊</param>
    /// <returns>更新的銷售資訊</returns>
    public async Task<UserSalesInformation> UpdateAsync(UserSalesInformation salesInfo)
    {
        try
        {
            _logger.LogInformation("正在更新用戶 {UserId} 的銷售資訊", salesInfo.User_Id);

            _context.UserSalesInformation.Update(salesInfo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("成功更新用戶 {UserId} 的銷售資訊，新餘額: {Balance}", salesInfo.User_Id, salesInfo.UserSales_Wallet);
            return salesInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用戶 {UserId} 銷售資訊時發生錯誤", salesInfo.User_Id);
            throw;
        }
    }

    /// <summary>
    /// 取得銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>銷售錢包餘額</returns>
    public async Task<int> GetSalesWalletBalanceAsync(int userId)
    {
        try
        {
            _logger.LogInformation("正在查詢用戶 {UserId} 的銷售錢包餘額", userId);

            var salesInfo = await _context.UserSalesInformation
                .FirstOrDefaultAsync(si => si.User_Id == userId);

            var balance = salesInfo?.UserSales_Wallet ?? 0;
            _logger.LogInformation("用戶 {UserId} 的銷售錢包餘額: {Balance}", userId, balance);

            return balance;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢用戶 {UserId} 銷售錢包餘額時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 更新銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="balance">新餘額</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateSalesWalletBalanceAsync(int userId, int balance)
    {
        try
        {
            _logger.LogInformation("正在更新用戶 {UserId} 的銷售錢包餘額為 {Balance}", userId, balance);

            var salesInfo = await _context.UserSalesInformation
                .FirstOrDefaultAsync(si => si.User_Id == userId);

            if (salesInfo == null)
            {
                _logger.LogWarning("用戶 {UserId} 的銷售資訊不存在，無法更新餘額", userId);
                return false;
            }

            var oldBalance = salesInfo.UserSales_Wallet;
            salesInfo.UserSales_Wallet = balance;
            await _context.SaveChangesAsync();

            _logger.LogInformation("成功更新用戶 {UserId} 的銷售錢包餘額: {OldBalance} -> {NewBalance}", userId, oldBalance, balance);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用戶 {UserId} 銷售錢包餘額時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 增加銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="amount">增加的金額</param>
    /// <returns>是否增加成功</returns>
    public async Task<bool> AddToSalesWalletAsync(int userId, int amount)
    {
        try
        {
            _logger.LogInformation("正在為用戶 {UserId} 增加銷售錢包餘額 {Amount}", userId, amount);

            var salesInfo = await _context.UserSalesInformation
                .FirstOrDefaultAsync(si => si.User_Id == userId);

            if (salesInfo == null)
            {
                _logger.LogWarning("用戶 {UserId} 的銷售資訊不存在，無法增加餘額", userId);
                return false;
            }

            var oldBalance = salesInfo.UserSales_Wallet;
            salesInfo.UserSales_Wallet += amount;
            await _context.SaveChangesAsync();

            _logger.LogInformation("成功為用戶 {UserId} 增加銷售錢包餘額: {OldBalance} + {Amount} = {NewBalance}", 
                userId, oldBalance, amount, salesInfo.UserSales_Wallet);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "為用戶 {UserId} 增加銷售錢包餘額時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 扣除銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="amount">扣除的金額</param>
    /// <returns>是否扣除成功 (餘額不足會返回 false)</returns>
    public async Task<bool> DeductFromSalesWalletAsync(int userId, int amount)
    {
        try
        {
            _logger.LogInformation("正在為用戶 {UserId} 扣除銷售錢包餘額 {Amount}", userId, amount);

            var salesInfo = await _context.UserSalesInformation
                .FirstOrDefaultAsync(si => si.User_Id == userId);

            if (salesInfo == null)
            {
                _logger.LogWarning("用戶 {UserId} 的銷售資訊不存在，無法扣除餘額", userId);
                return false;
            }

            if (salesInfo.UserSales_Wallet < amount)
            {
                _logger.LogWarning("用戶 {UserId} 的銷售錢包餘額 {Balance} 不足，無法扣除 {Amount}", 
                    userId, salesInfo.UserSales_Wallet, amount);
                return false;
            }

            var oldBalance = salesInfo.UserSales_Wallet;
            salesInfo.UserSales_Wallet -= amount;
            await _context.SaveChangesAsync();

            _logger.LogInformation("成功為用戶 {UserId} 扣除銷售錢包餘額: {OldBalance} - {Amount} = {NewBalance}", 
                userId, oldBalance, amount, salesInfo.UserSales_Wallet);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "為用戶 {UserId} 扣除銷售錢包餘額時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 刪除銷售資訊
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(int userId)
    {
        try
        {
            _logger.LogInformation("正在刪除用戶 {UserId} 的銷售資訊", userId);

            var salesInfo = await _context.UserSalesInformation
                .FirstOrDefaultAsync(si => si.User_Id == userId);

            if (salesInfo == null)
            {
                _logger.LogWarning("用戶 {UserId} 的銷售資訊不存在，無法刪除", userId);
                return false;
            }

            _context.UserSalesInformation.Remove(salesInfo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("成功刪除用戶 {UserId} 的銷售資訊", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除用戶 {UserId} 銷售資訊時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 取得所有銷售資訊列表 (管理者用)
    /// </summary>
    /// <returns>銷售資訊列表</returns>
    public async Task<List<UserSalesInformation>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("正在查詢所有銷售資訊");

            var salesInfoList = await _context.UserSalesInformation
                .Include(si => si.User)
                .OrderBy(si => si.User_Id)
                .ToListAsync();

            _logger.LogInformation("查詢到 {Count} 筆銷售資訊", salesInfoList.Count);
            return salesInfoList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢所有銷售資訊時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 取得銷售收入統計 (按用戶)
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="fromDate">開始日期</param>
    /// <param name="toDate">結束日期</param>
    /// <returns>收入統計</returns>
    public async Task<decimal> GetSalesRevenueStatisticsAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            _logger.LogInformation("正在計算用戶 {UserId} 的銷售收入統計，期間: {FromDate} ~ {ToDate}", 
                userId, fromDate, toDate);

            // 這裡應該根據實際的交易記錄來計算，但由於沒有專門的交易記錄表
            // 我們先返回當前銷售錢包餘額作為統計值
            var salesInfo = await _context.UserSalesInformation
                .FirstOrDefaultAsync(si => si.User_Id == userId);

            var revenue = salesInfo?.UserSales_Wallet ?? 0;

            _logger.LogInformation("用戶 {UserId} 的銷售收入統計: {Revenue}", userId, revenue);
            return revenue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "計算用戶 {UserId} 銷售收入統計時發生錯誤", userId);
            throw;
        }
    }
}
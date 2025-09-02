using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 用戶錢包資料存取層
/// 實作 IUserWalletRepository 介面
/// </summary>
public class UserWalletRepository : IUserWalletRepository
{
    private readonly GameCoreDbContext _context;

    public UserWalletRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據用戶ID取得用戶錢包
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>用戶錢包資料</returns>
    public async Task<UserWallet?> GetByUserIdAsync(int userId)
    {
        // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能，避免 EF Core 變更追蹤開銷
        return await _context.UserWallets
            .AsNoTracking() // 效能優化：唯讀查詢不需要變更追蹤
            .FirstOrDefaultAsync(uw => uw.User_Id == userId);
    }

    /// <summary>
    /// 新增用戶錢包
    /// </summary>
    /// <param name="userWallet">用戶錢包資料</param>
    /// <returns>新增的用戶錢包資料</returns>
    public async Task<UserWallet> AddAsync(UserWallet userWallet)
    {
        _context.UserWallets.Add(userWallet);
        await _context.SaveChangesAsync();
        return userWallet;
    }

    /// <summary>
    /// 更新用戶錢包
    /// </summary>
    /// <param name="userWallet">用戶錢包資料</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateAsync(UserWallet userWallet)
    {
        _context.UserWallets.Update(userWallet);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除用戶錢包
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(int userId)
    {
        var userWallet = await _context.UserWallets.FindAsync(userId);
        if (userWallet == null)
            return false;

        _context.UserWallets.Remove(userWallet);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 扣除用戶點數
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="points">扣除點數</param>
    /// <param name="reason">扣除原因</param>
    /// <returns>是否扣除成功</returns>
    public async Task<bool> DeductPointsAsync(int userId, int points, string reason)
    {
        var userWallet = await _context.UserWallets.FindAsync(userId);
        if (userWallet == null)
            return false;

        if (userWallet.User_Point < points)
            return false;

        userWallet.User_Point -= points;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 增加用戶點數
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="points">增加點數</param>
    /// <param name="reason">增加原因</param>
    /// <returns>是否增加成功</returns>
    public async Task<bool> AddPointsAsync(int userId, int points, string reason)
    {
        var userWallet = await _context.UserWallets.FindAsync(userId);
        if (userWallet == null)
            return false;

        userWallet.User_Point += points;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 設定用戶點數
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="points">設定點數</param>
    /// <returns>是否設定成功</returns>
    public async Task<bool> SetPointsAsync(int userId, int points)
    {
        var userWallet = await _context.UserWallets.FindAsync(userId);
        if (userWallet == null)
            return false;

        userWallet.User_Point = points;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 根據點數範圍取得用戶列表
    /// </summary>
    /// <param name="minPoints">最小點數</param>
    /// <param name="maxPoints">最大點數</param>
    /// <returns>用戶錢包列表</returns>
    public async Task<IEnumerable<UserWallet>> GetByPointsRangeAsync(int minPoints, int maxPoints)
    {
        return await _context.UserWallets
            .Where(uw => uw.User_Point >= minPoints && uw.User_Point <= maxPoints)
            .ToListAsync();
    }

    /// <summary>
    /// 取得所有用戶錢包
    /// </summary>
    /// <returns>用戶錢包列表</returns>
    public async Task<IEnumerable<UserWallet>> GetAllAsync()
    {
        return await _context.UserWallets.ToListAsync();
    }

    /// <summary>
    /// 分頁取得用戶錢包
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>用戶錢包列表</returns>
    public async Task<IEnumerable<UserWallet>> GetPagedAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        return await _context.UserWallets
            // 效能優化：唯讀分頁查詢不需要變更追蹤
            // 前後差異：加入 AsNoTracking()，減少記憶體與追蹤成本
            // 風險與回滾：風險極低，如需追蹤請在更新邏輯處理
            .AsNoTracking()
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// 取得用戶錢包總數
    /// </summary>
    /// <returns>用戶錢包總數</returns>
    public async Task<int> GetCountAsync()
    {
        // 效能優化：計數查詢不需要變更追蹤
        // 前後差異：加入 AsNoTracking()，行為不變但可讀性更一致
        // 風險：無
        return await _context.UserWallets
            .AsNoTracking()
            .CountAsync();
    }

    /// <summary>
    /// 取得用戶交易記錄
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>交易記錄列表</returns>
    public async Task<IEnumerable<UserWalletTransaction>> GetTransactionsByUserIdAsync(int userId, int page = 1, int pageSize = 20)
    {
        return await _context.UserWalletTransactions
            .Where(t => t.UserID == userId)
            .OrderByDescending(t => t.TransactionTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// 取得用戶交易記錄總數
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>交易記錄總數</returns>
    public async Task<int> GetTransactionCountByUserIdAsync(int userId)
    {
        return await _context.UserWalletTransactions
            .CountAsync(t => t.UserID == userId);
    }

    /// <summary>
    /// 新增交易記錄
    /// </summary>
    /// <param name="transaction">交易記錄實體</param>
    /// <returns>新增的交易記錄實體</returns>
    public async Task<UserWalletTransaction> AddTransactionAsync(UserWalletTransaction transaction)
    {
        _context.UserWalletTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }
}

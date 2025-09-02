using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 使用者銷售資訊存取層
/// 實作 IUserSalesInformationRepository 介面
/// </summary>
public class UserSalesInformationRepository : IUserSalesInformationRepository
{
    private readonly GameCoreDbContext _context;

    public UserSalesInformationRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據用戶ID取得銷售資訊
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>銷售資訊</returns>
    public async Task<UserSalesInformation?> GetByUserIdAsync(int userId)
    {
        return await _context.UserSalesInformations
            .Include(u => u.User)
            .FirstOrDefaultAsync(u => u.User_Id == userId);
    }

    /// <summary>
    /// 新增銷售資訊
    /// </summary>
    /// <param name="info">銷售資訊</param>
    /// <returns>是否成功</returns>
    public async Task<bool> AddAsync(UserSalesInformation info)
    {
        _context.UserSalesInformations.Add(info);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 更新銷售資訊
    /// </summary>
    /// <param name="info">銷售資訊</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateAsync(UserSalesInformation info)
    {
        _context.UserSalesInformations.Update(info);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除銷售資訊
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteAsync(int userId)
    {
        var info = await GetByUserIdAsync(userId);
        if (info == null) return false;

        _context.UserSalesInformations.Remove(info);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 取得所有銷售資訊
    /// </summary>
    /// <returns>銷售資訊列表</returns>
    public async Task<List<UserSalesInformation>> GetAllAsync()
    {
        return await _context.UserSalesInformations
            .Include(u => u.User)
            .OrderBy(u => u.User_Id)
            .ToListAsync();
    }

    /// <summary>
    /// 更新銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="amount">金額變動</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateWalletBalanceAsync(int userId, int amount)
    {
        var info = await GetByUserIdAsync(userId);
        if (info == null) return false;

        info.UserSales_Wallet += amount;
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 檢查用戶是否已有銷售資訊
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否存在</returns>
    public async Task<bool> ExistsAsync(int userId)
    {
        return await _context.UserSalesInformations.AnyAsync(usi => usi.User_Id == userId);
    }

    /// <summary>
    /// 取得用戶的客戶列表
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>客戶列表</returns>
    public async Task<List<User>> GetCustomersByUserIdAsync(int userId)
    {
        // 這裡需要根據實際業務邏輯實現
        // 暫時返回空列表，實際應該查詢該用戶的客戶關係
        await Task.CompletedTask; // 存根實作，避免 CS1998 警告
        return new List<User>();
    }

    /// <summary>
    /// 取得頂級銷售用戶
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>頂級銷售用戶列表</returns>
    public async Task<List<UserSalesInformation>> GetTopSalesUsersAsync(int top = 10)
    {
        return await _context.UserSalesInformations
            .OrderByDescending(usi => usi.UserSales_Wallet)
            .Take(top)
            .ToListAsync();
    }
} 
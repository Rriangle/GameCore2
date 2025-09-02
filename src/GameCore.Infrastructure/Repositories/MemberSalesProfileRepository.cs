using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 會員銷售資料存取層
/// 實作 IMemberSalesProfileRepository 介面
/// </summary>
public class MemberSalesProfileRepository : IMemberSalesProfileRepository
{
    private readonly GameCoreDbContext _context;

    public MemberSalesProfileRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據用戶ID取得銷售資料
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>銷售資料</returns>
    public async Task<MemberSalesProfile?> GetByUserIdAsync(int userId)
    {
        return await _context.MemberSalesProfiles
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.User_Id == userId);
    }

    /// <summary>
    /// 新增銷售資料
    /// </summary>
    /// <param name="profile">銷售資料</param>
    /// <returns>是否成功</returns>
    public async Task<bool> AddAsync(MemberSalesProfile profile)
    {
        _context.MemberSalesProfiles.Add(profile);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 更新銷售資料
    /// </summary>
    /// <param name="profile">銷售資料</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateAsync(MemberSalesProfile profile)
    {
        _context.MemberSalesProfiles.Update(profile);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除銷售資料
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteAsync(int userId)
    {
        var profile = await GetByUserIdAsync(userId);
        if (profile == null) return false;

        _context.MemberSalesProfiles.Remove(profile);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 取得所有銷售資料
    /// </summary>
    /// <returns>銷售資料列表</returns>
    public async Task<List<MemberSalesProfile>> GetAllAsync()
    {
        return await _context.MemberSalesProfiles
            .Include(m => m.User)
            .OrderBy(m => m.User_Id)
            .ToListAsync();
    }

    /// <summary>
    /// 根據銀行代碼取得銷售資料
    /// </summary>
    /// <param name="bankCode">銀行代碼</param>
    /// <returns>銷售資料列表</returns>
    public async Task<List<MemberSalesProfile>> GetByBankCodeAsync(int bankCode)
    {
        return await _context.MemberSalesProfiles
            .Include(m => m.User)
            .Where(m => m.BankCode == bankCode)
            .OrderBy(m => m.User_Id)
            .ToListAsync();
    }

    /// <summary>
    /// 檢查用戶是否已有銷售資料
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否存在</returns>
    public async Task<bool> ExistsAsync(int userId)
    {
        return await _context.MemberSalesProfiles
            .AnyAsync(m => m.User_Id == userId);
    }
} 
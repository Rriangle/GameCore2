using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 會員銷售資料存取實作
/// </summary>
public class MemberSalesProfileRepository : IMemberSalesProfileRepository
{
    private readonly GameCoreDbContext _context;

    public MemberSalesProfileRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據使用者ID取得會員銷售資料
    /// </summary>
    public async Task<MemberSalesProfile?> GetByUserIdAsync(int userId)
    {
        return await _context.MemberSalesProfiles
            .Include(msp => msp.User)
            .FirstOrDefaultAsync(msp => msp.UserId == userId);
    }

    /// <summary>
    /// 創建會員銷售資料
    /// </summary>
    public async Task<MemberSalesProfile> CreateAsync(MemberSalesProfile salesProfile)
    {
        _context.MemberSalesProfiles.Add(salesProfile);
        await _context.SaveChangesAsync();
        return salesProfile;
    }

    /// <summary>
    /// 更新會員銷售資料
    /// </summary>
    public async Task<MemberSalesProfile> UpdateAsync(MemberSalesProfile salesProfile)
    {
        _context.MemberSalesProfiles.Update(salesProfile);
        await _context.SaveChangesAsync();
        return salesProfile;
    }

    /// <summary>
    /// 刪除會員銷售資料
    /// </summary>
    public async Task<bool> DeleteAsync(int userId)
    {
        var salesProfile = await _context.MemberSalesProfiles
            .FirstOrDefaultAsync(msp => msp.UserId == userId);
        
        if (salesProfile == null)
            return false;

        _context.MemberSalesProfiles.Remove(salesProfile);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 檢查使用者是否已有銷售資料
    /// </summary>
    public async Task<bool> ExistsAsync(int userId)
    {
        return await _context.MemberSalesProfiles
            .AnyAsync(msp => msp.UserId == userId);
    }

    /// <summary>
    /// 取得所有已申請銷售功能的會員列表
    /// </summary>
    public async Task<IEnumerable<MemberSalesProfile>> GetAllAsync(int skip = 0, int take = 100)
    {
        return await _context.MemberSalesProfiles
            .Include(msp => msp.User)
            .OrderByDescending(msp => msp.UserId) // 按使用者ID降序排列，最新申請的在前面
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    /// <summary>
    /// 取得已申請銷售功能的會員總數
    /// </summary>
    public async Task<int> GetCountAsync()
    {
        return await _context.MemberSalesProfiles.CountAsync();
    }
}
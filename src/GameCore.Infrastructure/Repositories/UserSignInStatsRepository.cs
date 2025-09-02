using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 用戶簽到統計 Repository
/// </summary>
public class UserSignInStatsRepository : IUserSignInStatsRepository
{
    private readonly GameCoreDbContext _context;

    public UserSignInStatsRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據用戶ID和日期獲取簽到記錄
    /// </summary>
    public async Task<UserSignInStats?> GetByUserIdAndDateAsync(int userId, DateTime date)
    {
        return await _context.UserSignInStats
            .Where(s => s.UserID == userId && s.SignTime.Date == date.Date)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// 根據用戶ID和日期範圍獲取簽到記錄
    /// </summary>
    public async Task<IEnumerable<UserSignInStats>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        return await _context.UserSignInStats
            .Where(s => s.UserID == userId && s.SignTime.Date >= startDate.Date && s.SignTime.Date <= endDate.Date)
            .OrderByDescending(s => s.SignTime)
            .ToListAsync();
    }

    /// <summary>
    /// 獲取用戶最後一次簽到記錄
    /// </summary>
    public async Task<UserSignInStats?> GetLastSignInAsync(int userId)
    {
        return await _context.UserSignInStats
            .Where(s => s.UserID == userId)
            .OrderByDescending(s => s.SignTime)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// 根據用戶ID獲取簽到記錄（分頁）
    /// </summary>
    public async Task<IEnumerable<UserSignInStats>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 20)
    {
        return await _context.UserSignInStats
            .Where(s => s.UserID == userId)
            .OrderByDescending(s => s.SignTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// 獲取用戶簽到記錄總數
    /// </summary>
    public async Task<int> GetCountByUserIdAsync(int userId)
    {
        return await _context.UserSignInStats
            .Where(s => s.UserID == userId)
            .CountAsync();
    }

    /// <summary>
    /// 根據用戶ID和日期範圍獲取簽到記錄計數 - 效能優化：避免載入完整資料到記憶體
    /// </summary>
    public async Task<int> GetCountByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        return await _context.UserSignInStats
            .Where(s => s.UserID == userId && s.SignTime.Date >= startDate.Date && s.SignTime.Date <= endDate.Date)
            .CountAsync();
    }

    /// <summary>
    /// 新增簽到記錄
    /// </summary>
    public async Task<UserSignInStats> AddAsync(UserSignInStats signInStats)
    {
        _context.UserSignInStats.Add(signInStats);
        await _context.SaveChangesAsync();
        return signInStats;
    }

    /// <summary>
    /// 更新簽到記錄
    /// </summary>
    public async Task<bool> UpdateAsync(UserSignInStats signInStats)
    {
        _context.UserSignInStats.Update(signInStats);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除簽到記錄
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var signInStats = await _context.UserSignInStats.FindAsync(id);
        if (signInStats == null)
            return false;

        _context.UserSignInStats.Remove(signInStats);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
} 
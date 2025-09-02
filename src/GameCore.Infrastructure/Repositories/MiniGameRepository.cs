using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 小遊戲 Repository
/// </summary>
public class MiniGameRepository : IMiniGameRepository
{
    private readonly GameCoreDbContext _context;

    public MiniGameRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據用戶ID獲取遊戲記錄（分頁）
    /// </summary>
    public async Task<IEnumerable<MiniGame>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 20, string? result = null)
    {
        var query = _context.MiniGames.Where(g => g.UserID == userId);
        
        if (!string.IsNullOrEmpty(result))
        {
            query = query.Where(g => g.Result == result);
        }

        return await query
            .OrderByDescending(g => g.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// 獲取用戶遊戲記錄總數
    /// </summary>
    public async Task<int> GetCountByUserIdAsync(int userId, string? result = null)
    {
        var query = _context.MiniGames.Where(g => g.UserID == userId);
        
        if (!string.IsNullOrEmpty(result))
        {
            query = query.Where(g => g.Result == result);
        }

        return await query.CountAsync();
    }

    /// <summary>
    /// 獲取用戶所有遊戲記錄
    /// </summary>
    public async Task<IEnumerable<MiniGame>> GetAllByUserIdAsync(int userId)
    {
        return await _context.MiniGames
            .Where(g => g.UserID == userId)
            .OrderByDescending(g => g.StartTime)
            .ToListAsync();
    }

    /// <summary>
    /// 獲取用戶最後一次遊戲記錄
    /// </summary>
    public async Task<MiniGame?> GetLastGameAsync(int userId)
    {
        return await _context.MiniGames
            .Where(g => g.UserID == userId)
            .OrderByDescending(g => g.StartTime)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// 獲取用戶今日遊戲次數
    /// </summary>
    public async Task<int> GetTodayGamesCountAsync(int userId, DateTime today)
    {
        return await _context.MiniGames
            .Where(g => g.UserID == userId && g.StartTime.Date == today.Date)
            .CountAsync();
    }

    /// <summary>
    /// 根據ID獲取遊戲記錄
    /// </summary>
    public async Task<MiniGame?> GetByIdAsync(int id)
    {
        return await _context.MiniGames.FindAsync(id);
    }

    /// <summary>
    /// 新增遊戲記錄
    /// </summary>
    public async Task<MiniGame> AddAsync(MiniGame miniGame)
    {
        _context.MiniGames.Add(miniGame);
        await _context.SaveChangesAsync();
        return miniGame;
    }

    /// <summary>
    /// 更新遊戲記錄
    /// </summary>
    public async Task<bool> UpdateAsync(MiniGame miniGame)
    {
        _context.MiniGames.Update(miniGame);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除遊戲記錄
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var miniGame = await _context.MiniGames.FindAsync(id);
        if (miniGame == null)
            return false;

        _context.MiniGames.Remove(miniGame);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    // 效能優化：新增資料庫層面計數方法，避免記憶體載入
    /// <summary>
    /// 獲取用戶總遊戲次數
    /// </summary>
    public async Task<int> GetTotalGamesCountAsync(int userId)
    {
        return await _context.MiniGames
            .Where(g => g.UserID == userId)
            .CountAsync();
    }

    /// <summary>
    /// 獲取用戶勝利遊戲次數
    /// </summary>
    public async Task<int> GetWinGamesCountAsync(int userId)
    {
        return await _context.MiniGames
            .Where(g => g.UserID == userId && g.Result == "Win")
            .CountAsync();
    }

    /// <summary>
    /// 獲取用戶失敗遊戲次數
    /// </summary>
    public async Task<int> GetLoseGamesCountAsync(int userId)
    {
        return await _context.MiniGames
            .Where(g => g.UserID == userId && g.Result == "Lose")
            .CountAsync();
    }

    /// <summary>
    /// 獲取用戶中止遊戲次數
    /// </summary>
    public async Task<int> GetAbortedGamesCountAsync(int userId)
    {
        return await _context.MiniGames
            .Where(g => g.UserID == userId && g.Aborted)
            .CountAsync();
    }

    /// <summary>
    /// 獲取用戶總經驗值
    /// </summary>
    public async Task<int> GetTotalExpGainedAsync(int userId)
    {
        return await _context.MiniGames
            .Where(g => g.UserID == userId)
            .SumAsync(g => g.ExpGained);
    }

    /// <summary>
    /// 獲取用戶總點數
    /// </summary>
    public async Task<int> GetTotalPointsGainedAsync(int userId)
    {
        return await _context.MiniGames
            .Where(g => g.UserID == userId)
            .SumAsync(g => g.PointsChanged);
    }

    /// <summary>
    /// 獲取用戶最高等級
    /// </summary>
    public async Task<int> GetHighestLevelAsync(int userId)
    {
        var maxLevel = await _context.MiniGames
            .Where(g => g.UserID == userId)
            .MaxAsync(g => (int?)g.Level);
        
        return maxLevel ?? 0;
    }
} 
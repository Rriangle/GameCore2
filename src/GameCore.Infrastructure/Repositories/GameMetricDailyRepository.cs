using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 遊戲每日指標資料庫操作實作
/// </summary>
public class GameMetricDailyRepository : IGameMetricDailyRepository
{
    private readonly GameCoreDbContext _context;

    public GameMetricDailyRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據ID取得遊戲每日指標
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>遊戲每日指標實體</returns>
    public async Task<GameMetricDaily?> GetByIdAsync(int id)
    {
        return await _context.GameMetricDailies
            .Include(g => g.Game)
            .Include(g => g.Metric)
            .FirstOrDefaultAsync(g => g.ID == id);
    }

    /// <summary>
    /// 新增遊戲每日指標
    /// </summary>
    /// <param name="metric">遊戲每日指標實體</param>
    /// <returns>新增的遊戲每日指標實體</returns>
    public async Task<GameMetricDaily> AddAsync(GameMetricDaily metric)
    {
        _context.GameMetricDailies.Add(metric);
        await _context.SaveChangesAsync();
        return metric;
    }

    /// <summary>
    /// 更新遊戲每日指標
    /// </summary>
    /// <param name="metric">遊戲每日指標實體</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateAsync(GameMetricDaily metric)
    {
        _context.GameMetricDailies.Update(metric);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除遊戲每日指標
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var metric = await _context.GameMetricDailies.FindAsync(id);
        if (metric == null)
            return false;

        _context.GameMetricDailies.Remove(metric);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    public IQueryable<GameMetricDaily> GetQueryable()
    {
        return _context.GameMetricDailies
            .Include(g => g.Game)
            .Include(g => g.Metric);
    }

    /// <summary>
    /// 根據遊戲ID和日期取得指標資料
    /// </summary>
    /// <param name="gameId">遊戲ID</param>
    /// <param name="date">日期</param>
    /// <returns>指標資料列表</returns>
    public async Task<List<GameMetricDaily>> GetByGameAndDateAsync(int gameId, DateTime date)
    {
        return await _context.GameMetricDailies
            .Include(g => g.Game)
            .Include(g => g.Metric)
            .Where(g => g.GameID == gameId && g.Date.Date == date.Date)
            .ToListAsync();
    }
} 
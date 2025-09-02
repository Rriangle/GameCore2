using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 遊戲每日指標資料庫操作介面
/// </summary>
public interface IGameMetricDailyRepository
{
    /// <summary>
    /// 根據ID取得遊戲每日指標
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>遊戲每日指標實體</returns>
    Task<GameMetricDaily?> GetByIdAsync(int id);

    /// <summary>
    /// 新增遊戲每日指標
    /// </summary>
    /// <param name="metric">遊戲每日指標實體</param>
    /// <returns>新增的遊戲每日指標實體</returns>
    Task<GameMetricDaily> AddAsync(GameMetricDaily metric);

    /// <summary>
    /// 更新遊戲每日指標
    /// </summary>
    /// <param name="metric">遊戲每日指標實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(GameMetricDaily metric);

    /// <summary>
    /// 刪除遊戲每日指標
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    IQueryable<GameMetricDaily> GetQueryable();

    /// <summary>
    /// 根據遊戲ID和日期取得指標資料
    /// </summary>
    /// <param name="gameId">遊戲ID</param>
    /// <param name="date">日期</param>
    /// <returns>指標資料列表</returns>
    Task<List<GameMetricDaily>> GetByGameAndDateAsync(int gameId, DateTime date);
} 
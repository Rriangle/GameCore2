using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 排行榜快照資料庫操作介面
/// </summary>
public interface ILeaderboardSnapshotRepository
{
    /// <summary>
    /// 根據ID取得排行榜快照
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>排行榜快照實體</returns>
    Task<LeaderboardSnapshot?> GetByIdAsync(int id);

    /// <summary>
    /// 新增排行榜快照
    /// </summary>
    /// <param name="snapshot">排行榜快照實體</param>
    /// <returns>新增的排行榜快照實體</returns>
    Task<LeaderboardSnapshot> AddAsync(LeaderboardSnapshot snapshot);

    /// <summary>
    /// 更新排行榜快照
    /// </summary>
    /// <param name="snapshot">排行榜快照實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(LeaderboardSnapshot snapshot);

    /// <summary>
    /// 刪除排行榜快照
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    IQueryable<LeaderboardSnapshot> GetQueryable();
} 
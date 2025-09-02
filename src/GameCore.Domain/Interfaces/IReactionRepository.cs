using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 反應資料庫操作介面
/// </summary>
public interface IReactionRepository
{
    /// <summary>
    /// 根據ID取得反應
    /// </summary>
    /// <param name="reactionId">反應ID</param>
    /// <returns>反應實體</returns>
    Task<Reaction?> GetByIdAsync(long reactionId);

    /// <summary>
    /// 新增反應
    /// </summary>
    /// <param name="reaction">反應實體</param>
    /// <returns>新增的反應實體</returns>
    Task<Reaction> AddAsync(Reaction reaction);

    /// <summary>
    /// 更新反應
    /// </summary>
    /// <param name="reaction">反應實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(Reaction reaction);

    /// <summary>
    /// 刪除反應
    /// </summary>
    /// <param name="reactionId">反應ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(long reactionId);

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    IQueryable<Reaction> GetQueryable();
} 
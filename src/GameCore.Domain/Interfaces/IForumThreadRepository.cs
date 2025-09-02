using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 論壇主題資料庫操作介面
/// </summary>
public interface IForumThreadRepository
{
    /// <summary>
    /// 根據ID取得主題
    /// </summary>
    /// <param name="threadId">主題ID</param>
    /// <returns>主題實體</returns>
    Task<ForumThread?> GetByIdAsync(long threadId);

    /// <summary>
    /// 新增主題
    /// </summary>
    /// <param name="thread">主題實體</param>
    /// <returns>新增的主題實體</returns>
    Task<ForumThread> AddAsync(ForumThread thread);

    /// <summary>
    /// 更新主題
    /// </summary>
    /// <param name="thread">主題實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(ForumThread thread);

    /// <summary>
    /// 刪除主題
    /// </summary>
    /// <param name="threadId">主題ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(long threadId);

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    IQueryable<ForumThread> GetQueryable();
} 
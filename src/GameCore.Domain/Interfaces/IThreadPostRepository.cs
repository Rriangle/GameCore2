using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 主題回覆資料庫操作介面
/// </summary>
public interface IThreadPostRepository
{
    /// <summary>
    /// 根據ID取得回覆
    /// </summary>
    /// <param name="postId">回覆ID</param>
    /// <returns>回覆實體</returns>
    Task<ThreadPost?> GetByIdAsync(long postId);

    /// <summary>
    /// 新增回覆
    /// </summary>
    /// <param name="post">回覆實體</param>
    /// <returns>新增的回覆實體</returns>
    Task<ThreadPost> AddAsync(ThreadPost post);

    /// <summary>
    /// 更新回覆
    /// </summary>
    /// <param name="post">回覆實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(ThreadPost post);

    /// <summary>
    /// 刪除回覆
    /// </summary>
    /// <param name="postId">回覆ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(long postId);

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    IQueryable<ThreadPost> GetQueryable();
} 
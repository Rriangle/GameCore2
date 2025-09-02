using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 書籤資料庫操作介面
/// </summary>
public interface IBookmarkRepository
{
    /// <summary>
    /// 根據ID取得書籤
    /// </summary>
    /// <param name="bookmarkId">書籤ID</param>
    /// <returns>書籤實體</returns>
    Task<Bookmark?> GetByIdAsync(int bookmarkId);

    /// <summary>
    /// 新增書籤
    /// </summary>
    /// <param name="bookmark">書籤實體</param>
    /// <returns>新增的書籤實體</returns>
    Task<Bookmark> AddAsync(Bookmark bookmark);

    /// <summary>
    /// 更新書籤
    /// </summary>
    /// <param name="bookmark">書籤實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(Bookmark bookmark);

    /// <summary>
    /// 刪除書籤
    /// </summary>
    /// <param name="bookmarkId">書籤ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int bookmarkId);

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    IQueryable<Bookmark> GetQueryable();
} 
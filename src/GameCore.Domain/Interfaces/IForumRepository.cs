using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 論壇資料存取介面
/// </summary>
public interface IForumRepository
{
    /// <summary>
    /// 根據ID取得論壇
    /// </summary>
    /// <param name="id">論壇ID</param>
    /// <returns>論壇資訊</returns>
    Task<Forum?> GetByIdAsync(int id);

    /// <summary>
    /// 新增論壇
    /// </summary>
    /// <param name="forum">論壇資訊</param>
    /// <returns>新增的論壇</returns>
    Task<Forum?> AddAsync(Forum forum);

    /// <summary>
    /// 更新論壇
    /// </summary>
    /// <param name="forum">論壇資訊</param>
    /// <returns>更新結果</returns>
    Task<bool> UpdateAsync(Forum forum);

    /// <summary>
    /// 刪除論壇
    /// </summary>
    /// <param name="id">論壇ID</param>
    /// <returns>刪除結果</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 取得可查詢的資料集
    /// </summary>
    /// <returns>IQueryable 資料集</returns>
    IQueryable<Forum> GetQueryable();
} 
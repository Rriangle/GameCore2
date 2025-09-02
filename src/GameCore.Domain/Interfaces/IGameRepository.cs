using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 遊戲資料存取介面
/// </summary>
public interface IGameRepository
{
    /// <summary>
    /// 根據ID取得遊戲
    /// </summary>
    /// <param name="id">遊戲ID</param>
    /// <returns>遊戲資訊</returns>
    Task<Game?> GetByIdAsync(int id);

    /// <summary>
    /// 新增遊戲
    /// </summary>
    /// <param name="game">遊戲資訊</param>
    /// <returns>新增的遊戲</returns>
    Task<Game?> AddAsync(Game game);

    /// <summary>
    /// 更新遊戲
    /// </summary>
    /// <param name="game">遊戲資訊</param>
    /// <returns>更新結果</returns>
    Task<bool> UpdateAsync(Game game);

    /// <summary>
    /// 刪除遊戲
    /// </summary>
    /// <param name="id">遊戲ID</param>
    /// <returns>刪除結果</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 取得可查詢的資料集
    /// </summary>
    /// <returns>IQueryable 資料集</returns>
    IQueryable<Game> GetQueryable();
} 
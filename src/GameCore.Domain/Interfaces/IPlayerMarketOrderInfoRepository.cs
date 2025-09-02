using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 玩家市場訂單資料存取介面
/// </summary>
public interface IPlayerMarketOrderInfoRepository
{
    /// <summary>
    /// 根據ID取得訂單
    /// </summary>
    /// <param name="id">訂單ID</param>
    /// <returns>訂單資訊</returns>
    Task<PlayerMarketOrderInfo?> GetByIdAsync(int id);

    /// <summary>
    /// 新增訂單
    /// </summary>
    /// <param name="order">訂單資訊</param>
    /// <returns>新增的訂單</returns>
    Task<PlayerMarketOrderInfo?> AddAsync(PlayerMarketOrderInfo order);

    /// <summary>
    /// 更新訂單
    /// </summary>
    /// <param name="order">訂單資訊</param>
    /// <returns>更新結果</returns>
    Task<bool> UpdateAsync(PlayerMarketOrderInfo order);

    /// <summary>
    /// 刪除訂單
    /// </summary>
    /// <param name="id">訂單ID</param>
    /// <returns>刪除結果</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 取得可查詢的資料集
    /// </summary>
    /// <returns>IQueryable 資料集</returns>
    IQueryable<PlayerMarketOrderInfo> GetQueryable();
} 
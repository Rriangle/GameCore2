using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 玩家市場商品資料存取介面
/// </summary>
public interface IPlayerMarketProductInfoRepository
{
    /// <summary>
    /// 根據ID取得商品
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>商品資訊</returns>
    Task<PlayerMarketProductInfo?> GetByIdAsync(int id);

    /// <summary>
    /// 新增商品
    /// </summary>
    /// <param name="product">商品資訊</param>
    /// <returns>新增的商品</returns>
    Task<PlayerMarketProductInfo?> AddAsync(PlayerMarketProductInfo product);

    /// <summary>
    /// 更新商品
    /// </summary>
    /// <param name="product">商品資訊</param>
    /// <returns>更新結果</returns>
    Task<bool> UpdateAsync(PlayerMarketProductInfo product);

    /// <summary>
    /// 刪除商品
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>刪除結果</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 取得可查詢的資料集
    /// </summary>
    /// <returns>IQueryable 資料集</returns>
    IQueryable<PlayerMarketProductInfo> GetQueryable();
} 
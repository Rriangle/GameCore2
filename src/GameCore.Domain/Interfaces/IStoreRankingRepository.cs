using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 商城排行榜資料存取介面，定義排行榜相關的資料庫操作
/// </summary>
public interface IStoreRankingRepository
{
    /// <summary>
    /// 根據編號取得排行榜記錄
    /// </summary>
    /// <param name="rankingId">排行榜編號</param>
    /// <returns>排行榜記錄，不存在則返回 null</returns>
    Task<OfficialStoreRanking?> GetByIdAsync(int rankingId);

    /// <summary>
    /// 取得排行榜資料 (依查詢條件)
    /// </summary>
    /// <param name="query">查詢條件</param>
    /// <returns>排行榜資料列表</returns>
    Task<List<StoreRankingDto>> GetRankingsAsync(RankingQueryDto query);

    /// <summary>
    /// 取得商品在特定期間的排名
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <param name="periodType">期間類型 (daily, weekly, monthly)</param>
    /// <param name="rankingDate">排名日期</param>
    /// <returns>排名記錄，不存在則返回 null</returns>
    Task<OfficialStoreRanking?> GetProductRankingAsync(int productId, string periodType, DateTime rankingDate);

    /// <summary>
    /// 取得熱門商品排行榜 (前N名)
    /// </summary>
    /// <param name="periodType">期間類型 (daily, weekly, monthly)</param>
    /// <param name="rankingDate">排名日期</param>
    /// <param name="limit">取得數量</param>
    /// <returns>熱門商品排行榜</returns>
    Task<List<StoreRankingDto>> GetTopProductsAsync(string periodType, DateTime rankingDate, int limit = 10);

    /// <summary>
    /// 建立或更新排行榜記錄
    /// </summary>
    /// <param name="ranking">排行榜資料</param>
    /// <returns>建立或更新的排行榜記錄</returns>
    Task<OfficialStoreRanking> CreateOrUpdateAsync(OfficialStoreRanking ranking);

    /// <summary>
    /// 批次建立或更新排行榜記錄
    /// </summary>
    /// <param name="rankings">排行榜資料列表</param>
    /// <returns>處理的記錄數量</returns>
    Task<int> BatchCreateOrUpdateAsync(List<OfficialStoreRanking> rankings);

    /// <summary>
    /// 刪除過期的排行榜記錄
    /// </summary>
    /// <param name="olderThan">刪除早於此日期的記錄</param>
    /// <returns>刪除的記錄數量</returns>
    Task<int> DeleteOldRecordsAsync(DateTime olderThan);

    /// <summary>
    /// 計算商品的排行榜數據
    /// </summary>
    /// <param name="startDate">開始日期</param>
    /// <param name="endDate">結束日期</param>
    /// <param name="periodType">期間類型 (daily, weekly, monthly)</param>
    /// <returns>計算結果列表</returns>
    Task<List<OfficialStoreRanking>> CalculateRankingsAsync(DateTime startDate, DateTime endDate, string periodType);

    /// <summary>
    /// 取得排行榜統計資訊
    /// </summary>
    /// <param name="periodType">期間類型</param>
    /// <param name="startDate">開始日期</param>
    /// <param name="endDate">結束日期</param>
    /// <returns>統計資訊</returns>
    Task<Dictionary<string, object>> GetRankingStatisticsAsync(string periodType, DateTime startDate, DateTime endDate);

    /// <summary>
    /// 檢查排行榜記錄是否存在
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <param name="periodType">期間類型</param>
    /// <param name="rankingDate">排名日期</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(int productId, string periodType, DateTime rankingDate);
}
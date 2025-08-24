using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 熱度服務介面，定義遊戲熱度指數、排行榜相關的業務邏輯
/// </summary>
public interface IPopularityService
{
    /// <summary>
    /// 取得遊戲列表
    /// </summary>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁遊戲列表</returns>
    Task<PagedGamesDto> GetGamesAsync(GameQueryDto query);

    /// <summary>
    /// 取得遊戲詳情
    /// </summary>
    /// <param name="gameId">遊戲編號</param>
    /// <returns>遊戲詳情，不存在則返回 null</returns>
    Task<GameDetailDto?> GetGameDetailAsync(int gameId);

    /// <summary>
    /// 取得指標來源列表
    /// </summary>
    /// <returns>指標來源列表</returns>
    Task<List<MetricSourceDto>> GetMetricSourcesAsync();

    /// <summary>
    /// 取得指標列表
    /// </summary>
    /// <param name="sourceId">來源編號 (可選)</param>
    /// <returns>指標列表</returns>
    Task<List<MetricDto>> GetMetricsAsync(int? sourceId = null);

    /// <summary>
    /// 取得遊戲的指標數據 (時間序列)
    /// </summary>
    /// <param name="gameId">遊戲編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁指標數據</returns>
    Task<PagedGameMetricsDto> GetGameMetricsAsync(int gameId, GameMetricQueryDto query);

    /// <summary>
    /// 取得遊戲的熱度指數歷史
    /// </summary>
    /// <param name="gameId">遊戲編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁熱度指數數據</returns>
    Task<PagedPopularityIndexDto> GetGamePopularityHistoryAsync(int gameId, PopularityIndexQueryDto query);

    /// <summary>
    /// 取得排行榜快照
    /// </summary>
    /// <param name="query">查詢條件</param>
    /// <returns>排行榜快照</returns>
    Task<LeaderboardSnapshotDto> GetLeaderboardSnapshotAsync(LeaderboardQueryDto query);

    /// <summary>
    /// 取得最新排行榜
    /// </summary>
    /// <param name="period">期間類型 (daily/weekly)</param>
    /// <param name="limit">取得數量</param>
    /// <returns>排行榜項目列表</returns>
    Task<List<LeaderboardItemDto>> GetLatestLeaderboardAsync(string period = "daily", int limit = 10);

    /// <summary>
    /// 取得遊戲排名歷史
    /// </summary>
    /// <param name="gameId">遊戲編號</param>
    /// <param name="period">期間類型 (daily/weekly)</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁排名歷史</returns>
    Task<PagedGameRankHistoryDto> GetGameRankHistoryAsync(int gameId, string period, GameRankQueryDto query);

    /// <summary>
    /// 計算熱度指數 (系統後台用)
    /// </summary>
    /// <param name="gameId">遊戲編號</param>
    /// <param name="date">日期</param>
    /// <returns>計算的熱度指數</returns>
    Task<decimal> CalculatePopularityIndexAsync(int gameId, DateOnly date);

    /// <summary>
    /// 產生排行榜快照 (系統後台用)
    /// </summary>
    /// <param name="period">期間類型</param>
    /// <param name="date">日期</param>
    /// <returns>產生的快照數量</returns>
    Task<int> GenerateLeaderboardSnapshotAsync(string period, DateOnly date);

    /// <summary>
    /// 取得遊戲來源對應
    /// </summary>
    /// <param name="gameId">遊戲編號</param>
    /// <returns>遊戲來源對應列表</returns>
    Task<List<GameSourceMapDto>> GetGameSourceMapsAsync(int gameId);

    /// <summary>
    /// 搜尋遊戲
    /// </summary>
    /// <param name="query">搜尋條件</param>
    /// <returns>分頁搜尋結果</returns>
    Task<PagedGamesDto> SearchGamesAsync(GameSearchQueryDto query);

    /// <summary>
    /// 取得熱門遊戲
    /// </summary>
    /// <param name="period">期間 (daily/weekly)</param>
    /// <param name="limit">取得數量</param>
    /// <returns>熱門遊戲列表</returns>
    Task<List<GameDto>> GetTrendingGamesAsync(string period = "daily", int limit = 10);

    /// <summary>
    /// 取得遊戲統計摘要
    /// </summary>
    /// <param name="gameId">遊戲編號</param>
    /// <returns>遊戲統計摘要</returns>
    Task<GameStatsDto> GetGameStatsAsync(int gameId);

    /// <summary>
    /// 比較多個遊戲的指標
    /// </summary>
    /// <param name="gameIds">遊戲編號列表</param>
    /// <param name="metricId">指標編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>遊戲指標比較數據</returns>
    Task<GameMetricComparisonDto> CompareGamesMetricAsync(List<int> gameIds, int metricId, GameMetricQueryDto query);
}
using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces
{
    /// <summary>
    /// 遊戲熱度服務接口
    /// </summary>
    public interface IPopularityService
    {
        /// <summary>
        /// 獲取所有遊戲
        /// </summary>
        Task<IEnumerable<Game>> GetAllGamesAsync();
        
        /// <summary>
        /// 獲取遊戲熱度指數
        /// </summary>
        Task<IEnumerable<PopularityIndexDaily>> GetGamePopularityAsync(int gameId, DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// 獲取排行榜快照
        /// </summary>
        Task<IEnumerable<LeaderboardSnapshot>> GetLeaderboardAsync(string period, DateTime? timestamp = null);
        
        /// <summary>
        /// 計算並更新遊戲熱度指數
        /// </summary>
        Task CalculatePopularityIndexAsync(int gameId, DateTime date);
        
        /// <summary>
        /// 生成排行榜快照
        /// </summary>
        Task GenerateLeaderboardSnapshotAsync(string period, DateTime timestamp);
        
        /// <summary>
        /// 獲取遊戲指標數據
        /// </summary>
        Task<IEnumerable<GameMetricDaily>> GetGameMetricsAsync(int gameId, DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// 獲取指標來源
        /// </summary>
        Task<IEnumerable<MetricSource>> GetMetricSourcesAsync();
        
        /// <summary>
        /// 獲取指標定義
        /// </summary>
        Task<IEnumerable<Metric>> GetMetricsAsync(int? sourceId = null);
    }
}
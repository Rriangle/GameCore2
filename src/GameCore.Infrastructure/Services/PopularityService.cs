using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 遊戲熱度服務實現 - 優化版本
/// 增強性能、快取、輸入驗證、錯誤處理和可維護性
/// </summary>
public class PopularityService : IPopularityService
{
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<PopularityService> _logger;

    // 常數定義，提高可維護性
    private const int MaxPageSize = 1000;
    private const int DefaultPageSize = 100;
    private const int CacheExpirationMinutes = 15;
    private const string AllGamesCacheKey = "Popularity_AllGames";
    private const string GamePopularityCacheKey = "Popularity_Game_{0}_{1:yyyyMMdd}_{2:yyyyMMdd}";
    private const string LeaderboardCacheKey = "Popularity_Leaderboard_{0}_{1:yyyyMMdd_HHmmss}";
    private const string GameMetricsCacheKey = "Popularity_Metrics_{0}_{1:yyyyMMdd}_{2:yyyyMMdd}";
    private const string MetricSourcesCacheKey = "Popularity_MetricSources";
    private const string MetricsCacheKey = "Popularity_Metrics_{0}";

    // 指標權重配置
    private static readonly Dictionary<string, decimal> MetricWeights = new()
    {
        { "concurrent_users", 0.4m },
        { "forum_posts", 0.2m },
        { "social_mentions", 0.15m },
        { "stream_viewers", 0.15m },
        { "news_articles", 0.1m }
    };

    public PopularityService(
        GameCoreDbContext context,
        IMemoryCache memoryCache,
        ILogger<PopularityService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 獲取所有遊戲 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Game>> GetAllGamesAsync()
    {
        _logger.LogInformation("開始獲取所有遊戲");

        try
        {
            // 嘗試從快取獲取
            if (_memoryCache.TryGetValue(AllGamesCacheKey, out IEnumerable<Game> cachedGames))
            {
                _logger.LogDebug("從快取獲取所有遊戲，數量: {Count}", cachedGames.Count());
                return cachedGames;
            }

            // 從資料庫獲取
            var games = await _context.Games
                .Include(g => g.PopularityIndexDailies.OrderByDescending(p => p.date).Take(1))
                .OrderBy(g => g.name)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(AllGamesCacheKey, games, cacheOptions);

            _logger.LogInformation("成功獲取所有遊戲，數量: {Count}", games.Count);
            return games;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲列表時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取遊戲熱度指數 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<PopularityIndexDaily>> GetGamePopularityAsync(int gameId, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("開始獲取遊戲熱度指數，遊戲ID: {GameId}, 開始日期: {StartDate}, 結束日期: {EndDate}", 
            gameId, startDate, endDate);

        try
        {
            // 輸入驗證
            var validationResult = ValidateDateRange(startDate, endDate);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("日期範圍驗證失敗: {Error}", validationResult.ErrorMessage);
                return Enumerable.Empty<PopularityIndexDaily>();
            }

            if (gameId <= 0)
            {
                _logger.LogWarning("無效的遊戲ID: {GameId}", gameId);
                return Enumerable.Empty<PopularityIndexDaily>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(GamePopularityCacheKey, gameId, startDate, endDate);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<PopularityIndexDaily> cachedData))
            {
                _logger.LogDebug("從快取獲取遊戲熱度指數，遊戲ID: {GameId}", gameId);
                return cachedData;
            }

            // 從資料庫獲取
            var popularityData = await _context.PopularityIndexDailies
                .Include(p => p.Game)
                .Where(p => p.game_id == gameId && p.date >= startDate && p.date <= endDate)
                .OrderBy(p => p.date)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, popularityData, cacheOptions);

            _logger.LogInformation("成功獲取遊戲熱度指數，遊戲ID: {GameId}, 記錄數: {Count}", gameId, popularityData.Count);
            return popularityData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲 {GameId} 熱度指數時發生錯誤", gameId);
            throw;
        }
    }

    /// <summary>
    /// 獲取排行榜快照 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<LeaderboardSnapshot>> GetLeaderboardAsync(string period, DateTime? timestamp = null)
    {
        _logger.LogInformation("開始獲取排行榜快照，時段: {Period}, 時間戳: {Timestamp}", period, timestamp);

        try
        {
            // 輸入驗證
            if (string.IsNullOrWhiteSpace(period))
            {
                _logger.LogWarning("時段參數不能為空");
                return Enumerable.Empty<LeaderboardSnapshot>();
            }

            var effectiveTimestamp = timestamp ?? DateTime.UtcNow;
            var cacheKey = string.Format(LeaderboardCacheKey, period, effectiveTimestamp);

            // 嘗試從快取獲取
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<LeaderboardSnapshot> cachedLeaderboard))
            {
                _logger.LogDebug("從快取獲取排行榜，時段: {Period}", period);
                return cachedLeaderboard;
            }

            // 從資料庫獲取
            var query = _context.LeaderboardSnapshots
                .Include(l => l.Game)
                .Where(l => l.period == period);

            if (timestamp.HasValue)
            {
                query = query.Where(l => l.ts == timestamp.Value);
            }
            else
            {
                // 獲取最新的快照
                var latestTimestamp = await _context.LeaderboardSnapshots
                    .Where(l => l.period == period)
                    .MaxAsync(l => l.ts);
                query = query.Where(l => l.ts == latestTimestamp);
            }

            var leaderboard = await query
                .OrderBy(l => l.rank)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, leaderboard, cacheOptions);

            _logger.LogInformation("成功獲取排行榜，時段: {Period}, 記錄數: {Count}", period, leaderboard.Count);
            return leaderboard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取 {Period} 排行榜時發生錯誤", period);
            throw;
        }
    }

    /// <summary>
    /// 計算並更新遊戲熱度指數 - 優化版本
    /// </summary>
    public async Task CalculatePopularityIndexAsync(int gameId, DateTime date)
    {
        _logger.LogInformation("開始計算遊戲熱度指數，遊戲ID: {GameId}, 日期: {Date}", gameId, date);

        try
        {
            // 輸入驗證
            if (gameId <= 0)
            {
                _logger.LogWarning("無效的遊戲ID: {GameId}", gameId);
                throw new ArgumentException("無效的遊戲ID");
            }

            if (date > DateTime.UtcNow.Date)
            {
                _logger.LogWarning("日期不能是未來日期: {Date}", date);
                throw new ArgumentException("日期不能是未來日期");
            }

            // 檢查是否已存在當日的指數
            var existingIndex = await _context.PopularityIndexDailies
                .FirstOrDefaultAsync(p => p.game_id == gameId && p.date == date);

            if (existingIndex != null)
            {
                _logger.LogInformation("遊戲 {GameId} 在 {Date} 的熱度指數已存在，跳過計算", gameId, date);
                return;
            }

            // 獲取當日的指標數據
            var metrics = await _context.GameMetricDailies
                .Include(m => m.Metric)
                .Where(m => m.game_id == gameId && m.date == date && m.quality == "real")
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            if (!metrics.Any())
            {
                _logger.LogWarning("遊戲 {GameId} 在 {Date} 沒有指標數據，無法計算熱度指數", gameId, date);
                return;
            }

            // 計算加權熱度指數
            var indexValue = CalculateWeightedIndex(metrics);

            // 創建新的熱度指數記錄
            var popularityIndex = new PopularityIndexDaily
            {
                game_id = gameId,
                date = date,
                index_value = indexValue,
                created_at = DateTime.UtcNow
            };

            _context.PopularityIndexDailies.Add(popularityIndex);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearPopularityRelatedCache(gameId);

            _logger.LogInformation("成功計算遊戲 {GameId} 在 {Date} 的熱度指數: {IndexValue}", gameId, date, indexValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "計算遊戲 {GameId} 熱度指數時發生錯誤", gameId);
            throw;
        }
    }

    /// <summary>
    /// 生成排行榜快照 - 優化版本
    /// </summary>
    public async Task GenerateLeaderboardSnapshotAsync(string period, DateTime timestamp)
    {
        _logger.LogInformation("開始生成排行榜快照，時段: {Period}, 時間戳: {Timestamp}", period, timestamp);

        try
        {
            // 輸入驗證
            if (string.IsNullOrWhiteSpace(period))
            {
                _logger.LogWarning("時段參數不能為空");
                throw new ArgumentException("時段參數不能為空");
            }

            if (timestamp > DateTime.UtcNow)
            {
                _logger.LogWarning("時間戳不能是未來時間: {Timestamp}", timestamp);
                throw new ArgumentException("時間戳不能是未來時間");
            }

            // 檢查是否已存在該時段的快照
            var existingSnapshot = await _context.LeaderboardSnapshots
                .AnyAsync(l => l.period == period && l.ts == timestamp);

            if (existingSnapshot)
            {
                _logger.LogInformation("時段 {Period} 在 {Timestamp} 的快照已存在，跳過生成", period, timestamp);
                return;
            }

            // 根據時段獲取相應的熱度指數數據
            var startDate = GetStartDateForPeriod(period, timestamp);
            var endDate = timestamp;

            var gamePopularity = await GetGamePopularityForPeriod(startDate, endDate);

            // 生成排行榜快照
            var snapshots = CreateLeaderboardSnapshots(period, timestamp, gamePopularity);

            _context.LeaderboardSnapshots.AddRange(snapshots);
            await _context.SaveChangesAsync();

            // 清除相關快取
            ClearLeaderboardRelatedCache(period);

            _logger.LogInformation("成功生成時段 {Period} 在 {Timestamp} 的排行榜快照，共 {Count} 個遊戲", 
                period, timestamp, snapshots.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成排行榜快照時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取遊戲指標數據 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<GameMetricDaily>> GetGameMetricsAsync(int gameId, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation("開始獲取遊戲指標數據，遊戲ID: {GameId}, 開始日期: {StartDate}, 結束日期: {EndDate}", 
            gameId, startDate, endDate);

        try
        {
            // 輸入驗證
            var validationResult = ValidateDateRange(startDate, endDate);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("日期範圍驗證失敗: {Error}", validationResult.ErrorMessage);
                return Enumerable.Empty<GameMetricDaily>();
            }

            if (gameId <= 0)
            {
                _logger.LogWarning("無效的遊戲ID: {GameId}", gameId);
                return Enumerable.Empty<GameMetricDaily>();
            }

            // 嘗試從快取獲取
            var cacheKey = string.Format(GameMetricsCacheKey, gameId, startDate, endDate);
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<GameMetricDaily> cachedMetrics))
            {
                _logger.LogDebug("從快取獲取遊戲指標數據，遊戲ID: {GameId}", gameId);
                return cachedMetrics;
            }

            // 從資料庫獲取
            var metrics = await _context.GameMetricDailies
                .Include(m => m.Metric)
                .ThenInclude(metric => metric.Source)
                .Where(m => m.game_id == gameId && m.date >= startDate && m.date <= endDate)
                .OrderBy(m => m.date)
                .ThenBy(m => m.metric_id)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, metrics, cacheOptions);

            _logger.LogInformation("成功獲取遊戲指標數據，遊戲ID: {GameId}, 記錄數: {Count}", gameId, metrics.Count);
            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲 {GameId} 指標數據時發生錯誤", gameId);
            throw;
        }
    }

    /// <summary>
    /// 獲取指標來源 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<MetricSource>> GetMetricSourcesAsync()
    {
        _logger.LogInformation("開始獲取指標來源");

        try
        {
            // 嘗試從快取獲取
            if (_memoryCache.TryGetValue(MetricSourcesCacheKey, out IEnumerable<MetricSource> cachedSources))
            {
                _logger.LogDebug("從快取獲取指標來源，數量: {Count}", cachedSources.Count());
                return cachedSources;
            }

            // 從資料庫獲取
            var sources = await _context.MetricSources
                .Include(s => s.Metrics.Where(m => m.is_active))
                .OrderBy(s => s.name)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(MetricSourcesCacheKey, sources, cacheOptions);

            _logger.LogInformation("成功獲取指標來源，數量: {Count}", sources.Count);
            return sources;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取指標來源時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 獲取指標定義 - 優化版本（含快取）
    /// </summary>
    public async Task<IEnumerable<Metric>> GetMetricsAsync(int? sourceId = null)
    {
        _logger.LogInformation("開始獲取指標定義，來源ID: {SourceId}", sourceId);

        try
        {
            var cacheKey = string.Format(MetricsCacheKey, sourceId ?? 0);

            // 嘗試從快取獲取
            if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Metric> cachedMetrics))
            {
                _logger.LogDebug("從快取獲取指標定義，來源ID: {SourceId}", sourceId);
                return cachedMetrics;
            }

            // 從資料庫獲取
            var query = _context.Metrics
                .Include(m => m.Source)
                .Where(m => m.is_active);

            if (sourceId.HasValue)
            {
                query = query.Where(m => m.source_id == sourceId.Value);
            }

            var metrics = await query
                .OrderBy(m => m.source_id)
                .ThenBy(m => m.code)
                .AsNoTracking() // 提高查詢性能
                .ToListAsync();

            // 存入快取
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
            _memoryCache.Set(cacheKey, metrics, cacheOptions);

            _logger.LogInformation("成功獲取指標定義，來源ID: {SourceId}, 數量: {Count}", sourceId, metrics.Count);
            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取指標定義時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 清除熱度相關快取 - 新增方法
    /// </summary>
    public void ClearPopularityRelatedCache(int gameId)
    {
        try
        {
            _memoryCache.Remove(AllGamesCacheKey);
            
            // 清除特定遊戲的熱度快取（使用模式匹配）
            var keysToRemove = new List<string>();
            foreach (var key in _memoryCache.GetKeys())
            {
                if (key is string stringKey && stringKey.Contains($"Popularity_Game_{gameId}_"))
                {
                    keysToRemove.Add(stringKey);
                }
            }

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }

            _logger.LogDebug("已清除遊戲 {GameId} 的熱度相關快取", gameId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除熱度相關快取時發生錯誤");
        }
    }

    /// <summary>
    /// 清除排行榜相關快取 - 新增方法
    /// </summary>
    public void ClearLeaderboardRelatedCache(string period)
    {
        try
        {
            // 清除特定時段的排行榜快取
            var keysToRemove = new List<string>();
            foreach (var key in _memoryCache.GetKeys())
            {
                if (key is string stringKey && stringKey.Contains($"Popularity_Leaderboard_{period}_"))
                {
                    keysToRemove.Add(stringKey);
                }
            }

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }

            _logger.LogDebug("已清除時段 {Period} 的排行榜相關快取", period);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清除排行榜相關快取時發生錯誤");
        }
    }

    #region 私有方法

    /// <summary>
    /// 驗證日期範圍 - 新增方法
    /// </summary>
    private ValidationResult ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        var result = new ValidationResult();

        if (startDate > endDate)
            result.AddError("開始日期不能晚於結束日期");

        if (startDate > DateTime.UtcNow.Date)
            result.AddError("開始日期不能是未來日期");

        if (endDate > DateTime.UtcNow.Date)
            result.AddError("結束日期不能是未來日期");

        var dateRange = endDate - startDate;
        if (dateRange.TotalDays > 365)
            result.AddError("日期範圍不能超過一年");

        return result;
    }

    /// <summary>
    /// 計算加權熱度指數 - 優化版本
    /// </summary>
    private decimal CalculateWeightedIndex(List<GameMetricDaily> metrics)
    {
        if (metrics == null || !metrics.Any())
        {
            _logger.LogWarning("指標數據為空，無法計算熱度指數");
            return 0;
        }

        decimal totalWeightedValue = 0;
        decimal totalWeight = 0;

        foreach (var metric in metrics)
        {
            if (MetricWeights.TryGetValue(metric.Metric.code, out var weight))
            {
                totalWeightedValue += metric.value * weight;
                totalWeight += weight;
            }
        }

        // 如果沒有找到權重，使用簡單平均
        if (totalWeight == 0)
        {
            totalWeightedValue = metrics.Sum(m => m.value);
            totalWeight = metrics.Count;
        }

        var result = totalWeight > 0 ? totalWeightedValue / totalWeight : 0;
        
        _logger.LogDebug("計算熱度指數完成，指標數量: {Count}, 加權值: {WeightedValue}, 總權重: {TotalWeight}, 結果: {Result}", 
            metrics.Count, totalWeightedValue, totalWeight, result);

        return result;
    }

    /// <summary>
    /// 根據時段獲取開始日期 - 優化版本
    /// </summary>
    private DateTime GetStartDateForPeriod(string period, DateTime timestamp)
    {
        return period.ToLowerInvariant() switch
        {
            "daily" => timestamp.Date,
            "weekly" => timestamp.Date.AddDays(-6),
            "monthly" => timestamp.Date.AddMonths(-1),
            "quarterly" => timestamp.Date.AddMonths(-3),
            "yearly" => timestamp.Date.AddYears(-1),
            _ => timestamp.Date.AddDays(-6) // 默認週
        };
    }

    /// <summary>
    /// 獲取時段內的遊戲熱度數據 - 優化版本
    /// </summary>
    private async Task<List<GamePopularityData>> GetGamePopularityForPeriod(DateTime startDate, DateTime endDate)
    {
        var query = from g in _context.Games
                   join pid in _context.PopularityIndexDailies on g.game_id equals pid.game_id
                   where pid.date >= startDate && pid.date <= endDate
                   group new { g, pid } by g.game_id into g
                   select new GamePopularityData
                   {
                       GameId = g.Key,
                       GameName = g.First().g.name,
                       AverageIndexValue = g.Average(x => x.pid.index_value),
                       MaxIndexValue = g.Max(x => x.pid.index_value),
                       MinIndexValue = g.Min(x => x.pid.index_value),
                       DataPointCount = g.Count()
                   };

        return await query
            .OrderByDescending(x => x.AverageIndexValue)
            .Take(MaxPageSize) // 使用常數限制前100名
            .AsNoTracking() // 提高查詢性能
            .ToListAsync();
    }

    /// <summary>
    /// 創建排行榜快照 - 新增方法
    /// </summary>
    private List<LeaderboardSnapshot> CreateLeaderboardSnapshots(string period, DateTime timestamp, List<GamePopularityData> gamePopularity)
    {
        var snapshots = new List<LeaderboardSnapshot>();
        var rank = 1;

        foreach (var game in gamePopularity)
        {
            var snapshot = new LeaderboardSnapshot
            {
                period = period,
                ts = timestamp,
                rank = rank++,
                game_id = game.GameId,
                index_value = game.AverageIndexValue,
                created_at = DateTime.UtcNow
            };

            snapshots.Add(snapshot);
        }

        return snapshots;
    }

    /// <summary>
    /// 遊戲熱度數據內部類
    /// </summary>
    private class GamePopularityData
    {
        public int GameId { get; set; }
        public string GameName { get; set; } = string.Empty;
        public decimal AverageIndexValue { get; set; }
        public decimal MaxIndexValue { get; set; }
        public decimal MinIndexValue { get; set; }
        public int DataPointCount { get; set; }
    }

    #endregion
}
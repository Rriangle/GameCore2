using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GameCore.Infrastructure.Services
{
    /// <summary>
    /// 遊戲熱度服務實現
    /// </summary>
    public class PopularityService : IPopularityService
    {
        private readonly GameCoreDbContext _context;
        private readonly ILogger<PopularityService> _logger;

        public PopularityService(GameCoreDbContext context, ILogger<PopularityService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 獲取所有遊戲
        /// </summary>
        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            try
            {
                return await _context.Games
                    .Include(g => g.PopularityIndexDailies.OrderByDescending(p => p.date).Take(1))
                    .OrderBy(g => g.name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取遊戲列表時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 獲取遊戲熱度指數
        /// </summary>
        public async Task<IEnumerable<PopularityIndexDaily>> GetGamePopularityAsync(int gameId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.PopularityIndexDailies
                    .Include(p => p.Game)
                    .Where(p => p.game_id == gameId && p.date >= startDate && p.date <= endDate)
                    .OrderBy(p => p.date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取遊戲 {GameId} 熱度指數時發生錯誤", gameId);
                throw;
            }
        }

        /// <summary>
        /// 獲取排行榜快照
        /// </summary>
        public async Task<IEnumerable<LeaderboardSnapshot>> GetLeaderboardAsync(string period, DateTime? timestamp = null)
        {
            try
            {
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

                return await query
                    .OrderBy(l => l.rank)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取 {Period} 排行榜時發生錯誤", period);
                throw;
            }
        }

        /// <summary>
        /// 計算並更新遊戲熱度指數
        /// </summary>
        public async Task CalculatePopularityIndexAsync(int gameId, DateTime date)
        {
            try
            {
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

                _logger.LogInformation("成功計算遊戲 {GameId} 在 {Date} 的熱度指數: {IndexValue}", gameId, date, indexValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "計算遊戲 {GameId} 熱度指數時發生錯誤", gameId);
                throw;
            }
        }

        /// <summary>
        /// 生成排行榜快照
        /// </summary>
        public async Task GenerateLeaderboardSnapshotAsync(string period, DateTime timestamp)
        {
            try
            {
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
                var rank = 1;
                var snapshots = new List<LeaderboardSnapshot>();

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

                _context.LeaderboardSnapshots.AddRange(snapshots);
                await _context.SaveChangesAsync();

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
        /// 獲取遊戲指標數據
        /// </summary>
        public async Task<IEnumerable<GameMetricDaily>> GetGameMetricsAsync(int gameId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.GameMetricDailies
                    .Include(m => m.Metric)
                    .ThenInclude(metric => metric.Source)
                    .Where(m => m.game_id == gameId && m.date >= startDate && m.date <= endDate)
                    .OrderBy(m => m.date)
                    .ThenBy(m => m.metric_id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取遊戲 {GameId} 指標數據時發生錯誤", gameId);
                throw;
            }
        }

        /// <summary>
        /// 獲取指標來源
        /// </summary>
        public async Task<IEnumerable<MetricSource>> GetMetricSourcesAsync()
        {
            try
            {
                return await _context.MetricSources
                    .Include(s => s.Metrics.Where(m => m.is_active))
                    .OrderBy(s => s.name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取指標來源時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 獲取指標定義
        /// </summary>
        public async Task<IEnumerable<Metric>> GetMetricsAsync(int? sourceId = null)
        {
            try
            {
                var query = _context.Metrics
                    .Include(m => m.Source)
                    .Where(m => m.is_active);

                if (sourceId.HasValue)
                {
                    query = query.Where(m => m.source_id == sourceId.Value);
                }

                return await query
                    .OrderBy(m => m.source_id)
                    .ThenBy(m => m.code)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取指標定義時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 計算加權熱度指數
        /// </summary>
        private decimal CalculateWeightedIndex(List<GameMetricDaily> metrics)
        {
            // 定義不同指標的權重
            var weights = new Dictionary<string, decimal>
            {
                { "concurrent_users", 0.4m },
                { "forum_posts", 0.2m },
                { "social_mentions", 0.15m },
                { "stream_viewers", 0.15m },
                { "news_articles", 0.1m }
            };

            decimal totalWeightedValue = 0;
            decimal totalWeight = 0;

            foreach (var metric in metrics)
            {
                if (weights.TryGetValue(metric.Metric.code, out var weight))
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

            return totalWeight > 0 ? totalWeightedValue / totalWeight : 0;
        }

        /// <summary>
        /// 根據時段獲取開始日期
        /// </summary>
        private DateTime GetStartDateForPeriod(string period, DateTime timestamp)
        {
            return period.ToLower() switch
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
        /// 獲取時段內的遊戲熱度數據
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
                .Take(100) // 限制前100名
                .ToListAsync();
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
    }
}
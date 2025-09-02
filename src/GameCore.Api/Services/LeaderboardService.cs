using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

/// <summary>
/// 排行榜服務
/// 處理遊戲熱門度計算和排行榜生成
/// </summary>
public class LeaderboardService
{
    private readonly IGameRepository _gameRepository;
    private readonly IMetricRepository _metricRepository;
    private readonly IGameMetricDailyRepository _metricDailyRepository;
    private readonly IPopularityIndexDailyRepository _popularityRepository;
    private readonly ILeaderboardSnapshotRepository _leaderboardRepository;

    public LeaderboardService(
        IGameRepository gameRepository,
        IMetricRepository metricRepository,
        IGameMetricDailyRepository metricDailyRepository,
        IPopularityIndexDailyRepository popularityRepository,
        ILeaderboardSnapshotRepository leaderboardRepository)
    {
        _gameRepository = gameRepository;
        _metricRepository = metricRepository;
        _metricDailyRepository = metricDailyRepository;
        _popularityRepository = popularityRepository;
        _leaderboardRepository = leaderboardRepository;
    }

    /// <summary>
    /// 取得排行榜資料
    /// </summary>
    /// <param name="searchParams">搜尋參數</param>
    /// <returns>排行榜資料列表</returns>
    public async Task<List<LeaderboardItemDto>> GetLeaderboardAsync(LeaderboardSearchDto searchParams)
    {
        var query = _popularityRepository.GetQueryable()
            .Where(p => p.Date == (searchParams.Date ?? DateTime.UtcNow.Date))
            .Include(p => p.Game)
            .AsQueryable();

        // 套用分類篩選
        if (!string.IsNullOrEmpty(searchParams.Category))
        {
            query = query.Where(p => p.Game.Genre == searchParams.Category);
        }

        // 套用排序
        query = searchParams.SortBy switch
        {
            "index" => searchParams.SortOrder == "asc" 
                ? query.OrderBy(p => p.IndexValue) 
                : query.OrderByDescending(p => p.IndexValue),
            _ => searchParams.SortOrder == "asc" 
                ? query.OrderBy(p => p.IndexValue) 
                : query.OrderByDescending(p => p.IndexValue)
        };

        var leaderboardData = await query
            .Take(searchParams.Top)
            .ToListAsync();

        var result = new List<LeaderboardItemDto>();
        for (int i = 0; i < leaderboardData.Count; i++)
        {
            var item = leaderboardData[i];
            var change = await CalculateRankChangeAsync(item.GameID, searchParams.Date ?? DateTime.UtcNow.Date);
            
            result.Add(new LeaderboardItemDto
            {
                Rank = i + 1,
                GameID = item.GameID,
                GameName = item.Game?.Name ?? "未知遊戲",
                Category = item.Game?.Genre ?? "未知分類",
                PopularityScore = item.IndexValue,
                ConcurrentUsers = 0, // 暫時設為0，因為實體中沒有這個屬性
                TotalPosts = 0, // 暫時設為0，因為實體中沒有這個屬性
                TotalViews = 0, // 暫時設為0，因為實體中沒有這個屬性
                AverageRating = 0, // 暫時設為0，因為實體中沒有這個屬性
                Change = change,
                LastUpdated = item.Date
            });
        }

        return result;
    }

    /// <summary>
    /// 取得遊戲熱門度指標
    /// </summary>
    /// <param name="gameId">遊戲ID</param>
    /// <param name="date">日期</param>
    /// <returns>熱門度指標列表</returns>
    public async Task<List<PopularityMetricDto>> GetGameMetricsAsync(int gameId, DateTime date)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game == null)
        {
            return new List<PopularityMetricDto>(); // 返回空列表而不是錯誤
        }

        var metrics = await _metricDailyRepository.GetByGameAndDateAsync(game.GameID, date);

        var result = new List<PopularityMetricDto>();
        foreach (var metric in metrics)
        {
            var previousDay = await _metricDailyRepository.GetQueryable()
                .Where(m => m.GameID == gameId && m.MetricID == metric.MetricID && m.Date == date.AddDays(-1).Date)
                .FirstOrDefaultAsync();

            var changePercent = 0.0m;
            var trend = "stable";
            
            if (previousDay != null && previousDay.Value > 0)
            {
                changePercent = ((metric.Value - previousDay.Value) / previousDay.Value) * 100;
                trend = changePercent > 0 ? "increasing" : changePercent < 0 ? "decreasing" : "stable";
            }

            result.Add(new PopularityMetricDto
            {
                GameID = gameId,
                GameName = game.Name,
                MetricCode = metric.Metric.Code,
                MetricName = metric.Metric.Description ?? metric.Metric.Code,
                Value = metric.Value,
                Unit = metric.Metric.Unit ?? "",
                Date = metric.Date,
                Trend = trend,
                ChangePercent = changePercent
            });
        }

        return result;
    }

    /// <summary>
    /// 取得遊戲熱門度趨勢
    /// </summary>
    /// <param name="gameId">遊戲ID</param>
    /// <param name="days">天數</param>
    /// <param name="metricCode">指標代碼</param>
    /// <returns>熱門度趨勢資料</returns>
    public async Task<PopularityTrendDto?> GetGameTrendAsync(int gameId, int days, string metricCode)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game == null) return null;

        var endDate = DateTime.UtcNow.Date;
        var startDate = endDate.AddDays(-days);

        var metric = await _metricRepository.GetQueryable()
            .FirstOrDefaultAsync(m => m.Code == metricCode);

        if (metric == null) return null;

        var dataPoints = await _metricDailyRepository.GetQueryable()
            .Where(m => m.GameID == gameId && m.MetricID == metric.MetricID && m.Date >= startDate && m.Date <= endDate)
            .OrderBy(m => m.Date)
            .Select(m => new PopularityDataPoint
            {
                Date = m.Date,
                Value = m.Value,
                Metric = metricCode
            })
            .ToListAsync();

        return new PopularityTrendDto
        {
            GameID = gameId,
            GameName = game.Name,
            DataPoints = dataPoints
        };
    }

    /// <summary>
    /// 取得分類排行榜
    /// </summary>
    /// <param name="category">遊戲分類</param>
    /// <param name="top">前幾名</param>
    /// <returns>分類排行榜</returns>
    public async Task<CategoryLeaderboardDto?> GetCategoryLeaderboardAsync(string category, int top = 10)
    {
        var games = await _popularityRepository.GetQueryable()
            .Where(p => p.Game.Genre == category)
            .Include(p => p.Game)
            .OrderByDescending(p => p.IndexValue)
            .Take(top)
            .ToListAsync();

        if (!games.Any()) return null;

        var leaderboardItems = games.Select((item, index) => new LeaderboardItemDto
        {
            Rank = index + 1,
            GameID = item.GameID,
            GameName = item.Game?.Name ?? "未知遊戲",
            Category = item.Game?.Genre ?? "未知分類",
            PopularityScore = item.IndexValue,
            ConcurrentUsers = 0, // 暫時設為0
            TotalPosts = 0, // 暫時設為0
            TotalViews = 0, // 暫時設為0
            AverageRating = 0, // 暫時設為0
            Change = "flat", // 簡化版本
            LastUpdated = item.Date
        }).ToList();

        return new CategoryLeaderboardDto
        {
            Category = category,
            TopGames = leaderboardItems,
            LastUpdated = DateTime.UtcNow,
            // 效能優化：計數查詢不需要追蹤，加入 AsNoTracking() 降低 EF Core 追蹤成本
            // 前後差異：加入 AsNoTracking()，行為保持不變
            // 風險：無
            TotalGames = await _gameRepository.GetQueryable()
                .AsNoTracking()
                .Where(g => g.Genre == category)
                .CountAsync()
        };
    }

    /// <summary>
    /// 計算排名變化
    /// </summary>
    /// <param name="gameId">遊戲ID</param>
    /// <param name="date">日期</param>
    /// <returns>變化狀態</returns>
    private async Task<string> CalculateRankChangeAsync(int gameId, DateTime date)
    {
        var today = await _popularityRepository.GetQueryable()
            // 效能優化：唯讀單筆讀取不需要追蹤
            .AsNoTracking()
            .Where(p => p.GameID == gameId && p.Date == date)
            .FirstOrDefaultAsync();

        if (today == null) return "flat";

        var yesterday = await _popularityRepository.GetQueryable()
            // 效能優化：唯讀單筆讀取不需要追蹤
            .AsNoTracking()
            .Where(p => p.GameID == gameId && p.Date == date.AddDays(-1))
            .FirstOrDefaultAsync();

        if (yesterday == null) return "flat";

        // 簡化版本：根據熱門度指數變化判斷
        if (today.IndexValue > yesterday.IndexValue)
            return "up";
        else if (today.IndexValue < yesterday.IndexValue)
            return "down";
        else
            return "flat";
    }

    /// <summary>
    /// 手動更新遊戲熱門度指數
    /// </summary>
    /// <param name="gameId">遊戲ID</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdatePopularityIndexAsync(int gameId)
    {
        try
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null)
            {
                return false;
            }

            // 簡化版本：重新計算今日熱門度指數
            var today = DateTime.UtcNow.Date;
            var existingIndex = await _popularityRepository.GetQueryable()
                .FirstOrDefaultAsync(p => p.GameID == gameId && p.Date == today);

            if (existingIndex == null)
            {
                // 創建新的熱門度指數記錄
                var newIndex = new PopularityIndexDaily
                {
                    GameID = gameId,
                    Date = today,
                    IndexValue = 100, // 簡化版本：設為固定值
                    CreatedAt = DateTime.UtcNow
                };
                await _popularityRepository.AddAsync(newIndex);
            }
            else
            {
                // 更新現有記錄
                existingIndex.IndexValue = 100; // 簡化版本：設為固定值
                await _popularityRepository.UpdateAsync(existingIndex);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
} 
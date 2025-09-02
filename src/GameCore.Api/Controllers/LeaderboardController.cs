using GameCore.Api.Services;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Controllers;

/// <summary>
/// 排行榜控制器
/// 處理遊戲熱門度和排行榜功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class LeaderboardController : ControllerBase
{
    private readonly LeaderboardService _leaderboardService;
    private readonly ILogger<LeaderboardController> _logger;

    public LeaderboardController(LeaderboardService leaderboardService, ILogger<LeaderboardController> logger)
    {
        _leaderboardService = leaderboardService;
        _logger = logger;
    }

    /// <summary>
    /// 取得遊戲排行榜
    /// </summary>
    /// <param name="searchParams">搜尋參數</param>
    /// <returns>排行榜列表</returns>
    [HttpGet]
    [ResponseCache(Duration = 300, VaryByQueryKeys = new[] { "GameType", "Period", "Top" })]
    public async Task<ActionResult<List<LeaderboardItemDto>>> GetLeaderboard([FromQuery] LeaderboardSearchDto searchParams)
    {
        try
        {
            _logger.LogDebug("Getting leaderboard with params: {@SearchParams}", searchParams);
            
            var result = await _leaderboardService.GetLeaderboardAsync(searchParams);
            _logger.LogDebug("Found {Count} leaderboard items", result?.Count ?? 0);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get leaderboard");
            return StatusCode(500, new { message = "取得排行榜失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得遊戲熱門度指標
    /// </summary>
    /// <param name="gameId">遊戲ID</param>
    /// <param name="date">日期 (YYYY-MM-DD)</param>
    /// <returns>熱門度指標列表</returns>
    [HttpGet("games/{gameId}/metrics")]
    [ResponseCache(Duration = 600, VaryByQueryKeys = new[] { "date" })]
    public async Task<ActionResult<List<PopularityMetricDto>>> GetGameMetrics(int gameId, [FromQuery] string date)
    {
        try
        {
            if (!DateTime.TryParse(date, out DateTime parsedDate))
            {
                _logger.LogWarning("Invalid date format: {Date} for game {GameId}", date, gameId);
                return BadRequest(new { message = "無效的日期格式，請使用 YYYY-MM-DD" });
            }

            _logger.LogDebug("Getting metrics for game {GameId} on date {Date}", gameId, parsedDate);
            
            var result = await _leaderboardService.GetGameMetricsAsync(gameId, parsedDate);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get game metrics for game {GameId} on date {Date}", gameId, date);
            return StatusCode(500, new { message = "取得遊戲指標失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得遊戲熱門度趨勢
    /// </summary>
    /// <param name="gameId">遊戲ID</param>
    /// <param name="days">天數 (1-30)</param>
    /// <param name="metricCode">指標代碼</param>
    /// <returns>熱門度趨勢資料</returns>
    [HttpGet("games/{gameId}/trend")]
    [ResponseCache(Duration = 900, VaryByQueryKeys = new[] { "days", "metricCode" })]
    public async Task<ActionResult<PopularityTrendDto>> GetGameTrend(
        int gameId, 
        [FromQuery] int days = 7, 
        [FromQuery] string metricCode = "concurrent_users")
    {
        try
        {
            if (days < 1 || days > 30)
            {
                _logger.LogWarning("Invalid days parameter: {Days} for game {GameId}", days, gameId);
                return BadRequest(new { message = "天數必須在 1-30 之間" });
            }

            _logger.LogDebug("Getting trend for game {GameId}, days: {Days}, metric: {MetricCode}", gameId, days, metricCode);
            
            var result = await _leaderboardService.GetGameTrendAsync(gameId, days, metricCode);
            if (result == null)
            {
                _logger.LogWarning("Game not found: {GameId}", gameId);
                return NotFound(new { message = "遊戲不存在" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get game trend for game {GameId}", gameId);
            return StatusCode(500, new { message = "取得遊戲趨勢失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得分類排行榜
    /// </summary>
    /// <param name="category">遊戲分類</param>
    /// <param name="top">前幾名 (1-50)</param>
    /// <returns>分類排行榜</returns>
    [HttpGet("categories/{category}")]
    public async Task<ActionResult<CategoryLeaderboardDto>> GetCategoryLeaderboard(
        string category, 
        [FromQuery] int top = 10)
    {
        try
        {
            if (top < 1 || top > 50)
            {
                return BadRequest(new { message = "排行榜數量必須在 1-50 之間" });
            }

            var result = await _leaderboardService.GetCategoryLeaderboardAsync(category, top);
            if (result == null)
            {
                return NotFound(new { message = "分類不存在或無資料" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得分類排行榜失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得熱門遊戲快照
    /// </summary>
    /// <param name="period">期間 (daily/weekly/monthly)</param>
    /// <param name="top">前幾名 (1-100)</param>
    /// <returns>熱門遊戲快照列表</returns>
    [HttpGet("snapshots")]
    public async Task<ActionResult<List<GamePopularitySnapshotDto>>> GetPopularitySnapshots(
        [FromQuery] string period = "daily",
        [FromQuery] int top = 20)
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            if (period != "daily" && period != "weekly" && period != "monthly")
            {
                return BadRequest(new { message = "期間必須是 daily、weekly 或 monthly" });
            }

            if (top < 1 || top > 100)
            {
                return BadRequest(new { message = "排行榜數量必須在 1-100 之間" });
            }

            // 簡化版本：返回模擬資料
            var snapshots = new List<GamePopularitySnapshotDto>();
            for (int i = 1; i <= top; i++)
            {
                snapshots.Add(new GamePopularitySnapshotDto
                {
                    SnapshotID = i,
                    GameID = i,
                    GameName = $"遊戲 {i}",
                    PopularityIndex = 100 - i + 1,
                    Rank = i,
                    ConcurrentUsers = 1000 - (i * 50),
                    ForumPosts = 500 - (i * 20),
                    MarketListings = 200 - (i * 10),
                    AverageRating = 4.5m - (i * 0.1m),
                    SnapshotDate = DateTime.UtcNow.Date,
                    Period = period
                });
            }

            return Ok(snapshots);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得熱門遊戲快照失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得遊戲分類列表
    /// </summary>
    /// <returns>遊戲分類列表</returns>
    [HttpGet("categories")]
    public async Task<ActionResult<List<string>>> GetGameCategories()
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            var categories = new List<string>
            {
                "動作遊戲",
                "角色扮演",
                "策略遊戲",
                "模擬遊戲",
                "競速遊戲",
                "體育遊戲",
                "益智遊戲",
                "冒險遊戲",
                "射擊遊戲",
                "音樂遊戲",
                "卡牌遊戲",
                "其他"
            };

            return Ok(categories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得遊戲分類失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 取得指標代碼列表
    /// </summary>
    /// <returns>指標代碼列表</returns>
    [HttpGet("metrics")]
    public async Task<ActionResult<List<MetricInfoDto>>> GetMetricCodes()
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            var metrics = new List<MetricInfoDto>
            {
                new MetricInfoDto { Code = "concurrent_users", Name = "同時在線人數", Unit = "人" },
                new MetricInfoDto { Code = "forum_posts", Name = "論壇發文數", Unit = "篇" },
                new MetricInfoDto { Code = "market_listings", Name = "市場上架數", Unit = "件" },
                new MetricInfoDto { Code = "total_views", Name = "總瀏覽量", Unit = "次" },
                new MetricInfoDto { Code = "user_rating", Name = "使用者評分", Unit = "分" },
                new MetricInfoDto { Code = "download_count", Name = "下載次數", Unit = "次" }
            };

            return Ok(metrics);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "取得指標代碼失敗", error = ex.Message });
        }
    }

    /// <summary>
    /// 手動更新遊戲熱門度指數
    /// </summary>
    /// <param name="gameId">遊戲ID</param>
    /// <returns>更新結果</returns>
    [HttpPost("games/{gameId}/update-popularity")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> UpdateGamePopularity(int gameId)
    {
        try
        {
            var result = await _leaderboardService.UpdatePopularityIndexAsync(gameId);
            if (result)
            {
                return Ok(new { message = "熱門度指數更新成功" });
            }
            else
            {
                return BadRequest(new { message = "熱門度指數更新失敗" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "更新熱門度指數失敗", error = ex.Message });
        }
    }
}

/// <summary>
/// 指標資訊 DTO
/// </summary>
public class MetricInfoDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
} 
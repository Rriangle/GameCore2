using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Controllers
{
    /// <summary>
    /// 遊戲熱度控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PopularityController : ControllerBase
    {
        private readonly IPopularityService _popularityService;
        private readonly ILogger<PopularityController> _logger;

        public PopularityController(IPopularityService popularityService, ILogger<PopularityController> logger)
        {
            _popularityService = popularityService;
            _logger = logger;
        }

        /// <summary>
        /// 獲取所有遊戲
        /// </summary>
        [HttpGet("games")]
        public async Task<ActionResult<IEnumerable<GameResponseDto>>> GetGames()
        {
            try
            {
                var games = await _popularityService.GetAllGamesAsync();
                var gameDtos = games.Select(g => new GameResponseDto
                {
                    GameId = g.game_id,
                    Name = g.name,
                    Genre = g.genre,
                    CreatedAt = g.created_at,
                    CurrentIndexValue = g.PopularityIndexDailies.FirstOrDefault()?.index_value,
                    ThreadCount = 0, // TODO: 實現統計
                    PostCount = 0    // TODO: 實現統計
                });

                return Ok(gameDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取遊戲列表時發生錯誤");
                return StatusCode(500, new { message = "獲取遊戲列表時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 獲取遊戲熱度指數
        /// </summary>
        [HttpGet("games/{gameId}/popularity")]
        public async Task<ActionResult<IEnumerable<PopularityIndexResponseDto>>> GetGamePopularity(
            int gameId, 
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "開始日期不能晚於結束日期" });
                }

                var popularity = await _popularityService.GetGamePopularityAsync(gameId, startDate, endDate);
                var popularityDtos = popularity.Select(p => new PopularityIndexResponseDto
                {
                    GameId = p.game_id,
                    GameName = p.Game?.name ?? "未知遊戲",
                    Genre = p.Game?.genre ?? "未知類型",
                    Date = p.date,
                    IndexValue = p.index_value,
                    Trend = "stable", // TODO: 實現趨勢計算
                    ChangePercent = 0  // TODO: 實現變化百分比計算
                });

                return Ok(popularityDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取遊戲 {GameId} 熱度指數時發生錯誤", gameId);
                return StatusCode(500, new { message = "獲取遊戲熱度指數時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 獲取排行榜
        /// </summary>
        [HttpGet("leaderboards")]
        public async Task<ActionResult<IEnumerable<LeaderboardResponseDto>>> GetLeaderboard(
            [FromQuery] string period = "weekly",
            [FromQuery] DateTime? timestamp = null)
        {
            try
            {
                if (!IsValidPeriod(period))
                {
                    return BadRequest(new { message = "無效的時段參數，支持：daily, weekly, monthly, quarterly, yearly" });
                }

                var leaderboard = await _popularityService.GetLeaderboardAsync(period, timestamp);
                var leaderboardDtos = leaderboard.Select(l => new LeaderboardResponseDto
                {
                    Rank = l.rank,
                    GameId = l.game_id,
                    GameName = l.Game?.name ?? "未知遊戲",
                    Genre = l.Game?.genre ?? "未知類型",
                    IndexValue = l.index_value,
                    Trend = "stable", // TODO: 實現趨勢計算
                    ChangePercent = 0, // TODO: 實現變化百分比計算
                    Period = l.period,
                    Timestamp = l.ts
                });

                return Ok(leaderboardDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取 {Period} 排行榜時發生錯誤", period);
                return StatusCode(500, new { message = "獲取排行榜時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 獲取遊戲指標數據
        /// </summary>
        [HttpGet("games/{gameId}/metrics")]
        public async Task<ActionResult<IEnumerable<GameMetricResponseDto>>> GetGameMetrics(
            int gameId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(new { message = "開始日期不能晚於結束日期" });
                }

                var metrics = await _popularityService.GetGameMetricsAsync(gameId, startDate, endDate);
                var metricDtos = metrics.Select(m => new GameMetricResponseDto
                {
                    GameId = m.game_id,
                    GameName = m.Game?.name ?? "未知遊戲",
                    MetricId = m.metric_id,
                    MetricCode = m.Metric?.code ?? "未知指標",
                    MetricDescription = m.Metric?.description ?? "未知描述",
                    Unit = m.Metric?.unit ?? "未知單位",
                    Date = m.date,
                    Value = m.value,
                    Quality = m.quality
                });

                return Ok(metricDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取遊戲 {GameId} 指標數據時發生錯誤", gameId);
                return StatusCode(500, new { message = "獲取遊戲指標數據時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 獲取指標來源
        /// </summary>
        [HttpGet("metric-sources")]
        public async Task<ActionResult<IEnumerable<MetricSourceResponseDto>>> GetMetricSources()
        {
            try
            {
                var sources = await _popularityService.GetMetricSourcesAsync();
                var sourceDtos = sources.Select(s => new MetricSourceResponseDto
                {
                    SourceId = s.source_id,
                    Name = s.name,
                    Note = s.note,
                    CreatedAt = s.created_at,
                    MetricCount = s.Metrics?.Count ?? 0
                });

                return Ok(sourceDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取指標來源時發生錯誤");
                return StatusCode(500, new { message = "獲取指標來源時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 獲取指標定義
        /// </summary>
        [HttpGet("metrics")]
        public async Task<ActionResult<IEnumerable<MetricResponseDto>>> GetMetrics([FromQuery] int? sourceId = null)
        {
            try
            {
                var metrics = await _popularityService.GetMetricsAsync(sourceId);
                var metricDtos = metrics.Select(m => new MetricResponseDto
                {
                    MetricId = m.metric_id,
                    SourceId = m.source_id,
                    SourceName = m.Source?.name ?? "未知來源",
                    Code = m.code,
                    Unit = m.unit,
                    Description = m.description,
                    IsActive = m.is_active,
                    CreatedAt = m.created_at
                });

                return Ok(metricDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取指標定義時發生錯誤");
                return StatusCode(500, new { message = "獲取指標定義時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 獲取熱度分析
        /// </summary>
        [HttpPost("analysis")]
        public async Task<ActionResult<PopularityAnalysisResponseDto>> GetPopularityAnalysis(
            [FromBody] PopularityAnalysisRequestDto request)
        {
            try
            {
                if (request.StartDate > request.EndDate)
                {
                    return BadRequest(new { message = "開始日期不能晚於結束日期" });
                }

                // TODO: 實現完整的熱度分析邏輯
                var analysis = new PopularityAnalysisResponseDto
                {
                    Period = request.Period ?? "custom",
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TopGames = new List<PopularityIndexResponseDto>(),
                    TrendingGames = new List<PopularityIndexResponseDto>(),
                    DecliningGames = new List<PopularityIndexResponseDto>(),
                    GenreDistribution = new Dictionary<string, int>(),
                    AverageIndexValue = 0,
                    MaxIndexValue = 0,
                    MinIndexValue = 0
                };

                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取熱度分析時發生錯誤");
                return StatusCode(500, new { message = "獲取熱度分析時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 手動觸發熱度指數計算（管理員用）
        /// </summary>
        [HttpPost("games/{gameId}/calculate-index")]
        public async Task<ActionResult> CalculatePopularityIndex(int gameId, [FromQuery] DateTime date)
        {
            try
            {
                await _popularityService.CalculatePopularityIndexAsync(gameId, date);
                return Ok(new { message = "熱度指數計算已觸發" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "觸發遊戲 {GameId} 熱度指數計算時發生錯誤", gameId);
                return StatusCode(500, new { message = "觸發熱度指數計算時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 手動觸發排行榜快照生成（管理員用）
        /// </summary>
        [HttpPost("leaderboards/generate")]
        public async Task<ActionResult> GenerateLeaderboardSnapshot(
            [FromQuery] string period,
            [FromQuery] DateTime timestamp)
        {
            try
            {
                if (!IsValidPeriod(period))
                {
                    return BadRequest(new { message = "無效的時段參數" });
                }

                await _popularityService.GenerateLeaderboardSnapshotAsync(period, timestamp);
                return Ok(new { message = "排行榜快照生成已觸發" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "觸發排行榜快照生成時發生錯誤");
                return StatusCode(500, new { message = "觸發排行榜快照生成時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 驗證時段參數
        /// </summary>
        private static bool IsValidPeriod(string period)
        {
            var validPeriods = new[] { "daily", "weekly", "monthly", "quarterly", "yearly" };
            return validPeriods.Contains(period.ToLower());
        }
    }
}
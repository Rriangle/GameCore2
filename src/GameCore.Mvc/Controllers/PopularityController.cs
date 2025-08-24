using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using System.Security.Claims;

namespace GameCore.Mvc.Controllers;

/// <summary>
/// 熱度控制器 - 處理遊戲熱度指數、排行榜、指標相關功能
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
    /// 獲取遊戲列表
    /// </summary>
    [HttpGet("games")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedGamesDto>> GetGames([FromQuery] GameQueryDto query)
    {
        try
        {
            var result = await _popularityService.GetGamesAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取遊戲詳情
    /// </summary>
    [HttpGet("games/{gameId}")]
    [AllowAnonymous]
    public async Task<ActionResult<GameDetailDto>> GetGameDetail(int gameId)
    {
        try
        {
            var result = await _popularityService.GetGameDetailAsync(gameId);
            if (result == null)
                return NotFound(new { message = "遊戲不存在" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲詳情時發生錯誤: {GameId}", gameId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 搜尋遊戲
    /// </summary>
    [HttpGet("games/search")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedGamesDto>> SearchGames([FromQuery] GameSearchQueryDto query)
    {
        try
        {
            var result = await _popularityService.SearchGamesAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋遊戲時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取熱門遊戲
    /// </summary>
    [HttpGet("games/trending")]
    [AllowAnonymous]
    public async Task<ActionResult<List<GameDto>>> GetTrendingGames([FromQuery] string period = "daily", [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _popularityService.GetTrendingGamesAsync(period, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取熱門遊戲時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取遊戲統計摘要
    /// </summary>
    [HttpGet("games/{gameId}/stats")]
    [AllowAnonymous]
    public async Task<ActionResult<GameStatsDto>> GetGameStats(int gameId)
    {
        try
        {
            var result = await _popularityService.GetGameStatsAsync(gameId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲統計時發生錯誤: {GameId}", gameId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取遊戲來源對應
    /// </summary>
    [HttpGet("games/{gameId}/sources")]
    [AllowAnonymous]
    public async Task<ActionResult<List<GameSourceMapDto>>> GetGameSourceMaps(int gameId)
    {
        try
        {
            var result = await _popularityService.GetGameSourceMapsAsync(gameId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲來源對應時發生錯誤: {GameId}", gameId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取指標來源列表
    /// </summary>
    [HttpGet("metric-sources")]
    [AllowAnonymous]
    public async Task<ActionResult<List<MetricSourceDto>>> GetMetricSources()
    {
        try
        {
            var result = await _popularityService.GetMetricSourcesAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取指標來源列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取指標列表
    /// </summary>
    [HttpGet("metrics")]
    [AllowAnonymous]
    public async Task<ActionResult<List<MetricDto>>> GetMetrics([FromQuery] int? sourceId = null)
    {
        try
        {
            var result = await _popularityService.GetMetricsAsync(sourceId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取指標列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取遊戲指標數據
    /// </summary>
    [HttpGet("games/{gameId}/metrics")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedGameMetricsDto>> GetGameMetrics(int gameId, [FromQuery] GameMetricQueryDto query)
    {
        try
        {
            var result = await _popularityService.GetGameMetricsAsync(gameId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲指標數據時發生錯誤: {GameId}", gameId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 比較多個遊戲的指標
    /// </summary>
    [HttpPost("games/compare-metric")]
    [AllowAnonymous]
    public async Task<ActionResult<GameMetricComparisonDto>> CompareGamesMetric([FromBody] CompareGamesMetricRequestDto request)
    {
        try
        {
            var result = await _popularityService.CompareGamesMetricAsync(request.GameIds, request.MetricId, request.Query);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "比較遊戲指標時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取遊戲熱度指數歷史
    /// </summary>
    [HttpGet("games/{gameId}/popularity")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedPopularityIndexDto>> GetGamePopularityHistory(int gameId, [FromQuery] PopularityIndexQueryDto query)
    {
        try
        {
            var result = await _popularityService.GetGamePopularityHistoryAsync(gameId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲熱度指數歷史時發生錯誤: {GameId}", gameId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取排行榜快照
    /// </summary>
    [HttpGet("leaderboard")]
    [AllowAnonymous]
    public async Task<ActionResult<LeaderboardSnapshotDto>> GetLeaderboardSnapshot([FromQuery] LeaderboardQueryDto query)
    {
        try
        {
            var result = await _popularityService.GetLeaderboardSnapshotAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取排行榜快照時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取最新排行榜
    /// </summary>
    [HttpGet("leaderboard/latest")]
    [AllowAnonymous]
    public async Task<ActionResult<List<LeaderboardItemDto>>> GetLatestLeaderboard([FromQuery] string period = "daily", [FromQuery] int limit = 10)
    {
        try
        {
            var result = await _popularityService.GetLatestLeaderboardAsync(period, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取最新排行榜時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取遊戲排名歷史
    /// </summary>
    [HttpGet("games/{gameId}/rank-history")]
    [AllowAnonymous]
    public async Task<ActionResult<PagedGameRankHistoryDto>> GetGameRankHistory(int gameId, [FromQuery] string period, [FromQuery] GameRankQueryDto query)
    {
        try
        {
            var result = await _popularityService.GetGameRankHistoryAsync(gameId, period, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲排名歷史時發生錯誤: {GameId}", gameId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 計算熱度指數 (系統後台用)
    /// </summary>
    [HttpPost("calculate-index")]
    [Authorize] // 需要管理員權限
    public async Task<ActionResult<decimal>> CalculatePopularityIndex([FromBody] CalculateIndexRequestDto request)
    {
        try
        {
            var result = await _popularityService.CalculatePopularityIndexAsync(request.GameId, request.Date);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "計算熱度指數時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 產生排行榜快照 (系統後台用)
    /// </summary>
    [HttpPost("generate-snapshot")]
    [Authorize] // 需要管理員權限
    public async Task<ActionResult<int>> GenerateLeaderboardSnapshot([FromBody] GenerateSnapshotRequestDto request)
    {
        try
        {
            var result = await _popularityService.GenerateLeaderboardSnapshotAsync(request.Period, request.Date);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "產生排行榜快照時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }
}

/// <summary>
/// 比較遊戲指標請求 DTO
/// </summary>
public class CompareGamesMetricRequestDto
{
    public List<int> GameIds { get; set; } = new();
    public int MetricId { get; set; }
    public GameMetricQueryDto Query { get; set; } = new();
}

/// <summary>
/// 計算指數請求 DTO
/// </summary>
public class CalculateIndexRequestDto
{
    public int GameId { get; set; }
    public DateOnly Date { get; set; }
}

/// <summary>
/// 產生快照請求 DTO
/// </summary>
public class GenerateSnapshotRequestDto
{
    public string Period { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
}
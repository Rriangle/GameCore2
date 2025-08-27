using GameCore.Api.DTOs;
using GameCore.Api.Extensions;
using GameCore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCore.Api.Controllers;

/// <summary>
/// 遊戲控制器，處理遊戲相關的 API 端點
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ILogger<GameController> _logger;

    public GameController(IGameService gameService, ILogger<GameController> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取遊戲列表
    /// </summary>
    /// <param name="query">查詢參數</param>
    /// <returns>遊戲列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GameListResponseDto), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetGames([FromQuery] GameListQueryDto query)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取遊戲列表請求: {CorrelationId}", correlationId);

        try
        {
            var result = await _gameService.GetGamesAsync(query);
            _logger.LogInformation("遊戲列表獲取成功: {CorrelationId}, Count={Count}", correlationId, result.Games.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲列表時發生錯誤: {CorrelationId}", correlationId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("獲取遊戲列表失敗"));
        }
    }

    /// <summary>
    /// 根據 ID 獲取遊戲資訊
    /// </summary>
    /// <param name="id">遊戲 ID</param>
    /// <returns>遊戲資訊</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GameDto), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetGame(int id)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取遊戲資訊請求: {CorrelationId}, GameId={GameId}", correlationId, id);

        try
        {
            var game = await _gameService.GetGameByIdAsync(id);
            if (game == null)
            {
                _logger.LogWarning("遊戲不存在: {CorrelationId}, GameId={GameId}", correlationId, id);
                return NotFound(ApiResponse<object>.ErrorResult("遊戲不存在"));
            }

            _logger.LogInformation("遊戲資訊獲取成功: {CorrelationId}, GameId={GameId}", correlationId, id);
            return Ok(game);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲資訊時發生錯誤: {CorrelationId}, GameId={GameId}", correlationId, id);
            return StatusCode(500, ApiResponse<object>.ErrorResult("獲取遊戲資訊失敗"));
        }
    }

    /// <summary>
    /// 創建新遊戲
    /// </summary>
    /// <param name="request">創建遊戲請求</param>
    /// <returns>創建的遊戲資訊</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(GameDto), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 403)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到創建遊戲請求: {CorrelationId}", correlationId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("創建遊戲請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        try
        {
            var game = await _gameService.CreateGameAsync(request);
            _logger.LogInformation("遊戲創建成功: {CorrelationId}, GameId={GameId}, Name={Name}", correlationId, game.GameId, game.Name);
            return CreatedAtAction(nameof(GetGame), new { id = game.GameId }, game);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建遊戲時發生錯誤: {CorrelationId}", correlationId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("創建遊戲失敗"));
        }
    }

    /// <summary>
    /// 更新遊戲資訊
    /// </summary>
    /// <param name="id">遊戲 ID</param>
    /// <param name="request">更新遊戲請求</param>
    /// <returns>更新後的遊戲資訊</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(GameDto), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 403)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> UpdateGame(int id, [FromBody] UpdateGameRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到更新遊戲請求: {CorrelationId}, GameId={GameId}", correlationId, id);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("更新遊戲請求驗證失敗: {CorrelationId}, GameId={GameId}, Errors: {Errors}", correlationId, id, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        try
        {
            var game = await _gameService.UpdateGameAsync(id, request);
            if (game == null)
            {
                _logger.LogWarning("遊戲不存在: {CorrelationId}, GameId={GameId}", correlationId, id);
                return NotFound(ApiResponse<object>.ErrorResult("遊戲不存在"));
            }

            _logger.LogInformation("遊戲更新成功: {CorrelationId}, GameId={GameId}", correlationId, id);
            return Ok(game);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新遊戲時發生錯誤: {CorrelationId}, GameId={GameId}", correlationId, id);
            return StatusCode(500, ApiResponse<object>.ErrorResult("更新遊戲失敗"));
        }
    }

    /// <summary>
    /// 刪除遊戲
    /// </summary>
    /// <param name="id">遊戲 ID</param>
    /// <returns>刪除結果</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 403)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到刪除遊戲請求: {CorrelationId}, GameId={GameId}", correlationId, id);

        try
        {
            var success = await _gameService.DeleteGameAsync(id);
            if (!success)
            {
                _logger.LogWarning("遊戲不存在: {CorrelationId}, GameId={GameId}", correlationId, id);
                return NotFound(ApiResponse<object>.ErrorResult("遊戲不存在"));
            }

            _logger.LogInformation("遊戲刪除成功: {CorrelationId}, GameId={GameId}", correlationId, id);
            return Ok(ApiResponse<object>.SuccessResult("遊戲刪除成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除遊戲時發生錯誤: {CorrelationId}, GameId={GameId}", correlationId, id);
            return StatusCode(500, ApiResponse<object>.ErrorResult("刪除遊戲失敗"));
        }
    }

    /// <summary>
    /// 獲取遊戲分類列表
    /// </summary>
    /// <returns>分類列表</returns>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(List<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetCategories()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取遊戲分類請求: {CorrelationId}", correlationId);

        try
        {
            var categories = await _gameService.GetCategoriesAsync();
            _logger.LogInformation("遊戲分類獲取成功: {CorrelationId}, Count={Count}", correlationId, categories.Count);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取遊戲分類時發生錯誤: {CorrelationId}", correlationId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("獲取遊戲分類失敗"));
        }
    }
}
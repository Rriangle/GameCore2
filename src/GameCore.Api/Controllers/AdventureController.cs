using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs.AdventureDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdventureController : ControllerBase
{
    private readonly IAdventureService _adventureService;
    private readonly ILogger<AdventureController> _logger;

    public AdventureController(IAdventureService adventureService, ILogger<AdventureController> logger)
    {
        _adventureService = adventureService;
        _logger = logger;
    }

    /// <summary>
    /// Get available adventure templates for the user
    /// </summary>
    [HttpGet("templates")]
    public async Task<ActionResult<List<AdventureTemplateDto>>> GetAvailableTemplates()
    {
        try
        {
            var userId = GetUserIdFromToken();
            var templates = await _adventureService.GetAvailableTemplatesAsync(userId);
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available adventure templates for user");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get adventure template by ID
    /// </summary>
    [HttpGet("templates/{templateId}")]
    public async Task<ActionResult<AdventureTemplateDto>> GetTemplateById(int templateId)
    {
        try
        {
            var template = await _adventureService.GetTemplateByIdAsync(templateId);
            return Ok(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting adventure template {TemplateId}", templateId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Start a new adventure
    /// </summary>
    [HttpPost("start")]
    public async Task<ActionResult<AdventureResponseDto>> StartAdventure([FromBody] StartAdventureRequestDto request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var adventure = await _adventureService.StartAdventureAsync(userId, request.TemplateId);
            return Ok(adventure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting adventure for user");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get current adventure status
    /// </summary>
    [HttpGet("{adventureId}")]
    public async Task<ActionResult<AdventureResponseDto>> GetAdventure(int adventureId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var adventure = await _adventureService.GetAdventureAsync(adventureId, userId);
            return Ok(adventure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting adventure {AdventureId} for user", adventureId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Complete an adventure
    /// </summary>
    [HttpPost("{adventureId}/complete")]
    public async Task<ActionResult<AdventureResponseDto>> CompleteAdventure(int adventureId, [FromBody] CompleteAdventureRequestDto request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var adventure = await _adventureService.CompleteAdventureAsync(adventureId, userId);
            return Ok(adventure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing adventure {AdventureId} for user", adventureId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Fail an adventure
    /// </summary>
    [HttpPost("{adventureId}/fail")]
    public async Task<ActionResult<AdventureResponseDto>> FailAdventure(int adventureId, [FromBody] FailAdventureRequestDto request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var adventure = await _adventureService.FailAdventureAsync(adventureId, userId, request.FailureReason);
            return Ok(adventure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error failing adventure {AdventureId} for user", adventureId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Abandon an adventure
    /// </summary>
    [HttpPost("{adventureId}/abandon")]
    public async Task<ActionResult<AdventureResponseDto>> AbandonAdventure(int adventureId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var adventure = await _adventureService.AbandonAdventureAsync(adventureId, userId);
            return Ok(adventure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error abandoning adventure {AdventureId} for user", adventureId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get adventure progress
    /// </summary>
    [HttpGet("{adventureId}/progress")]
    public async Task<ActionResult<AdventureProgressDto>> GetAdventureProgress(int adventureId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var progress = await _adventureService.GetAdventureProgressAsync(adventureId, userId);
            return Ok(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting progress for adventure {AdventureId}", adventureId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Process a monster encounter during adventure
    /// </summary>
    [HttpPost("logs/{logId}/encounter")]
    public async Task<ActionResult<MonsterEncounterResponseDto>> ProcessMonsterEncounter(int logId, [FromBody] string monsterType)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var encounter = await _adventureService.ProcessMonsterEncounterAsync(logId, userId, monsterType);
            return Ok(encounter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing monster encounter for log {LogId}", logId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get monster encounters for an adventure
    /// </summary>
    [HttpGet("logs/{logId}/encounters")]
    public async Task<ActionResult<List<MonsterEncounterDto>>> GetAdventureMonsterEncounters(int logId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var encounters = await _adventureService.GetAdventureMonsterEncountersAsync(logId, userId);
            return Ok(encounters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting monster encounters for log {LogId}", logId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get user's adventure logs
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<AdventureLogResponseDto>> GetUserAdventureLogs([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var logs = await _adventureService.GetUserAdventureLogsAsync(userId, page, pageSize);
            return Ok(logs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting adventure logs for user");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get specific adventure log by ID
    /// </summary>
    [HttpGet("logs/{logId}")]
    public async Task<ActionResult<AdventureLogResponseDto>> GetAdventureLogById(int logId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var log = await _adventureService.GetAdventureLogByIdAsync(logId, userId);
            return Ok(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting adventure log {LogId}", logId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get user's adventure statistics
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<AdventureStatisticsDto>> GetUserAdventureStatistics()
    {
        try
        {
            var userId = GetUserIdFromToken();
            var statistics = await _adventureService.GetUserAdventureStatisticsAsync(userId);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting adventure statistics for user");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Check if user can start a specific adventure
    /// </summary>
    [HttpGet("templates/{templateId}/can-start")]
    public async Task<ActionResult<bool>> CanStartAdventure(int templateId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var canStart = await _adventureService.CanStartAdventureAsync(userId, templateId);
            return Ok(canStart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user can start adventure template {TemplateId}", templateId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Calculate adventure rewards for a template
    /// </summary>
    [HttpGet("templates/{templateId}/rewards")]
    public async Task<ActionResult<AdventureRewardsDto>> CalculateAdventureRewards(int templateId, [FromQuery] bool isSuccess = true)
    {
        try
        {
            var userId = GetUserIdFromToken();
            var rewards = await _adventureService.CalculateAdventureRewardsAsync(templateId, userId, isSuccess);
            return Ok(rewards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating rewards for adventure template {TemplateId}", templateId);
            return BadRequest(new { message = ex.Message });
        }
    }

    private int GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (int.TryParse(userIdClaim, out var userId))
            return userId;

        throw new UnauthorizedAccessException("Invalid user token");
    }
}
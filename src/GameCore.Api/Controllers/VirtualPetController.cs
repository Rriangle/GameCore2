using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs.VirtualPetDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCore.Api.Controllers;

/// <summary>
/// Controller for virtual pet operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VirtualPetController : ControllerBase
{
    private readonly IVirtualPetService _virtualPetService;
    private readonly ILogger<VirtualPetController> _logger;

    public VirtualPetController(IVirtualPetService virtualPetService, ILogger<VirtualPetController> logger)
    {
        _virtualPetService = virtualPetService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new virtual pet for the authenticated user
    /// </summary>
    /// <param name="request">Pet creation request</param>
    /// <returns>Created pet information</returns>
    [HttpPost("create")]
    public async Task<ActionResult<VirtualPetResponseDto>> CreatePet([FromBody] CreatePetRequestDto request)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var result = await _virtualPetService.CreatePetAsync(userId.Value, request);
            
            _logger.LogInformation("User {UserId} created virtual pet {PetName}", userId, result.Name);
            
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating virtual pet");
            return StatusCode(500, "An error occurred while creating the pet");
        }
    }

    /// <summary>
    /// Get the authenticated user's virtual pet
    /// </summary>
    /// <returns>Pet information</returns>
    [HttpGet("my-pet")]
    public async Task<ActionResult<VirtualPetResponseDto>> GetMyPet()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var pet = await _virtualPetService.GetUserPetAsync(userId.Value);
            return Ok(pet);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user's virtual pet");
            return StatusCode(500, "An error occurred while getting the pet");
        }
    }

    /// <summary>
    /// Feed the pet with a specific food item
    /// </summary>
    /// <param name="itemId">Food item ID</param>
    /// <returns>Feeding result</returns>
    [HttpPost("feed/{itemId:int}")]
    public async Task<ActionResult<PetCareResponseDto>> FeedPet(int itemId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var result = await _virtualPetService.FeedPetAsync(userId.Value, itemId);
            
            _logger.LogInformation("User {UserId} fed pet {PetName} with item {ItemId}", userId, result.PetName, itemId);
            
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error feeding pet with item {ItemId}", itemId);
            return StatusCode(500, "An error occurred while feeding the pet");
        }
    }

    /// <summary>
    /// Play with the pet using a specific toy
    /// </summary>
    /// <param name="itemId">Toy item ID</param>
    /// <returns>Play result</returns>
    [HttpPost("play/{itemId:int}")]
    public async Task<ActionResult<PetCareResponseDto>> PlayWithPet(int itemId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var result = await _virtualPetService.PlayWithPetAsync(userId.Value, itemId);
            
            _logger.LogInformation("User {UserId} played with pet {PetName} using item {ItemId}", userId, result.PetName, itemId);
            
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error playing with pet using item {ItemId}", itemId);
            return StatusCode(500, "An error occurred while playing with the pet");
        }
    }

    /// <summary>
    /// Clean the pet using a specific cleaning item
    /// </summary>
    /// <param name="itemId">Cleaning item ID</param>
    /// <returns>Cleaning result</returns>
    [HttpPost("clean/{itemId:int}")]
    public async Task<ActionResult<PetCareResponseDto>> CleanPet(int itemId)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var result = await _virtualPetService.CleanPetAsync(userId.Value, itemId);
            
            _logger.LogInformation("User {UserId} cleaned pet {PetName} with item {ItemId}", userId, result.PetName, itemId);
            
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning pet with item {ItemId}", itemId);
            return StatusCode(500, "An error occurred while cleaning the pet");
        }
    }

    /// <summary>
    /// Let the pet rest
    /// </summary>
    /// <returns>Rest result</returns>
    [HttpPost("rest")]
    public async Task<ActionResult<PetCareResponseDto>> RestPet()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var result = await _virtualPetService.RestPetAsync(userId.Value);
            
            _logger.LogInformation("User {UserId} let pet {PetName} rest", userId, result.PetName);
            
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error letting pet rest");
            return StatusCode(500, "An error occurred while letting the pet rest");
        }
    }

    /// <summary>
    /// Change the pet's color
    /// </summary>
    /// <param name="newColor">New color for the pet</param>
    /// <returns>Updated pet information</returns>
    [HttpPut("color")]
    public async Task<ActionResult<VirtualPetResponseDto>> ChangePetColor([FromBody] string newColor)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            if (string.IsNullOrWhiteSpace(newColor))
            {
                return BadRequest("Color cannot be empty");
            }

            var result = await _virtualPetService.ChangePetColorAsync(userId.Value, newColor);
            
            _logger.LogInformation("User {UserId} changed pet {PetName} color to {NewColor}", userId, result.Name, newColor);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing pet color to {NewColor}", newColor);
            return StatusCode(500, "An error occurred while changing the pet color");
        }
    }

    /// <summary>
    /// Get pet care history with pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <returns>Paginated care history</returns>
    [HttpGet("history")]
    public async Task<ActionResult<PetCareHistoryResponseDto>> GetPetCareHistory(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Invalid page or page size parameters");
            }

            var history = await _virtualPetService.GetPetCareHistoryAsync(userId.Value, page, pageSize);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pet care history");
            return StatusCode(500, "An error occurred while getting the care history");
        }
    }

    /// <summary>
    /// Get pet achievements
    /// </summary>
    /// <returns>List of pet achievements</returns>
    [HttpGet("achievements")]
    public async Task<ActionResult<List<PetAchievementDto>>> GetPetAchievements()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var achievements = await _virtualPetService.GetPetAchievementsAsync(userId.Value);
            return Ok(achievements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pet achievements");
            return StatusCode(500, "An error occurred while getting the achievements");
        }
    }

    /// <summary>
    /// Get available pet items
    /// </summary>
    /// <returns>List of available pet items</returns>
    [HttpGet("items")]
    public async Task<ActionResult<List<PetItemDto>>> GetAvailablePetItems()
    {
        try
        {
            var items = await _virtualPetService.GetAvailablePetItemsAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available pet items");
            return StatusCode(500, "An error occurred while getting the pet items");
        }
    }

    /// <summary>
    /// Get pet statistics
    /// </summary>
    /// <returns>Pet statistics</returns>
    [HttpGet("statistics")]
    public async Task<ActionResult<PetStatisticsDto>> GetPetStatistics()
    {
        try
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("Invalid user token");
            }

            var statistics = await _virtualPetService.GetPetStatisticsAsync(userId.Value);
            return Ok(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pet statistics");
            return StatusCode(500, "An error occurred while getting the statistics");
        }
    }

    /// <summary>
    /// Process pet status decay (admin only)
    /// </summary>
    /// <returns>Processing result</returns>
    [HttpPost("process-decay")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ProcessPetStatusDecay()
    {
        try
        {
            await _virtualPetService.ProcessPetStatusDecayAsync();
            
            _logger.LogInformation("Pet status decay processing completed");
            
            return Ok(new { message = "Pet status decay processing completed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing pet status decay");
            return StatusCode(500, "An error occurred while processing pet status decay");
        }
    }

    private int? GetUserIdFromToken()
    {
        // In a real implementation, this would extract the user ID from the JWT token
        // For now, we'll use a placeholder that would be replaced with actual token parsing
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        return null;
    }
}
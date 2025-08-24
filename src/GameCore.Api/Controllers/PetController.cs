using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetController : ControllerBase
{
    private readonly IPetService _petService;
    private readonly ILogger<PetController> _logger;

    public PetController(IPetService petService, ILogger<PetController> logger)
    {
        _petService = petService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取寵物資訊
    /// </summary>
    /// <returns>寵物資訊</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PetDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetPet()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取寵物資訊請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var pet = await _petService.GetPetAsync(userId);
        if (pet == null)
        {
            _logger.LogWarning("無法獲取寵物資訊: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return NotFound(ApiResponse<object>.ErrorResult("無法獲取寵物資訊"));
        }

        _logger.LogInformation("成功獲取寵物資訊: {CorrelationId}, UserId: {UserId}", correlationId, userId);
        return Ok(ApiResponse<PetDto>.SuccessResult(pet));
    }

    /// <summary>
    /// 與寵物互動
    /// </summary>
    /// <param name="request">互動請求</param>
    /// <returns>互動結果</returns>
    [HttpPost("interact")]
    [ProducesResponseType(typeof(ApiResponse<PetInteractionResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> InteractWithPet([FromBody] PetInteractionRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到寵物互動請求: {CorrelationId}, Type: {InteractionType}", correlationId, request.InteractionType);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("寵物互動請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _petService.InteractWithPetAsync(userId, request.InteractionType);

        if (result.Success)
        {
            _logger.LogInformation("寵物互動成功: {CorrelationId}, UserId: {UserId}, Type: {Type}, LeveledUp: {LeveledUp}", 
                correlationId, userId, request.InteractionType, result.LeveledUp);
            return Ok(ApiResponse<PetInteractionResult>.SuccessResult(result));
        }

        _logger.LogWarning("寵物互動失敗: {CorrelationId}, UserId: {UserId}, Type: {Type}, Message: {Message}", 
            correlationId, userId, request.InteractionType, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "互動失敗"));
    }

    /// <summary>
    /// 變更寵物顏色
    /// </summary>
    /// <param name="request">顏色變更請求</param>
    /// <returns>變更結果</returns>
    [HttpPost("color")]
    [ProducesResponseType(typeof(ApiResponse<PetColorChangeResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> ChangePetColor([FromBody] PetColorChangeRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到寵物顏色變更請求: {CorrelationId}, Color: {NewColor}", correlationId, request.NewColor);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("寵物顏色變更請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _petService.ChangePetColorAsync(userId, request.NewColor);

        if (result.Success)
        {
            _logger.LogInformation("寵物顏色變更成功: {CorrelationId}, UserId: {UserId}, Color: {Color}, RemainingBalance: {Balance}", 
                correlationId, userId, request.NewColor, result.RemainingBalance);
            return Ok(ApiResponse<PetColorChangeResult>.SuccessResult(result));
        }

        _logger.LogWarning("寵物顏色變更失敗: {CorrelationId}, UserId: {UserId}, Color: {Color}, Message: {Message}", 
            correlationId, userId, request.NewColor, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "顏色變更失敗"));
    }

    /// <summary>
    /// 檢查是否可以進行小遊戲
    /// </summary>
    /// <returns>是否可以進行小遊戲</returns>
    [HttpGet("can-play")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> CanPlayMiniGame()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到檢查小遊戲資格請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var canPlay = await _petService.CanPlayMiniGameAsync(userId);

        _logger.LogInformation("檢查小遊戲資格完成: {CorrelationId}, UserId: {UserId}, CanPlay: {CanPlay}", 
            correlationId, userId, canPlay);
        return Ok(ApiResponse<bool>.SuccessResult(canPlay));
    }
}

public class PetInteractionRequestDto
{
    [Required(ErrorMessage = "互動類型為必填項目")]
    [StringLength(20, ErrorMessage = "互動類型長度不能超過 20 個字元")]
    public string InteractionType { get; set; } = string.Empty; // feed, wash, play, rest
}

public class PetColorChangeRequestDto
{
    [Required(ErrorMessage = "新顏色為必填項目")]
    [StringLength(20, ErrorMessage = "顏色長度不能超過 20 個字元")]
    public string NewColor { get; set; } = string.Empty;
}
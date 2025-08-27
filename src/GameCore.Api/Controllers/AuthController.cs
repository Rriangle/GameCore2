using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

/// <summary>
/// 認證控制器，處理用戶註冊、登入和個人資料
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// 用戶註冊
    /// </summary>
    /// <param name="request">註冊請求</param>
    /// <returns>註冊結果</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到註冊請求: {CorrelationId}", correlationId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("註冊請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var result = await _authService.RegisterAsync(request);

        if (result.Success)
        {
            _logger.LogInformation("註冊成功: {CorrelationId}, UserAccount: {UserAccount}", correlationId, request.User_Account);
            return Ok(result);
        }

        _logger.LogWarning("註冊失敗: {CorrelationId}, UserAccount: {UserAccount}, Message: {Message}", 
            correlationId, request.User_Account, result.Message);
        return BadRequest(result);
    }

    /// <summary>
    /// 用戶登入
    /// </summary>
    /// <param name="request">登入請求</param>
    /// <returns>登入結果</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到登入請求: {CorrelationId}", correlationId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("登入請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var result = await _authService.LoginAsync(request);

        if (result.Success)
        {
            _logger.LogInformation("登入成功: {CorrelationId}, UserAccount: {UserAccount}", correlationId, request.User_Account);
            return Ok(result);
        }

        _logger.LogWarning("登入失敗: {CorrelationId}, UserAccount: {UserAccount}, Message: {Message}", 
            correlationId, request.User_Account, result.Message);
        return Unauthorized(result);
    }

    /// <summary>
    /// 獲取用戶個人資料
    /// </summary>
    /// <returns>用戶個人資料</returns>
    [Authorize]
    [HttpGet("profile")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetProfile()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取個人資料請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var profile = await _authService.GetUserProfileAsync(userId);
        if (profile == null)
        {
            _logger.LogWarning("用戶不存在: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return NotFound(ApiResponse<object>.ErrorResult("用戶不存在"));
        }

        _logger.LogInformation("成功獲取個人資料: {CorrelationId}, UserId: {UserId}", correlationId, userId);
        return Ok(ApiResponse<UserProfileDto>.SuccessResult(profile));
    }

    /// <summary>
    /// 獲取完整用戶資料
    /// </summary>
    /// <returns>完整用戶資料</returns>
    [Authorize]
    [HttpGet("profile/complete")]
    [ProducesResponseType(typeof(ApiResponse<CompleteUserProfileDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetCompleteProfile()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取完整個人資料請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var profile = await _authService.GetCompleteUserProfileAsync(userId);
        if (profile == null)
        {
            _logger.LogWarning("用戶不存在: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return NotFound(ApiResponse<object>.ErrorResult("用戶不存在"));
        }

        _logger.LogInformation("成功獲取完整個人資料: {CorrelationId}, UserId: {UserId}", correlationId, userId);
        return Ok(ApiResponse<CompleteUserProfileDto>.SuccessResult(profile));
    }

    /// <summary>
    /// 驗證 JWT Token
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>驗證結果</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> ValidateToken([FromBody] string token)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到 Token 驗證請求: {CorrelationId}", correlationId);

        try
        {
            var isValid = await _authService.ValidateTokenAsync(token);
            var userId = await _authService.GetUserIdFromTokenAsync(token);

            var response = new
            {
                IsValid = isValid,
                UserId = userId
            };

            _logger.LogInformation("Token 驗證完成: {CorrelationId}, IsValid: {IsValid}, UserId: {UserId}", 
                correlationId, isValid, userId);
            return Ok(ApiResponse<object>.SuccessResult(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token 驗證時發生錯誤: {CorrelationId}", correlationId);
            return BadRequest(ApiResponse<object>.ErrorResult("Token 驗證失敗"));
        }
    }
}

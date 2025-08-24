using GameCore.Api.DTOs;
using GameCore.Api.Extensions;
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

        var result = await _authService.RegisterAsync(request.Username, request.Email, request.Password);

        if (result.Success)
        {
            var response = new AuthResponseDto
            {
                Success = true,
                Token = result.Token,
                Message = "註冊成功",
                User = new UserProfileDto
                {
                    UserId = result.User!.UserId,
                    Username = result.User.Username,
                    Email = result.User.Email,
                    Balance = result.User.Balance,
                    CreatedAt = result.User.CreatedAt,
                    LastLoginAt = result.User.LastLoginAt
                }
            };

            _logger.LogInformation("註冊成功: {CorrelationId}, Username: {Username}", correlationId, request.Username);
            return Ok(response);
        }

        _logger.LogWarning("註冊失敗: {CorrelationId}, Username: {Username}, Message: {Message}", 
            correlationId, request.Username, result.ErrorMessage);
        return BadRequest(new AuthResponseDto { Success = false, Message = result.ErrorMessage });
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

        var result = await _authService.LoginAsync(request.Username, request.Password);

        if (result.Success)
        {
            var response = new AuthResponseDto
            {
                Success = true,
                Token = result.Token,
                Message = "登入成功",
                User = new UserProfileDto
                {
                    UserId = result.User!.UserId,
                    Username = result.User.Username,
                    Email = result.User.Email,
                    Balance = result.User.Balance,
                    CreatedAt = result.User.CreatedAt,
                    LastLoginAt = result.User.LastLoginAt
                }
            };

            _logger.LogInformation("登入成功: {CorrelationId}, Username: {Username}", correlationId, request.Username);
            return Ok(response);
        }

        _logger.LogWarning("登入失敗: {CorrelationId}, Username: {Username}, Message: {Message}", 
            correlationId, request.Username, result.ErrorMessage);
        return Unauthorized(new AuthResponseDto { Success = false, Message = result.ErrorMessage });
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

        var profileDto = new UserProfileDto
        {
            UserId = profile.UserId,
            Username = profile.Username,
            Email = profile.Email,
            Balance = profile.Balance,
            CreatedAt = profile.CreatedAt,
            LastLoginAt = profile.LastLoginAt
        };

        _logger.LogInformation("成功獲取個人資料: {CorrelationId}, UserId: {UserId}", correlationId, userId);
        return Ok(ApiResponse<UserProfileDto>.SuccessResult(profileDto));
    }
}

using GameCore.Api.Services;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCore.Api.Controllers;

/// <summary>
/// 認證控制器 - 處理用戶註冊、登入、密碼管理等
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
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
    /// <param name="registerDto">註冊請求</param>
    /// <returns>註冊結果</returns>
    /// <response code="200">註冊成功</response>
    /// <response code="400">請求資料無效</response>
    /// <response code="409">帳號或電子郵件已存在</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 409)]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 500)]
    public async Task<ActionResult<ApiResponseDto<UserInfoDto>>> Register([FromBody] UserRegisterDto registerDto)
    {
        try
        {
            _logger.LogInformation("用戶註冊請求：{UserAccount}", registerDto.UserAccount);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponseDto<UserInfoDto>
                {
                    Success = false,
                    Message = "請求資料無效",
                    Errors = errors
                });
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (result.Success)
            {
                _logger.LogInformation("用戶註冊成功：{UserAccount}", registerDto.UserAccount);
                return Ok(result);
            }
            else
            {
                if (result.Message.Contains("已存在"))
                {
                    return Conflict(result);
                }
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用戶註冊時發生錯誤：{UserAccount}", registerDto.UserAccount);
            return StatusCode(500, new ApiResponseDto<UserInfoDto>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 用戶登入
    /// </summary>
    /// <param name="loginDto">登入請求</param>
    /// <returns>登入結果</returns>
    /// <response code="200">登入成功</response>
    /// <response code="400">請求資料無效</response>
    /// <response code="401">帳號或密碼錯誤</response>
    /// <response code="403">帳號已被停權</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 403)]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 500)]
    public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login([FromBody] UserLoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("用戶登入請求：{UserAccount}", loginDto.UserAccount);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponseDto<LoginResponseDto>
                {
                    Success = false,
                    Message = "請求資料無效",
                    Errors = errors
                });
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result.Success)
            {
                _logger.LogInformation("用戶登入成功：{UserAccount}", loginDto.UserAccount);
                return Ok(result);
            }
            else
            {
                if (result.Message.Contains("帳號或密碼錯誤"))
                {
                    return Unauthorized(result);
                }
                else if (result.Message.Contains("已被停權"))
                {
                    return Forbid();
                }
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用戶登入時發生錯誤：{UserAccount}", loginDto.UserAccount);
            return StatusCode(500, new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// OAuth 登入
    /// </summary>
    /// <param name="oauthDto">OAuth 登入請求</param>
    /// <returns>登入結果</returns>
    /// <response code="200">登入成功</response>
    /// <response code="400">請求資料無效</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpPost("oauth")]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 500)]
    public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> OAuthLogin([FromBody] OAuthLoginDto oauthDto)
    {
        try
        {
            _logger.LogInformation("OAuth 登入請求：{Provider}", oauthDto.Provider);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponseDto<LoginResponseDto>
                {
                    Success = false,
                    Message = "請求資料無效",
                    Errors = errors
                });
            }

            var result = await _authService.OAuthLoginAsync(oauthDto);

            if (result.Success)
            {
                _logger.LogInformation("OAuth 登入成功：{Provider}", oauthDto.Provider);
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OAuth 登入時發生錯誤：{Provider}", oauthDto.Provider);
            return StatusCode(500, new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    /// <param name="refreshTokenDto">刷新令牌請求</param>
    /// <returns>新的登入結果</returns>
    /// <response code="200">令牌刷新成功</response>
    /// <response code="400">請求資料無效</response>
    /// <response code="401">刷新令牌無效</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<LoginResponseDto>), 500)]
    public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            _logger.LogInformation("刷新令牌請求");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponseDto<LoginResponseDto>
                {
                    Success = false,
                    Message = "請求資料無效",
                    Errors = errors
                });
            }

            var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

            if (result.Success)
            {
                _logger.LogInformation("令牌刷新成功");
                return Ok(result);
            }
            else
            {
                return Unauthorized(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新令牌時發生錯誤");
            return StatusCode(500, new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 變更密碼
    /// </summary>
    /// <param name="changePasswordDto">密碼變更請求</param>
    /// <returns>變更結果</returns>
    /// <response code="200">密碼變更成功</response>
    /// <response code="400">請求資料無效</response>
    /// <response code="401">當前密碼錯誤</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 500)]
    public async Task<ActionResult<ApiResponseDto<bool>>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "未授權的請求"
                });
            }

            _logger.LogInformation("用戶密碼變更請求：{UserId}", userId);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "請求資料無效",
                    Errors = errors
                });
            }

            var result = await _authService.ChangePasswordAsync(userId.Value, changePasswordDto);

            if (result.Success)
            {
                _logger.LogInformation("用戶密碼變更成功：{UserId}", userId);
                return Ok(result);
            }
            else
            {
                if (result.Message.Contains("當前密碼錯誤"))
                {
                    return Unauthorized(result);
                }
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            var userId = GetCurrentUserId();
            _logger.LogError(ex, "用戶密碼變更時發生錯誤：{UserId}", userId);
            return StatusCode(500, new ApiResponseDto<bool>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 忘記密碼
    /// </summary>
    /// <param name="forgotPasswordDto">忘記密碼請求</param>
    /// <returns>處理結果</returns>
    /// <response code="200">處理成功</response>
    /// <response code="400">請求資料無效</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 500)]
    public async Task<ActionResult<ApiResponseDto<bool>>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            _logger.LogInformation("忘記密碼請求：{Email}", forgotPasswordDto.Email);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "請求資料無效",
                    Errors = errors
                });
            }

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);

            if (result.Success)
            {
                _logger.LogInformation("忘記密碼處理成功：{Email}", forgotPasswordDto.Email);
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "忘記密碼處理時發生錯誤：{Email}", forgotPasswordDto.Email);
            return StatusCode(500, new ApiResponseDto<bool>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 重設密碼
    /// </summary>
    /// <param name="resetPasswordDto">重設密碼請求</param>
    /// <returns>重設結果</returns>
    /// <response code="200">密碼重設成功</response>
    /// <response code="400">請求資料無效</response>
    /// <response code="401">重設令牌無效</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 500)]
    public async Task<ActionResult<ApiResponseDto<bool>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            _logger.LogInformation("重設密碼請求");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "請求資料無效",
                    Errors = errors
                });
            }

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);

            if (result.Success)
            {
                _logger.LogInformation("密碼重設成功");
                return Ok(result);
            }
            else
            {
                return Unauthorized(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重設密碼時發生錯誤");
            return StatusCode(500, new ApiResponseDto<bool>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 取得當前用戶ID
    /// </summary>
    /// <returns>當前用戶ID</returns>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}

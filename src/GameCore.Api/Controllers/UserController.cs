using GameCore.Api.Services;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCore.Api.Controllers;

/// <summary>
/// 用戶管理控制器 - 處理用戶資料查詢、更新等
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 取得當前用戶資訊
    /// </summary>
    /// <returns>當前用戶資訊</returns>
    /// <response code="200">取得成功</response>
    /// <response code="401">未授權</response>
    /// <response code="404">用戶不存在</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 500)]
    public async Task<ActionResult<ApiResponseDto<UserInfoDto>>> GetCurrentUser()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<UserInfoDto>
                {
                    Success = false,
                    Message = "未授權的請求"
                });
            }

            _logger.LogInformation("取得當前用戶資訊：{UserId}", userId);

            var result = await _userService.GetUserInfoAsync(userId.Value);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                if (result.Message.Contains("不存在"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            var userId = GetCurrentUserId();
            _logger.LogError(ex, "取得當前用戶資訊時發生錯誤：{UserId}", userId);
            return StatusCode(500, new ApiResponseDto<UserInfoDto>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 取得指定用戶資訊
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>用戶資訊</returns>
    /// <response code="200">取得成功</response>
    /// <response code="401">未授權</response>
    /// <response code="404">用戶不存在</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<UserInfoDto>), 500)]
    public async Task<ActionResult<ApiResponseDto<UserInfoDto>>> GetUser(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponseDto<UserInfoDto>
                {
                    Success = false,
                    Message = "未授權的請求"
                });
            }

            _logger.LogInformation("取得用戶資訊：{CurrentUserId} -> {TargetUserId}", currentUserId, userId);

            var result = await _userService.GetUserInfoAsync(userId);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                if (result.Message.Contains("不存在"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            var currentUserId = GetCurrentUserId();
            _logger.LogError(ex, "取得用戶資訊時發生錯誤：{CurrentUserId} -> {TargetUserId}", currentUserId, userId);
            return StatusCode(500, new ApiResponseDto<UserInfoDto>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 更新當前用戶個資
    /// </summary>
    /// <param name="updateDto">更新資料</param>
    /// <returns>更新結果</returns>
    /// <response code="200">更新成功</response>
    /// <response code="400">請求資料無效</response>
    /// <response code="401">未授權</response>
    /// <response code="409">資料衝突</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpPut("me/profile")]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 409)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 500)]
    public async Task<ActionResult<ApiResponseDto<bool>>> UpdateCurrentUserProfile([FromBody] UpdateUserProfileDto updateDto)
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

            _logger.LogInformation("更新當前用戶個資：{UserId}", userId);

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

            var result = await _userService.UpdateUserProfileAsync(userId.Value, updateDto);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                if (result.Message.Contains("已被其他用戶使用"))
                {
                    return Conflict(result);
                }
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            var userId = GetCurrentUserId();
            _logger.LogError(ex, "更新當前用戶個資時發生錯誤：{UserId}", userId);
            return StatusCode(500, new ApiResponseDto<bool>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 取得當前用戶權限
    /// </summary>
    /// <returns>用戶權限</returns>
    /// <response code="200">取得成功</response>
    /// <response code="401">未授權</response>
    /// <response code="404">權限不存在</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpGet("me/rights")]
    [ProducesResponseType(typeof(ApiResponseDto<UserRightsDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<UserRightsDto>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<UserRightsDto>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<UserRightsDto>), 500)]
    public async Task<ActionResult<ApiResponseDto<UserRightsDto>>> GetCurrentUserRights()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<UserRightsDto>
                {
                    Success = false,
                    Message = "未授權的請求"
                });
            }

            _logger.LogInformation("取得當前用戶權限：{UserId}", userId);

            var result = await _userService.GetUserRightsAsync(userId.Value);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                if (result.Message.Contains("不存在"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            var userId = GetCurrentUserId();
            _logger.LogError(ex, "取得當前用戶權限時發生錯誤：{UserId}", userId);
            return StatusCode(500, new ApiResponseDto<UserRightsDto>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 取得當前用戶錢包
    /// </summary>
    /// <returns>用戶錢包</returns>
    /// <response code="200">取得成功</response>
    /// <response code="401">未授權</response>
    /// <response code="404">錢包不存在</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpGet("me/wallet")]
    [ProducesResponseType(typeof(ApiResponseDto<UserWalletDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<UserWalletDto>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<UserWalletDto>), 404)]
    [ProducesResponseType(typeof(ApiResponseDto<UserWalletDto>), 500)]
    public async Task<ActionResult<ApiResponseDto<UserWalletDto>>> GetCurrentUserWallet()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<UserWalletDto>
                {
                    Success = false,
                    Message = "未授權的請求"
                });
            }

            _logger.LogInformation("取得當前用戶錢包：{UserId}", userId);

            var result = await _userService.GetUserWalletAsync(userId.Value);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                if (result.Message.Contains("不存在"))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            var userId = GetCurrentUserId();
            _logger.LogError(ex, "取得當前用戶錢包時發生錯誤：{UserId}", userId);
            return StatusCode(500, new ApiResponseDto<UserWalletDto>
            {
                Success = false,
                Message = "伺服器內部錯誤"
            });
        }
    }

    /// <summary>
    /// 搜尋用戶
    /// </summary>
    /// <param name="searchTerm">搜尋關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>搜尋結果</returns>
    /// <response code="200">搜尋成功</response>
    /// <response code="400">請求資料無效</response>
    /// <response code="401">未授權</response>
    /// <response code="500">伺服器錯誤</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponseDto<List<UserInfoDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<List<UserInfoDto>>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<List<UserInfoDto>>), 401)]
    [ProducesResponseType(typeof(ApiResponseDto<List<UserInfoDto>>), 500)]
    public async Task<ActionResult<ApiResponseDto<List<UserInfoDto>>>> SearchUsers(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponseDto<List<UserInfoDto>>
                {
                    Success = false,
                    Message = "未授權的請求"
                });
            }

            _logger.LogInformation("搜尋用戶：{CurrentUserId}, {SearchTerm}, 頁面：{Page}, 大小：{PageSize}", 
                currentUserId, searchTerm, page, pageSize);

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new ApiResponseDto<List<UserInfoDto>>
                {
                    Success = false,
                    Message = "搜尋關鍵字不能為空"
                });
            }

            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new ApiResponseDto<List<UserInfoDto>>
                {
                    Success = false,
                    Message = "頁碼和頁面大小必須大於 0，頁面大小不能超過 100"
                });
            }

            var result = await _userService.SearchUsersAsync(searchTerm, page, pageSize);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            var currentUserId = GetCurrentUserId();
            _logger.LogError(ex, "搜尋用戶時發生錯誤：{CurrentUserId}, {SearchTerm}", currentUserId, searchTerm);
            return StatusCode(500, new ApiResponseDto<List<UserInfoDto>>
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
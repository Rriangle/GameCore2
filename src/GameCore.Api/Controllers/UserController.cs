using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

/// <summary>
/// 用戶控制器，處理用戶資料管理、權限和銷售申請
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
    /// 更新用戶介紹
    /// </summary>
    /// <param name="request">更新請求</param>
    /// <returns>更新結果</returns>
    [HttpPut("introduce")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> UpdateIntroduce([FromBody] UpdateUserIntroduceDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到更新用戶介紹請求: {CorrelationId}, UserId: {UserId}", correlationId, userId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("更新用戶介紹請求驗證失敗: {CorrelationId}, UserId: {UserId}, Errors: {Errors}", 
                correlationId, userId, string.Join(", ", errors));
            
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        try
        {
            var result = await _userService.UpdateUserIntroduceAsync(userId, request);
            
            if (result)
            {
                _logger.LogInformation("用戶介紹更新成功: {CorrelationId}, UserId: {UserId}", correlationId, userId);
                return Ok(ApiResponse<bool>.SuccessResult(true, "用戶介紹更新成功"));
            }
            else
            {
                _logger.LogWarning("用戶介紹更新失敗: {CorrelationId}, UserId: {UserId}", correlationId, userId);
                return BadRequest(ApiResponse<object>.ErrorResult("用戶介紹更新失敗"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用戶介紹時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 取得用戶介紹
    /// </summary>
    /// <returns>用戶介紹</returns>
    [HttpGet("introduce")]
    [ProducesResponseType(typeof(ApiResponse<UserIntroduceDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetIntroduce()
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到獲取用戶介紹請求: {CorrelationId}, UserId: {UserId}", correlationId, userId);

        try
        {
            var introduce = await _userService.GetUserIntroduceAsync(userId);
            
            if (introduce == null)
            {
                _logger.LogWarning("用戶介紹不存在: {CorrelationId}, UserId: {UserId}", correlationId, userId);
                return NotFound(ApiResponse<object>.ErrorResult("用戶介紹不存在"));
            }

            _logger.LogInformation("成功獲取用戶介紹: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return Ok(ApiResponse<UserIntroduceDto>.SuccessResult(introduce));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶介紹時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 取得用戶權限
    /// </summary>
    /// <returns>用戶權限</returns>
    [HttpGet("rights")]
    [ProducesResponseType(typeof(ApiResponse<UserRightsDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetRights()
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到獲取用戶權限請求: {CorrelationId}, UserId: {UserId}", correlationId, userId);

        try
        {
            var rights = await _userService.GetUserRightsAsync(userId);
            
            if (rights == null)
            {
                _logger.LogWarning("用戶權限不存在: {CorrelationId}, UserId: {UserId}", correlationId, userId);
                return NotFound(ApiResponse<object>.ErrorResult("用戶權限不存在"));
            }

            _logger.LogInformation("成功獲取用戶權限: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return Ok(ApiResponse<UserRightsDto>.SuccessResult(rights));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶權限時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 申請銷售權限
    /// </summary>
    /// <param name="request">申請請求</param>
    /// <returns>申請結果</returns>
    [HttpPost("sales-permission")]
    [ProducesResponseType(typeof(ApiResponse<SalesPermissionResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> ApplySalesPermission([FromBody] SalesPermissionRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到銷售權限申請請求: {CorrelationId}, UserId: {UserId}", correlationId, userId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("銷售權限申請請求驗證失敗: {CorrelationId}, UserId: {UserId}, Errors: {Errors}", 
                correlationId, userId, string.Join(", ", errors));
            
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        try
        {
            var result = await _userService.ApplySalesPermissionAsync(userId, request);
            
            _logger.LogInformation("銷售權限申請成功: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return Ok(ApiResponse<SalesPermissionResponseDto>.SuccessResult(result, "銷售權限申請成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "申請銷售權限時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 取得銷售權限申請狀態
    /// </summary>
    /// <returns>申請狀態</returns>
    [HttpGet("sales-permission")]
    [ProducesResponseType(typeof(ApiResponse<SalesPermissionResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetSalesPermissionStatus()
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到獲取銷售權限申請狀態請求: {CorrelationId}, UserId: {UserId}", correlationId, userId);

        try
        {
            var status = await _userService.GetSalesPermissionStatusAsync(userId);
            
            if (status == null)
            {
                _logger.LogWarning("銷售權限申請不存在: {CorrelationId}, UserId: {UserId}", correlationId, userId);
                return NotFound(ApiResponse<object>.ErrorResult("銷售權限申請不存在"));
            }

            _logger.LogInformation("成功獲取銷售權限申請狀態: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return Ok(ApiResponse<SalesPermissionResponseDto>.SuccessResult(status));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取銷售權限申請狀態時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 取得銷售錢包
    /// </summary>
    /// <returns>銷售錢包</returns>
    [HttpGet("sales-wallet")]
    [ProducesResponseType(typeof(ApiResponse<SalesWalletDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetSalesWallet()
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到獲取銷售錢包請求: {CorrelationId}, UserId: {UserId}", correlationId, userId);

        try
        {
            var wallet = await _userService.GetSalesWalletAsync(userId);
            
            if (wallet == null)
            {
                _logger.LogWarning("銷售錢包不存在: {CorrelationId}, UserId: {UserId}", correlationId, userId);
                return NotFound(ApiResponse<object>.ErrorResult("銷售錢包不存在"));
            }

            _logger.LogInformation("成功獲取銷售錢包: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return Ok(ApiResponse<SalesWalletDto>.SuccessResult(wallet));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取銷售錢包時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 更新用戶頭像
    /// </summary>
    /// <param name="imageData">圖片資料</param>
    /// <returns>更新結果</returns>
    [HttpPut("avatar")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> UpdateAvatar([FromBody] byte[] imageData)
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到更新用戶頭像請求: {CorrelationId}, UserId: {UserId}", correlationId, userId);

        if (imageData == null || imageData.Length == 0)
        {
            _logger.LogWarning("圖片資料為空: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return BadRequest(ApiResponse<object>.ErrorResult("圖片資料不能為空"));
        }

        if (imageData.Length > 10 * 1024 * 1024) // 10MB 限制
        {
            _logger.LogWarning("圖片檔案過大: {CorrelationId}, UserId: {UserId}, Size: {Size} bytes", 
                correlationId, userId, imageData.Length);
            return BadRequest(ApiResponse<object>.ErrorResult("圖片檔案不能超過 10MB"));
        }

        try
        {
            var result = await _userService.UpdateUserAvatarAsync(userId, imageData);
            
            if (result)
            {
                _logger.LogInformation("用戶頭像更新成功: {CorrelationId}, UserId: {UserId}", correlationId, userId);
                return Ok(ApiResponse<bool>.SuccessResult(true, "用戶頭像更新成功"));
            }
            else
            {
                _logger.LogWarning("用戶頭像更新失敗: {CorrelationId}, UserId: {UserId}", correlationId, userId);
                return BadRequest(ApiResponse<object>.ErrorResult("用戶頭像更新失敗"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用戶頭像時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 取得用戶頭像
    /// </summary>
    /// <returns>用戶頭像</returns>
    [HttpGet("avatar")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetAvatar()
    {
        var correlationId = HttpContext.GetCorrelationId();
        var userId = GetCurrentUserId();
        
        _logger.LogInformation("收到獲取用戶頭像請求: {CorrelationId}, UserId: {UserId}", correlationId, userId);

        try
        {
            var imageData = await _userService.GetUserAvatarAsync(userId);
            
            if (imageData == null || imageData.Length == 0)
            {
                _logger.LogWarning("用戶頭像不存在: {CorrelationId}, UserId: {UserId}", correlationId, userId);
                return NotFound(ApiResponse<object>.ErrorResult("用戶頭像不存在"));
            }

            _logger.LogInformation("成功獲取用戶頭像: {CorrelationId}, UserId: {UserId}, Size: {Size} bytes", 
                correlationId, userId, imageData.Length);
            return File(imageData, "image/jpeg");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶頭像時發生錯誤: {CorrelationId}, UserId: {UserId}", correlationId, userId);
            return StatusCode(500, ApiResponse<object>.ErrorResult("內部伺服器錯誤"));
        }
    }

    /// <summary>
    /// 取得當前用戶 ID
    /// </summary>
    /// <returns>用戶 ID</returns>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("無效的認證資訊");
        }
        return userId;
    }
}
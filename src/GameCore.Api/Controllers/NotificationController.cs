using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Api.Services;
using GameCore.Shared.DTOs;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

/// <summary>
/// 通知控制器
/// 處理通知相關的 API 端點
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        INotificationService notificationService,
        ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// 取得用戶通知列表
    /// </summary>
    /// <param name="page">頁碼（預設：1）</param>
    /// <param name="pageSize">頁面大小（預設：20）</param>
    /// <param name="isRead">是否已讀（可選）</param>
    /// <returns>通知列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isRead = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value, page, pageSize, isRead);

            return Ok(new
            {
                success = true,
                message = "取得通知列表成功",
                data = new
                {
                    notifications,
                    pagination = new
                    {
                        page,
                        pageSize,
                        total = notifications.Count()
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得通知列表時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得通知列表時發生錯誤" });
        }
    }

    /// <summary>
    /// 標記通知為已讀
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns>操作結果</returns>
    [HttpPost("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _notificationService.MarkAsReadAsync(notificationId, userId.Value);

            if (result)
            {
                return Ok(new { success = true, message = "標記已讀成功" });
            }
            else
            {
                return BadRequest(new { success = false, message = "標記已讀失敗" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "標記通知為已讀時發生錯誤");
            return StatusCode(500, new { success = false, message = "標記已讀時發生錯誤" });
        }
    }

    /// <summary>
    /// 標記所有通知為已讀
    /// </summary>
    /// <returns>操作結果</returns>
    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var count = await _notificationService.MarkAllAsReadAsync(userId.Value);

            return Ok(new
            {
                success = true,
                message = $"標記 {count} 個通知為已讀成功",
                data = new { count }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "標記所有通知為已讀時發生錯誤");
            return StatusCode(500, new { success = false, message = "標記所有通知為已讀時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得通知統計
    /// </summary>
    /// <returns>通知統計</returns>
    [HttpGet("stats")]
    public async Task<IActionResult> GetNotificationStats()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var stats = await _notificationService.GetNotificationStatsAsync(userId.Value);

            return Ok(new
            {
                success = true,
                message = "取得通知統計成功",
                data = stats
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得通知統計時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得通知統計時發生錯誤" });
        }
    }

    /// <summary>
    /// 刪除通知
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns>操作結果</returns>
    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> DeleteNotification(int notificationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _notificationService.DeleteNotificationAsync(notificationId, userId.Value);

            if (result)
            {
                return Ok(new { success = true, message = "刪除通知成功" });
            }
            else
            {
                return BadRequest(new { success = false, message = "刪除通知失敗" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除通知時發生錯誤");
            return StatusCode(500, new { success = false, message = "刪除通知時發生錯誤" });
        }
    }

    /// <summary>
    /// 建立通知（管理員功能）
    /// </summary>
    /// <param name="request">建立通知請求</param>
    /// <returns>操作結果</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "請求資料無效", errors = ModelState });
            }

            var result = await _notificationService.CreateNotificationAsync(request);

            if (result)
            {
                return Ok(new { success = true, message = "建立通知成功" });
            }
            else
            {
                return BadRequest(new { success = false, message = "建立通知失敗" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立通知時發生錯誤");
            return StatusCode(500, new { success = false, message = "建立通知時發生錯誤" });
        }
    }

    /// <summary>
    /// 建立系統通知（管理員功能）
    /// </summary>
    /// <param name="title">通知標題</param>
    /// <param name="message">通知內容</param>
    /// <param name="recipientIds">接收者ID列表</param>
    /// <returns>操作結果</returns>
    [HttpPost("system")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateSystemNotification(
        [FromBody] CreateSystemNotificationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "請求資料無效", errors = ModelState });
            }

            var result = await _notificationService.CreateSystemNotificationAsync(
                request.Title, request.Message, request.RecipientIds);

            if (result)
            {
                return Ok(new { success = true, message = "建立系統通知成功" });
            }
            else
            {
                return BadRequest(new { success = false, message = "建立系統通知失敗" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立系統通知時發生錯誤");
            return StatusCode(500, new { success = false, message = "建立系統通知時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得當前用戶ID
    /// </summary>
    /// <returns>用戶ID</returns>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }
}

/// <summary>
/// 建立系統通知請求 DTO
/// </summary>
public class CreateSystemNotificationRequest
{
    /// <summary>
    /// 通知標題
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 通知內容
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 接收者ID列表
    /// </summary>
    public List<int> RecipientIds { get; set; } = new();
} 
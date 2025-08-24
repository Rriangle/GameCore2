using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using System.Security.Claims;

namespace GameCore.Mvc.Controllers;

/// <summary>
/// 通知控制器 - 處理通知收件匣與相關功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationsService _notificationsService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationsService notificationsService, ILogger<NotificationsController> logger)
    {
        _notificationsService = notificationsService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取用戶通知收件匣
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedNotificationsDto>> GetNotifications([FromQuery] NotificationQueryDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _notificationsService.GetUserNotificationsAsync(userId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶通知時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 標記通知為已讀
    /// </summary>
    [HttpPost("{notificationId}/read")]
    public async Task<ActionResult> MarkAsRead(int notificationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _notificationsService.MarkNotificationAsReadAsync(userId, notificationId);
            if (!result)
                return NotFound(new { message = "通知不存在或無權限" });

            return Ok(new { message = "通知已標記為已讀" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "標記通知已讀時發生錯誤: {NotificationId}", notificationId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 批量標記通知為已讀
    /// </summary>
    [HttpPost("mark-read")]
    public async Task<ActionResult> MarkMultipleAsRead([FromBody] MarkNotificationsReadRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var count = await _notificationsService.MarkNotificationsAsReadAsync(userId, request.NotificationIds);
            return Ok(new { message = $"已標記 {count} 個通知為已讀" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量標記通知已讀時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 標記所有通知為已讀
    /// </summary>
    [HttpPost("mark-all-read")]
    public async Task<ActionResult> MarkAllAsRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            var count = await _notificationsService.MarkAllNotificationsAsReadAsync(userId);
            return Ok(new { message = $"已標記 {count} 個通知為已讀" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "標記所有通知已讀時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取未讀通知數量
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            var count = await _notificationsService.GetUnreadNotificationCountAsync(userId);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取未讀通知數量時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取通知詳情
    /// </summary>
    [HttpGet("{notificationId}")]
    public async Task<ActionResult<NotificationDetailDto>> GetNotificationDetail(int notificationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _notificationsService.GetNotificationDetailAsync(userId, notificationId);
            if (result == null)
                return NotFound(new { message = "通知不存在或無權限" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取通知詳情時發生錯誤: {NotificationId}", notificationId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 刪除通知
    /// </summary>
    [HttpDelete("{notificationId}")]
    public async Task<ActionResult> DeleteNotification(int notificationId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _notificationsService.DeleteNotificationAsync(userId, notificationId);
            if (!result)
                return NotFound(new { message = "通知不存在或無權限" });

            return Ok(new { message = "通知已刪除" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除通知時發生錯誤: {NotificationId}", notificationId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取通知來源列表
    /// </summary>
    [HttpGet("sources")]
    [AllowAnonymous]
    public async Task<ActionResult<List<NotificationSourceDto>>> GetNotificationSources()
    {
        try
        {
            var result = await _notificationsService.GetNotificationSourcesAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取通知來源列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取通知行為列表
    /// </summary>
    [HttpGet("actions")]
    [AllowAnonymous]
    public async Task<ActionResult<List<NotificationActionDto>>> GetNotificationActions()
    {
        try
        {
            var result = await _notificationsService.GetNotificationActionsAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取通知行為列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 創建用戶通知 (系統或其他用戶發送)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<NotificationDto>> CreateNotification([FromBody] CreateUserNotificationRequestDto request)
    {
        try
        {
            var senderId = GetCurrentUserId();
            var result = await _notificationsService.CreateUserNotificationAsync(senderId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建用戶通知時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("無效的用戶身份");
        }
        return userId;
    }
}

/// <summary>
/// 批量標記通知已讀請求 DTO
/// </summary>
public class MarkNotificationsReadRequestDto
{
    public List<int> NotificationIds { get; set; } = new();
}
using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// 獲取用戶通知列表
    /// </summary>
    /// <param name="isRead">是否已讀</param>
    /// <returns>通知列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetNotifications([FromQuery] bool? isRead)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取通知列表請求: {CorrelationId}, IsRead: {IsRead}", correlationId, isRead);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var notifications = await _notificationService.GetUserNotificationsAsync(userId, isRead);

        _logger.LogInformation("成功獲取通知列表: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
            correlationId, userId, notifications.Count());
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResult(notifications));
    }

    /// <summary>
    /// 獲取通知詳情
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns>通知詳情</returns>
    [HttpGet("{notificationId}")]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetNotification(int notificationId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取通知詳情請求: {CorrelationId}, NotificationId: {NotificationId}", 
            correlationId, notificationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var notification = await _notificationService.GetNotificationAsync(notificationId, userId);
        if (notification == null)
        {
            _logger.LogWarning("通知不存在: {CorrelationId}, NotificationId: {NotificationId}", 
                correlationId, notificationId);
            return NotFound(ApiResponse<object>.ErrorResult("通知不存在"));
        }

        _logger.LogInformation("成功獲取通知詳情: {CorrelationId}, NotificationId: {NotificationId}", 
            correlationId, notificationId);
        return Ok(ApiResponse<NotificationDto>.SuccessResult(notification));
    }

    /// <summary>
    /// 標記通知為已讀
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns>標記結果</returns>
    [HttpPut("{notificationId}/read")]
    [ProducesResponseType(typeof(ApiResponse<NotificationResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到標記通知已讀請求: {CorrelationId}, NotificationId: {NotificationId}", 
            correlationId, notificationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _notificationService.MarkAsReadAsync(notificationId, userId);

        if (result.Success)
        {
            _logger.LogInformation("通知標記已讀成功: {CorrelationId}, NotificationId: {NotificationId}", 
                correlationId, notificationId);
            return Ok(ApiResponse<NotificationResult>.SuccessResult(result));
        }

        _logger.LogWarning("通知標記已讀失敗: {CorrelationId}, NotificationId: {NotificationId}, Message: {Message}", 
            correlationId, notificationId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "標記已讀失敗"));
    }

    /// <summary>
    /// 標記所有通知為已讀
    /// </summary>
    /// <returns>標記結果</returns>
    [HttpPut("read-all")]
    [ProducesResponseType(typeof(ApiResponse<NotificationResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到標記所有通知已讀請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _notificationService.MarkAllAsReadAsync(userId);

        if (result.Success)
        {
            _logger.LogInformation("所有通知標記已讀成功: {CorrelationId}, UserId: {UserId}", 
                correlationId, userId);
            return Ok(ApiResponse<NotificationResult>.SuccessResult(result));
        }

        _logger.LogWarning("所有通知標記已讀失敗: {CorrelationId}, UserId: {UserId}, Message: {Message}", 
            correlationId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "標記已讀失敗"));
    }

    /// <summary>
    /// 刪除通知
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns>刪除結果</returns>
    [HttpDelete("{notificationId}")]
    [ProducesResponseType(typeof(ApiResponse<NotificationResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> DeleteNotification(int notificationId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到刪除通知請求: {CorrelationId}, NotificationId: {NotificationId}", 
            correlationId, notificationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _notificationService.DeleteNotificationAsync(notificationId, userId);

        if (result.Success)
        {
            _logger.LogInformation("通知刪除成功: {CorrelationId}, NotificationId: {NotificationId}", 
                correlationId, notificationId);
            return Ok(ApiResponse<NotificationResult>.SuccessResult(result));
        }

        _logger.LogWarning("通知刪除失敗: {CorrelationId}, NotificationId: {NotificationId}, Message: {Message}", 
            correlationId, notificationId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "刪除失敗"));
    }

    /// <summary>
    /// 獲取未讀通知數量
    /// </summary>
    /// <returns>未讀數量</returns>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取未讀通知數量請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var count = await _notificationService.GetUnreadCountAsync(userId);

        _logger.LogInformation("成功獲取未讀通知數量: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
            correlationId, userId, count);
        return Ok(ApiResponse<int>.SuccessResult(count));
    }
}
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Controllers
{
    /// <summary>
    /// 通知控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
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
        [HttpGet]
        public async Task<ActionResult<NotificationListResponseDto>> GetUserNotifications(
            [FromQuery] int userId,
            [FromQuery] bool? isRead = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var notifications = await _notificationService.GetUserNotificationsAsync(userId, isRead, page, pageSize);
                var unreadCount = await _notificationService.GetUnreadCountAsync(userId);

                var response = new NotificationListResponseDto
                {
                    Notifications = notifications.Select(n => new NotificationResponseDto
                    {
                        NotificationId = n.notification_id,
                        SourceName = n.Source?.source_name ?? "未知",
                        ActionName = n.Action?.action_name ?? "未知",
                        SenderName = n.Sender?.User_name ?? "未知",
                        SenderManagerName = n.SenderManager?.Manager_Name,
                        Title = n.notification_title,
                        Message = n.notification_message,
                        CreatedAt = n.created_at,
                        GroupName = n.Group?.group_name,
                        IsRead = n.Recipients.FirstOrDefault(r => r.user_id == userId)?.is_read ?? false,
                        ReadAt = n.Recipients.FirstOrDefault(r => r.user_id == userId)?.read_at
                    }).ToList(),
                    TotalCount = notifications.Count(),
                    Page = page,
                    PageSize = pageSize,
                    UnreadCount = unreadCount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶 {UserId} 通知列表時發生錯誤", userId);
                return StatusCode(500, new { error = "獲取通知列表失敗" });
            }
        }

        /// <summary>
        /// 獲取單個通知
        /// </summary>
        [HttpGet("{notificationId}")]
        public async Task<ActionResult<NotificationResponseDto>> GetNotification(int notificationId, [FromQuery] int userId)
        {
            try
            {
                var notification = await _notificationService.GetNotificationAsync(notificationId);
                if (notification == null)
                    return NotFound(new { error = "通知不存在" });

                var response = new NotificationResponseDto
                {
                    NotificationId = notification.notification_id,
                    SourceName = notification.Source?.source_name ?? "未知",
                    ActionName = notification.Action?.action_name ?? "未知",
                    SenderName = notification.Sender?.User_name ?? "未知",
                    SenderManagerName = notification.SenderManager?.Manager_Name,
                    Title = notification.notification_title,
                    Message = notification.notification_message,
                    CreatedAt = notification.created_at,
                    GroupName = notification.Group?.group_name,
                    IsRead = notification.Recipients.FirstOrDefault(r => r.user_id == userId)?.is_read ?? false,
                    ReadAt = notification.Recipients.FirstOrDefault(r => r.user_id == userId)?.read_at
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取通知 {NotificationId} 時發生錯誤", notificationId);
                return StatusCode(500, new { error = "獲取通知失敗" });
            }
        }

        /// <summary>
        /// 創建通知
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<NotificationResponseDto>> CreateNotification([FromBody] CreateNotificationRequestDto request)
        {
            try
            {
                var notification = new GameCore.Domain.Entities.Notification
                {
                    source_id = request.SourceId,
                    action_id = request.ActionId,
                    sender_id = request.SenderId,
                    sender_manager_id = request.SenderManagerId,
                    notification_title = request.Title,
                    notification_message = request.Message,
                    group_id = request.GroupId,
                    created_at = DateTime.UtcNow
                };

                var createdNotification = await _notificationService.CreateNotificationAsync(notification, request.RecipientUserIds);

                var response = new NotificationResponseDto
                {
                    NotificationId = createdNotification.notification_id,
                    SourceName = "新通知",
                    ActionName = "創建",
                    SenderName = "系統",
                    Title = createdNotification.notification_title,
                    Message = createdNotification.notification_message,
                    CreatedAt = createdNotification.created_at,
                    IsRead = false
                };

                return CreatedAtAction(nameof(GetNotification), new { notificationId = createdNotification.notification_id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建通知時發生錯誤");
                return StatusCode(500, new { error = "創建通知失敗" });
            }
        }

        /// <summary>
        /// 標記通知為已讀
        /// </summary>
        [HttpPost("{notificationId}/read")]
        public async Task<ActionResult> MarkAsRead(int notificationId, [FromBody] MarkNotificationReadRequestDto request)
        {
            try
            {
                if (request.NotificationId != notificationId)
                    return BadRequest(new { error = "通知ID不匹配" });

                var success = await _notificationService.MarkNotificationAsReadAsync(notificationId, request.UserId);
                if (!success)
                    return NotFound(new { error = "通知或用戶不存在" });

                return Ok(new { message = "通知已標記為已讀" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "標記通知 {NotificationId} 為已讀時發生錯誤", notificationId);
                return StatusCode(500, new { error = "標記已讀失敗" });
            }
        }

        /// <summary>
        /// 標記所有通知為已讀
        /// </summary>
        [HttpPost("mark-all-read")]
        public async Task<ActionResult> MarkAllAsRead([FromQuery] int userId)
        {
            try
            {
                var success = await _notificationService.MarkAllNotificationsAsReadAsync(userId);
                if (!success)
                    return NotFound(new { error = "用戶不存在" });

                return Ok(new { message = "所有通知已標記為已讀" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "標記用戶 {UserId} 所有通知為已讀時發生錯誤", userId);
                return StatusCode(500, new { error = "標記已讀失敗" });
            }
        }

        /// <summary>
        /// 刪除通知
        /// </summary>
        [HttpDelete("{notificationId}")]
        public async Task<ActionResult> DeleteNotification(int notificationId, [FromQuery] int userId)
        {
            try
            {
                var success = await _notificationService.DeleteNotificationAsync(notificationId, userId);
                if (!success)
                    return NotFound(new { error = "通知或用戶不存在" });

                return Ok(new { message = "通知已刪除" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除通知 {NotificationId} 時發生錯誤", notificationId);
                return StatusCode(500, new { error = "刪除通知失敗" });
            }
        }

        /// <summary>
        /// 獲取通知來源列表
        /// </summary>
        [HttpGet("sources")]
        public async Task<ActionResult<IEnumerable<NotificationSourceResponseDto>>> GetNotificationSources()
        {
            try
            {
                var sources = await _notificationService.GetNotificationSourcesAsync();
                var response = sources.Select(s => new NotificationSourceResponseDto
                {
                    SourceId = s.source_id,
                    SourceName = s.source_name
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取通知來源列表時發生錯誤");
                return StatusCode(500, new { error = "獲取通知來源失敗" });
            }
        }

        /// <summary>
        /// 獲取通知行為列表
        /// </summary>
        [HttpGet("actions")]
        public async Task<ActionResult<IEnumerable<NotificationActionResponseDto>>> GetNotificationActions()
        {
            try
            {
                var actions = await _notificationService.GetNotificationActionsAsync();
                var response = actions.Select(a => new NotificationActionResponseDto
                {
                    ActionId = a.action_id,
                    ActionName = a.action_name
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取通知行為列表時發生錯誤");
                return StatusCode(500, new { error = "獲取通知行為失敗" });
            }
        }

        /// <summary>
        /// 創建系統通知
        /// </summary>
        [HttpPost("system")]
        public async Task<ActionResult<NotificationResponseDto>> CreateSystemNotification([FromBody] SystemNotificationRequestDto request)
        {
            try
            {
                var notification = await _notificationService.CreateSystemNotificationAsync(
                    request.Title, 
                    request.Message, 
                    request.RecipientUserIds, 
                    request.SenderManagerId);

                var response = new NotificationResponseDto
                {
                    NotificationId = notification.notification_id,
                    SourceName = "系統",
                    ActionName = "通知",
                    SenderName = "系統",
                    Title = notification.notification_title,
                    Message = notification.notification_message,
                    CreatedAt = notification.created_at,
                    IsRead = false
                };

                return CreatedAtAction(nameof(GetNotification), new { notificationId = notification.notification_id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建系統通知時發生錯誤");
                return StatusCode(500, new { error = "創建系統通知失敗" });
            }
        }
    }
}
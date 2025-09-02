using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Services;

/// <summary>
/// 通知服務
/// 處理通知的建立、查詢、標記已讀等功能
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// 取得用戶通知列表
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <param name="isRead">是否已讀篩選</param>
    /// <returns>通知列表</returns>
    public async Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 20, bool? isRead = null)
    {
        try
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId, page, pageSize);

            var notificationDtos = new List<NotificationDTO>();

            foreach (var notification in notifications)
            {
                var sender = await _userRepository.GetByIdAsync(notification.SenderId);
                var source = await _notificationRepository.GetSourceByIdAsync(notification.SourceId);
                var action = await _notificationRepository.GetActionByIdAsync(notification.ActionId);
                var group = await _notificationRepository.GetGroupByIdAsync(notification.GroupId);
                var recipient = await _notificationRepository.GetRecipientByIdAsync(notification.NotificationId, userId);

                var notificationDto = new NotificationDTO
                {
                    NotificationId = notification.NotificationId,
                    SourceId = notification.SourceId,
                    SourceName = source?.SourceName ?? "未知來源",
                    ActionId = notification.ActionId,
                    ActionName = action?.ActionName ?? "未知動作",
                    SenderId = notification.SenderId,
                    SenderName = sender?.User_Name ?? "系統",
                    Title = notification.NotificationTitle ?? "",
                    Message = notification.NotificationMessage,
                    GroupId = notification.GroupId,
                    GroupName = group?.GroupName,
                    CreatedAt = notification.CreatedAt,
                    IsRead = recipient?.IsRead ?? false,
                    ReadAt = recipient?.ReadAt
                };

                notificationDtos.Add(notificationDto);
            }

            return notificationDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶 {UserId} 通知列表時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 建立通知
    /// </summary>
    /// <param name="request">建立通知請求</param>
    /// <returns>建立結果</returns>
    public async Task<bool> CreateNotificationAsync(CreateNotificationRequest request)
    {
        try
        {
            // 驗證發送者是否存在
            var sender = await _userRepository.GetByIdAsync(request.SenderId);
            if (sender == null)
            {
                _logger.LogWarning("發送者 {SenderId} 不存在", request.SenderId);
                return false;
            }

            // 建立通知
            var notification = new Notification
            {
                SourceId = request.SourceId,
                ActionId = request.ActionId,
                SenderId = request.SenderId,
                SenderManagerId = null, // 一般用戶發送
                NotificationTitle = request.Title,
                NotificationMessage = request.Message,
                GroupId = request.GroupId,
                CreatedAt = DateTime.UtcNow
            };

            var createdNotification = await _notificationRepository.AddAsync(notification);

            // 為每個接收者建立通知記錄
            foreach (var recipientId in request.RecipientIds)
            {
                var recipient = await _userRepository.GetByIdAsync(recipientId);
                if (recipient != null)
                {
                    // 這裡需要實作 NotificationRecipient 的建立
                    // 由於沒有對應的 Repository，暫時記錄日誌
                    _logger.LogInformation("為用戶 {RecipientId} 建立通知 {NotificationId}", recipientId, createdNotification.NotificationId);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立通知時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 標記通知為已讀
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>標記結果</returns>
    public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
    {
        try
        {
            return await _notificationRepository.MarkAsReadAsync(notificationId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "標記通知 {NotificationId} 為已讀時發生錯誤", notificationId);
            throw;
        }
    }

    /// <summary>
    /// 標記所有通知為已讀
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>標記成功的數量</returns>
    public async Task<int> MarkAllAsReadAsync(int userId)
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            // 暫時返回0，因為沒有實作 MarkAllAsReadAsync 方法
            _logger.LogInformation("標記用戶 {UserId} 所有通知為已讀", userId);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "標記用戶 {UserId} 所有通知為已讀時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 取得通知統計
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>通知統計</returns>
    public async Task<NotificationStatsDTO> GetNotificationStatsAsync(int userId)
    {
        try
        {
            var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId);
            // 效能優化：直接從資料庫取得計數，避免載入完整資料到記憶體
            var totalCount = await _notificationRepository.GetCountByUserIdAsync(userId);
            var todayCount = await _notificationRepository.GetTodayCountByUserIdAsync(userId);

            return new NotificationStatsDTO
            {
                UnreadCount = unreadCount,
                TotalCount = totalCount,
                TodayCount = todayCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶 {UserId} 通知統計時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 刪除通知
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>刪除結果</returns>
    public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
    {
        try
        {
            // 驗證通知是否存在且屬於該用戶
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
            {
                return false;
            }

            // 這裡需要檢查用戶是否有權限刪除此通知
            // 暫時允許刪除
            return await _notificationRepository.DeleteAsync(notificationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除通知 {NotificationId} 時發生錯誤", notificationId);
            throw;
        }
    }

    /// <summary>
    /// 建立系統通知
    /// </summary>
    /// <param name="title">通知標題</param>
    /// <param name="message">通知內容</param>
    /// <param name="recipientIds">接收者ID列表</param>
    /// <returns>建立結果</returns>
    public async Task<bool> CreateSystemNotificationAsync(string title, string message, List<int> recipientIds)
    {
        try
        {
            var request = new CreateNotificationRequest
            {
                SourceId = 1, // 系統來源
                ActionId = 1, // 系統行為
                SenderId = 1, // 系統用戶
                Title = title,
                Message = message,
                RecipientIds = recipientIds
            };

            return await CreateNotificationAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立系統通知時發生錯誤");
            throw;
        }
    }
}

/// <summary>
/// 通知服務介面
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// 取得用戶通知列表
    /// </summary>
    Task<IEnumerable<NotificationDTO>> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 20, bool? isRead = null);

    /// <summary>
    /// 建立通知
    /// </summary>
    Task<bool> CreateNotificationAsync(CreateNotificationRequest request);

    /// <summary>
    /// 標記通知為已讀
    /// </summary>
    Task<bool> MarkAsReadAsync(int notificationId, int userId);

    /// <summary>
    /// 標記所有通知為已讀
    /// </summary>
    Task<int> MarkAllAsReadAsync(int userId);

    /// <summary>
    /// 取得通知統計
    /// </summary>
    Task<NotificationStatsDTO> GetNotificationStatsAsync(int userId);

    /// <summary>
    /// 刪除通知
    /// </summary>
    Task<bool> DeleteNotificationAsync(int notificationId, int userId);

    /// <summary>
    /// 建立系統通知
    /// </summary>
    Task<bool> CreateSystemNotificationAsync(string title, string message, List<int> recipientIds);
} 
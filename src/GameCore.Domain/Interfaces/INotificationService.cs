using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces
{
    /// <summary>
    /// 通知服務接口
    /// </summary>
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool? isRead = null, int page = 1, int pageSize = 20);
        Task<Notification?> GetNotificationAsync(int notificationId);
        Task<Notification> CreateNotificationAsync(Notification notification, List<int> recipientUserIds);
        Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllNotificationsAsReadAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
        Task<IEnumerable<NotificationSource>> GetNotificationSourcesAsync();
        Task<IEnumerable<NotificationAction>> GetNotificationActionsAsync();
        Task<Notification> CreateSystemNotificationAsync(string title, string message, List<int> recipientUserIds, int? senderManagerId = null);
    }
}
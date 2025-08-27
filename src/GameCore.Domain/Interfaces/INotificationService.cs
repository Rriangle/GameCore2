namespace GameCore.Domain.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId, bool? isRead = null);
    Task<NotificationDto?> GetNotificationAsync(int notificationId, int userId);
    Task<NotificationResult> CreateNotificationAsync(int userId, string title, string content, string type);
    Task<NotificationResult> MarkAsReadAsync(int notificationId, int userId);
    Task<NotificationResult> MarkAllAsReadAsync(int userId);
    Task<NotificationResult> DeleteNotificationAsync(int notificationId, int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task<NotificationResult> CreateSystemNotificationAsync(string title, string content);
    Task<NotificationResult> CreatePointsAdjustmentNotificationAsync(int userId, decimal amount, string reason);
    Task<NotificationResult> CreatePetColorChangeNotificationAsync(int userId, string oldColor, string newColor);
}

public class NotificationDto
{
    public int NotificationId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}

public class NotificationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationDto? Notification { get; set; }
    public int UnreadCount { get; set; }
}
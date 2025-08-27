using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 通知服務介面，定義通知相關的業務邏輯
/// </summary>
public interface INotificationsService
{
    /// <summary>
    /// 取得用戶的通知收件匣 (分頁查詢)
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁通知列表</returns>
    Task<PagedNotificationsDto> GetUserNotificationsAsync(int userId, NotificationQueryDto query);

    /// <summary>
    /// 標記通知為已讀
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="notificationId">通知編號</param>
    /// <returns>是否標記成功</returns>
    Task<bool> MarkNotificationAsReadAsync(int userId, int notificationId);

    /// <summary>
    /// 批量標記通知為已讀
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="notificationIds">通知編號列表</param>
    /// <returns>標記成功的數量</returns>
    Task<int> MarkNotificationsAsReadAsync(int userId, List<int> notificationIds);

    /// <summary>
    /// 標記所有通知為已讀
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <returns>標記成功的數量</returns>
    Task<int> MarkAllNotificationsAsReadAsync(int userId);

    /// <summary>
    /// 取得未讀通知數量
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <returns>未讀通知數量</returns>
    Task<int> GetUnreadNotificationCountAsync(int userId);

    /// <summary>
    /// 建立系統通知
    /// </summary>
    /// <param name="request">建立通知請求</param>
    /// <returns>建立的通知資料</returns>
    Task<NotificationDto> CreateSystemNotificationAsync(CreateSystemNotificationRequestDto request);

    /// <summary>
    /// 建立用戶通知
    /// </summary>
    /// <param name="senderId">發送者編號</param>
    /// <param name="request">建立通知請求</param>
    /// <returns>建立的通知資料</returns>
    Task<NotificationDto> CreateUserNotificationAsync(int senderId, CreateUserNotificationRequestDto request);

    /// <summary>
    /// 建立群組通知
    /// </summary>
    /// <param name="senderId">發送者編號</param>
    /// <param name="groupId">群組編號</param>
    /// <param name="request">建立通知請求</param>
    /// <returns>建立的通知資料</returns>
    Task<NotificationDto> CreateGroupNotificationAsync(int senderId, int groupId, CreateGroupNotificationRequestDto request);

    /// <summary>
    /// 刪除通知 (軟刪除)
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="notificationId">通知編號</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteNotificationAsync(int userId, int notificationId);

    /// <summary>
    /// 取得通知來源列表
    /// </summary>
    /// <returns>通知來源列表</returns>
    Task<List<NotificationSourceDto>> GetNotificationSourcesAsync();

    /// <summary>
    /// 取得通知行為列表
    /// </summary>
    /// <returns>通知行為列表</returns>
    Task<List<NotificationActionDto>> GetNotificationActionsAsync();

    /// <summary>
    /// 取得通知詳情
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="notificationId">通知編號</param>
    /// <returns>通知詳情，不存在或無權限則返回 null</returns>
    Task<NotificationDetailDto?> GetNotificationDetailAsync(int userId, int notificationId);
}
using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 通知資料庫操作介面
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// 根據ID取得通知
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns>通知實體</returns>
    Task<Notification?> GetByIdAsync(int notificationId);

    /// <summary>
    /// 根據用戶ID取得通知列表
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>通知列表</returns>
    Task<IEnumerable<Notification>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 20);

    /// <summary>
    /// 新增通知
    /// </summary>
    /// <param name="notification">通知實體</param>
    /// <returns>新增的通知實體</returns>
    Task<Notification> AddAsync(Notification notification);

    /// <summary>
    /// 更新通知
    /// </summary>
    /// <param name="notification">通知實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(Notification notification);

    /// <summary>
    /// 刪除通知
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int notificationId);

    /// <summary>
    /// 取得用戶未讀通知數量
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>未讀通知數量</returns>
    Task<int> GetUnreadCountAsync(int userId);

    /// <summary>
    /// 根據用戶ID取得通知總數 - 效能優化：避免載入完整資料到記憶體
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>通知總數</returns>
    Task<int> GetCountByUserIdAsync(int userId);

    /// <summary>
    /// 根據用戶ID取得今日通知數量 - 效能優化：避免載入完整資料到記憶體
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>今日通知數量</returns>
    Task<int> GetTodayCountByUserIdAsync(int userId);

    /// <summary>
    /// 標記通知為已讀
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否標記成功</returns>
    Task<bool> MarkAsReadAsync(int notificationId, int userId);

    /// <summary>
    /// 根據ID取得通知來源
    /// </summary>
    /// <param name="sourceId">來源ID</param>
    /// <returns>通知來源實體</returns>
    Task<NotificationSource?> GetSourceByIdAsync(int sourceId);

    /// <summary>
    /// 根據ID取得通知動作
    /// </summary>
    /// <param name="actionId">動作ID</param>
    /// <returns>通知動作實體</returns>
    Task<NotificationAction?> GetActionByIdAsync(int actionId);

    /// <summary>
    /// 根據ID取得群組
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>群組實體</returns>
    Task<Group?> GetGroupByIdAsync(int? groupId);

    /// <summary>
    /// 根據通知ID和用戶ID取得通知接收者
    /// </summary>
    /// <param name="notificationId">通知ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>通知接收者實體</returns>
    Task<NotificationRecipient?> GetRecipientByIdAsync(int notificationId, int userId);
} 
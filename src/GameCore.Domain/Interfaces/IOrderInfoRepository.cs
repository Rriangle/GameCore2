using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 訂單資訊資料存取介面
/// 定義訂單相關的資料庫操作
/// </summary>
public interface IOrderInfoRepository
{
    /// <summary>
    /// 根據ID取得訂單
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <returns>訂單資訊</returns>
    Task<OrderInfo?> GetByIdAsync(int orderId);

    /// <summary>
    /// 新增訂單
    /// </summary>
    /// <param name="order">訂單資訊</param>
    /// <returns>訂單ID</returns>
    Task<int> AddAsync(OrderInfo order);

    /// <summary>
    /// 更新訂單
    /// </summary>
    /// <param name="order">訂單資訊</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(OrderInfo order);

    /// <summary>
    /// 刪除訂單
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int orderId);

    /// <summary>
    /// 根據使用者ID取得訂單列表
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>訂單列表</returns>
    Task<List<OrderInfo>> GetByUserIdAsync(int userId, int page, int pageSize);

    /// <summary>
    /// 根據使用者ID取得訂單數量
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>訂單數量</returns>
    Task<int> GetCountByUserIdAsync(int userId);

    /// <summary>
    /// 根據狀態取得訂單列表
    /// </summary>
    /// <param name="status">訂單狀態</param>
    /// <returns>訂單列表</returns>
    Task<List<OrderInfo>> GetByStatusAsync(string status);

    /// <summary>
    /// 根據日期範圍取得訂單列表
    /// </summary>
    /// <param name="startDate">開始日期</param>
    /// <param name="endDate">結束日期</param>
    /// <returns>訂單列表</returns>
    Task<List<OrderInfo>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// 新增訂單項目
    /// </summary>
    /// <param name="orderItem">訂單項目</param>
    /// <returns>是否成功</returns>
    Task<bool> AddOrderItemAsync(OrderItem orderItem);

    /// <summary>
    /// 根據訂單ID取得訂單項目列表
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <returns>訂單項目列表</returns>
    Task<List<OrderItem>> GetOrderItemsAsync(int orderId);

    /// <summary>
    /// 更新訂單狀態
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <param name="status">新狀態</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateStatusAsync(int orderId, string status);

    /// <summary>
    /// 根據總金額範圍取得訂單列表
    /// </summary>
    /// <param name="minAmount">最小金額</param>
    /// <param name="maxAmount">最大金額</param>
    /// <returns>訂單列表</returns>
    Task<List<OrderInfo>> GetByAmountRangeAsync(decimal minAmount, decimal maxAmount);
} 
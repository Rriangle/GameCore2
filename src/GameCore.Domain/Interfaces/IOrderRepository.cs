using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 訂單資料存取介面，定義訂單相關的資料庫操作
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// 根據編號取得訂單 (包含明細和商品資訊)
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <returns>訂單資料，不存在則返回 null</returns>
    Task<OrderInfo?> GetByIdAsync(int orderId);

    /// <summary>
    /// 根據會員編號取得訂單列表
    /// </summary>
    /// <param name="userId">會員編號</param>
    /// <returns>訂單列表</returns>
    Task<List<OrderInfo>> GetByUserIdAsync(int userId);

    /// <summary>
    /// 搜尋訂單 (分頁查詢)
    /// </summary>
    /// <param name="query">搜尋條件</param>
    /// <returns>分頁訂單列表</returns>
    Task<PagedOrdersDto> SearchOrdersAsync(OrderSearchQueryDto query);

    /// <summary>
    /// 建立新訂單
    /// </summary>
    /// <param name="order">訂單資料</param>
    /// <returns>建立的訂單資料</returns>
    Task<OrderInfo> CreateAsync(OrderInfo order);

    /// <summary>
    /// 更新訂單資料
    /// </summary>
    /// <param name="order">訂單資料</param>
    /// <returns>更新的訂單資料</returns>
    Task<OrderInfo> UpdateAsync(OrderInfo order);

    /// <summary>
    /// 更新訂單狀態
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <param name="orderStatus">訂單狀態</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateOrderStatusAsync(int orderId, string orderStatus);

    /// <summary>
    /// 更新付款狀態
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <param name="paymentStatus">付款狀態</param>
    /// <param name="paymentAt">付款時間</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdatePaymentStatusAsync(int orderId, string paymentStatus, DateTime? paymentAt = null);

    /// <summary>
    /// 設定出貨時間
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <param name="shippedAt">出貨時間</param>
    /// <returns>是否更新成功</returns>
    Task<bool> SetShippedAtAsync(int orderId, DateTime shippedAt);

    /// <summary>
    /// 設定完成時間
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <param name="completedAt">完成時間</param>
    /// <returns>是否更新成功</returns>
    Task<bool> SetCompletedAtAsync(int orderId, DateTime completedAt);

    /// <summary>
    /// 取得訂單統計 (依狀態)
    /// </summary>
    /// <param name="userId">會員編號 (null 表示全部會員)</param>
    /// <param name="startDate">開始日期</param>
    /// <param name="endDate">結束日期</param>
    /// <returns>訂單統計</returns>
    Task<Dictionary<string, int>> GetOrderStatisticsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// 取得銷售統計 (依日期)
    /// </summary>
    /// <param name="startDate">開始日期</param>
    /// <param name="endDate">結束日期</param>
    /// <returns>銷售統計</returns>
    Task<Dictionary<DateOnly, decimal>> GetSalesStatisticsAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// 檢查訂單是否屬於指定會員
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <param name="userId">會員編號</param>
    /// <returns>是否屬於該會員</returns>
    Task<bool> IsOrderOwnedByUserAsync(int orderId, int userId);

    /// <summary>
    /// 取得待處理訂單數量
    /// </summary>
    /// <param name="orderStatus">訂單狀態篩選</param>
    /// <returns>待處理訂單數量</returns>
    Task<int> GetPendingOrderCountAsync(string? orderStatus = null);
}
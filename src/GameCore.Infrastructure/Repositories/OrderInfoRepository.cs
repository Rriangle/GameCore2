using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 訂單資訊資料存取層
/// 實作 IOrderInfoRepository 介面
/// </summary>
public class OrderInfoRepository : IOrderInfoRepository
{
    private readonly GameCoreDbContext _context;

    public OrderInfoRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據ID取得訂單
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <returns>訂單資訊</returns>
    public async Task<OrderInfo?> GetByIdAsync(int orderId)
    {
        return await _context.OrderInfos
            .FirstOrDefaultAsync(o => o.OrderID == orderId);
    }

    /// <summary>
    /// 新增訂單
    /// </summary>
    /// <param name="order">訂單資訊</param>
    /// <returns>訂單ID</returns>
    public async Task<int> AddAsync(OrderInfo order)
    {
        _context.OrderInfos.Add(order);
        await _context.SaveChangesAsync();
        return order.OrderID;
    }

    /// <summary>
    /// 更新訂單
    /// </summary>
    /// <param name="order">訂單資訊</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateAsync(OrderInfo order)
    {
        _context.OrderInfos.Update(order);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除訂單
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteAsync(int orderId)
    {
        var order = await GetByIdAsync(orderId);
        if (order == null) return false;

        _context.OrderInfos.Remove(order);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 根據使用者ID取得訂單列表
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>訂單列表</returns>
    public async Task<List<OrderInfo>> GetByUserIdAsync(int userId, int page, int pageSize)
    {
        return await _context.OrderInfos
            .Where(o => o.UserID == userId)
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// 根據使用者ID取得訂單數量
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>訂單數量</returns>
    public async Task<int> GetCountByUserIdAsync(int userId)
    {
        return await _context.OrderInfos
            .CountAsync(o => o.UserID == userId);
    }

    /// <summary>
    /// 根據狀態取得訂單列表
    /// </summary>
    /// <param name="status">訂單狀態</param>
    /// <returns>訂單列表</returns>
    public async Task<List<OrderInfo>> GetByStatusAsync(string status)
    {
        return await _context.OrderInfos
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    /// <summary>
    /// 根據日期範圍取得訂單列表
    /// </summary>
    /// <param name="startDate">開始日期</param>
    /// <param name="endDate">結束日期</param>
    /// <returns>訂單列表</returns>
    public async Task<List<OrderInfo>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.OrderInfos
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    /// <summary>
    /// 新增訂單項目
    /// </summary>
    /// <param name="orderItem">訂單項目</param>
    /// <returns>是否成功</returns>
    public async Task<bool> AddOrderItemAsync(OrderItem orderItem)
    {
        _context.OrderItems.Add(orderItem);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 根據訂單ID取得訂單項目列表
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <returns>訂單項目列表</returns>
    public async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
    {
        return await _context.OrderItems
            .Where(oi => oi.OrderID == orderId)
            .ToListAsync();
    }

    /// <summary>
    /// 更新訂單狀態
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <param name="status">新狀態</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateStatusAsync(int orderId, string status)
    {
        var order = await GetByIdAsync(orderId);
        if (order == null) return false;

        order.OrderStatus = status;
        order.UpdatedAt = DateTime.UtcNow;

        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 根據總金額範圍取得訂單列表
    /// </summary>
    /// <param name="minAmount">最小金額</param>
    /// <param name="maxAmount">最大金額</param>
    /// <returns>訂單列表</returns>
    public async Task<List<OrderInfo>> GetByAmountRangeAsync(decimal minAmount, decimal maxAmount)
    {
        return await _context.OrderInfos
            .Where(o => o.TotalAmount >= minAmount && o.TotalAmount <= maxAmount)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
} 
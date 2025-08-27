using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 訂單資料存取實作，提供訂單相關的資料庫操作
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly GameCoreDbContext _context;

    public OrderRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據編號取得訂單 (包含明細和商品資訊)
    /// </summary>
    public async Task<OrderInfo?> GetByIdAsync(int orderId)
    {
        return await _context.OrderInfos
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    /// <summary>
    /// 根據會員編號取得訂單列表
    /// </summary>
    public async Task<List<OrderInfo>> GetByUserIdAsync(int userId)
    {
        return await _context.OrderInfos
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    /// <summary>
    /// 搜尋訂單 (分頁查詢)
    /// </summary>
    public async Task<PagedOrdersDto> SearchOrdersAsync(OrderSearchQueryDto query)
    {
        var queryable = _context.OrderInfos
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .AsQueryable();

        // 會員篩選
        if (query.UserId.HasValue)
        {
            queryable = queryable.Where(o => o.UserId == query.UserId.Value);
        }

        // 訂單狀態篩選
        if (!string.IsNullOrWhiteSpace(query.OrderStatus))
        {
            queryable = queryable.Where(o => o.OrderStatus == query.OrderStatus);
        }

        // 付款狀態篩選
        if (!string.IsNullOrWhiteSpace(query.PaymentStatus))
        {
            queryable = queryable.Where(o => o.PaymentStatus == query.PaymentStatus);
        }

        // 日期範圍篩選
        if (query.StartDate.HasValue)
        {
            queryable = queryable.Where(o => o.OrderDate >= query.StartDate.Value);
        }
        if (query.EndDate.HasValue)
        {
            queryable = queryable.Where(o => o.OrderDate <= query.EndDate.Value);
        }

        // 移除金額範圍篩選 - DTO中沒有這些屬性

        // 排序
        queryable = query.SortBy?.ToLower() switch
        {
            "date_asc" => queryable.OrderBy(o => o.OrderDate),
            "date_desc" => queryable.OrderByDescending(o => o.OrderDate),
            "total_asc" => queryable.OrderBy(o => o.OrderTotal),
            "total_desc" => queryable.OrderByDescending(o => o.OrderTotal),
            _ => queryable.OrderByDescending(o => o.OrderDate)
        };

        var totalCount = await queryable.CountAsync();
        var orders = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                UserName = o.User.User_name,
                OrderDate = o.OrderDate,
                OrderStatus = o.OrderStatus,
                PaymentStatus = o.PaymentStatus,
                OrderTotal = o.OrderTotal,
                PaymentAt = o.PaymentAt,
                ShippedAt = o.ShippedAt,
                CompletedAt = o.CompletedAt,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ItemId = oi.ItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.ProductName,
                    LineNo = oi.LineNo,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    Subtotal = oi.Subtotal
                }).ToList()
            })
            .ToListAsync();

        return new PagedOrdersDto
        {
            Orders = orders,
            TotalCount = totalCount,
            CurrentPage = query.Page,
            PageSize = query.PageSize
        };
    }

    /// <summary>
    /// 建立新訂單
    /// </summary>
    public async Task<OrderInfo> CreateAsync(OrderInfo order)
    {
        order.OrderDate = DateTime.UtcNow;
        
        _context.OrderInfos.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    /// <summary>
    /// 更新訂單資料
    /// </summary>
    public async Task<OrderInfo> UpdateAsync(OrderInfo order)
    {
        _context.OrderInfos.Update(order);
        await _context.SaveChangesAsync();

        return order;
    }

    /// <summary>
    /// 更新訂單狀態
    /// </summary>
    public async Task<bool> UpdateOrderStatusAsync(int orderId, string orderStatus)
    {
        var order = await _context.OrderInfos.FindAsync(orderId);
        if (order == null)
            return false;

        order.OrderStatus = orderStatus;
        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// 更新付款狀態
    /// </summary>
    public async Task<bool> UpdatePaymentStatusAsync(int orderId, string paymentStatus, DateTime? paymentAt = null)
    {
        var order = await _context.OrderInfos.FindAsync(orderId);
        if (order == null)
            return false;

        order.PaymentStatus = paymentStatus;
        if (paymentAt.HasValue)
        {
            order.PaymentAt = paymentAt.Value;
        }

        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// 設定出貨時間
    /// </summary>
    public async Task<bool> SetShippedAtAsync(int orderId, DateTime shippedAt)
    {
        var order = await _context.OrderInfos.FindAsync(orderId);
        if (order == null)
            return false;

        order.ShippedAt = shippedAt;
        order.OrderStatus = "Shipped";

        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// 設定完成時間
    /// </summary>
    public async Task<bool> SetCompletedAtAsync(int orderId, DateTime completedAt)
    {
        var order = await _context.OrderInfos.FindAsync(orderId);
        if (order == null)
            return false;

        order.CompletedAt = completedAt;
        order.OrderStatus = "Completed";

        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// 取得訂單統計 (依狀態)
    /// </summary>
    public async Task<Dictionary<string, int>> GetOrderStatisticsAsync(int? userId = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryable = _context.OrderInfos.AsQueryable();

        if (userId.HasValue)
        {
            queryable = queryable.Where(o => o.UserId == userId.Value);
        }

        if (startDate.HasValue)
        {
            queryable = queryable.Where(o => o.OrderDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            queryable = queryable.Where(o => o.OrderDate <= endDate.Value);
        }

        return await queryable
            .GroupBy(o => o.OrderStatus)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// 取得銷售統計 (依日期)
    /// </summary>
    public async Task<Dictionary<DateOnly, decimal>> GetSalesStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var orders = await _context.OrderInfos
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.PaymentStatus == "Paid")
            .GroupBy(o => DateOnly.FromDateTime(o.OrderDate))
            .Select(g => new { Date = g.Key, Total = g.Sum(o => o.OrderTotal) })
            .ToListAsync();

        return orders.ToDictionary(x => x.Date, x => x.Total);
    }

    /// <summary>
    /// 檢查訂單是否屬於指定會員
    /// </summary>
    public async Task<bool> IsOrderOwnedByUserAsync(int orderId, int userId)
    {
        return await _context.OrderInfos
            .AnyAsync(o => o.OrderId == orderId && o.UserId == userId);
    }

    /// <summary>
    /// 取得待處理訂單數量
    /// </summary>
    public async Task<int> GetPendingOrderCountAsync(string? orderStatus = null)
    {
        var queryable = _context.OrderInfos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(orderStatus))
        {
            queryable = queryable.Where(o => o.OrderStatus == orderStatus);
        }
        else
        {
            // 預設返回待處理的訂單 (未完成的訂單)
            queryable = queryable.Where(o => o.OrderStatus != "Completed" && o.OrderStatus != "Cancelled");
        }

        return await queryable.CountAsync();
    }
}
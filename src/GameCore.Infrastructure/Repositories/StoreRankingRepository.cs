using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 商城排行榜資料存取實作，提供排行榜相關的資料庫操作
/// </summary>
public class StoreRankingRepository : IStoreRankingRepository
{
    private readonly GameCoreDbContext _context;

    public StoreRankingRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據編號取得排行榜記錄
    /// </summary>
    public async Task<OfficialStoreRanking?> GetByIdAsync(int rankingId)
    {
        return await _context.OfficialStoreRankings
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.RankingId == rankingId);
    }

    /// <summary>
    /// 取得排行榜資料 (依查詢條件)
    /// </summary>
    public async Task<List<StoreRankingDto>> GetRankingsAsync(RankingQueryDto query)
    {
        var queryable = _context.OfficialStoreRankings
            .Include(r => r.Product)
            .AsQueryable();

        // 期間類型篩選
        if (!string.IsNullOrWhiteSpace(query.PeriodType))
        {
            queryable = queryable.Where(r => r.PeriodType == query.PeriodType);
        }

        // 日期篩選
        if (query.Date.HasValue)
        {
            queryable = queryable.Where(r => r.RankingDate == query.Date.Value);
        }

        // 排名指標篩選
        if (!string.IsNullOrWhiteSpace(query.RankingMetric))
        {
            queryable = queryable.Where(r => r.RankingMetric == query.RankingMetric);
        }

        // 排序 (依排名位置)
        queryable = queryable.OrderBy(r => r.RankingPosition);

        // 限制數量
        if (query.Limit > 0)
        {
            queryable = queryable.Take(query.Limit);
        }

        return await queryable
            .Select(r => new StoreRankingDto
            {
                RankingId = r.RankingId,
                PeriodType = r.PeriodType,
                RankingDate = r.RankingDate,
                ProductId = r.ProductId,
                ProductName = r.Product.ProductName,
                RankingMetric = r.RankingMetric,
                RankingPosition = r.RankingPosition,
                TradingAmount = r.TradingAmount,
                TradingVolume = r.TradingVolume,
                RankingUpdatedAt = r.RankingUpdatedAt
            })
            .ToListAsync();
    }

    /// <summary>
    /// 取得商品在特定期間的排名
    /// </summary>
    public async Task<OfficialStoreRanking?> GetProductRankingAsync(int productId, string periodType, DateTime rankingDate)
    {
        var targetDateOnly = DateOnly.FromDateTime(rankingDate);
        return await _context.OfficialStoreRankings
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.ProductId == productId && 
                                    r.PeriodType == periodType && 
                                    r.RankingDate == targetDateOnly);
    }

    /// <summary>
    /// 取得熱門商品排行榜 (前N名)
    /// </summary>
    public async Task<List<StoreRankingDto>> GetTopProductsAsync(string periodType, DateTime rankingDate, int limit = 10)
    {
        var targetDateOnly = DateOnly.FromDateTime(rankingDate);
        return await _context.OfficialStoreRankings
            .Include(r => r.Product)
            .Where(r => r.PeriodType == periodType && r.RankingDate == targetDateOnly)
            .OrderBy(r => r.RankingPosition)
            .Take(limit)
            .Select(r => new StoreRankingDto
            {
                RankingId = r.RankingId,
                PeriodType = r.PeriodType,
                RankingDate = r.RankingDate,
                ProductId = r.ProductId,
                ProductName = r.Product.ProductName,
                RankingMetric = r.RankingMetric,
                RankingPosition = r.RankingPosition,
                TradingAmount = r.TradingAmount,
                TradingVolume = r.TradingVolume,
                RankingUpdatedAt = r.RankingUpdatedAt
            })
            .ToListAsync();
    }

    /// <summary>
    /// 建立或更新排行榜記錄
    /// </summary>
    public async Task<OfficialStoreRanking> CreateOrUpdateAsync(OfficialStoreRanking ranking)
    {
        var existing = await GetProductRankingAsync(ranking.ProductId, ranking.PeriodType, ranking.RankingDate.ToDateTime(TimeOnly.MinValue));
        
        if (existing != null)
        {
            // 更新現有記錄
            existing.RankingMetric = ranking.RankingMetric;
            existing.RankingPosition = ranking.RankingPosition;
            existing.TradingAmount = ranking.TradingAmount;
            existing.TradingVolume = ranking.TradingVolume;
            existing.RankingUpdatedAt = DateTime.UtcNow;
            
            _context.OfficialStoreRankings.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }
        else
        {
            // 建立新記錄
            ranking.RankingUpdatedAt = DateTime.UtcNow;
            _context.OfficialStoreRankings.Add(ranking);
            await _context.SaveChangesAsync();
            return ranking;
        }
    }

    /// <summary>
    /// 批次建立或更新排行榜記錄
    /// </summary>
    public async Task<int> BatchCreateOrUpdateAsync(List<OfficialStoreRanking> rankings)
    {
        int processedCount = 0;

        foreach (var ranking in rankings)
        {
            await CreateOrUpdateAsync(ranking);
            processedCount++;
        }

        return processedCount;
    }

    /// <summary>
    /// 刪除過期的排行榜記錄
    /// </summary>
    public async Task<int> DeleteOldRecordsAsync(DateTime olderThan)
    {
        var olderThanDateOnly = DateOnly.FromDateTime(olderThan);
        var oldRecords = await _context.OfficialStoreRankings
            .Where(r => r.RankingDate < olderThanDateOnly)
            .ToListAsync();

        if (oldRecords.Any())
        {
            _context.OfficialStoreRankings.RemoveRange(oldRecords);
            await _context.SaveChangesAsync();
        }

        return oldRecords.Count;
    }

    /// <summary>
    /// 計算商品的排行榜數據
    /// </summary>
    public async Task<List<OfficialStoreRanking>> CalculateRankingsAsync(DateTime startDate, DateTime endDate, string periodType)
    {
        // 基於訂單數據計算排行榜
        var productSales = await _context.OrderItems
            .Include(oi => oi.Order)
            .Include(oi => oi.Product)
            .Where(oi => oi.Order.OrderDate >= startDate && 
                        oi.Order.OrderDate <= endDate && 
                        oi.Order.PaymentStatus == "Paid")
            .GroupBy(oi => oi.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalAmount = g.Sum(oi => oi.Subtotal),
                TotalVolume = g.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync();

        var rankings = new List<OfficialStoreRanking>();
        int position = 1;

        foreach (var sale in productSales)
        {
            rankings.Add(new OfficialStoreRanking
            {
                PeriodType = periodType,
                RankingDate = DateOnly.FromDateTime(endDate),
                ProductId = sale.ProductId,
                RankingMetric = "Sales",
                RankingPosition = (byte)Math.Min(position, 255),
                TradingAmount = sale.TotalAmount,
                TradingVolume = sale.TotalVolume,
                RankingUpdatedAt = DateTime.UtcNow
            });
            position++;
        }

        return rankings;
    }

    /// <summary>
    /// 取得排行榜統計資訊
    /// </summary>
    public async Task<Dictionary<string, object>> GetRankingStatisticsAsync(string periodType, DateTime startDate, DateTime endDate)
    {
        var startDateOnly = DateOnly.FromDateTime(startDate);
        var endDateOnly = DateOnly.FromDateTime(endDate);
        var rankings = await _context.OfficialStoreRankings
            .Where(r => r.PeriodType == periodType && 
                       r.RankingDate >= startDateOnly && 
                       r.RankingDate <= endDateOnly)
            .ToListAsync();

        var statistics = new Dictionary<string, object>
        {
            ["TotalRecords"] = rankings.Count,
            ["TotalProducts"] = rankings.Select(r => r.ProductId).Distinct().Count(),
            ["TotalTradingAmount"] = rankings.Sum(r => r.TradingAmount),
            ["TotalTradingVolume"] = rankings.Sum(r => r.TradingVolume),
            ["AverageTradingAmount"] = rankings.Any() ? rankings.Average(r => r.TradingAmount) : 0,
            ["AverageTradingVolume"] = rankings.Any() ? rankings.Average(r => r.TradingVolume) : 0,
            ["PeriodType"] = periodType,
            ["StartDate"] = startDate,
            ["EndDate"] = endDate
        };

        return statistics;
    }

    /// <summary>
    /// 檢查排行榜記錄是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(int productId, string periodType, DateTime rankingDate)
    {
        var targetDateOnly = DateOnly.FromDateTime(rankingDate);
        return await _context.OfficialStoreRankings
            .AnyAsync(r => r.ProductId == productId && 
                          r.PeriodType == periodType && 
                          r.RankingDate == targetDateOnly);
    }
}
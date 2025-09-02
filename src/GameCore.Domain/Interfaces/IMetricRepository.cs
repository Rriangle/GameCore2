using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 指標資料庫操作介面
/// </summary>
public interface IMetricRepository
{
    /// <summary>
    /// 根據ID取得指標
    /// </summary>
    /// <param name="metricId">指標ID</param>
    /// <returns>指標實體</returns>
    Task<Metric?> GetByIdAsync(int metricId);

    /// <summary>
    /// 新增指標
    /// </summary>
    /// <param name="metric">指標實體</param>
    /// <returns>新增的指標實體</returns>
    Task<Metric> AddAsync(Metric metric);

    /// <summary>
    /// 更新指標
    /// </summary>
    /// <param name="metric">指標實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(Metric metric);

    /// <summary>
    /// 刪除指標
    /// </summary>
    /// <param name="metricId">指標ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int metricId);

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    IQueryable<Metric> GetQueryable();
} 
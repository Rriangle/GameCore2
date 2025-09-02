using GameCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 每日熱門度指數資料庫操作介面
/// </summary>
public interface IPopularityIndexDailyRepository
{
    /// <summary>
    /// 根據ID取得每日熱門度指數
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>每日熱門度指數實體</returns>
    Task<PopularityIndexDaily?> GetByIdAsync(int id);

    /// <summary>
    /// 新增每日熱門度指數
    /// </summary>
    /// <param name="popularity">每日熱門度指數實體</param>
    /// <returns>新增的每日熱門度指數實體</returns>
    Task<PopularityIndexDaily> AddAsync(PopularityIndexDaily popularity);

    /// <summary>
    /// 更新每日熱門度指數
    /// </summary>
    /// <param name="popularity">每日熱門度指數實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(PopularityIndexDaily popularity);

    /// <summary>
    /// 刪除每日熱門度指數
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 取得可查詢的 IQueryable
    /// </summary>
    /// <returns>IQueryable</returns>
    IQueryable<PopularityIndexDaily> GetQueryable();
} 
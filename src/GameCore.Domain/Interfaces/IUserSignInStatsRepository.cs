using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶簽到統計 Repository 介面
/// </summary>
public interface IUserSignInStatsRepository
{
    /// <summary>
    /// 根據用戶ID和日期獲取簽到記錄
    /// </summary>
    Task<UserSignInStats?> GetByUserIdAndDateAsync(int userId, DateTime date);

    /// <summary>
    /// 根據用戶ID和日期範圍獲取簽到記錄
    /// </summary>
    Task<IEnumerable<UserSignInStats>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// 獲取用戶最後一次簽到記錄
    /// </summary>
    Task<UserSignInStats?> GetLastSignInAsync(int userId);

    /// <summary>
    /// 根據用戶ID獲取簽到記錄（分頁）
    /// </summary>
    Task<IEnumerable<UserSignInStats>> GetByUserIdAsync(int userId, int page = 1, int pageSize = 20);

    /// <summary>
    /// 獲取用戶簽到記錄總數
    /// </summary>
    Task<int> GetCountByUserIdAsync(int userId);

    /// <summary>
    /// 根據用戶ID和日期範圍獲取簽到記錄計數 - 效能優化：避免載入完整資料到記憶體
    /// </summary>
    Task<int> GetCountByUserIdAndDateRangeAsync(int userId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// 新增簽到記錄
    /// </summary>
    Task<UserSignInStats> AddAsync(UserSignInStats signInStats);

    /// <summary>
    /// 更新簽到記錄
    /// </summary>
    Task<bool> UpdateAsync(UserSignInStats signInStats);

    /// <summary>
    /// 刪除簽到記錄
    /// </summary>
    Task<bool> DeleteAsync(int id);
} 
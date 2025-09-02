using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 使用者銷售資訊存取介面
/// </summary>
public interface IUserSalesInformationRepository
{
    /// <summary>
    /// 根據用戶ID取得銷售資訊
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>銷售資訊</returns>
    Task<UserSalesInformation?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 新增銷售資訊
    /// </summary>
    /// <param name="info">銷售資訊</param>
    /// <returns>是否成功</returns>
    Task<bool> AddAsync(UserSalesInformation info);

    /// <summary>
    /// 更新銷售資訊
    /// </summary>
    /// <param name="info">銷售資訊</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(UserSalesInformation info);

    /// <summary>
    /// 刪除銷售資訊
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 取得所有銷售資訊
    /// </summary>
    /// <returns>銷售資訊列表</returns>
    Task<List<UserSalesInformation>> GetAllAsync();

    /// <summary>
    /// 更新銷售錢包餘額
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <param name="amount">金額變動</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateWalletBalanceAsync(int userId, int amount);

    /// <summary>
    /// 檢查用戶是否已有銷售資訊
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(int userId);

    /// <summary>
    /// 取得用戶的客戶列表
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>客戶列表</returns>
    Task<List<User>> GetCustomersByUserIdAsync(int userId);

    /// <summary>
    /// 取得頂級銷售用戶
    /// </summary>
    /// <param name="top">前幾名</param>
    /// <returns>頂級銷售用戶列表</returns>
    Task<List<UserSalesInformation>> GetTopSalesUsersAsync(int top = 10);
} 
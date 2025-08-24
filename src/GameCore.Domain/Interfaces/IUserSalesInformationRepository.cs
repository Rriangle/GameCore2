using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 使用者銷售資訊資料存取介面
/// </summary>
public interface IUserSalesInformationRepository
{
    /// <summary>
    /// 根據使用者ID取得使用者銷售資訊
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>使用者銷售資訊，如果不存在則返回null</returns>
    Task<UserSalesInformation?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 創建使用者銷售資訊
    /// </summary>
    /// <param name="salesInfo">使用者銷售資訊</param>
    /// <returns>創建的使用者銷售資訊</returns>
    Task<UserSalesInformation> CreateAsync(UserSalesInformation salesInfo);

    /// <summary>
    /// 更新使用者銷售資訊
    /// </summary>
    /// <param name="salesInfo">使用者銷售資訊</param>
    /// <returns>更新的使用者銷售資訊</returns>
    Task<UserSalesInformation> UpdateAsync(UserSalesInformation salesInfo);

    /// <summary>
    /// 刪除使用者銷售資訊
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 增加使用者銷售錢包金額
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="amount">增加金額</param>
    /// <returns>更新後的銷售錢包金額</returns>
    Task<int> AddWalletAmountAsync(int userId, int amount);

    /// <summary>
    /// 扣除使用者銷售錢包金額
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="amount">扣除金額</param>
    /// <returns>更新後的銷售錢包金額，如果餘額不足則返回-1</returns>
    Task<int> DeductWalletAmountAsync(int userId, int amount);

    /// <summary>
    /// 取得使用者銷售錢包餘額
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>銷售錢包餘額</returns>
    Task<int> GetWalletBalanceAsync(int userId);

    /// <summary>
    /// 批量取得多個使用者的銷售資訊
    /// </summary>
    /// <param name="userIds">使用者ID列表</param>
    /// <returns>使用者銷售資訊列表</returns>
    Task<IEnumerable<UserSalesInformation>> GetByUserIdsAsync(IEnumerable<int> userIds);
}
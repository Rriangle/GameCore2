using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 使用者錢包資料存取介面
/// </summary>
public interface IUserWalletRepository
{
    /// <summary>
    /// 根據使用者ID取得錢包資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>錢包資料，如果不存在則返回null</returns>
    Task<UserWallet?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 創建錢包資料
    /// </summary>
    /// <param name="wallet">錢包資料</param>
    /// <returns>創建的錢包資料</returns>
    Task<UserWallet> CreateAsync(UserWallet wallet);

    /// <summary>
    /// 更新錢包資料
    /// </summary>
    /// <param name="wallet">錢包資料</param>
    /// <returns>更新的錢包資料</returns>
    Task<UserWallet> UpdateAsync(UserWallet wallet);

    /// <summary>
    /// 刪除錢包資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 取得使用者點數餘額
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>點數餘額</returns>
    Task<int> GetUserPointAsync(int userId);

    /// <summary>
    /// 增加使用者點數
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="points">增加點數</param>
    /// <returns>更新後的點數餘額</returns>
    Task<int> AddUserPointAsync(int userId, int points);

    /// <summary>
    /// 扣除使用者點數
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="points">扣除點數</param>
    /// <returns>更新後的點數餘額，如果餘額不足則返回-1</returns>
    Task<int> DeductUserPointAsync(int userId, int points);

    /// <summary>
    /// 設定優惠券編號
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="couponNumber">優惠券編號</param>
    /// <returns>是否設定成功</returns>
    Task<bool> SetCouponNumberAsync(int userId, string? couponNumber);

    /// <summary>
    /// 取得使用者優惠券編號
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>優惠券編號</returns>
    Task<string?> GetCouponNumberAsync(int userId);

    /// <summary>
    /// 取得錢包餘額（舊版本相容）
    /// </summary>
    [Obsolete("請使用 GetUserPointAsync 方法")]
    Task<decimal> GetBalanceAsync(int userId);
}

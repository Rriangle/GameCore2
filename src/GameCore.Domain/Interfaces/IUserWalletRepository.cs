using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶錢包資料存取介面，定義用戶錢包相關的資料庫操作
/// </summary>
public interface IUserWalletRepository
{
    /// <summary>
    /// 根據用戶編號取得錢包資料
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>錢包資料，不存在則返回 null</returns>
    Task<UserWallet?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 建立新錢包
    /// </summary>
    /// <param name="wallet">錢包資料</param>
    /// <returns>建立的錢包資料</returns>
    Task<UserWallet> CreateAsync(UserWallet wallet);

    /// <summary>
    /// 更新錢包資料
    /// </summary>
    /// <param name="wallet">錢包資料</param>
    /// <returns>更新的錢包資料</returns>
    Task<UserWallet> UpdateAsync(UserWallet wallet);

    /// <summary>
    /// 取得用戶點數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>點數餘額</returns>
    Task<int> GetPointsAsync(int userId);

    /// <summary>
    /// 更新用戶點數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="points">新點數餘額</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdatePointsAsync(int userId, int points);

    /// <summary>
    /// 增加用戶點數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="pointsToAdd">要增加的點數</param>
    /// <returns>是否增加成功</returns>
    Task<bool> AddPointsAsync(int userId, int pointsToAdd);

    /// <summary>
    /// 扣除用戶點數
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="pointsToDeduct">要扣除的點數</param>
    /// <returns>是否扣除成功</returns>
    Task<bool> DeductPointsAsync(int userId, int pointsToDeduct);
}

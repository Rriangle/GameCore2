using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶介紹資料庫操作介面
/// </summary>
public interface IUserIntroduceRepository
{
    /// <summary>
    /// 根據用戶 ID 取得用戶介紹
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>用戶介紹實體</returns>
    Task<UserIntroduce?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 新增用戶介紹
    /// </summary>
    /// <param name="userIntroduce">用戶介紹實體</param>
    /// <returns>新增的用戶介紹</returns>
    Task<UserIntroduce> AddAsync(UserIntroduce userIntroduce);

    /// <summary>
    /// 更新用戶介紹
    /// </summary>
    /// <param name="userIntroduce">用戶介紹實體</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(UserIntroduce userIntroduce);

    /// <summary>
    /// 刪除用戶介紹
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 檢查暱稱是否已存在
    /// </summary>
    /// <param name="nickname">暱稱</param>
    /// <returns>是否已存在</returns>
    Task<bool> ExistsByNicknameAsync(string nickname);

    /// <summary>
    /// 檢查身分證字號是否已存在
    /// </summary>
    /// <param name="idNumber">身分證字號</param>
    /// <returns>是否已存在</returns>
    Task<bool> ExistsByIdNumberAsync(string idNumber);

    /// <summary>
    /// 檢查手機號碼是否已存在
    /// </summary>
    /// <param name="cellphone">手機號碼</param>
    /// <returns>是否已存在</returns>
    Task<bool> ExistsByCellphoneAsync(string cellphone);

    /// <summary>
    /// 檢查電子郵件是否已存在
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>是否已存在</returns>
    Task<bool> ExistsByEmailAsync(string email);
}
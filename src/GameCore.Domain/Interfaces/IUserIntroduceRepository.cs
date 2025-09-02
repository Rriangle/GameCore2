using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 用戶介紹資料庫操作介面
/// </summary>
public interface IUserIntroduceRepository
{
    /// <summary>
    /// 根據用戶ID取得用戶介紹
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>用戶介紹實體</returns>
    Task<UserIntroduce?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 根據暱稱取得用戶介紹
    /// </summary>
    /// <param name="nickName">暱稱</param>
    /// <returns>用戶介紹實體</returns>
    Task<UserIntroduce?> GetByNickNameAsync(string nickName);

    /// <summary>
    /// 根據身分證字號取得用戶介紹
    /// </summary>
    /// <param name="idNumber">身分證字號</param>
    /// <returns>用戶介紹實體</returns>
    Task<UserIntroduce?> GetByIdNumberAsync(string idNumber);

    /// <summary>
    /// 根據手機號碼取得用戶介紹
    /// </summary>
    /// <param name="cellphone">手機號碼</param>
    /// <returns>用戶介紹實體</returns>
    Task<UserIntroduce?> GetByCellphoneAsync(string cellphone);

    /// <summary>
    /// 根據電子郵件取得用戶介紹
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>用戶介紹實體</returns>
    Task<UserIntroduce?> GetByEmailAsync(string email);

    /// <summary>
    /// 新增用戶介紹
    /// </summary>
    /// <param name="userIntroduce">用戶介紹實體</param>
    /// <returns>新增的用戶介紹實體</returns>
    Task<UserIntroduce> AddAsync(UserIntroduce userIntroduce);

    /// <summary>
    /// 更新用戶介紹
    /// </summary>
    /// <param name="userIntroduce">用戶介紹實體</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateAsync(UserIntroduce userIntroduce);

    /// <summary>
    /// 刪除用戶介紹
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 檢查暱稱是否存在
    /// </summary>
    /// <param name="nickName">暱稱</param>
    /// <returns>是否存在</returns>
    Task<bool> NickNameExistsAsync(string nickName);

    /// <summary>
    /// 檢查身分證字號是否存在
    /// </summary>
    /// <param name="idNumber">身分證字號</param>
    /// <returns>是否存在</returns>
    Task<bool> IdNumberExistsAsync(string idNumber);

    /// <summary>
    /// 檢查手機號碼是否存在
    /// </summary>
    /// <param name="cellphone">手機號碼</param>
    /// <returns>是否存在</returns>
    Task<bool> CellphoneExistsAsync(string cellphone);

    /// <summary>
    /// 檢查電子郵件是否存在
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>是否存在</returns>
    Task<bool> EmailExistsAsync(string email);
} 
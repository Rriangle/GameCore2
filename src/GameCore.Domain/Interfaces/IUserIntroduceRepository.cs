using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 使用者介紹資料存取介面
/// </summary>
public interface IUserIntroduceRepository
{
    /// <summary>
    /// 根據使用者ID取得使用者介紹資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>使用者介紹資料，如果不存在則返回null</returns>
    Task<UserIntroduce?> GetByUserIdAsync(int userId);

    /// <summary>
    /// 創建使用者介紹資料
    /// </summary>
    /// <param name="userIntroduce">使用者介紹資料</param>
    /// <returns>創建的使用者介紹資料</returns>
    Task<UserIntroduce> CreateAsync(UserIntroduce userIntroduce);

    /// <summary>
    /// 更新使用者介紹資料
    /// </summary>
    /// <param name="userIntroduce">使用者介紹資料</param>
    /// <returns>更新的使用者介紹資料</returns>
    Task<UserIntroduce> UpdateAsync(UserIntroduce userIntroduce);

    /// <summary>
    /// 刪除使用者介紹資料
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteAsync(int userId);

    /// <summary>
    /// 檢查暱稱是否已存在
    /// </summary>
    /// <param name="nickname">暱稱</param>
    /// <param name="excludeUserId">排除的使用者ID（用於更新時檢查）</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsNicknameExistsAsync(string nickname, int? excludeUserId = null);

    /// <summary>
    /// 檢查身分證字號是否已存在
    /// </summary>
    /// <param name="idNumber">身分證字號</param>
    /// <param name="excludeUserId">排除的使用者ID（用於更新時檢查）</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsIdNumberExistsAsync(string idNumber, int? excludeUserId = null);

    /// <summary>
    /// 檢查電話是否已存在
    /// </summary>
    /// <param name="cellphone">電話</param>
    /// <param name="excludeUserId">排除的使用者ID（用於更新時檢查）</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsCellphoneExistsAsync(string cellphone, int? excludeUserId = null);

    /// <summary>
    /// 檢查Email是否已存在
    /// </summary>
    /// <param name="email">Email</param>
    /// <param name="excludeUserId">排除的使用者ID（用於更新時檢查）</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null);
}
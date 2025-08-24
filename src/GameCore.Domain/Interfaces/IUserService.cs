using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 使用者服務介面
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 根據使用者編號取得個人資料
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <returns>使用者個人資料</returns>
    Task<UserProfileDto?> GetUserProfileAsync(int userId);

    /// <summary>
    /// 根據登入帳號取得使用者
    /// </summary>
    /// <param name="userAccount">登入帳號</param>
    /// <returns>使用者個人資料</returns>
    Task<UserProfileDto?> GetUserByAccountAsync(string userAccount);

    /// <summary>
    /// 更新使用者個人資料
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="request">更新請求資料</param>
    /// <returns>更新結果</returns>
    Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileRequestDto request);

    /// <summary>
    /// 修改密碼
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="request">修改密碼請求資料</param>
    /// <returns>修改結果</returns>
    Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);

    /// <summary>
    /// 上傳頭像圖片
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="imageData">圖片資料</param>
    /// <returns>上傳結果</returns>
    Task<(bool Success, string Message)> UploadAvatarAsync(int userId, byte[] imageData);

    /// <summary>
    /// 檢查帳號是否已存在
    /// </summary>
    /// <param name="userAccount">登入帳號</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsAccountExistsAsync(string userAccount);

    /// <summary>
    /// 檢查暱稱是否已存在
    /// </summary>
    /// <param name="nickName">暱稱</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsNickNameExistsAsync(string nickName);

    /// <summary>
    /// 檢查身分證字號是否已存在
    /// </summary>
    /// <param name="idNumber">身分證字號</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsIdNumberExistsAsync(string idNumber);

    /// <summary>
    /// 檢查手機號碼是否已存在
    /// </summary>
    /// <param name="cellphone">手機號碼</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsCellphoneExistsAsync(string cellphone);

    /// <summary>
    /// 檢查電子郵件是否已存在
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>是否已存在</returns>
    Task<bool> IsEmailExistsAsync(string email);
}
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 用戶服務實作類別，處理用戶個人資料管理功能
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserWalletRepository _walletRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IUserWalletRepository walletRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    /// <summary>
    /// 根據用戶編號取得個人資料
    /// </summary>
    public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
    {
        try
        {
            _logger.LogDebug("取得用戶個人資料: User_ID = {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("用戶不存在: User_ID = {UserId}", userId);
                return null;
            }

            var profile = new UserProfileDto
            {
                User_ID = user.User_ID,
                User_name = user.User_name,
                User_Account = user.User_Account,
                User_NickName = user.UserIntroduce?.User_NickName ?? user.User_name,
                Gender = user.UserIntroduce?.Gender ?? "M",
                Cellphone = user.UserIntroduce?.Cellphone ?? string.Empty,
                Email = user.UserIntroduce?.Email ?? string.Empty,
                Address = user.UserIntroduce?.Address ?? string.Empty,
                DateOfBirth = user.UserIntroduce?.DateOfBirth ?? DateTime.UtcNow.AddYears(-20),
                Create_Account = user.UserIntroduce?.Create_Account ?? DateTime.UtcNow,
                User_Introduce = user.UserIntroduce?.User_Introduce,
                User_Picture_Base64 = user.UserIntroduce?.User_Picture != null ? 
                    Convert.ToBase64String(user.UserIntroduce.User_Picture) : null,

                // 用戶權限（暫時使用預設值）
                UserRights = new UserRightsDto
                {
                    User_Status = true,
                    ShoppingPermission = user.UserRights?.ShoppingPermission ?? true,
                    MessagePermission = user.UserRights?.MessagePermission ?? true,
                    SalesAuthority = user.UserRights?.SalesAuthority ?? false
                },

                // 錢包資訊
                UserWallet = new UserWalletDto
                {
                    User_Id = user.UserWallet?.User_Id ?? userId,
                    User_Point = user.UserWallet?.User_Point ?? 0,
                    Coupon_Number = user.UserWallet?.Coupon_Number
                }
            };

            _logger.LogDebug("成功取得用戶個人資料: User_ID = {UserId}", userId);
            return profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶個人資料時發生錯誤: User_ID = {UserId}, Error = {Error}", userId, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 根據登入帳號取得用戶
    /// </summary>
    public async Task<UserProfileDto?> GetUserByAccountAsync(string userAccount)
    {
        try
        {
            _logger.LogDebug("根據帳號取得用戶資料: User_Account = {UserAccount}", userAccount);

            var user = await _userRepository.GetByAccountAsync(userAccount);
            if (user == null)
            {
                _logger.LogWarning("用戶不存在: User_Account = {UserAccount}", userAccount);
                return null;
            }

            return await GetUserProfileAsync(user.User_ID);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據帳號取得用戶資料時發生錯誤: User_Account = {UserAccount}, Error = {Error}", userAccount, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 更新用戶個人資料
    /// </summary>
    public async Task<(bool Success, string Message)> UpdateProfileAsync(int userId, UpdateProfileRequestDto request)
    {
        try
        {
            _logger.LogInformation("更新用戶個人資料: User_ID = {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, "用戶不存在");
            }

            // 更新用戶介紹資料
            if (user.UserIntroduce != null)
            {
                if (!string.IsNullOrEmpty(request.User_NickName))
                    user.UserIntroduce.User_NickName = request.User_NickName;
                
                if (!string.IsNullOrEmpty(request.Gender))
                    user.UserIntroduce.Gender = request.Gender;
                
                if (!string.IsNullOrEmpty(request.Cellphone))
                    user.UserIntroduce.Cellphone = request.Cellphone;
                
                if (!string.IsNullOrEmpty(request.Email))
                    user.UserIntroduce.Email = request.Email;
                
                if (!string.IsNullOrEmpty(request.Address))
                    user.UserIntroduce.Address = request.Address;
                
                if (!string.IsNullOrEmpty(request.User_Introduce))
                    user.UserIntroduce.User_Introduce = request.User_Introduce;
            }

            // 儲存變更
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("用戶個人資料更新成功: User_ID = {UserId}", userId);
            return (true, "個人資料更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用戶個人資料時發生錯誤: User_ID = {UserId}, Error = {Error}", userId, ex.Message);
            return (false, "更新個人資料時發生錯誤，請稍後再試");
        }
    }

    /// <summary>
    /// 更改密碼
    /// </summary>
    public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
    {
        try
        {
            _logger.LogInformation("用戶更改密碼: User_ID = {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, "用戶不存在");
            }

            // 驗證舊密碼
            var oldPasswordHash = HashPassword(request.OldPassword);
            if (user.User_Password != oldPasswordHash)
            {
                _logger.LogWarning("更改密碼失敗：舊密碼錯誤 - User_ID = {UserId}", userId);
                return (false, "舊密碼錯誤");
            }

            // 設置新密碼
            user.User_Password = HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("用戶密碼更改成功: User_ID = {UserId}", userId);
            return (true, "密碼更改成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更改密碼時發生錯誤: User_ID = {UserId}, Error = {Error}", userId, ex.Message);
            return (false, "更改密碼時發生錯誤，請稍後再試");
        }
    }

    /// <summary>
    /// 上傳頭像圖片
    /// </summary>
    public async Task<(bool Success, string Message)> UploadAvatarAsync(int userId, byte[] imageData)
    {
        try
        {
            _logger.LogInformation("上傳頭像: User_ID = {UserId}", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, "用戶不存在");
            }

            // 檢查圖片大小（限制 5MB）
            if (imageData.Length > 5 * 1024 * 1024)
            {
                return (false, "圖片大小不能超過 5MB");
            }

            // 更新頭像
            if (user.UserIntroduce != null)
            {
                user.UserIntroduce.User_Picture = imageData;
                await _userRepository.UpdateAsync(user);
            }

            _logger.LogInformation("頭像上傳成功: User_ID = {UserId}", userId);
            return (true, "頭像上傳成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "上傳頭像時發生錯誤: User_ID = {UserId}, Error = {Error}", userId, ex.Message);
            return (false, "上傳頭像時發生錯誤，請稍後再試");
        }
    }

    /// <summary>
    /// 檢查帳號是否已存在
    /// </summary>
    public async Task<bool> IsAccountExistsAsync(string userAccount)
    {
        try
        {
            return !await _userRepository.IsAccountUniqueAsync(userAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查帳號存在性時發生錯誤: Account = {Account}, Error = {Error}", userAccount, ex.Message);
            return true; // 發生錯誤時保守回傳已存在
        }
    }

    /// <summary>
    /// 檢查暱稱是否已存在
    /// </summary>
    public async Task<bool> IsNickNameExistsAsync(string nickName)
    {
        try
        {
            // 這裡需要實作暱稱唯一性檢查，暫時返回 false
            await Task.CompletedTask;
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查暱稱存在性時發生錯誤: NickName = {NickName}, Error = {Error}", nickName, ex.Message);
            return true;
        }
    }

    /// <summary>
    /// 檢查身分證字號是否已存在
    /// </summary>
    public async Task<bool> IsIdNumberExistsAsync(string idNumber)
    {
        try
        {
            // 這裡需要實作身分證字號唯一性檢查，暫時返回 false
            await Task.CompletedTask;
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查身分證字號存在性時發生錯誤: IdNumber = {IdNumber}, Error = {Error}", idNumber, ex.Message);
            return true;
        }
    }

    /// <summary>
    /// 檢查手機號碼是否已存在
    /// </summary>
    public async Task<bool> IsCellphoneExistsAsync(string cellphone)
    {
        try
        {
            // 這裡需要實作手機號碼唯一性檢查，暫時返回 false
            await Task.CompletedTask;
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查手機號碼存在性時發生錯誤: Cellphone = {Cellphone}, Error = {Error}", cellphone, ex.Message);
            return true;
        }
    }

    /// <summary>
    /// 檢查電子郵件是否已存在
    /// </summary>
    public async Task<bool> IsEmailExistsAsync(string email)
    {
        try
        {
            // 這裡需要實作電子郵件唯一性檢查，暫時返回 false
            await Task.CompletedTask;
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查電子郵件存在性時發生錯誤: Email = {Email}, Error = {Error}", email, ex.Message);
            return true;
        }
    }

    /// <summary>
    /// 密碼雜湊處理（與 AuthService 保持一致）
    /// </summary>
    private string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "GameCore_Salt_2024"));
        return Convert.ToBase64String(hashedBytes);
    }
}
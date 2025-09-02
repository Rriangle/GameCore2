using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;

namespace GameCore.Api.Services;

/// <summary>
/// 用戶管理服務 - 處理用戶資料管理、權限查詢等
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIntroduceRepository _userIntroduceRepository;
    private readonly IUserRightsRepository _userRightsRepository;
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IUserIntroduceRepository userIntroduceRepository,
        IUserRightsRepository userRightsRepository,
        IUserWalletRepository userWalletRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _userIntroduceRepository = userIntroduceRepository;
        _userRightsRepository = userRightsRepository;
        _userWalletRepository = userWalletRepository;
        _logger = logger;
    }

    /// <summary>
    /// 取得用戶完整資訊
    /// </summary>
    public async Task<ApiResponseDto<UserInfoDto>> GetUserInfoAsync(int userId)
    {
        try
        {
            _logger.LogInformation("取得用戶資訊：{UserId}", userId);

            // 取得用戶基本資料
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponseDto<UserInfoDto>
                {
                    Success = false,
                    Message = "用戶不存在"
                };
            }

            // 取得用戶介紹資料
            var userIntroduce = await _userIntroduceRepository.GetByUserIdAsync(userId);

            // 取得用戶權限
            var userRights = await _userRightsRepository.GetByUserIdAsync(userId);

            // 取得用戶錢包
            var userWallet = await _userWalletRepository.GetByUserIdAsync(userId);

            // 構建用戶資訊
            var userInfo = new UserInfoDto
            {
                UserID = user.User_ID,
                UserName = user.User_Name,
                UserAccount = user.User_Account,
                NickName = userIntroduce?.User_NickName,
                Email = userIntroduce?.Email,
                Cellphone = userIntroduce?.Cellphone,
                CreatedAt = user.Created_At,
                Rights = new UserRightsDto
                {
                    UserStatus = userRights?.User_Status ?? false,
                    ShoppingPermission = userRights?.ShoppingPermission ?? false,
                    MessagePermission = userRights?.MessagePermission ?? false,
                    SalesAuthority = userRights?.SalesAuthority ?? false
                },
                Wallet = new UserWalletDto
                {
                    UserPoint = userWallet?.User_Point ?? 0,
                    CouponNumber = userWallet?.Coupon_Number
                }
            };

            return new ApiResponseDto<UserInfoDto>
            {
                Success = true,
                Message = "取得用戶資訊成功",
                Data = userInfo
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶資訊時發生錯誤：{UserId}", userId);
            return new ApiResponseDto<UserInfoDto>
            {
                Success = false,
                Message = "取得用戶資訊失敗：" + ex.Message
            };
        }
    }

    /// <summary>
    /// 更新用戶個資
    /// </summary>
    public async Task<ApiResponseDto<bool>> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto)
    {
        try
        {
            _logger.LogInformation("更新用戶個資：{UserId}", userId);

            // 檢查用戶是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "用戶不存在"
                };
            }

            // 取得用戶介紹資料
            var userIntroduce = await _userIntroduceRepository.GetByUserIdAsync(userId);
            if (userIntroduce == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "用戶介紹資料不存在"
                };
            }

            // 檢查唯一性約束
            if (!string.IsNullOrEmpty(updateDto.Email))
            {
                var existingEmail = await _userRepository.GetByEmailAsync(updateDto.Email);
                if (existingEmail != null && existingEmail.User_ID != userId)
                {
                    return new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = "電子郵件已被其他用戶使用"
                    };
                }
            }

            if (!string.IsNullOrEmpty(updateDto.Cellphone))
            {
                var existingPhone = await _userRepository.GetByPhoneAsync(updateDto.Cellphone);
                if (existingPhone != null && existingPhone.User_ID != userId)
                {
                    return new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = "手機號碼已被其他用戶使用"
                    };
                }
            }

            if (!string.IsNullOrEmpty(updateDto.IdNumber))
            {
                var existingIdNumber = await _userIntroduceRepository.GetByIdNumberAsync(updateDto.IdNumber);
                if (existingIdNumber != null && existingIdNumber.User_ID != userId)
                {
                    return new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = "身分證字號已被其他用戶使用"
                    };
                }
            }

            // 更新用戶介紹資料
            if (!string.IsNullOrEmpty(updateDto.NickName))
                userIntroduce.User_NickName = updateDto.NickName;

            if (!string.IsNullOrEmpty(updateDto.Gender))
                userIntroduce.Gender = updateDto.Gender;

            if (!string.IsNullOrEmpty(updateDto.IdNumber))
                userIntroduce.IdNumber = updateDto.IdNumber;

            if (!string.IsNullOrEmpty(updateDto.Cellphone))
                userIntroduce.Cellphone = updateDto.Cellphone;

            if (!string.IsNullOrEmpty(updateDto.Email))
                userIntroduce.Email = updateDto.Email;

            if (!string.IsNullOrEmpty(updateDto.Address))
                userIntroduce.Address = updateDto.Address;

            if (updateDto.DateOfBirth.HasValue)
                userIntroduce.DateOfBirth = updateDto.DateOfBirth.Value;

            if (!string.IsNullOrEmpty(updateDto.UserIntroduce))
                userIntroduce.User_Introduce = updateDto.UserIntroduce;

            // 儲存更新
            await _userIntroduceRepository.UpdateAsync(userIntroduce);

            _logger.LogInformation("用戶個資更新成功：{UserId}", userId);

            return new ApiResponseDto<bool>
            {
                Success = true,
                Message = "用戶個資更新成功",
                Data = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用戶個資時發生錯誤：{UserId}", userId);
            return new ApiResponseDto<bool>
            {
                Success = false,
                Message = "更新用戶個資失敗：" + ex.Message
            };
        }
    }

    /// <summary>
    /// 取得用戶權限
    /// </summary>
    public async Task<ApiResponseDto<UserRightsDto>> GetUserRightsAsync(int userId)
    {
        try
        {
            _logger.LogInformation("取得用戶權限：{UserId}", userId);

            var userRights = await _userRightsRepository.GetByUserIdAsync(userId);
            if (userRights == null)
            {
                return new ApiResponseDto<UserRightsDto>
                {
                    Success = false,
                    Message = "用戶權限不存在"
                };
            }

            var rightsDto = new UserRightsDto
            {
                UserStatus = userRights.User_Status,
                ShoppingPermission = userRights.ShoppingPermission,
                MessagePermission = userRights.MessagePermission,
                SalesAuthority = userRights.SalesAuthority
            };

            return new ApiResponseDto<UserRightsDto>
            {
                Success = true,
                Message = "取得用戶權限成功",
                Data = rightsDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶權限時發生錯誤：{UserId}", userId);
            return new ApiResponseDto<UserRightsDto>
            {
                Success = false,
                Message = "取得用戶權限失敗：" + ex.Message
            };
        }
    }

    /// <summary>
    /// 取得用戶錢包
    /// </summary>
    public async Task<ApiResponseDto<UserWalletDto>> GetUserWalletAsync(int userId)
    {
        try
        {
            _logger.LogInformation("取得用戶錢包：{UserId}", userId);

            var userWallet = await _userWalletRepository.GetByUserIdAsync(userId);
            if (userWallet == null)
            {
                return new ApiResponseDto<UserWalletDto>
                {
                    Success = false,
                    Message = "用戶錢包不存在"
                };
            }

            var walletDto = new UserWalletDto
            {
                UserPoint = userWallet.User_Point,
                CouponNumber = userWallet.Coupon_Number
            };

            return new ApiResponseDto<UserWalletDto>
            {
                Success = true,
                Message = "取得用戶錢包成功",
                Data = walletDto
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶錢包時發生錯誤：{UserId}", userId);
            return new ApiResponseDto<UserWalletDto>
            {
                Success = false,
                Message = "取得用戶錢包失敗：" + ex.Message
            };
        }
    }

    /// <summary>
    /// 檢查用戶權限
    /// </summary>
    public async Task<bool> CheckUserPermissionAsync(int userId, string permission)
    {
        try
        {
            var userRights = await _userRightsRepository.GetByUserIdAsync(userId);
            if (userRights == null || !userRights.User_Status)
            {
                return false;
            }

            return permission switch
            {
                "shopping" => userRights.ShoppingPermission,
                "message" => userRights.MessagePermission,
                "sales" => userRights.SalesAuthority,
                _ => false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查用戶權限時發生錯誤：{UserId}, {Permission}", userId, permission);
            return false;
        }
    }

    /// <summary>
    /// 搜尋用戶
    /// </summary>
    public async Task<ApiResponseDto<List<UserInfoDto>>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 20)
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            _logger.LogInformation("搜尋用戶：{SearchTerm}, 頁面：{Page}, 大小：{PageSize}", searchTerm, page, pageSize);

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new ApiResponseDto<List<UserInfoDto>>
                {
                    Success = false,
                    Message = "搜尋關鍵字不能為空"
                };
            }

            // 這裡應該實現用戶搜尋邏輯
            // 目前先返回空結果，後續實現
            var users = new List<UserInfoDto>();

            return new ApiResponseDto<List<UserInfoDto>>
            {
                Success = true,
                Message = "搜尋完成",
                Data = users
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋用戶時發生錯誤：{SearchTerm}", searchTerm);
            return new ApiResponseDto<List<UserInfoDto>>
            {
                Success = false,
                Message = "搜尋用戶失敗：" + ex.Message
            };
        }
    }
} 
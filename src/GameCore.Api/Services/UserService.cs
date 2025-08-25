using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Services;

/// <summary>
/// 用戶服務實現
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIntroduceRepository _userIntroduceRepository;
    private readonly IUserRightsRepository _userRightsRepository;
    private readonly IMemberSalesProfileRepository _memberSalesProfileRepository;
    private readonly IUserSalesInformationRepository _userSalesInformationRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IUserIntroduceRepository userIntroduceRepository,
        IUserRightsRepository userRightsRepository,
        IMemberSalesProfileRepository memberSalesProfileRepository,
        IUserSalesInformationRepository userSalesInformationRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _userIntroduceRepository = userIntroduceRepository;
        _userRightsRepository = userRightsRepository;
        _memberSalesProfileRepository = memberSalesProfileRepository;
        _userSalesInformationRepository = userSalesInformationRepository;
        _logger = logger;
    }

    /// <summary>
    /// 更新用戶介紹
    /// </summary>
    public async Task<bool> UpdateUserIntroduceAsync(int userId, UpdateUserIntroduceDto request)
    {
        try
        {
            var existingUserIntroduce = await _userIntroduceRepository.GetByUserIdAsync(userId);
            if (existingUserIntroduce == null)
            {
                _logger.LogWarning("用戶介紹不存在，用戶ID: {UserId}", userId);
                return false;
            }

            // 更新用戶介紹資訊
            existingUserIntroduce.User_NickName = request.User_NickName;
            existingUserIntroduce.Gender = request.Gender;
            existingUserIntroduce.IdNumber = request.IdNumber;
            existingUserIntroduce.Cellphone = request.Cellphone;
            existingUserIntroduce.Email = request.Email;
            existingUserIntroduce.Address = request.Address;
            existingUserIntroduce.DateOfBirth = request.DateOfBirth;
            existingUserIntroduce.User_Introduce = request.User_Introduce;
            existingUserIntroduce.UpdatedAt = DateTime.UtcNow;

            var result = await _userIntroduceRepository.UpdateAsync(existingUserIntroduce);
            if (result)
            {
                _logger.LogInformation("用戶介紹更新成功，用戶ID: {UserId}", userId);
            }
            else
            {
                _logger.LogError("用戶介紹更新失敗，用戶ID: {UserId}", userId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用戶介紹時發生錯誤，用戶ID: {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// 獲取用戶介紹
    /// </summary>
    public async Task<UserIntroduceDto?> GetUserIntroduceAsync(int userId)
    {
        try
        {
            var userIntroduce = await _userIntroduceRepository.GetByUserIdAsync(userId);
            if (userIntroduce == null)
            {
                _logger.LogWarning("用戶介紹不存在，用戶ID: {UserId}", userId);
                return null;
            }

            return new UserIntroduceDto
            {
                User_ID = userIntroduce.User_ID,
                User_NickName = userIntroduce.User_NickName,
                Gender = userIntroduce.Gender,
                IdNumber = userIntroduce.IdNumber,
                Cellphone = userIntroduce.Cellphone,
                Email = userIntroduce.Email,
                Address = userIntroduce.Address,
                DateOfBirth = userIntroduce.DateOfBirth,
                User_Introduce = userIntroduce.User_Introduce,
                CreatedAt = userIntroduce.CreatedAt,
                UpdatedAt = userIntroduce.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶介紹時發生錯誤，用戶ID: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// 獲取用戶權限
    /// </summary>
    public async Task<UserRightsDto?> GetUserRightsAsync(int userId)
    {
        try
        {
            var userRights = await _userRightsRepository.GetByUserIdAsync(userId);
            if (userRights == null)
            {
                _logger.LogWarning("用戶權限不存在，用戶ID: {UserId}", userId);
                return null;
            }

            return new UserRightsDto
            {
                User_Id = userRights.User_Id,
                User_Status = userRights.User_Status,
                ShoppingPermission = userRights.ShoppingPermission,
                MessagePermission = userRights.MessagePermission,
                SalesAuthority = userRights.SalesAuthority,
                CreatedAt = userRights.CreatedAt,
                UpdatedAt = userRights.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶權限時發生錯誤，用戶ID: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// 申請銷售權限
    /// </summary>
    public async Task<SalesPermissionResponseDto> ApplySalesPermissionAsync(int userId, SalesPermissionRequestDto request)
    {
        try
        {
            // 檢查是否已經申請過
            var existingProfile = await _memberSalesProfileRepository.GetByUserIdAsync(userId);
            if (existingProfile != null)
            {
                _logger.LogWarning("用戶已經申請過銷售權限，用戶ID: {UserId}", userId);
                return new SalesPermissionResponseDto
                {
                    User_Id = userId,
                    BankCode = existingProfile.BankCode,
                    BankAccountNumber = existingProfile.BankAccountNumber,
                    Status = existingProfile.Status,
                    AppliedAt = existingProfile.AppliedAt,
                    ReviewedAt = existingProfile.ReviewedAt,
                    ReviewNotes = existingProfile.ReviewNotes
                };
            }

            // 創建新的銷售權限申請
            var newProfile = new MemberSalesProfile
            {
                User_Id = userId,
                BankCode = request.BankCode,
                BankAccountNumber = request.BankAccountNumber,
                Status = "Pending",
                AppliedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _memberSalesProfileRepository.AddAsync(newProfile);
            if (result != null)
            {
                _logger.LogInformation("銷售權限申請提交成功，用戶ID: {UserId}", userId);
                return new SalesPermissionResponseDto
                {
                    User_Id = result.User_Id,
                    BankCode = result.BankCode,
                    BankAccountNumber = result.BankAccountNumber,
                    Status = result.Status,
                    AppliedAt = result.AppliedAt
                };
            }
            else
            {
                _logger.LogError("銷售權限申請提交失敗，用戶ID: {UserId}", userId);
                throw new InvalidOperationException("銷售權限申請提交失敗");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "申請銷售權限時發生錯誤，用戶ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// 獲取銷售權限狀態
    /// </summary>
    public async Task<SalesPermissionResponseDto?> GetSalesPermissionStatusAsync(int userId)
    {
        try
        {
            var profile = await _memberSalesProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                _logger.LogWarning("銷售權限申請不存在，用戶ID: {UserId}", userId);
                return null;
            }

            return new SalesPermissionResponseDto
            {
                User_Id = profile.User_Id,
                BankCode = profile.BankCode,
                BankAccountNumber = profile.BankAccountNumber,
                Status = profile.Status,
                AppliedAt = profile.AppliedAt,
                ReviewedAt = profile.ReviewedAt,
                ReviewNotes = profile.ReviewNotes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取銷售權限狀態時發生錯誤，用戶ID: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// 獲取銷售錢包
    /// </summary>
    public async Task<SalesWalletDto?> GetSalesWalletAsync(int userId)
    {
        try
        {
            var salesInfo = await _userSalesInformationRepository.GetByUserIdAsync(userId);
            if (salesInfo == null)
            {
                _logger.LogWarning("銷售錢包不存在，用戶ID: {UserId}", userId);
                return null;
            }

            return new SalesWalletDto
            {
                User_Id = salesInfo.User_Id,
                UserSales_Wallet = salesInfo.UserSales_Wallet,
                CreatedAt = salesInfo.CreatedAt,
                UpdatedAt = salesInfo.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取銷售錢包時發生錯誤，用戶ID: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// 更新用戶頭像
    /// </summary>
    public async Task<bool> UpdateUserAvatarAsync(int userId, byte[] imageData)
    {
        try
        {
            var userIntroduce = await _userIntroduceRepository.GetByUserIdAsync(userId);
            if (userIntroduce == null)
            {
                _logger.LogWarning("用戶介紹不存在，無法更新頭像，用戶ID: {UserId}", userId);
                return false;
            }

            userIntroduce.User_Picture = imageData;
            userIntroduce.UpdatedAt = DateTime.UtcNow;

            var result = await _userIntroduceRepository.UpdateAsync(userIntroduce);
            if (result)
            {
                _logger.LogInformation("用戶頭像更新成功，用戶ID: {UserId}", userId);
            }
            else
            {
                _logger.LogError("用戶頭像更新失敗，用戶ID: {UserId}", userId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用戶頭像時發生錯誤，用戶ID: {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// 獲取用戶頭像
    /// </summary>
    public async Task<byte[]?> GetUserAvatarAsync(int userId)
    {
        try
        {
            var userIntroduce = await _userIntroduceRepository.GetByUserIdAsync(userId);
            if (userIntroduce == null)
            {
                _logger.LogWarning("用戶介紹不存在，無法獲取頭像，用戶ID: {UserId}", userId);
                return null;
            }

            return userIntroduce.User_Picture;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶頭像時發生錯誤，用戶ID: {UserId}", userId);
            return null;
        }
    }
}
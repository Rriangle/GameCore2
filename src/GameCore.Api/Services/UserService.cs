using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace GameCore.Api.Services;

/// <summary>
/// 用戶服務實現 - 優化版本
/// 增強安全性、輸入驗證、錯誤處理和性能
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIntroduceRepository _userIntroduceRepository;
    private readonly IUserRightsRepository _userRightsRepository;
    private readonly IMemberSalesProfileRepository _memberSalesProfileRepository;
    private readonly IUserSalesInformationRepository _userSalesInformationRepository;
    private readonly ILogger<UserService> _logger;

    // 常數定義，提高可維護性
    private const int NicknameMaxLength = 50;
    private const int GenderMaxLength = 10;
    private const int IdNumberMaxLength = 20;
    private const int PhoneMaxLength = 20;
    private const int AddressMaxLength = 200;
    private const int IntroduceMaxLength = 500;
    private const int MinAge = 13;
    private const int MaxAge = 120;

    // 正則表達式模式
    private static readonly Regex PhonePattern = new(@"^(\+?886|0)?9\d{8}$", RegexOptions.Compiled);
    private static readonly Regex IdNumberPattern = new(@"^[A-Z][12]\d{8}$", RegexOptions.Compiled);
    private static readonly Regex GenderPattern = new(@"^(男|女|其他|未指定)$", RegexOptions.Compiled);

    public UserService(
        IUserRepository userRepository,
        IUserIntroduceRepository userIntroduceRepository,
        IUserRightsRepository userRightsRepository,
        IMemberSalesProfileRepository memberSalesProfileRepository,
        IUserSalesInformationRepository userSalesInformationRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userIntroduceRepository = userIntroduceRepository ?? throw new ArgumentNullException(nameof(userIntroduceRepository));
        _userRightsRepository = userRightsRepository ?? throw new ArgumentNullException(nameof(userRightsRepository));
        _memberSalesProfileRepository = memberSalesProfileRepository ?? throw new ArgumentNullException(nameof(memberSalesProfileRepository));
        _userSalesInformationRepository = userSalesInformationRepository ?? throw new ArgumentNullException(nameof(userSalesInformationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 更新用戶介紹 - 優化版本
    /// </summary>
    public async Task<bool> UpdateUserIntroduceAsync(int userId, UpdateUserIntroduceDto request)
    {
        _logger.LogInformation("開始更新用戶介紹，用戶ID: {UserId}", userId);

        try
        {
            // 輸入驗證
            var validationResult = ValidateUserIntroduceRequest(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("用戶介紹更新請求驗證失敗: {UserId}, 錯誤: {Error}", userId, validationResult.ErrorMessage);
                return false;
            }

            // 檢查用戶是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("用戶不存在，用戶ID: {UserId}", userId);
                return false;
            }

            var existingUserIntroduce = await _userIntroduceRepository.GetByUserIdAsync(userId);
            if (existingUserIntroduce == null)
            {
                _logger.LogWarning("用戶介紹不存在，用戶ID: {UserId}", userId);
                return false;
            }

            // 更新用戶介紹資訊
            UpdateUserIntroduceFields(existingUserIntroduce, request);
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
    /// 獲取用戶介紹 - 優化版本
    /// </summary>
    public async Task<UserIntroduceDto?> GetUserIntroduceAsync(int userId)
    {
        _logger.LogInformation("開始獲取用戶介紹，用戶ID: {UserId}", userId);

        try
        {
            var userIntroduce = await _userIntroduceRepository.GetByUserIdAsync(userId);
            if (userIntroduce == null)
            {
                _logger.LogWarning("用戶介紹不存在，用戶ID: {UserId}", userId);
                return null;
            }

            var dto = MapToUserIntroduceDto(userIntroduce);
            _logger.LogInformation("成功獲取用戶介紹，用戶ID: {UserId}", userId);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶介紹時發生錯誤，用戶ID: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// 獲取用戶完整資料 - 優化版本
    /// </summary>
    public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
    {
        _logger.LogInformation("開始獲取用戶完整資料，用戶ID: {UserId}", userId);

        try
        {
            // 並行獲取用戶相關資料，提高性能
            var tasks = new[]
            {
                _userRepository.GetByIdAsync(userId),
                _userIntroduceRepository.GetByUserIdAsync(userId),
                _userRightsRepository.GetByUserIdAsync(userId),
                _userSalesInformationRepository.GetByUserIdAsync(userId)
            };

            var results = await Task.WhenAll(tasks);
            var user = results[0] as User;
            var userIntroduce = results[1] as UserIntroduce;
            var userRights = results[2] as UserRights;
            var userSalesInfo = results[3] as UserSalesInformation;

            if (user == null)
            {
                _logger.LogWarning("用戶不存在，用戶ID: {UserId}", userId);
                return null;
            }

            var profile = MapToUserProfileDto(user, userIntroduce, userRights, userSalesInfo);
            _logger.LogInformation("成功獲取用戶完整資料，用戶ID: {UserId}", userId);
            return profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶完整資料時發生錯誤，用戶ID: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// 更新用戶權限 - 優化版本
    /// </summary>
    public async Task<bool> UpdateUserRightsAsync(int userId, UpdateUserRightsDto request)
    {
        _logger.LogInformation("開始更新用戶權限，用戶ID: {UserId}", userId);

        try
        {
            // 輸入驗證
            var validationResult = ValidateUserRightsRequest(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("用戶權限更新請求驗證失敗: {UserId}, 錯誤: {Error}", userId, validationResult.ErrorMessage);
                return false;
            }

            var existingRights = await _userRightsRepository.GetByUserIdAsync(userId);
            if (existingRights == null)
            {
                _logger.LogWarning("用戶權限不存在，用戶ID: {UserId}", userId);
                return false;
            }

            // 更新權限
            UpdateUserRightsFields(existingRights, request);
            existingRights.UpdatedAt = DateTime.UtcNow;

            var result = await _userRightsRepository.UpdateAsync(existingRights);
            if (result)
            {
                _logger.LogInformation("用戶權限更新成功，用戶ID: {UserId}", userId);
            }
            else
            {
                _logger.LogError("用戶權限更新失敗，用戶ID: {UserId}", userId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用戶權限時發生錯誤，用戶ID: {UserId}", userId);
            return false;
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

    /// <summary>
    /// 驗證用戶介紹請求 - 新增方法，提高可讀性
    /// </summary>
    private ValidationResult ValidateUserIntroduceRequest(UpdateUserIntroduceDto request)
    {
        var result = new ValidationResult();

        // 檢查必填欄位
        if (string.IsNullOrWhiteSpace(request.User_NickName))
            result.AddError("暱稱不能為空");

        if (string.IsNullOrWhiteSpace(request.Gender))
            result.AddError("性別不能為空");

        if (string.IsNullOrWhiteSpace(request.IdNumber))
            result.AddError("身分證字號不能為空");

        if (string.IsNullOrWhiteSpace(request.Cellphone))
            result.AddError("手機號碼不能為空");

        if (string.IsNullOrWhiteSpace(request.Email))
            result.AddError("電子郵件不能為空");

        if (string.IsNullOrWhiteSpace(request.Address))
            result.AddError("地址不能為空");

        // 檢查長度限制
        if (request.User_NickName?.Length > NicknameMaxLength)
            result.AddError($"暱稱不能超過{NicknameMaxLength}個字符");

        if (request.Gender?.Length > GenderMaxLength)
            result.AddError($"性別不能超過{GenderMaxLength}個字符");

        if (request.IdNumber?.Length > IdNumberMaxLength)
            result.AddError($"身分證字號不能超過{IdNumberMaxLength}個字符");

        if (request.Cellphone?.Length > PhoneMaxLength)
            result.AddError($"手機號碼不能超過{PhoneMaxLength}個字符");

        if (request.Address?.Length > AddressMaxLength)
            result.AddError($"地址不能超過{AddressMaxLength}個字符");

        if (request.User_Introduce?.Length > IntroduceMaxLength)
            result.AddError($"用戶介紹不能超過{IntroduceMaxLength}個字符");

        // 檢查格式
        if (!string.IsNullOrEmpty(request.Gender) && !GenderPattern.IsMatch(request.Gender))
            result.AddError("性別格式不正確，請選擇：男、女、其他、未指定");

        if (!string.IsNullOrEmpty(request.IdNumber) && !IdNumberPattern.IsMatch(request.IdNumber))
            result.AddError("身分證字號格式不正確");

        if (!string.IsNullOrEmpty(request.Cellphone) && !PhonePattern.IsMatch(request.Cellphone))
            result.AddError("手機號碼格式不正確");

        if (!string.IsNullOrEmpty(request.Email) && !IsValidEmail(request.Email))
            result.AddError("電子郵件格式不正確");

        // 檢查年齡
        if (request.DateOfBirth.HasValue)
        {
            var age = DateTime.Today.Year - request.DateOfBirth.Value.Year;
            if (request.DateOfBirth.Value > DateTime.Today.AddYears(-age))
                age--;

            if (age < MinAge || age > MaxAge)
                result.AddError($"年齡必須在{MinAge}-{MaxAge}歲之間");
        }

        return result;
    }

    /// <summary>
    /// 驗證用戶權限請求 - 新增方法
    /// </summary>
    private ValidationResult ValidateUserRightsRequest(UpdateUserRightsDto request)
    {
        var result = new ValidationResult();

        // 檢查必填欄位
        if (request.User_Status == null)
            result.AddError("用戶狀態不能為空");

        if (request.ShoppingPermission == null)
            result.AddError("購物權限不能為空");

        if (request.MessagePermission == null)
            result.AddError("留言權限不能為空");

        if (request.SalesAuthority == null)
            result.AddError("銷售權限不能為空");

        return result;
    }

    /// <summary>
    /// 驗證電子郵件格式 - 新增方法
    /// </summary>
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 更新用戶介紹欄位 - 新增方法，提高可讀性
    /// </summary>
    private void UpdateUserIntroduceFields(UserIntroduce userIntroduce, UpdateUserIntroduceDto request)
    {
        userIntroduce.User_NickName = request.User_NickName?.Trim() ?? string.Empty;
        userIntroduce.Gender = request.Gender?.Trim() ?? string.Empty;
        userIntroduce.IdNumber = request.IdNumber?.Trim() ?? string.Empty;
        userIntroduce.Cellphone = request.Cellphone?.Trim() ?? string.Empty;
        userIntroduce.Email = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        userIntroduce.Address = request.Address?.Trim() ?? string.Empty;
        userIntroduce.DateOfBirth = request.DateOfBirth ?? DateTime.Today.AddYears(-18);
        userIntroduce.User_Introduce = request.User_Introduce?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// 更新用戶權限欄位 - 新增方法
    /// </summary>
    private void UpdateUserRightsFields(UserRights userRights, UpdateUserRightsDto request)
    {
        if (request.User_Status.HasValue)
            userRights.User_Status = request.User_Status.Value;
        if (request.ShoppingPermission.HasValue)
            userRights.ShoppingPermission = request.ShoppingPermission.Value;
        if (request.MessagePermission.HasValue)
            userRights.MessagePermission = request.MessagePermission.Value;
        if (request.SalesAuthority.HasValue)
            userRights.SalesAuthority = request.SalesAuthority.Value;
    }

    /// <summary>
    /// 映射到用戶介紹DTO - 新增方法
    /// </summary>
    private UserIntroduceDto MapToUserIntroduceDto(UserIntroduce userIntroduce)
    {
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
            User_Picture = userIntroduce.User_Picture,
            User_Introduce = userIntroduce.User_Introduce,
            Create_Account = userIntroduce.Create_Account
        };
    }

    /// <summary>
    /// 映射到用戶完整資料DTO - 新增方法
    /// </summary>
    private UserProfileDto MapToUserProfileDto(User user, UserIntroduce? userIntroduce, UserRights? userRights, UserSalesInformation? userSalesInfo)
    {
        return new UserProfileDto
        {
            User_ID = user.User_ID,
            User_name = user.User_name,
            User_Account = user.User_Account,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive,
            IsEmailVerified = user.IsEmailVerified,
            UserIntroduce = userIntroduce != null ? MapToUserIntroduceDto(userIntroduce) : null,
            UserRights = userRights != null ? new UserRightsDto
            {
                User_Id = userRights.User_Id,
                User_Status = userRights.User_Status,
                ShoppingPermission = userRights.ShoppingPermission,
                MessagePermission = userRights.MessagePermission,
                SalesAuthority = userRights.SalesAuthority,
                CreatedAt = userRights.CreatedAt,
                UpdatedAt = userRights.UpdatedAt
            } : null,
            UserSalesInfo = userSalesInfo != null ? new UserSalesInformationDto
            {
                User_Id = userSalesInfo.User_Id,
                UserSales_Wallet = userSalesInfo.UserSales_Wallet,
                CreatedAt = userSalesInfo.CreatedAt,
                UpdatedAt = userSalesInfo.UpdatedAt
            } : null
        };
    }
}
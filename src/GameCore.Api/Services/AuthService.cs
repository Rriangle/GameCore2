using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace GameCore.Api.Services;

/// <summary>
/// 認證服務 - 處理使用者註冊、登入和認證相關功能
/// 嚴格按照規格文件的使用者資料表結構實作
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRightsRepository _userRightsRepository;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IUserRightsRepository userRightsRepository,
        JwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _userRightsRepository = userRightsRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// 使用者註冊 - 按照規格建立完整的使用者資料
    /// </summary>
    public async Task<AuthResult> RegisterAsync(string userName, string userAccount, string password)
    {
        _logger.LogInformation("開始處理使用者註冊請求: UserAccount = {UserAccount}", userAccount);

        try
        {
            // 檢查使用者姓名是否已存在
            if (await _userRepository.ExistsByUserNameAsync(userName))
            {
                _logger.LogWarning("註冊失敗：使用者姓名已存在: {UserName}", userName);
                return new AuthResult { Success = false, ErrorMessage = "使用者姓名已存在" };
            }

            // 檢查登入帳號是否已存在
            if (await _userRepository.ExistsByUserAccountAsync(userAccount))
            {
                _logger.LogWarning("註冊失敗：登入帳號已存在: {UserAccount}", userAccount);
                return new AuthResult { Success = false, ErrorMessage = "登入帳號已存在" };
            }

            // 建立新使用者（UserRepository.CreateAsync 會自動建立權限和錢包）
            var user = new User
            {
                UserName = userName.Trim(),
                UserAccount = userAccount.Trim(),
                UserPassword = BC.HashPassword(password)
            };

            var createdUser = await _userRepository.CreateAsync(user);

            // 生成 JWT Token
            var token = _jwtService.GenerateToken(createdUser.UserId, createdUser.UserAccount);
            var expiresAt = DateTime.UtcNow.AddHours(24); // Token 有效期 24 小時

            _logger.LogInformation("使用者註冊成功: UserId = {UserId}, UserAccount = {UserAccount}", 
                createdUser.UserId, createdUser.UserAccount);

            return new AuthResult
            {
                Success = true,
                Token = token,
                User = createdUser,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "註冊過程中發生未預期的錯誤: UserAccount = {UserAccount}", userAccount);
            return new AuthResult { Success = false, ErrorMessage = "註冊失敗，請稍後再試" };
        }
    }

    /// <summary>
    /// 使用者登入 - 驗證帳號密碼和使用者狀態
    /// </summary>
    public async Task<AuthResult> LoginAsync(string userAccount, string password)
    {
        _logger.LogInformation("開始處理使用者登入請求: UserAccount = {UserAccount}", userAccount);

        try
        {
            // 查找使用者
            var user = await _userRepository.GetByUserAccountForLoginAsync(userAccount.Trim());
            if (user == null)
            {
                _logger.LogWarning("登入失敗：使用者不存在: {UserAccount}", userAccount);
                return new AuthResult { Success = false, ErrorMessage = "帳號或密碼錯誤" };
            }

            // 驗證密碼
            if (!BC.Verify(password, user.UserPassword))
            {
                _logger.LogWarning("登入失敗：密碼錯誤: {UserAccount}", userAccount);
                return new AuthResult { Success = false, ErrorMessage = "帳號或密碼錯誤" };
            }

            // 檢查使用者權限狀態
            var userRights = await _userRightsRepository.GetByUserIdAsync(user.UserId);
            if (userRights == null || !userRights.UserStatus)
            {
                _logger.LogWarning("登入失敗：帳戶已被停用: {UserId}", user.UserId);
                return new AuthResult { Success = false, ErrorMessage = "帳戶已被停用，請聯繫管理員" };
            }

            // 取得完整使用者資料（包含所有關聯資料）
            var completeUser = await _userRepository.GetCompleteUserDataAsync(user.UserId);
            if (completeUser == null)
            {
                _logger.LogError("無法取得完整使用者資料: {UserId}", user.UserId);
                return new AuthResult { Success = false, ErrorMessage = "登入失敗，請稍後再試" };
            }

            // 生成 JWT Token
            var token = _jwtService.GenerateToken(user.UserId, user.UserAccount);
            var expiresAt = DateTime.UtcNow.AddHours(24); // Token 有效期 24 小時

            _logger.LogInformation("使用者登入成功: UserId = {UserId}, UserAccount = {UserAccount}", 
                user.UserId, user.UserAccount);

            return new AuthResult
            {
                Success = true,
                Token = token,
                User = completeUser,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入過程中發生未預期的錯誤: UserAccount = {UserAccount}", userAccount);
            return new AuthResult { Success = false, ErrorMessage = "登入失敗，請稍後再試" };
        }
    }

    /// <summary>
    /// 取得使用者基本資料
    /// </summary>
    public async Task<User?> GetUserProfileAsync(int userId)
    {
        _logger.LogDebug("獲取使用者資料: {UserId}", userId);

        try
        {
            var user = await _userRepository.GetCompleteUserDataAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("使用者不存在: {UserId}", userId);
                return null;
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取使用者資料時發生錯誤: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// 驗證使用者權限
    /// </summary>
    public async Task<bool> ValidateUserPermissionAsync(int userId, UserPermissionType permission)
    {
        try
        {
            var userRights = await _userRightsRepository.GetByUserIdAsync(userId);
            if (userRights == null)
            {
                _logger.LogWarning("使用者權限不存在: {UserId}", userId);
                return false;
            }

            return permission switch
            {
                UserPermissionType.UserStatus => userRights.UserStatus,
                UserPermissionType.Shopping => userRights.ShoppingPermission,
                UserPermissionType.Message => userRights.MessagePermission,
                UserPermissionType.Sales => userRights.SalesAuthority,
                _ => false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "驗證使用者權限時發生錯誤: {UserId}, {Permission}", userId, permission);
            return false;
        }
    }
}

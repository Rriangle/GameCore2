using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 身份驗證服務實作類別，處理用戶註冊、登入、密碼驗證等功能
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserWalletRepository _walletRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IUserWalletRepository walletRepository,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// 用戶註冊功能
    /// </summary>
    public async Task<(bool Success, string Message, LoginResponseDto? Response)> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            _logger.LogInformation("開始用戶註冊流程: User_name = {UserName}, User_Account = {UserAccount}", 
                request.User_name, request.User_Account);

            // 檢查用戶姓名是否已存在
            if (!await _userRepository.IsUsernameUniqueAsync(request.User_name))
            {
                _logger.LogWarning("註冊失敗：用戶姓名已存在 - {UserName}", request.User_name);
                return (false, "用戶姓名已存在", null);
            }

            // 檢查登入帳號是否已存在
            if (!await _userRepository.IsAccountUniqueAsync(request.User_Account))
            {
                _logger.LogWarning("註冊失敗：登入帳號已存在 - {UserAccount}", request.User_Account);
                return (false, "登入帳號已存在", null);
            }

            // 建立用戶資料
            var user = new User
            {
                User_name = request.User_name,
                User_Account = request.User_Account,
                User_Password = HashPassword(request.User_Password)
            };

            // 儲存用戶
            var createdUser = await _userRepository.CreateAsync(user);

            // 建立用戶錢包（初始點數為0）
            var wallet = new UserWallet
            {
                User_Id = createdUser.User_ID,
                User_Point = 0,
                Coupon_Number = null
            };
            await _walletRepository.CreateAsync(wallet);

            // 產生 JWT Token
            var token = _jwtService.GenerateAccessToken(createdUser.User_ID, createdUser.User_Account);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var response = new LoginResponseDto
            {
                User_ID = createdUser.User_ID,
                User_name = createdUser.User_name,
                User_Account = createdUser.User_Account,
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                TokenType = "Bearer"
            };

            _logger.LogInformation("用戶註冊成功: User_ID = {UserId}", createdUser.User_ID);
            return (true, "註冊成功", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用戶註冊過程中發生錯誤: {Message}", ex.Message);
            return (false, "註冊過程中發生錯誤，請稍後再試", null);
        }
    }

    /// <summary>
    /// 用戶登入功能
    /// </summary>
    public async Task<(bool Success, string Message, LoginResponseDto? Response)> LoginAsync(LoginRequestDto request)
    {
        try
        {
            _logger.LogInformation("用戶登入嘗試: User_Account = {UserAccount}", request.User_Account);

            // 驗證憑證
            var passwordHash = HashPassword(request.User_Password);
            var user = await _userRepository.ValidateCredentialsAsync(request.User_Account, passwordHash);

            if (user == null)
            {
                _logger.LogWarning("登入失敗：無效的帳號或密碼 - {UserAccount}", request.User_Account);
                return (false, "帳號或密碼錯誤", null);
            }

            // 產生 JWT Token
            var token = _jwtService.GenerateAccessToken(user.User_ID, user.User_Account);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var response = new LoginResponseDto
            {
                User_ID = user.User_ID,
                User_name = user.User_name,
                User_Account = user.User_Account,
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                TokenType = "Bearer"
            };

            _logger.LogInformation("用戶登入成功: User_ID = {UserId}", user.User_ID);
            return (true, "登入成功", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用戶登入過程中發生錯誤: {Message}", ex.Message);
            return (false, "登入過程中發生錯誤，請稍後再試", null);
        }
    }

    /// <summary>
    /// OAuth 登入功能（Google、Facebook、Discord）
    /// </summary>
    public async Task<(bool Success, string Message, LoginResponseDto? Response)> OAuthLoginAsync(OAuthLoginRequestDto request)
    {
        try
        {
            _logger.LogInformation("OAuth 登入嘗試: Provider = {Provider}, Email = {Email}", 
                request.Provider, request.Email);

            // 根據 OAuth 提供者和 Email 查找用戶
            var existingUser = await _userRepository.GetByAccountAsync(request.Email);

            if (existingUser != null)
            {
                // 用戶已存在，直接登入
                var token = _jwtService.GenerateAccessToken(existingUser.User_ID, existingUser.User_Account);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var loginResponse = new LoginResponseDto
                {
                    User_ID = existingUser.User_ID,
                    User_name = existingUser.User_name,
                    User_Account = existingUser.User_Account,
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    TokenType = "Bearer"
                };

                _logger.LogInformation("OAuth 登入成功（既有用戶）: User_ID = {UserId}", existingUser.User_ID);
                return (true, "登入成功", loginResponse);
            }
            else
            {
                // 新用戶，建立帳號
                var user = new User
                {
                    User_name = request.Name ?? request.Email.Split('@')[0],
                    User_Account = request.Email,
                    User_Password = HashPassword(Guid.NewGuid().ToString()) // OAuth 用戶使用隨機密碼
                };

                var createdUser = await _userRepository.CreateAsync(user);

                // 建立錢包
                var wallet = new UserWallet
                {
                    User_Id = createdUser.User_ID,
                    User_Point = 0
                };
                await _walletRepository.CreateAsync(wallet);

                var token = _jwtService.GenerateAccessToken(createdUser.User_ID, createdUser.User_Account);
                var refreshToken = _jwtService.GenerateRefreshToken();

                var response = new LoginResponseDto
                {
                    User_ID = createdUser.User_ID,
                    User_name = createdUser.User_name,
                    User_Account = createdUser.User_Account,
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    TokenType = "Bearer"
                };

                _logger.LogInformation("OAuth 登入成功（新用戶）: User_ID = {UserId}", createdUser.User_ID);
                return (true, "登入成功", response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OAuth 登入過程中發生錯誤: {Message}", ex.Message);
            return (false, "OAuth 登入失敗，請稍後再試", null);
        }
    }

    /// <summary>
    /// 重新整理 Token
    /// </summary>
    public async Task<(bool Success, string Message, LoginResponseDto? Response)> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // 驗證 Refresh Token
            var (isValid, principal) = _jwtService.ValidateToken(refreshToken);
            
            if (!isValid || principal == null)
            {
                return (false, "無效的 Refresh Token", null);
            }

            var userIdClaim = principal.FindFirst("sub")?.Value;
            var userAccountClaim = principal.FindFirst("unique_name")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userAccountClaim))
            {
                return (false, "Token 資訊不完整", null);
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return (false, "Token 格式錯誤", null);
            }

            // 重新產生 Token
            var newToken = _jwtService.GenerateAccessToken(userId, userAccountClaim);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, "用戶不存在", null);
            }

            var response = new LoginResponseDto
            {
                User_ID = user.User_ID,
                User_name = user.User_name,
                User_Account = user.User_Account,
                AccessToken = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                TokenType = "Bearer"
            };

            return (true, "Token 重新整理成功", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重新整理 Token 過程中發生錯誤: {Message}", ex.Message);
            return (false, "Token 重新整理失敗", null);
        }
    }

    /// <summary>
    /// 登出功能
    /// </summary>
    public async Task<bool> LogoutAsync(int userId)
    {
        try
        {
            _logger.LogInformation("用戶登出: User_ID = {UserId}", userId);
            
            // 在實際應用中，可能需要將 JWT 加入黑名單或撤銷 Refresh Token
            // 這裡暫時只記錄日誌
            
            await Task.CompletedTask; // 避免編譯器警告
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用戶登出時發生錯誤: User_ID = {UserId}, Error = {Error}", userId, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 驗證密碼
    /// </summary>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        var hash = HashPassword(password);
        return hash == hashedPassword;
    }

    /// <summary>
    /// 密碼雜湊處理
    /// </summary>
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "GameCore_Salt_2024"));
        return Convert.ToBase64String(hashedBytes);
    }
}
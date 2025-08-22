using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace GameCore.Api.Services;

/// <summary>
/// 認證服務，處理用戶註冊、登入和個人資料管理
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserWalletRepository _walletRepository;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IUserWalletRepository walletRepository,
        JwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(string username, string email, string password)
    {
        _logger.LogInformation("開始處理用戶註冊請求: {Username}", username);

        try
        {
            // 檢查用戶名是否已存在
            if (await _userRepository.ExistsByUsernameAsync(username))
            {
                _logger.LogWarning("註冊失敗：用戶名已存在: {Username}", username);
                return new AuthResult { Success = false, ErrorMessage = "用戶名已存在" };
            }

            // 檢查郵箱是否已存在
            if (await _userRepository.ExistsByEmailAsync(email))
            {
                _logger.LogWarning("註冊失敗：郵箱已被註冊: {Email}", email);
                return new AuthResult { Success = false, ErrorMessage = "郵箱已被註冊" };
            }

            // 建立新用戶
            var user = new User
            {
                Username = username.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = BC.HashPassword(password),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdUser = await _userRepository.CreateAsync(user);

            // 建立用戶錢包
            var wallet = new UserWallet
            {
                UserId = createdUser.UserId,
                Balance = 100.00m, // 初始點數
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _walletRepository.CreateAsync(wallet);

            // 生成 JWT Token
            var token = _jwtService.GenerateToken(createdUser.UserId, createdUser.Username, createdUser.Email);

            _logger.LogInformation("用戶註冊成功: {UserId} {Username}", createdUser.UserId, createdUser.Username);

            return new AuthResult
            {
                Success = true,
                Token = token,
                User = new UserProfile
                {
                    UserId = createdUser.UserId,
                    Username = createdUser.Username,
                    Email = createdUser.Email,
                    Balance = wallet.Balance,
                    CreatedAt = createdUser.CreatedAt,
                    LastLoginAt = createdUser.LastLoginAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "註冊過程中發生未預期的錯誤: {Username}", username);
            return new AuthResult { Success = false, ErrorMessage = "註冊失敗，請稍後再試" };
        }
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        _logger.LogInformation("開始處理用戶登入請求: {Username}", username);

        try
        {
            // 查找用戶
            var user = await _userRepository.GetByUsernameAsync(username.Trim());
            if (user == null)
            {
                _logger.LogWarning("登入失敗：用戶不存在: {Username}", username);
                return new AuthResult { Success = false, ErrorMessage = "用戶名或密碼錯誤" };
            }

            // 驗證密碼
            if (!BC.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("登入失敗：密碼錯誤: {Username}", username);
                return new AuthResult { Success = false, ErrorMessage = "用戶名或密碼錯誤" };
            }

            // 檢查用戶是否啟用
            if (!user.IsActive)
            {
                _logger.LogWarning("登入失敗：帳戶已被停用: {UserId}", user.UserId);
                return new AuthResult { Success = false, ErrorMessage = "帳戶已被停用" };
            }

            // 更新最後登入時間
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // 獲取錢包餘額
            var balance = await _walletRepository.GetBalanceAsync(user.UserId);

            // 生成 JWT Token
            var token = _jwtService.GenerateToken(user.UserId, user.Username, user.Email);

            _logger.LogInformation("用戶登入成功: {UserId} {Username}", user.UserId, user.Username);

            return new AuthResult
            {
                Success = true,
                Token = token,
                User = new UserProfile
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Balance = balance,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入過程中發生未預期的錯誤: {Username}", username);
            return new AuthResult { Success = false, ErrorMessage = "登入失敗，請稍後再試" };
        }
    }

    public async Task<UserProfile?> GetUserProfileAsync(int userId)
    {
        _logger.LogDebug("獲取用戶資料: {UserId}", userId);

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("用戶不存在: {UserId}", userId);
                return null;
            }

            var balance = await _walletRepository.GetBalanceAsync(userId);

            return new UserProfile
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Balance = balance,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶資料時發生錯誤: {UserId}", userId);
            return null;
        }
    }
}

using GameCore.Api.DTOs;
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

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        _logger.LogInformation("開始處理用戶註冊請求: {Username}", request.Username);

        try
        {
            // 檢查用戶名是否已存在
            if (await _userRepository.ExistsByUsernameAsync(request.Username))
            {
                _logger.LogWarning("註冊失敗：用戶名已存在: {Username}", request.Username);
                return CreateErrorResponse("用戶名已存在");
            }

            // 檢查郵箱是否已存在
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                _logger.LogWarning("註冊失敗：郵箱已被註冊: {Email}", request.Email);
                return CreateErrorResponse("郵箱已被註冊");
            }

            // 建立新用戶
            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                PasswordHash = BC.HashPassword(request.Password),
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

            return CreateSuccessResponse(token, new UserProfileDto
            {
                UserId = createdUser.UserId,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Balance = wallet.Balance,
                CreatedAt = createdUser.CreatedAt,
                LastLoginAt = createdUser.LastLoginAt
            }, "註冊成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "註冊過程中發生未預期的錯誤: {Username}", request.Username);
            return CreateErrorResponse("註冊失敗，請稍後再試");
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        _logger.LogInformation("開始處理用戶登入請求: {Username}", request.Username);

        try
        {
            // 查找用戶
            var user = await _userRepository.GetByUsernameAsync(request.Username.Trim());
            if (user == null)
            {
                _logger.LogWarning("登入失敗：用戶不存在: {Username}", request.Username);
                return CreateErrorResponse("用戶名或密碼錯誤");
            }

            // 驗證密碼
            if (!BC.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("登入失敗：密碼錯誤: {Username}", request.Username);
                return CreateErrorResponse("用戶名或密碼錯誤");
            }

            // 檢查用戶是否啟用
            if (!user.IsActive)
            {
                _logger.LogWarning("登入失敗：帳戶已被停用: {UserId}", user.UserId);
                return CreateErrorResponse("帳戶已被停用");
            }

            // 更新最後登入時間
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // 獲取錢包餘額
            var balance = await _walletRepository.GetBalanceAsync(user.UserId);

            // 生成 JWT Token
            var token = _jwtService.GenerateToken(user.UserId, user.Username, user.Email);

            _logger.LogInformation("用戶登入成功: {UserId} {Username}", user.UserId, user.Username);

            return CreateSuccessResponse(token, new UserProfileDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Balance = balance,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            }, "登入成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入過程中發生未預期的錯誤: {Username}", request.Username);
            return CreateErrorResponse("登入失敗，請稍後再試");
        }
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
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

            return new UserProfileDto
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

    private static AuthResponseDto CreateSuccessResponse(string token, UserProfileDto user, string message)
    {
        return new AuthResponseDto
        {
            Success = true,
            Token = token,
            Message = message,
            User = user
        };
    }

    private static AuthResponseDto CreateErrorResponse(string message)
    {
        return new AuthResponseDto
        {
            Success = false,
            Message = message
        };
    }
}

using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using GameCore.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

namespace GameCore.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuthService> _logger;
        private readonly SemaphoreSlim _loginSemaphore = new SemaphoreSlim(1, 1);

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            IMemoryCache cache,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AuthResult> RegisterAsync(string username, string email, string password)
        {
            try
            {
                // 輸入驗證
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    return AuthResult.Failure("所有欄位都是必填的");
                }

                if (ContainsSqlInjection(username) || ContainsSqlInjection(email))
                {
                    return AuthResult.Failure("輸入包含無效字符");
                }

                if (ContainsXss(username) || ContainsXss(email))
                {
                    return AuthResult.Failure("輸入包含無效字符");
                }

                if (!IsValidPassword(password))
                {
                    return AuthResult.Failure("密碼不符合安全要求");
                }

                // 檢查用戶名和郵箱唯一性
                var existingUserByUsername = await _userRepository.GetByUsernameAsync(username);
                var existingUserByEmail = await _userRepository.GetByEmailAsync(email);
                if (existingUserByUsername != null || existingUserByEmail != null)
                {
                    return AuthResult.Failure("用戶名或郵箱已存在");
                }

                // 創建新用戶
                var user = new User
                {
                    Username = username.Trim().ToLowerInvariant(),
                    Email = email.Trim().ToLowerInvariant(),
                    PasswordHash = HashPassword(password),
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow,
                    IsActive = true,
                    Role = GameCore.Domain.Enums.UserRole.User
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                // 生成 JWT Token
                var token = _jwtService.GenerateToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // 儲存 refresh token
                await _userRepository.UpdateRefreshTokenAsync(user.UserId, refreshToken, DateTime.UtcNow.AddDays(7));

                _logger.LogInformation("新用戶註冊成功: {Username}", user.Username);

                var userDto = new UserProfileDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Balance = user.Balance,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                };

                return AuthResult.SuccessResult(token, refreshToken, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用戶註冊失敗: {Username}", username);
                return AuthResult.Failure("註冊失敗，請稍後再試");
            }
        }

        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            var cacheKey = $"login_attempts_{username?.ToLowerInvariant()}";

            try
            {
                await _loginSemaphore.WaitAsync();

                // 檢查登入嘗試次數
                var attempts = await GetLoginAttemptsAsync(cacheKey);
                if (attempts >= 5)
                {
                    _logger.LogWarning("登入嘗試次數過多: {Username}, 嘗試次數: {Attempts}", username, attempts);
                    return AuthResult.Failure("登入嘗試次數過多，請稍後再試");
                }

                // 輸入驗證
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    await IncrementLoginAttemptsAsync(cacheKey);
                    return AuthResult.Failure("用戶名和密碼不能為空");
                }

                // 防止 SQL 注入和 XSS 攻擊
                if (ContainsSqlInjection(username) || ContainsXss(username))
                {
                    _logger.LogWarning("檢測到惡意輸入嘗試: {Username}", username);
                    await IncrementLoginAttemptsAsync(cacheKey);
                    return AuthResult.Failure("無效的輸入格式");
                }

                // 查找用戶
                var user = await _userRepository.GetByUsernameAsync(username.Trim().ToLowerInvariant());
                if (user == null)
                {
                    await IncrementLoginAttemptsAsync(cacheKey);
                    return AuthResult.Failure("用戶名或密碼錯誤");
                }

                // 檢查用戶狀態
                if (!user.IsActive)
                {
                    _logger.LogWarning("停用帳戶嘗試登入: {Username}", user.Username);
                    await IncrementLoginAttemptsAsync(cacheKey);
                    return AuthResult.Failure("帳戶已被停用");
                }

                // 驗證密碼
                if (!VerifyPassword(password, user.PasswordHash))
                {
                    await IncrementLoginAttemptsAsync(cacheKey);
                    return AuthResult.Failure("用戶名或密碼錯誤");
                }

                // 清除登入嘗試記錄
                await ClearLoginAttemptsAsync(cacheKey);

                // 更新最後登入時間
                user.LastLoginAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                // 生成新的 JWT Token
                var token = _jwtService.GenerateToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // 儲存 refresh token
                await _userRepository.UpdateRefreshTokenAsync(user.UserId, refreshToken, DateTime.UtcNow.AddDays(7));

                _logger.LogInformation("用戶登入成功: {Username}", user.Username);

                var userDto = new UserProfileDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Balance = user.Balance,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                };

                return AuthResult.SuccessResult(token, refreshToken, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用戶登入失敗: {Username}", username);
                await IncrementLoginAttemptsAsync(cacheKey);
                return AuthResult.Failure("登入失敗，請稍後再試");
            }
            finally
            {
                _loginSemaphore.Release();
            }
        }

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return AuthResult.Failure("Refresh token 不能為空");
                }

                var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
                if (user == null)
                {
                    return AuthResult.Failure("無效的 refresh token");
                }

                if (user.RefreshTokenExpiry < DateTime.UtcNow)
                {
                    return AuthResult.Failure("Refresh token 已過期");
                }

                // 生成新的 token
                var newToken = _jwtService.GenerateToken(user);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                // 更新 refresh token
                await _userRepository.UpdateRefreshTokenAsync(user.UserId, newRefreshToken, DateTime.UtcNow.AddDays(7));

                var userDto = new UserProfileDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Balance = user.Balance,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                };

                return AuthResult.SuccessResult(newToken, newRefreshToken, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token 刷新失敗");
                return AuthResult.Failure("Token 刷新失敗");
            }
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            try
            {
                await _userRepository.ClearRefreshTokenAsync(userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用戶登出失敗: {UserId}", userId);
                return false;
            }
        }



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

        private bool HasStrongPassword(string password)
        {
            // 密碼強度要求：
            // 1. 至少一個大寫字母
            // 2. 至少一個小寫字母
            // 3. 至少一個數字
            // 4. 至少一個特殊字符
            // 5. 不能包含常見弱密碼模式
            if (string.IsNullOrWhiteSpace(password))
                return false;

            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            // 檢查常見弱密碼模式
            var weakPatterns = new[] { "password", "123456", "qwerty", "admin", "user" };
            var isWeakPattern = weakPatterns.Any(pattern =>
                password.ToLowerInvariant().Contains(pattern));

            return hasUpper && hasLower && hasDigit && hasSpecial && !isWeakPattern;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // 改進密碼驗證
        private bool VerifyPassword(string password, string hash)
        {
            try
            {
                using var sha256 = SHA256.Create();
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = sha256.ComputeHash(passwordBytes);
                var computedHash = Convert.ToBase64String(hashBytes);

                return computedHash == hash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "密碼驗證失敗");
                return false;
            }
        }

        private async Task<int> GetLoginAttemptsAsync(string cacheKey)
        {
            return await _cache.GetOrCreateAsync(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return Task.FromResult(0);
            });
        }

        private async Task IncrementLoginAttemptsAsync(string cacheKey)
        {
            var attempts = await GetLoginAttemptsAsync(cacheKey);
            _cache.Set(cacheKey, attempts + 1, TimeSpan.FromMinutes(15));
        }

        private Task ClearLoginAttemptsAsync(string cacheKey)
        {
            _cache.Remove(cacheKey);
            return Task.CompletedTask;
        }

        private bool IsValidPassword(string password)
        {
            // 密碼驗證邏輯：
            // 1. 至少 8 個字符
            // 2. 至少包含一個大寫字母
            // 3. 至少包含一個小寫字母
            // 4. 至少包含一個數字
            // 5. 至少包含一個特殊字符
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            var hasUpperCase = password.Any(char.IsUpper);
            var hasLowerCase = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }

        // 新增安全驗證方法
        private bool ContainsSqlInjection(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            var sqlKeywords = new[] { "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER", "EXEC", "EXECUTE", "UNION", "OR", "AND" };
            var upperInput = input.ToUpperInvariant();

            return sqlKeywords.Any(keyword => upperInput.Contains(keyword));
        }

        private bool ContainsXss(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            var xssPatterns = new[] { "<script", "javascript:", "onload=", "onerror=", "onclick=" };
            var lowerInput = input.ToLowerInvariant();

            return xssPatterns.Any(pattern => lowerInput.Contains(pattern));
        }

        public async Task<AuthResult> GetUserProfileAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return AuthResult.Failure("用戶不存在");
                }

                var userDto = new UserProfileDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Balance = user.Balance,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                };

                return AuthResult.SuccessResult("", "", userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶資料失敗: {UserId}", userId);
                return AuthResult.Failure("獲取用戶資料失敗");
            }
        }
    }
}

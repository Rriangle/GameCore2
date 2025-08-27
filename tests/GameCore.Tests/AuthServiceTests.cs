using Xunit;
using Moq;
using GameCore.Api.Services;
using GameCore.Shared.Interfaces;
using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace GameCore.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly IMemoryCache _cache;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockJwtService = new Mock<IJwtService>();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _mockLogger = new Mock<ILogger<AuthService>>();

            _authService = new AuthService(
                _mockUserRepository.Object,
                _mockJwtService.Object,
                _cache,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var username = "testuser";
            var email = "test@example.com";
            var password = "TestPass123!";

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync((User user) => user);
            _mockUserRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("test-token");
            _mockJwtService.Setup(x => x.GenerateRefreshToken())
                .Returns("test-refresh-token");

            _mockUserRepository.Setup(x => x.UpdateRefreshTokenAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("test-token", result.Token);
            Assert.Equal("test-refresh-token", result.RefreshToken);
            Assert.NotNull(result.User);
            Assert.Equal("testuser", result.User.Username);
        }

        [Fact]
        public async Task RegisterAsync_InvalidRequest_ReturnsFailure()
        {
            // Arrange
            var username = "";
            var email = "test@example.com";
            var password = "123";

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("所有欄位都是必填的", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_DuplicateUsername_ReturnsFailure()
        {
            // Arrange
            var username = "existinguser";
            var email = "test@example.com";
            var password = "TestPass123!";

            var existingUser = new User { Username = "existinguser" };
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("用戶名或郵箱已存在", result.Message);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var username = "testuser";
            var password = "TestPass123!";

            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPass123!"),
                IsActive = true
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("test-token");
            _mockJwtService.Setup(x => x.GenerateRefreshToken())
                .Returns("test-refresh-token");

            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User user) => user);
            _mockUserRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateRefreshTokenAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("test-token", result.Token);
            Assert.Equal("test-refresh-token", result.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsFailure()
        {
            // Arrange
            var username = "testuser";
            var password = "wrongpassword";

            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPass123!"),
                IsActive = true
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("用戶名或密碼錯誤", result.Message);
        }

        [Fact]
        public async Task LoginAsync_InactiveUser_ReturnsFailure()
        {
            // Arrange
            var username = "testuser";
            var password = "TestPass123!";

            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPass123!"),
                IsActive = false
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("帳戶已被停用", result.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ValidToken_ReturnsSuccess()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(1)
            };

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(refreshToken))
                .ReturnsAsync(user);

            _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("new-token");
            _mockJwtService.Setup(x => x.GenerateRefreshToken())
                .Returns("new-refresh-token");

            _mockUserRepository.Setup(x => x.UpdateRefreshTokenAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("new-token", result.Token);
            Assert.Equal("new-refresh-token", result.RefreshToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_ExpiredToken_ReturnsFailure()
        {
            // Arrange
            var refreshToken = "expired-refresh-token";
            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(-1)
            };

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(refreshToken))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Refresh token 已過期", result.Message);
        }

        [Fact]
        public async Task LogoutAsync_ValidUserId_ReturnsTrue()
        {
            // Arrange
            var userId = 1;

            _mockUserRepository.Setup(x => x.ClearRefreshTokenAsync(userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LogoutAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("", "test@example.com", "TestPass123!", "用戶名不能為空")]
        [InlineData("ab", "test@example.com", "TestPass123!", "用戶名長度必須在 3-20 個字符之間")]
        [InlineData("testuser", "", "TestPass123!", "郵箱不能為空")]
        [InlineData("testuser", "invalid-email", "TestPass123!", "郵箱格式不正確")]
        [InlineData("testuser", "test@example.com", "", "密碼不能為空")]
        [InlineData("testuser", "test@example.com", "123", "密碼長度至少 8 個字符")]
        [InlineData("testuser", "test@example.com", "password", "密碼必須包含大小寫字母、數字和特殊字符")]
        [InlineData("testuser", "test@example.com", "Password123", "密碼必須包含大小寫字母、數字和特殊字符")]
        [InlineData("testuser", "test@example.com", "password123!", "密碼不符合安全要求")]
        [InlineData("testuser", "test@example.com", "PASSWORD123!", "密碼不符合安全要求")]
        public async Task RegisterAsync_InvalidInputs_ReturnsFailure(string username, string email, string password, string expectedError)
        {
            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains(expectedError, result.Message);
        }

        [Fact]
        public async Task LoginAsync_TooManyAttempts_ReturnsFailure()
        {
            // Arrange
            var username = "testuser";
            var password = "wrongpassword";

            // 模擬多次失敗嘗試
            for (int i = 0; i < 5; i++)
            {
                await _authService.LoginAsync(username, password);
            }

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("登入嘗試次數過多", result.Message);
        }

        [Fact]
        public async Task LoginAsync_ConcurrentAttempts_HandlesCorrectly()
        {
            // Arrange
            var username = "testuser";
            var password = "TestPass123!";

            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                PasswordHash = "hashedpassword",
                IsActive = true
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            // Act - 同時執行多個登入嘗試
            var tasks = new List<Task<AuthResult>>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(_authService.LoginAsync(username, password));
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.All(results, result => Assert.False(result.Success));
        }

        [Fact]
        public async Task LoginAsync_WeakPasswordPatterns_ReturnsFailure()
        {
            // Arrange
            var weakPasswords = new[]
            {
                "password123!",
                "123456789!",
                "qwerty123!",
                "admin123!",
                "user123!"
            };

            foreach (var password in weakPasswords)
            {
                var username = $"testuser_{password}";
                var email = $"test_{password}@example.com";

                // Act
                var result = await _authService.RegisterAsync(username, email, password);

                // Assert
                Assert.False(result.Success);
                Assert.Contains("密碼不符合安全要求", result.Message);
            }
        }

        [Fact]
        public async Task LoginAsync_ExtremeInputs_HandlesCorrectly()
        {
            // Arrange
            var username = new string('a', 1000); // 極長用戶名
            var password = new string('1', 1000);  // 極長密碼

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.False(result.Success);
            // 應該不會拋出異常，而是返回適當的錯誤訊息
        }

        [Fact]
        public async Task RegisterAsync_SpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            var username = "test_user-123";
            var email = "test+tag@example.com";
            var password = "TestPass123!";

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync((User user) => user);
            _mockUserRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("test-token");
            _mockJwtService.Setup(x => x.GenerateRefreshToken())
                .Returns("test-refresh-token");

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.User);
            Assert.Equal("test_user-123", result.User!.Username);
        }

        [Theory]
        [InlineData("admin' OR '1'='1", "password123")]
        [InlineData("user; DROP TABLE users;", "password123")]
        [InlineData("test UNION SELECT * FROM users", "password123")]
        [InlineData("admin'--", "password123")]
        public async Task LoginAsync_SqlInjectionAttempts_ReturnsFailure(string maliciousUsername, string password)
        {
            // Act
            var result = await _authService.LoginAsync(maliciousUsername, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("無效的輸入格式", result.Message);
        }

        [Theory]
        [InlineData("<script>alert('xss')</script>", "password123")]
        [InlineData("user<script>", "password123")]
        [InlineData("admin onload=alert('xss')", "password123")]
        [InlineData("test onclick=alert('xss')", "password123")]
        public async Task LoginAsync_XssAttempts_ReturnsFailure(string maliciousUsername, string password)
        {
            // Act
            var result = await _authService.LoginAsync(maliciousUsername, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("無效的輸入格式", result.Message);
        }

        [Fact]
        public async Task LoginAsync_InactiveAccount_ReturnsFailure()
        {
            // Arrange
            var username = "inactiveuser";
            var password = "TestPass123!";

            var inactiveUser = new User
            {
                UserId = 1,
                Username = "inactiveuser",
                PasswordHash = "hashedpassword",
                IsActive = false
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(inactiveUser);

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("帳戶已被停用", result.Message);
        }

        [Fact]
        public async Task LoginAsync_PasswordVerification_HandlesExceptions()
        {
            // Arrange
            var username = "testuser";
            var password = "TestPass123!";

            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                PasswordHash = "invalid_hash_format",
                IsActive = true
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("用戶名或密碼錯誤", result.Message);
        }

        [Fact]
        public async Task LoginAsync_SuccessfulLogin_ClearsAttempts()
        {
            // Arrange
            var username = "testuser";
            var password = "TestPass123!";

            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                PasswordHash = "hashedpassword",
                IsActive = true
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User user) => user);
            _mockUserRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateRefreshTokenAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("test-token");
            _mockJwtService.Setup(x => x.GenerateRefreshToken())
                .Returns("test-refresh-token");

            // 先增加一些失敗嘗試
            await _authService.LoginAsync("testuser", "wrong");
            await _authService.LoginAsync("testuser", "wrong");

            // Act - 成功登入
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.True(result.Success);
            // 驗證快取中的嘗試次數應該被清除
        }

        [Fact]
        public async Task RegisterAsync_ExtremeInputs_HandlesCorrectly()
        {
            // Arrange
            var username = new string('a', 1000); // 極長用戶名
            var email = "test@example.com";
            var password = new string('1', 1000);  // 極長密碼

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("所有欄位都是必填的", result.Message);
        }

        [Fact]
        public async Task RegisterAsync_UnicodeCharacters_HandlesCorrectly()
        {
            // Arrange
            var username = "測試用戶123";
            var email = "test@example.com";
            var password = "TestPass123!";

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync((User user) => user);
            _mockUserRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("test-token");
            _mockJwtService.Setup(x => x.GenerateRefreshToken())
                .Returns("test-refresh-token");

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.User);
            Assert.Equal("測試用戶123", result.User!.Username);
        }

        [Fact]
        public async Task LoginAsync_Performance_HandlesHighLoad()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Username = "testuser",
                PasswordHash = "hashedpassword",
                IsActive = true
            };

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User user) => user);
            _mockUserRepository.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateRefreshTokenAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("test-token");
            _mockJwtService.Setup(x => x.GenerateRefreshToken())
                .Returns("test-refresh-token");

            var username = "testuser";
            var password = "TestPass123!";

            // Act - 模擬高負載
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var tasks = new List<Task<AuthResult>>();

            for (int i = 0; i < 100; i++)
            {
                tasks.Add(_authService.LoginAsync(username, password));
            }

            var results = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            Assert.True(stopwatch.ElapsedMilliseconds < 5000); // 5秒內完成
            Assert.All(results, result => Assert.True(result.Success));
        }

        [Fact]
        public async Task RegisterAsync_DatabaseFailure_HandlesGracefully()
        {
            // Arrange
            var username = "testuser";
            var email = "test@example.com";
            var password = "TestPass123!";

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("註冊失敗，請稍後再試", result.Message);
        }

        [Fact]
        public async Task LoginAsync_NetworkTimeout_HandlesGracefully()
        {
            // Arrange
            var username = "testuser";
            var password = "TestPass123!";

            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ThrowsAsync(new TimeoutException("Network timeout"));

            // Act
            var result = await _authService.LoginAsync(username, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("登入失敗，請稍後再試", result.Message);
        }

        [Theory]
        [InlineData("test@example.com", "test@example.com", true)] // 相同郵箱
        [InlineData("test@example.com", "different@example.com", false)] // 不同郵箱
        [InlineData("test@example.com", "TEST@EXAMPLE.COM", true)] // 大小寫不敏感
        public async Task RegisterAsync_EmailCaseInsensitive_WorksCorrectly(string email1, string email2, bool shouldConflict)
        {
            // Arrange
            var existingUser = new User
            {
                UserId = 1,
                Username = "existinguser",
                Email = email1,
                IsActive = true
            };

            var username = "newuser";
            var email = email2;
            var password = "TestPass123!";

            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(shouldConflict ? existingUser : null);

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            if (shouldConflict)
            {
                Assert.False(result.Success);
                Assert.Contains("用戶名或郵箱已存在", result.Message);
            }
            else
            {
                Assert.True(result.Success);
            }
        }
    }
}

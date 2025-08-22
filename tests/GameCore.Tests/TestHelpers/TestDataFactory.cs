using AutoFixture;
using GameCore.Api.DTOs;
using GameCore.Domain.Entities;

namespace GameCore.Tests.TestHelpers;

/// <summary>
/// 測試資料工廠，用於生成一致的測試資料
/// </summary>
public static class TestDataFactory
{
    private static readonly Fixture _fixture = new();

    static TestDataFactory()
    {
        // 設定 AutoFixture 行為
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    /// <summary>
    /// 建立有效的註冊請求
    /// </summary>
    public static RegisterRequestDto CreateValidRegisterRequest()
    {
        return new RegisterRequestDto
        {
            Username = _fixture.Create<string>().Substring(0, Math.Min(10, _fixture.Create<string>().Length)),
            Email = _fixture.Create<System.Net.Mail.MailAddress>().Address,
            Password = "ValidPass123!",
            ConfirmPassword = "ValidPass123!"
        };
    }

    /// <summary>
    /// 建立無效的註冊請求
    /// </summary>
    public static RegisterRequestDto CreateInvalidRegisterRequest()
    {
        return new RegisterRequestDto
        {
            Username = "a", // 太短
            Email = "invalid-email",
            Password = "weak",
            ConfirmPassword = "different"
        };
    }

    /// <summary>
    /// 建立有效的登入請求
    /// </summary>
    public static LoginRequestDto CreateValidLoginRequest()
    {
        return new LoginRequestDto
        {
            Username = _fixture.Create<string>().Substring(0, Math.Min(10, _fixture.Create<string>().Length)),
            Password = "ValidPass123!"
        };
    }

    /// <summary>
    /// 建立無效的登入請求
    /// </summary>
    public static LoginRequestDto CreateInvalidLoginRequest()
    {
        return new LoginRequestDto
        {
            Username = "",
            Password = ""
        };
    }

    /// <summary>
    /// 建立測試用戶
    /// </summary>
    public static User CreateTestUser(int? userId = null)
    {
        return new User
        {
            UserId = userId ?? _fixture.Create<int>(),
            Username = _fixture.Create<string>().Substring(0, Math.Min(10, _fixture.Create<string>().Length)),
            Email = _fixture.Create<System.Net.Mail.MailAddress>().Address,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("ValidPass123!"),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            LastLoginAt = DateTime.UtcNow,
            IsActive = true,
            IsEmailVerified = true
        };
    }

    /// <summary>
    /// 建立測試用戶錢包
    /// </summary>
    public static UserWallet CreateTestWallet(int userId, decimal balance = 100.00m)
    {
        return new UserWallet
        {
            UserId = userId,
            Balance = balance,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// 建立測試用戶資料
    /// </summary>
    public static UserProfileDto CreateTestUserProfile(int? userId = null)
    {
        var id = userId ?? _fixture.Create<int>();
        return new UserProfileDto
        {
            UserId = id,
            Username = _fixture.Create<string>().Substring(0, Math.Min(10, _fixture.Create<string>().Length)),
            Email = _fixture.Create<System.Net.Mail.MailAddress>().Address,
            Balance = 100.00m,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            LastLoginAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// 建立成功的認證回應
    /// </summary>
    public static AuthResponseDto CreateSuccessAuthResponse()
    {
        return new AuthResponseDto
        {
            Success = true,
            Token = _fixture.Create<string>(),
            Message = "操作成功",
            User = CreateTestUserProfile(),
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// 建立失敗的認證回應
    /// </summary>
    public static AuthResponseDto CreateFailureAuthResponse(string message = "操作失敗")
    {
        return new AuthResponseDto
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// 建立邊界值測試資料
    /// </summary>
    public static class BoundaryValues
    {
        public static string MinUsername => "abc";
        public static string MaxUsername => new string('a', 20);
        public static string MinPassword => "Abc123!@";
        public static string MaxEmail => new string('a', 90) + "@test.com";
        public static string InvalidUsername => "a";
        public static string InvalidPassword => "weak";
        public static string InvalidEmail => "invalid-email";
    }
}

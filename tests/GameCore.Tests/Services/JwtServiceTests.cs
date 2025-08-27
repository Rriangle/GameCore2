using FluentAssertions;
using GameCore.Api.Services;
using GameCore.Tests.TestHelpers;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace GameCore.Tests.Services;

public class JwtServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();

        // 設定 JWT 配置
        _configurationMock.Setup(x => x["Jwt:SecretKey"]).Returns("your-super-secret-key-with-at-least-32-characters");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("GameCore");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("GameCoreUsers");
        _configurationMock.Setup(x => x["Jwt:ExpirationHours"]).Returns("24");

        _jwtService = new JwtService(_configurationMock.Object);
    }

    [Fact]
    public void GenerateToken_WithValidData_ShouldReturnValidToken()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(1);

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().Contain(".");
        token.Split('.').Should().HaveCount(3); // JWT 格式：header.payload.signature
    }

    [Fact]
    public void GenerateToken_WithDifferentUsers_ShouldGenerateDifferentTokens()
    {
        // Arrange
        var user1 = TestDataFactory.CreateTestUser(1);
        var user2 = TestDataFactory.CreateTestUser(2);

        // Act
        var token1 = _jwtService.GenerateToken(user1);
        var token2 = _jwtService.GenerateToken(user2);

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void GenerateToken_WithSameUser_ShouldGenerateDifferentTokens()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(1);

        // Act
        var token1 = _jwtService.GenerateToken(user);
        var token2 = _jwtService.GenerateToken(user);

        // Assert
        token1.Should().NotBe(token2); // 因為包含時間戳，每次都會不同
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(1);
        var token = _jwtService.GenerateToken(user);

        // Act
        var isValid = _jwtService.ValidateToken(token);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var isValid = _jwtService.ValidateToken(invalidToken);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateToken_WithEmptyToken_ShouldReturnFalse()
    {
        // Arrange
        var emptyToken = "";

        // Act
        var isValid = _jwtService.ValidateToken(emptyToken);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateToken_WithNullToken_ShouldReturnFalse()
    {
        // Arrange
        string? nullToken = null;

        // Act
        var isValid = _jwtService.ValidateToken(nullToken!);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void GenerateToken_WithBoundaryValues_ShouldHandleCorrectly()
    {
        // Arrange
        var user = new GameCore.Domain.Entities.User
        {
            UserId = int.MaxValue,
            Username = new string('a', 50),
            Email = new string('a', 90) + "@test.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsEmailVerified = true
        };

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var isValid = _jwtService.ValidateToken(token);
        isValid.Should().BeTrue();
    }

    [Fact]
    public void GenerateToken_WithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var user = new GameCore.Domain.Entities.User
        {
            UserId = 1,
            Username = "user@123!",
            Email = "user+tag@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsEmailVerified = true
        };

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var isValid = _jwtService.ValidateToken(token);
        isValid.Should().BeTrue();
    }

    [Fact]
    public void Token_ShouldContainRequiredClaims()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(1);

        // Act
        var token = _jwtService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

        // Assert
        jsonToken.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier && c.Value == user.UserId.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Name && c.Value == user.Username);
        jsonToken.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Email && c.Value == user.Email);
        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public void Token_ShouldHaveCorrectExpiration()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(1);

        // Act
        var token = _jwtService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

        // Assert
        jsonToken.ValidTo.Should().BeAfter(DateTime.UtcNow);
        jsonToken.ValidTo.Should().BeBefore(DateTime.UtcNow.AddHours(25)); // 24小時 + 緩衝
        jsonToken.ValidFrom.Should().BeBefore(DateTime.UtcNow.AddMinutes(1));
    }

    [Fact]
    public void Token_ShouldHaveCorrectIssuerAndAudience()
    {
        // Arrange
        var user = new GameCore.Domain.Entities.User
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsEmailVerified = true
        };

        // Act
        var token = _jwtService.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

        // Assert
        jsonToken.Issuer.Should().Be("GameCore");
        jsonToken.Audiences.Should().Contain("GameCoreUsers");
    }
}

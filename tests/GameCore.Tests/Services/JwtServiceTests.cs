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
        var userId = 1;
        var username = "testuser";
        var email = "test@example.com";

        // Act
        var token = _jwtService.GenerateToken(userId, username, email);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().Contain(".");
        token.Split('.').Should().HaveCount(3); // JWT 格式：header.payload.signature
    }

    [Fact]
    public void GenerateToken_WithDifferentUsers_ShouldGenerateDifferentTokens()
    {
        // Arrange
        var user1 = (1, "user1", "user1@example.com");
        var user2 = (2, "user2", "user2@example.com");

        // Act
        var token1 = _jwtService.GenerateToken(user1.Item1, user1.Item2, user1.Item3);
        var token2 = _jwtService.GenerateToken(user2.Item1, user2.Item2, user2.Item3);

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void GenerateToken_WithSameUser_ShouldGenerateDifferentTokens()
    {
        // Arrange
        var userId = 1;
        var username = "testuser";
        var email = "test@example.com";

        // Act
        var token1 = _jwtService.GenerateToken(userId, username, email);
        var token2 = _jwtService.GenerateToken(userId, username, email);

        // Assert
        token1.Should().NotBe(token2); // 因為包含時間戳，每次都會不同
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnValidPrincipal()
    {
        // Arrange
        var userId = 1;
        var username = "testuser";
        var email = "test@example.com";
        var token = _jwtService.GenerateToken(userId, username, email);

        // Act
        var principal = _jwtService.ValidateToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.Identity!.Name.Should().Be(username);
        principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value.Should().Be(userId.ToString());
        principal.FindFirst(System.Security.Claims.ClaimTypes.Email)!.Value.Should().Be(email);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var principal = _jwtService.ValidateToken(invalidToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_WithEmptyToken_ShouldReturnNull()
    {
        // Arrange
        var emptyToken = "";

        // Act
        var principal = _jwtService.ValidateToken(emptyToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_WithNullToken_ShouldReturnNull()
    {
        // Arrange
        string? nullToken = null;

        // Act
        var principal = _jwtService.ValidateToken(nullToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void GenerateToken_WithBoundaryValues_ShouldHandleCorrectly()
    {
        // Arrange
        var maxUserId = int.MaxValue;
        var maxUsername = new string('a', 50);
        var maxEmail = new string('a', 90) + "@test.com";

        // Act
        var token = _jwtService.GenerateToken(maxUserId, maxUsername, maxEmail);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var principal = _jwtService.ValidateToken(token);
        principal.Should().NotBeNull();
        principal!.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value.Should().Be(maxUserId.ToString());
        principal.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value.Should().Be(maxUsername);
        principal.FindFirst(System.Security.Claims.ClaimTypes.Email)!.Value.Should().Be(maxEmail);
    }

    [Fact]
    public void GenerateToken_WithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var userId = 1;
        var username = "user@123!";
        var email = "user+tag@example.com";

        // Act
        var token = _jwtService.GenerateToken(userId, username, email);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var principal = _jwtService.ValidateToken(token);
        principal.Should().NotBeNull();
        principal!.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value.Should().Be(username);
        principal.FindFirst(System.Security.Claims.ClaimTypes.Email)!.Value.Should().Be(email);
    }

    [Fact]
    public void Token_ShouldContainRequiredClaims()
    {
        // Arrange
        var userId = 1;
        var username = "testuser";
        var email = "test@example.com";

        // Act
        var token = _jwtService.GenerateToken(userId, username, email);
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

        // Assert
        jsonToken.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Name && c.Value == username);
        jsonToken.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Email && c.Value == email);
        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public void Token_ShouldHaveCorrectExpiration()
    {
        // Arrange
        var userId = 1;
        var username = "testuser";
        var email = "test@example.com";

        // Act
        var token = _jwtService.GenerateToken(userId, username, email);
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
        var userId = 1;
        var username = "testuser";
        var email = "test@example.com";

        // Act
        var token = _jwtService.GenerateToken(userId, username, email);
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);

        // Assert
        jsonToken.Issuer.Should().Be("GameCore");
        jsonToken.Audiences.Should().Contain("GameCoreUsers");
    }
}

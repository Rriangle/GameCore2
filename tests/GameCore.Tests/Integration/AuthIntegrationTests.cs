using GameCore.Api.DTOs;
using GameCore.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace GameCore.Tests.Integration;

/// <summary>
/// 認證功能整合測試
/// </summary>
public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // 使用記憶體資料庫進行測試
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GameCoreDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<GameCoreDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("註冊成功", result.Message);
        Assert.NotNull(result.Token);
        Assert.NotNull(result.User);
        Assert.Equal(request.Username, result.User.Username);
        Assert.Equal(request.Email, result.User.Email);
        Assert.Equal(100.00m, result.User.Balance);
    }

    [Fact]
    public async Task Register_WithExistingUsername_ShouldReturnError()
    {
        // Arrange
        var request1 = new RegisterRequestDto
        {
            Username = "existinguser",
            Email = "user1@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var request2 = new RegisterRequestDto
        {
            Username = "existinguser",
            Email = "user2@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act - 第一次註冊
        await _client.PostAsJsonAsync("/api/auth/register", request1);

        // Act - 第二次註冊（重複用戶名）
        var response = await _client.PostAsJsonAsync("/api/auth/register", request2);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("用戶名已存在", result.Message);
    }

    [Fact]
    public async Task Register_WithInvalidPassword_ShouldReturnValidationError()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "weak",
            ConfirmPassword = "weak"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("密碼長度至少需要 8 個字元", content);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto
        {
            Username = "loginuser",
            Email = "login@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var loginRequest = new LoginRequestDto
        {
            Username = "loginuser",
            Password = "Password123!"
        };

        // 先註冊用戶
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("登入成功", result.Message);
        Assert.NotNull(result.Token);
        Assert.NotNull(result.User);
        Assert.Equal(loginRequest.Username, result.User.Username);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnError()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "nonexistentuser",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AuthResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("用戶名或密碼錯誤", result.Message);
    }

    [Fact]
    public async Task GetProfile_WithValidToken_ShouldReturnUserProfile()
    {
        // Arrange
        var registerRequest = new RegisterRequestDto
        {
            Username = "profileuser",
            Email = "profile@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // 註冊並登入
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", registerResult!.Token);

        // Act
        var response = await _client.GetAsync("/api/auth/profile");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<UserProfileDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(registerRequest.Username, result.Data.Username);
        Assert.Equal(registerRequest.Email, result.Data.Email);
    }

    [Fact]
    public async Task GetProfile_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/profile");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithRateLimit_ShouldBeLimited()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Username = "ratelimituser",
            Email = "ratelimit@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act - 發送多個請求
        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 65; i++) // 超過每分鐘 60 次限制
        {
            var modifiedRequest = new RegisterRequestDto
            {
                Username = $"ratelimituser{i}",
                Email = $"ratelimit{i}@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            tasks.Add(_client.PostAsJsonAsync("/api/auth/register", modifiedRequest));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert - 檢查是否有請求被限制
        var rateLimitedResponses = responses.Where(r => r.StatusCode == HttpStatusCode.TooManyRequests);
        Assert.True(rateLimitedResponses.Any(), "應該有請求被 Rate Limiting 限制");
    }
}

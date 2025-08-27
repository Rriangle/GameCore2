using GameCore.Api.DTOs;
using GameCore.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace GameCore.Tests.Integration;

/// <summary>
/// 使用者系統整合測試
/// 測試完整的 API 流程，包括註冊、登入、個資管理等
/// </summary>
public class UserIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UserIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // 移除原有的 DbContext 註冊
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<GameCoreDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // 使用 In-Memory 資料庫進行測試
                services.AddDbContext<GameCoreDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });

                // 降低日誌等級避免測試輸出過多訊息
                services.AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Warning);
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task RegisterAndLogin_ShouldWorkCorrectly()
    {
        // 1. 註冊使用者
        var registerRequest = new RegisterUserRequest
        {
            UserName = "整合測試使用者",
            UserAccount = "integrationtest",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var registerResult = await registerResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(registerResult);
        Assert.NotEmpty(registerResult.Token);
        Assert.Equal("整合測試使用者", registerResult.User.UserName);
        Assert.Equal("integrationtest", registerResult.User.UserAccount);

        // 2. 使用註冊時獲得的 Token 測試獲取個人資料
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", registerResult.Token);

        var meResponse = await _client.GetAsync("/api/users/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        var meResult = await meResponse.Content.ReadFromJsonAsync<UserCompleteResponse>();
        Assert.NotNull(meResult);
        Assert.Equal("整合測試使用者", meResult.UserName);
        Assert.Equal("integrationtest", meResult.UserAccount);
        Assert.NotNull(meResult.UserRights);
        Assert.NotNull(meResult.Wallet);
        Assert.True(meResult.UserRights.UserStatus);
        Assert.True(meResult.UserRights.ShoppingPermission);
        Assert.True(meResult.UserRights.MessagePermission);
        Assert.False(meResult.UserRights.SalesAuthority);
        Assert.Equal(100, meResult.Wallet.UserPoint); // 新用戶贈送 100 點數

        // 3. 測試登入
        var loginRequest = new LoginUserRequest
        {
            UserAccount = "integrationtest",
            Password = "password123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(loginResult);
        Assert.NotEmpty(loginResult.Token);
        Assert.Equal("整合測試使用者", loginResult.User.UserName);
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUserAccountExists()
    {
        // 1. 第一次註冊
        var registerRequest = new RegisterUserRequest
        {
            UserName = "第一個使用者",
            UserAccount = "duplicatetest",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        var firstRegisterResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        Assert.Equal(HttpStatusCode.OK, firstRegisterResponse.StatusCode);

        // 2. 嘗試用相同帳號再次註冊
        var duplicateRequest = new RegisterUserRequest
        {
            UserName = "第二個使用者",
            UserAccount = "duplicatetest", // 相同的帳號
            Password = "password456",
            ConfirmPassword = "password456"
        };

        var duplicateResponse = await _client.PostAsJsonAsync("/api/auth/register", duplicateRequest);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsInvalid()
    {
        // 嘗試用不存在的帳號登入
        var loginRequest = new LoginUserRequest
        {
            UserAccount = "nonexistent",
            Password = "wrongpassword"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }

    [Fact]
    public async Task UpdateUserIntroduce_ShouldWorkCorrectly()
    {
        // 1. 先註冊並登入
        var token = await RegisterAndGetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. 更新使用者介紹
        var updateRequest = new UpdateUserIntroduceRequest
        {
            UserNickName = "整合測試暱稱",
            Gender = "M",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "integration@test.com",
            Address = "台北市信義區測試路123號",
            DateOfBirth = new DateTime(1990, 5, 15),
            UserIntroduceText = "這是整合測試的自我介紹"
        };

        var updateResponse = await _client.PutAsJsonAsync("/api/users/me/introduce", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updateResult = await updateResponse.Content.ReadFromJsonAsync<UserIntroduceResponse>();
        Assert.NotNull(updateResult);
        Assert.Equal("整合測試暱稱", updateResult.UserNickName);
        Assert.Equal("M", updateResult.Gender);
        Assert.Equal("integration@test.com", updateResult.Email);
        Assert.Equal("這是整合測試的自我介紹", updateResult.UserIntroduceText);

        // 3. 驗證資料是否正確更新
        var meResponse = await _client.GetAsync("/api/users/me");
        var meResult = await meResponse.Content.ReadFromJsonAsync<UserCompleteResponse>();
        Assert.NotNull(meResult?.UserIntroduce);
        Assert.Equal("整合測試暱稱", meResult.UserIntroduce.UserNickName);
    }

    [Fact]
    public async Task ApplySales_ShouldWorkCorrectly()
    {
        // 1. 先註冊並登入
        var token = await RegisterAndGetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. 申請銷售功能
        var applyRequest = new ApplySalesRequest
        {
            BankCode = 700,
            BankAccountNumber = "123456789012"
        };

        var applyResponse = await _client.PostAsJsonAsync("/api/users/me/apply-sales", applyRequest);
        Assert.Equal(HttpStatusCode.OK, applyResponse.StatusCode);

        var applyResult = await applyResponse.Content.ReadFromJsonAsync<MemberSalesProfileResponse>();
        Assert.NotNull(applyResult);
        Assert.Equal(700, applyResult.BankCode);
        Assert.Equal("123456789012", applyResult.BankAccountNumber);

        // 3. 驗證銷售錢包已創建
        var salesWalletResponse = await _client.GetAsync("/api/users/me/sales-wallet");
        Assert.Equal(HttpStatusCode.OK, salesWalletResponse.StatusCode);

        var salesWalletResult = await salesWalletResponse.Content.ReadFromJsonAsync<UserSalesInformationResponse>();
        Assert.NotNull(salesWalletResult);
        Assert.Equal(0, salesWalletResult.UserSalesWallet); // 初始為 0

        // 4. 嘗試重複申請應該失敗
        var duplicateApplyResponse = await _client.PostAsJsonAsync("/api/users/me/apply-sales", applyRequest);
        Assert.Equal(HttpStatusCode.BadRequest, duplicateApplyResponse.StatusCode);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnPagedResults()
    {
        // 1. 創建多個使用者
        for (int i = 1; i <= 5; i++)
        {
            var registerRequest = new RegisterUserRequest
            {
                UserName = $"測試使用者{i}",
                UserAccount = $"testuser{i:D3}",
                Password = "password123",
                ConfirmPassword = "password123"
            };

            await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        }

        // 2. 使用第一個使用者的 Token 查詢使用者列表
        var token = await RegisterAndGetTokenAsync("listtest", "列表測試使用者");
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 3. 測試分頁查詢
        var usersResponse = await _client.GetAsync("/api/users?page=1&pageSize=3");
        Assert.Equal(HttpStatusCode.OK, usersResponse.StatusCode);

        var usersResult = await usersResponse.Content.ReadFromJsonAsync<UsersListResponse>();
        Assert.NotNull(usersResult);
        Assert.True(usersResult.TotalCount >= 5); // 至少有 5 個使用者
        Assert.Equal(1, usersResult.CurrentPage);
        Assert.Equal(3, usersResult.PageSize);
        Assert.True(usersResult.Users.Count() <= 3);
    }

    [Fact]
    public async Task GetWallet_ShouldReturnCorrectBalance()
    {
        // 1. 註冊並登入
        var token = await RegisterAndGetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // 2. 查詢錢包資訊
        var walletResponse = await _client.GetAsync("/api/users/me/wallet");
        Assert.Equal(HttpStatusCode.OK, walletResponse.StatusCode);

        var walletResult = await walletResponse.Content.ReadFromJsonAsync<UserWalletResponse>();
        Assert.NotNull(walletResult);
        Assert.Equal(100, walletResult.UserPoint); // 新用戶預設 100 點數
        Assert.Null(walletResult.CouponNumber);
    }

    /// <summary>
    /// 輔助方法：註冊使用者並獲取 Token
    /// </summary>
    private async Task<string> RegisterAndGetTokenAsync(string? userAccount = null, string? userName = null)
    {
        var account = userAccount ?? $"testuser{Guid.NewGuid():N}";
        var name = userName ?? $"測試使用者{Guid.NewGuid():N}";

        var registerRequest = new RegisterUserRequest
        {
            UserName = name,
            UserAccount = account,
            Password = "password123",
            ConfirmPassword = "password123"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<LoginResponse>();
        
        return registerResult?.Token ?? throw new InvalidOperationException("無法獲取 Token");
    }
}
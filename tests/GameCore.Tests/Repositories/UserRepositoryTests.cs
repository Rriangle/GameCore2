using GameCore.Domain.Entities;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Repositories;

/// <summary>
/// 使用者資料庫存取測試
/// </summary>
public class UserRepositoryTests : IDisposable
{
    private readonly GameCoreDbContext _context;
    private readonly UserRepository _repository;
    private readonly Mock<ILogger<UserRepository>> _mockLogger;

    public UserRepositoryTests()
    {
        // 使用 In-Memory 資料庫進行測試
        var options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(options);
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _repository = new UserRepository(_context, _mockLogger.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUserWithDefaultRightsAndWallet()
    {
        // Arrange
        var user = new User
        {
            UserName = "測試使用者",
            UserAccount = "testuser",
            UserPassword = "hashedpassword"
        };

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.UserId > 0);
        Assert.Equal("測試使用者", result.UserName);
        Assert.Equal("testuser", result.UserAccount);

        // 驗證是否自動創建了權限和錢包
        var userRights = await _context.UserRights.FirstOrDefaultAsync(ur => ur.UserId == result.UserId);
        Assert.NotNull(userRights);
        Assert.True(userRights.UserStatus);
        Assert.True(userRights.ShoppingPermission);
        Assert.True(userRights.MessagePermission);
        Assert.False(userRights.SalesAuthority);

        var userWallet = await _context.UserWallets.FirstOrDefaultAsync(uw => uw.UserId == result.UserId);
        Assert.NotNull(userWallet);
        Assert.Equal(100, userWallet.UserPoint); // 新用戶贈送 100 點數
    }

    [Fact]
    public async Task GetByUserAccountAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            UserName = "測試使用者",
            UserAccount = "testuser",
            UserPassword = "hashedpassword"
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByUserAccountAsync("testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.UserAccount);
        Assert.NotNull(result.UserRights);
        Assert.NotNull(result.Wallet);
    }

    [Fact]
    public async Task GetByUserAccountAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Act
        var result = await _repository.GetByUserAccountAsync("nonexistentuser");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsByUserNameAsync_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            UserName = "測試使用者",
            UserAccount = "testuser",
            UserPassword = "hashedpassword"
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.ExistsByUserNameAsync("測試使用者");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByUserNameAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Act
        var result = await _repository.ExistsByUserNameAsync("不存在的使用者");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByUserAccountAsync_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            UserName = "測試使用者",
            UserAccount = "testuser",
            UserPassword = "hashedpassword"
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.ExistsByUserAccountAsync("testuser");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnPagedResults()
    {
        // Arrange
        for (int i = 1; i <= 25; i++)
        {
            var user = new User
            {
                UserName = $"使用者{i}",
                UserAccount = $"user{i:D3}",
                UserPassword = "hashedpassword"
            };
            await _repository.CreateAsync(user);
        }

        // Act
        var result = await _repository.GetUsersAsync(skip: 10, take: 5);
        var resultList = result.ToList();

        // Assert
        Assert.Equal(5, resultList.Count);
        // 結果應該按 UserId 降序排列，所以第 11-15 個使用者
        Assert.Contains(resultList, u => u.UserAccount == "user015");
        Assert.Contains(resultList, u => u.UserAccount == "user011");
    }

    [Fact]
    public async Task GetUsersAsync_WithSearchKeyword_ShouldFilterResults()
    {
        // Arrange
        var users = new[]
        {
            new User { UserName = "王小明", UserAccount = "wang001", UserPassword = "hash" },
            new User { UserName = "李小華", UserAccount = "lee001", UserPassword = "hash" },
            new User { UserName = "張大明", UserAccount = "zhang001", UserPassword = "hash" }
        };

        foreach (var user in users)
        {
            await _repository.CreateAsync(user);
        }

        // Act
        var result = await _repository.GetUsersAsync(searchKeyword: "小");
        var resultList = result.ToList();

        // Assert
        Assert.Equal(2, resultList.Count);
        Assert.Contains(resultList, u => u.UserName == "王小明");
        Assert.Contains(resultList, u => u.UserName == "李小華");
    }

    [Fact]
    public async Task GetCompleteUserDataAsync_ShouldReturnUserWithAllRelations()
    {
        // Arrange
        var user = new User
        {
            UserName = "完整測試使用者",
            UserAccount = "completeuser",
            UserPassword = "hashedpassword"
        };
        var createdUser = await _repository.CreateAsync(user);

        // 添加使用者介紹
        var userIntroduce = new UserIntroduce
        {
            UserId = createdUser.UserId,
            UserNickName = "完整測試暱稱",
            Gender = "M",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "complete@test.com",
            Address = "台北市信義區",
            DateOfBirth = new DateTime(1990, 1, 1),
            CreateAccount = DateTime.UtcNow
        };
        _context.UserIntroduces.Add(userIntroduce);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCompleteUserDataAsync(createdUser.UserId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.UserIntroduce);
        Assert.NotNull(result.UserRights);
        Assert.NotNull(result.Wallet);
        Assert.Equal("完整測試暱稱", result.UserIntroduce.UserNickName);
        Assert.Equal("complete@test.com", result.UserIntroduce.Email);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUserAndRelatedData()
    {
        // Arrange
        var user = new User
        {
            UserName = "待刪除使用者",
            UserAccount = "deleteuser",
            UserPassword = "hashedpassword"
        };
        var createdUser = await _repository.CreateAsync(user);

        // Act
        var result = await _repository.DeleteAsync(createdUser.UserId);

        // Assert
        Assert.True(result);

        // 驗證使用者已被刪除
        var deletedUser = await _repository.GetByIdAsync(createdUser.UserId);
        Assert.Null(deletedUser);

        // 驗證相關資料也被刪除（由於級聯刪除）
        var userRights = await _context.UserRights.FirstOrDefaultAsync(ur => ur.UserId == createdUser.UserId);
        Assert.Null(userRights);

        var userWallet = await _context.UserWallets.FirstOrDefaultAsync(uw => uw.UserId == createdUser.UserId);
        Assert.Null(userWallet);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        Assert.False(result);
    }
}
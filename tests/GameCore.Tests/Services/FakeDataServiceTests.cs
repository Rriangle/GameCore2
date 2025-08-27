using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 假資料生成服務測試
/// </summary>
public class FakeDataServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserIntroduceRepository> _mockUserIntroduceRepository;
    private readonly Mock<IUserRightsRepository> _mockUserRightsRepository;
    private readonly Mock<IUserWalletRepository> _mockUserWalletRepository;
    private readonly Mock<IMemberSalesProfileRepository> _mockMemberSalesProfileRepository;
    private readonly Mock<IUserSalesInformationRepository> _mockUserSalesInformationRepository;
    private readonly Mock<ILogger<FakeDataService>> _mockLogger;
    private readonly FakeDataService _fakeDataService;

    public FakeDataServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserIntroduceRepository = new Mock<IUserIntroduceRepository>();
        _mockUserRightsRepository = new Mock<IUserRightsRepository>();
        _mockUserWalletRepository = new Mock<IUserWalletRepository>();
        _mockMemberSalesProfileRepository = new Mock<IMemberSalesProfileRepository>();
        _mockUserSalesInformationRepository = new Mock<IUserSalesInformationRepository>();
        _mockLogger = new Mock<ILogger<FakeDataService>>();

        _fakeDataService = new FakeDataService(
            _mockUserRepository.Object,
            _mockUserIntroduceRepository.Object,
            _mockUserRightsRepository.Object,
            _mockUserWalletRepository.Object,
            _mockMemberSalesProfileRepository.Object,
            _mockUserSalesInformationRepository.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public void GenerateFakeName_ReturnsValidName()
    {
        // Act
        var result = _fakeDataService.GenerateFakeName();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Length >= 2);
        Assert.True(result.Length <= 10);
    }

    [Fact]
    public void GenerateFakeAccount_ReturnsValidAccount()
    {
        // Act
        var result = _fakeDataService.GenerateFakeAccount();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Length >= 4);
        Assert.True(result.Length <= 20);
        Assert.Matches(@"^[a-zA-Z0-9_]+$", result);
    }

    [Fact]
    public void GenerateFakeEmail_ReturnsValidEmail()
    {
        // Act
        var result = _fakeDataService.GenerateFakeEmail();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("@", result);
        Assert.Contains(".", result);
        Assert.Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", result);
    }

    [Fact]
    public void GenerateFakeNickname_ReturnsValidNickname()
    {
        // Act
        var result = _fakeDataService.GenerateFakeNickname();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Length >= 2);
        Assert.True(result.Length <= 15);
    }

    [Fact]
    public void GenerateFakeIdNumber_ReturnsValidIdNumber()
    {
        // Act
        var result = _fakeDataService.GenerateFakeIdNumber();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(10, result.Length);
        Assert.Matches(@"^[A-Z][12]\d{8}$", result);
    }

    [Fact]
    public void GenerateFakeCellphone_ReturnsValidCellphone()
    {
        // Act
        var result = _fakeDataService.GenerateFakeCellphone();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(10, result.Length);
        Assert.Matches(@"^09\d{8}$", result);
    }

    [Fact]
    public void GenerateFakeAddress_ReturnsValidAddress()
    {
        // Act
        var result = _fakeDataService.GenerateFakeAddress();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Length >= 10);
        Assert.True(result.Length <= 100);
        Assert.Contains("市", result);
        Assert.Contains("區", result);
        Assert.Contains("路", result);
        Assert.Contains("號", result);
    }

    [Fact]
    public void GenerateFakeUserIntroduce_ReturnsValidUserIntroduce()
    {
        // Act
        var result = _fakeDataService.GenerateFakeUserIntroduce();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Length >= 20);
        Assert.True(result.Length <= 500);
    }

    [Fact]
    public void GenerateFakeCouponNumber_ReturnsValidCouponNumber()
    {
        // Act
        var result = _fakeDataService.GenerateFakeCouponNumber();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(12, result.Length);
        Assert.Matches(@"^[A-Z0-9]{12}$", result);
    }

    [Fact]
    public void GenerateFakeBankAccountNumber_ReturnsValidBankAccountNumber()
    {
        // Act
        var result = _fakeDataService.GenerateFakeBankAccountNumber();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result);
        Assert.Equal(16, result.Length);
        Assert.Matches(@"^\d{16}$", result);
    }

    [Fact]
    public void GenerateFakeReviewNotes_ReturnsValidReviewNotes()
    {
        // Act
        var result = _fakeDataService.GenerateFakeReviewNotes();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Length >= 10);
        Assert.True(result.Length <= 200);
    }

    [Fact]
    public async Task GenerateFakeUsersAsync_ValidCount_ReturnsCorrectCount()
    {
        // Arrange
        var count = 10;
        var fakeUsers = new List<User>();
        var fakeUserIntroduces = new List<UserIntroduce>();
        var fakeUserRights = new List<UserRights>();
        var fakeUserWallets = new List<UserWallet>();
        var fakeMemberSalesProfiles = new List<MemberSalesProfile>();
        var fakeUserSalesInformations = new List<UserSalesInformation>();

        for (int i = 0; i < count; i++)
        {
            var user = new User
            {
                User_ID = i + 1,
                User_name = $"測試用戶{i + 1}",
                User_Account = $"testuser{i + 1}",
                User_Password = "hashedpassword",
                Email = $"test{i + 1}@example.com",
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                LastLoginAt = DateTime.UtcNow.AddDays(-i),
                IsActive = true,
                IsEmailVerified = true
            };
            fakeUsers.Add(user);

            var userIntroduce = new UserIntroduce
            {
                User_ID = i + 1,
                User_NickName = $"暱稱{i + 1}",
                Gender = i % 2 == 0 ? "男" : "女",
                IdNumber = $"A{i:D9}",
                Cellphone = $"09{i:D8}",
                Email = $"contact{i + 1}@example.com",
                Address = $"台北市測試區測試路{i + 1}號",
                DateOfBirth = DateTime.Today.AddYears(-(20 + i)),
                User_Introduce = $"這是用戶{i + 1}的自我介紹"
            };
            fakeUserIntroduces.Add(userIntroduce);

            var userRights = new UserRights
            {
                User_Id = i + 1,
                User_Status = true,
                ShoppingPermission = true,
                MessagePermission = true,
                SalesAuthority = i % 3 == 0,
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                UpdatedAt = DateTime.UtcNow.AddDays(-i)
            };
            fakeUserRights.Add(userRights);

            var userWallet = new UserWallet
            {
                WalletId = i + 1,
                User_Id = i + 1,
                User_Point = Random.Shared.Next(0, 10000),
                Coupon_Number = $"COUPON{i:D8}",
                CreatedAt = DateTime.UtcNow.AddDays(-i),
                UpdatedAt = DateTime.UtcNow.AddDays(-i)
            };
            fakeUserWallets.Add(userWallet);

            if (i % 3 == 0)
            {
                var memberSalesProfile = new MemberSalesProfile
                {
                    User_Id = i + 1,
                    BankCode = Random.Shared.Next(1000, 9999),
                    BankAccountNumber = $"{Random.Shared.Next(1000000000000000, 9999999999999999)}",
                    Status = "Approved",
                    AppliedAt = DateTime.UtcNow.AddDays(-(i + 10)),
                    ReviewedAt = DateTime.UtcNow.AddDays(-i),
                    ReviewNotes = "資料齊全，審核通過"
                };
                fakeMemberSalesProfiles.Add(memberSalesProfile);

                var userSalesInformation = new UserSalesInformation
                {
                    User_Id = i + 1,
                    UserSales_Wallet = Random.Shared.Next(1000, 50000),
                    CreatedAt = DateTime.UtcNow.AddDays(-(i + 10)),
                    UpdatedAt = DateTime.UtcNow.AddDays(-i)
                };
                fakeUserSalesInformations.Add(userSalesInformation);
            }
        }

        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync<User, UserRepository, User>((user) => user);
        _mockUserIntroduceRepository.Setup(x => x.AddAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync<UserIntroduce, UserIntroduceRepository, UserIntroduce>((userIntroduce) => userIntroduce);
        _mockUserRightsRepository.Setup(x => x.AddAsync(It.IsAny<UserRights>()))
            .ReturnsAsync<UserRights, UserRightsRepository, UserRights>((userRights) => userRights);
        _mockUserWalletRepository.Setup(x => x.AddAsync(It.IsAny<UserWallet>()))
            .ReturnsAsync<UserWallet, UserWalletRepository, UserWallet>((userWallet) => userWallet);
        _mockMemberSalesProfileRepository.Setup(x => x.AddAsync(It.IsAny<MemberSalesProfile>()))
            .ReturnsAsync<MemberSalesProfile, MemberSalesProfileRepository, MemberSalesProfile>((profile) => profile);
        _mockUserSalesInformationRepository.Setup(x => x.AddAsync(It.IsAny<UserSalesInformation>()))
            .ReturnsAsync<UserSalesInformation, UserSalesInformationRepository, UserSalesInformation>((info) => info);

        // Act
        var result = await _fakeDataService.GenerateFakeUsersAsync(count);

        // Assert
        Assert.Equal(count, result);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Exactly(count));
        _mockUserIntroduceRepository.Verify(x => x.AddAsync(It.IsAny<UserIntroduce>()), Times.Exactly(count));
        _mockUserRightsRepository.Verify(x => x.AddAsync(It.IsAny<UserRights>()), Times.Exactly(count));
        _mockUserWalletRepository.Verify(x => x.AddAsync(It.IsAny<UserWallet>()), Times.Exactly(count));
    }

    [Fact]
    public async Task GetFakeDataStatsAsync_ReturnsCorrectStatistics()
    {
        // Arrange
        var expectedStats = new
        {
            TotalUsers = 100,
            TotalUserIntroduces = 100,
            TotalUserRights = 100,
            TotalUserWallets = 100,
            TotalMemberSalesProfiles = 33,
            TotalUserSalesInformations = 33
        };

        _mockUserRepository.Setup(x => x.GetCountAsync()).ReturnsAsync(expectedStats.TotalUsers);
        _mockUserIntroduceRepository.Setup(x => x.GetCountAsync()).ReturnsAsync(expectedStats.TotalUserIntroduces);
        _mockUserRightsRepository.Setup(x => x.GetCountAsync()).ReturnsAsync(expectedStats.TotalUserRights);
        _mockUserWalletRepository.Setup(x => x.GetCountAsync()).ReturnsAsync(expectedStats.TotalUserWallets);
        _mockMemberSalesProfileRepository.Setup(x => x.GetCountAsync()).ReturnsAsync(expectedStats.TotalMemberSalesProfiles);
        _mockUserSalesInformationRepository.Setup(x => x.GetCountAsync()).ReturnsAsync(expectedStats.TotalUserSalesInformations);

        // Act
        var result = await _fakeDataService.GetFakeDataStatsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedStats.TotalUsers, result.TotalUsers);
        Assert.Equal(expectedStats.TotalUserIntroduces, result.TotalUserIntroduces);
        Assert.Equal(expectedStats.TotalUserRights, result.TotalUserRights);
        Assert.Equal(expectedStats.TotalUserWallets, result.TotalUserWallets);
        Assert.Equal(expectedStats.TotalMemberSalesProfiles, result.TotalMemberSalesProfiles);
        Assert.Equal(expectedStats.TotalUserSalesInformations, result.TotalUserSalesInformations);
    }

    [Fact]
    public async Task CleanupFakeDataAsync_DeletesAllFakeData()
    {
        // Arrange
        var expectedDeletedCount = 100;

        _mockUserRepository.Setup(x => x.DeleteAllAsync()).ReturnsAsync(expectedDeletedCount);
        _mockUserIntroduceRepository.Setup(x => x.DeleteAllAsync()).ReturnsAsync(expectedDeletedCount);
        _mockUserRightsRepository.Setup(x => x.DeleteAllAsync()).ReturnsAsync(expectedDeletedCount);
        _mockUserWalletRepository.Setup(x => x.DeleteAllAsync()).ReturnsAsync(expectedDeletedCount);
        _mockMemberSalesProfileRepository.Setup(x => x.DeleteAllAsync()).ReturnsAsync(expectedDeletedCount);
        _mockUserSalesInformationRepository.Setup(x => x.DeleteAllAsync()).ReturnsAsync(expectedDeletedCount);

        // Act
        var result = await _fakeDataService.CleanupFakeDataAsync();

        // Assert
        Assert.Equal(expectedDeletedCount * 6, result);
        _mockUserRepository.Verify(x => x.DeleteAllAsync(), Times.Once);
        _mockUserIntroduceRepository.Verify(x => x.DeleteAllAsync(), Times.Once);
        _mockUserRightsRepository.Verify(x => x.DeleteAllAsync(), Times.Once);
        _mockUserWalletRepository.Verify(x => x.DeleteAllAsync(), Times.Once);
        _mockMemberSalesProfileRepository.Verify(x => x.DeleteAllAsync(), Times.Once);
        _mockUserSalesInformationRepository.Verify(x => x.DeleteAllAsync(), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task GenerateFakeUsersAsync_InvalidCount_ThrowsArgumentException(int invalidCount)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fakeDataService.GenerateFakeUsersAsync(invalidCount));
    }

    [Fact]
    public void GenerateFakeName_MultipleCalls_ReturnsDifferentNames()
    {
        // Act
        var name1 = _fakeDataService.GenerateFakeName();
        var name2 = _fakeDataService.GenerateFakeName();
        var name3 = _fakeDataService.GenerateFakeName();

        // Assert
        Assert.NotEqual(name1, name2);
        Assert.NotEqual(name2, name3);
        Assert.NotEqual(name1, name3);
    }

    [Fact]
    public void GenerateFakeEmail_MultipleCalls_ReturnsDifferentEmails()
    {
        // Act
        var email1 = _fakeDataService.GenerateFakeEmail();
        var email2 = _fakeDataService.GenerateFakeEmail();
        var email3 = _fakeDataService.GenerateFakeEmail();

        // Assert
        Assert.NotEqual(email1, email2);
        Assert.NotEqual(email2, email3);
        Assert.NotEqual(email1, email3);
    }
}
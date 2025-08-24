using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Services;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests;

/// <summary>
/// 銷售服務測試類別
/// </summary>
public class SalesServiceTests
{
    private readonly Mock<IMemberSalesProfileRepository> _mockSalesProfileRepository;
    private readonly Mock<IUserSalesInformationRepository> _mockSalesInformationRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<SalesService>> _mockLogger;
    private readonly SalesService _salesService;

    /// <summary>
    /// 測試建構函式，設定模擬物件
    /// </summary>
    public SalesServiceTests()
    {
        _mockSalesProfileRepository = new Mock<IMemberSalesProfileRepository>();
        _mockSalesInformationRepository = new Mock<IUserSalesInformationRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<SalesService>>();
        
        _salesService = new SalesService(
            _mockSalesProfileRepository.Object,
            _mockSalesInformationRepository.Object,
            _mockUserRepository.Object,
            _mockLogger.Object);
    }

    /// <summary>
    /// 測試申請銷售功能 - 成功情境
    /// </summary>
    [Fact]
    public async Task ApplySalesProfileAsync_ShouldReturnTrue_WhenValidApplication()
    {
        // Arrange
        var userId = 1;
        var application = new SalesProfileApplicationDto
        {
            BankCode = 822,
            BankAccountNumber = "1234567890123",
            AccountCoverPhotoBase64 = null
        };

        var mockUser = new User
        {
            User_ID = userId,
            User_name = "TestUser",
            User_Account = "test@example.com",
            User_Password = "hashedpassword"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(mockUser);
        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((MemberSalesProfile)null); // 沒有現有資料
        _mockSalesProfileRepository.Setup(x => x.IsBankAccountExistsAsync(application.BankCode, application.BankAccountNumber, null))
            .ReturnsAsync(false); // 銀行帳號不重複
        _mockSalesProfileRepository.Setup(x => x.CreateAsync(It.IsAny<MemberSalesProfile>()))
            .ReturnsAsync(new MemberSalesProfile { User_Id = userId });
        _mockSalesInformationRepository.Setup(x => x.CreateAsync(It.IsAny<UserSalesInformation>()))
            .ReturnsAsync(new UserSalesInformation { User_Id = userId });

        // Act
        var result = await _salesService.ApplySalesProfileAsync(userId, application);

        // Assert
        Assert.True(result);
        _mockSalesProfileRepository.Verify(x => x.CreateAsync(It.IsAny<MemberSalesProfile>()), Times.Once);
        _mockSalesInformationRepository.Verify(x => x.CreateAsync(It.IsAny<UserSalesInformation>()), Times.Once);
    }

    /// <summary>
    /// 測試申請銷售功能 - 用戶不存在
    /// </summary>
    [Fact]
    public async Task ApplySalesProfileAsync_ShouldReturnFalse_WhenUserNotExists()
    {
        // Arrange
        var userId = 999;
        var application = new SalesProfileApplicationDto
        {
            BankCode = 822,
            BankAccountNumber = "1234567890123"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User)null);

        // Act
        var result = await _salesService.ApplySalesProfileAsync(userId, application);

        // Assert
        Assert.False(result);
        _mockSalesProfileRepository.Verify(x => x.CreateAsync(It.IsAny<MemberSalesProfile>()), Times.Never);
    }

    /// <summary>
    /// 測試申請銷售功能 - 已有銷售資料
    /// </summary>
    [Fact]
    public async Task ApplySalesProfileAsync_ShouldReturnFalse_WhenProfileAlreadyExists()
    {
        // Arrange
        var userId = 1;
        var application = new SalesProfileApplicationDto
        {
            BankCode = 822,
            BankAccountNumber = "1234567890123"
        };

        var mockUser = new User { User_ID = userId };
        var existingProfile = new MemberSalesProfile { User_Id = userId };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(mockUser);
        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(existingProfile);

        // Act
        var result = await _salesService.ApplySalesProfileAsync(userId, application);

        // Assert
        Assert.False(result);
        _mockSalesProfileRepository.Verify(x => x.CreateAsync(It.IsAny<MemberSalesProfile>()), Times.Never);
    }

    /// <summary>
    /// 測試申請銷售功能 - 銀行帳號重複
    /// </summary>
    [Fact]
    public async Task ApplySalesProfileAsync_ShouldReturnFalse_WhenBankAccountExists()
    {
        // Arrange
        var userId = 1;
        var application = new SalesProfileApplicationDto
        {
            BankCode = 822,
            BankAccountNumber = "1234567890123"
        };

        var mockUser = new User { User_ID = userId };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(mockUser);
        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync((MemberSalesProfile)null);
        _mockSalesProfileRepository.Setup(x => x.IsBankAccountExistsAsync(application.BankCode, application.BankAccountNumber, null))
            .ReturnsAsync(true); // 銀行帳號重複

        // Act
        var result = await _salesService.ApplySalesProfileAsync(userId, application);

        // Assert
        Assert.False(result);
        _mockSalesProfileRepository.Verify(x => x.CreateAsync(It.IsAny<MemberSalesProfile>()), Times.Never);
    }

    /// <summary>
    /// 測試取得銷售資料 - 成功情境
    /// </summary>
    [Fact]
    public async Task GetSalesProfileAsync_ShouldReturnProfile_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var mockUser = new User
        {
            User_ID = userId,
            UserRights = new UserRights { SalesAuthority = true }
        };
        var mockProfile = new MemberSalesProfile
        {
            User_Id = userId,
            BankCode = 822,
            BankAccountNumber = "1234567890123"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(mockUser);
        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(mockProfile);

        // Act
        var result = await _salesService.GetSalesProfileAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.User_Id);
        Assert.Equal(822, result.BankCode);
        Assert.True(result.HasSalesAuthority);
        Assert.Equal("approved", result.ApplicationStatus);
    }

    /// <summary>
    /// 測試檢查用戶是否有銷售權限 - 有權限
    /// </summary>
    [Fact]
    public async Task HasSalesAuthorityAsync_ShouldReturnTrue_WhenUserHasAuthority()
    {
        // Arrange
        var userId = 1;
        var mockUser = new User
        {
            User_ID = userId,
            UserRights = new UserRights { SalesAuthority = true }
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(mockUser);

        // Act
        var result = await _salesService.HasSalesAuthorityAsync(userId);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 測試檢查用戶是否有銷售權限 - 沒有權限
    /// </summary>
    [Fact]
    public async Task HasSalesAuthorityAsync_ShouldReturnFalse_WhenUserHasNoAuthority()
    {
        // Arrange
        var userId = 1;
        var mockUser = new User
        {
            User_ID = userId,
            UserRights = new UserRights { SalesAuthority = false }
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(mockUser);

        // Act
        var result = await _salesService.HasSalesAuthorityAsync(userId);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 測試取得銷售錢包資訊 - 成功情境
    /// </summary>
    [Fact]
    public async Task GetSalesWalletAsync_ShouldReturnWallet_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var mockUser = new User
        {
            User_ID = userId,
            UserRights = new UserRights { SalesAuthority = true }
        };
        var mockSalesInfo = new UserSalesInformation
        {
            User_Id = userId,
            UserSales_Wallet = 5000
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(mockUser);
        _mockSalesInformationRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(mockSalesInfo);

        // Act
        var result = await _salesService.GetSalesWalletAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.User_Id);
        Assert.Equal(5000, result.UserSales_Wallet);
        Assert.True(result.HasSalesAuthority);
    }

    /// <summary>
    /// 測試銷售錢包交易 - 增加收入成功
    /// </summary>
    [Fact]
    public async Task AddSalesRevenueAsync_ShouldReturnTrue_WhenValidTransaction()
    {
        // Arrange
        var transaction = new SalesWalletTransactionDto
        {
            UserId = 1,
            Amount = 1000,
            Type = "market_sale",
            Note = "玩家市集銷售"
        };

        _mockSalesInformationRepository.Setup(x => x.AddToSalesWalletAsync(transaction.UserId, transaction.Amount))
            .ReturnsAsync(true);

        // Act
        var result = await _salesService.AddSalesRevenueAsync(transaction);

        // Assert
        Assert.True(result);
        _mockSalesInformationRepository.Verify(x => x.AddToSalesWalletAsync(transaction.UserId, transaction.Amount), Times.Once);
    }

    /// <summary>
    /// 測試銷售錢包交易 - 金額為零或負數
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task AddSalesRevenueAsync_ShouldReturnFalse_WhenInvalidAmount(int amount)
    {
        // Arrange
        var transaction = new SalesWalletTransactionDto
        {
            UserId = 1,
            Amount = amount,
            Type = "market_sale"
        };

        // Act
        var result = await _salesService.AddSalesRevenueAsync(transaction);

        // Assert
        Assert.False(result);
        _mockSalesInformationRepository.Verify(x => x.AddToSalesWalletAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    /// <summary>
    /// 測試銷售錢包交易 - 扣除費用成功
    /// </summary>
    [Fact]
    public async Task DeductSalesAmountAsync_ShouldReturnTrue_WhenSufficientBalance()
    {
        // Arrange
        var transaction = new SalesWalletTransactionDto
        {
            UserId = 1,
            Amount = 500,
            Type = "platform_fee",
            Note = "平台手續費"
        };

        _mockSalesInformationRepository.Setup(x => x.GetSalesWalletBalanceAsync(transaction.UserId))
            .ReturnsAsync(1000); // 足夠的餘額
        _mockSalesInformationRepository.Setup(x => x.DeductFromSalesWalletAsync(transaction.UserId, transaction.Amount))
            .ReturnsAsync(true);

        // Act
        var result = await _salesService.DeductSalesAmountAsync(transaction);

        // Assert
        Assert.True(result);
        _mockSalesInformationRepository.Verify(x => x.DeductFromSalesWalletAsync(transaction.UserId, transaction.Amount), Times.Once);
    }

    /// <summary>
    /// 測試銷售錢包交易 - 餘額不足
    /// </summary>
    [Fact]
    public async Task DeductSalesAmountAsync_ShouldReturnFalse_WhenInsufficientBalance()
    {
        // Arrange
        var transaction = new SalesWalletTransactionDto
        {
            UserId = 1,
            Amount = 1500,
            Type = "withdrawal",
            Note = "提領"
        };

        _mockSalesInformationRepository.Setup(x => x.GetSalesWalletBalanceAsync(transaction.UserId))
            .ReturnsAsync(1000); // 不足的餘額

        // Act
        var result = await _salesService.DeductSalesAmountAsync(transaction);

        // Assert
        Assert.False(result);
        _mockSalesInformationRepository.Verify(x => x.DeductFromSalesWalletAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    /// <summary>
    /// 測試管理者審核銷售申請 - 核准申請
    /// </summary>
    [Fact]
    public async Task ReviewSalesApplicationAsync_ShouldReturnTrue_WhenApproving()
    {
        // Arrange
        var adminUserId = 1;
        var userId = 2;
        var approved = true;
        var reason = "資料審核通過";

        var mockUser = new User
        {
            User_ID = userId,
            UserRights = new UserRights { SalesAuthority = false }
        };
        var mockProfile = new MemberSalesProfile { User_Id = userId };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(mockUser);
        _mockSalesProfileRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(mockProfile);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(mockUser);

        // Act
        var result = await _salesService.ReviewSalesApplicationAsync(adminUserId, userId, approved, reason);

        // Assert
        Assert.True(result);
        Assert.True(mockUser.UserRights.SalesAuthority);
        _mockUserRepository.Verify(x => x.UpdateAsync(mockUser), Times.Once);
    }

    /// <summary>
    /// 測試檢查銷售錢包餘額是否足夠 - 足夠
    /// </summary>
    [Fact]
    public async Task HasSufficientSalesBalanceAsync_ShouldReturnTrue_WhenSufficientBalance()
    {
        // Arrange
        var userId = 1;
        var requiredAmount = 500;

        _mockSalesInformationRepository.Setup(x => x.GetSalesWalletBalanceAsync(userId))
            .ReturnsAsync(1000);

        // Act
        var result = await _salesService.HasSufficientSalesBalanceAsync(userId, requiredAmount);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// 測試檢查銷售錢包餘額是否足夠 - 不足
    /// </summary>
    [Fact]
    public async Task HasSufficientSalesBalanceAsync_ShouldReturnFalse_WhenInsufficientBalance()
    {
        // Arrange
        var userId = 1;
        var requiredAmount = 1500;

        _mockSalesInformationRepository.Setup(x => x.GetSalesWalletBalanceAsync(userId))
            .ReturnsAsync(1000);

        // Act
        var result = await _salesService.HasSufficientSalesBalanceAsync(userId, requiredAmount);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// 測試取得待審核的銷售申請列表
    /// </summary>
    [Fact]
    public async Task GetPendingApplicationsAsync_ShouldReturnPendingApplications()
    {
        // Arrange
        var mockProfiles = new List<MemberSalesProfile>
        {
            new MemberSalesProfile { User_Id = 2, BankCode = 822, BankAccountNumber = "1234567890" },
            new MemberSalesProfile { User_Id = 3, BankCode = 700, BankAccountNumber = "0987654321" }
        };

        var mockUser2 = new User 
        { 
            User_ID = 2,
            UserRights = new UserRights { SalesAuthority = false } // 待審核
        };
        var mockUser3 = new User 
        { 
            User_ID = 3,
            UserRights = new UserRights { SalesAuthority = true } // 已核准
        };

        _mockSalesProfileRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(mockProfiles);
        _mockUserRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(mockUser2);
        _mockUserRepository.Setup(x => x.GetByIdAsync(3))
            .ReturnsAsync(mockUser3);

        // Act
        var result = await _salesService.GetPendingApplicationsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result); // 只有一個待審核的申請
        Assert.Equal(2, result[0].User_Id);
        Assert.Equal("pending", result[0].ApplicationStatus);
    }
}
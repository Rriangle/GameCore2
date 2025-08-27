using GameCore.Api.Controllers;
using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace GameCore.Tests.Controllers;

/// <summary>
/// 使用者控制器測試
/// </summary>
public class UsersControllerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserIntroduceRepository> _mockUserIntroduceRepository;
    private readonly Mock<IUserRightsRepository> _mockUserRightsRepository;
    private readonly Mock<IUserWalletRepository> _mockUserWalletRepository;
    private readonly Mock<IMemberSalesProfileRepository> _mockMemberSalesProfileRepository;
    private readonly Mock<IUserSalesInformationRepository> _mockUserSalesInformationRepository;
    private readonly Mock<ILogger<UsersController>> _mockLogger;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserIntroduceRepository = new Mock<IUserIntroduceRepository>();
        _mockUserRightsRepository = new Mock<IUserRightsRepository>();
        _mockUserWalletRepository = new Mock<IUserWalletRepository>();
        _mockMemberSalesProfileRepository = new Mock<IMemberSalesProfileRepository>();
        _mockUserSalesInformationRepository = new Mock<IUserSalesInformationRepository>();
        _mockLogger = new Mock<ILogger<UsersController>>();

        _controller = new UsersController(
            _mockUserRepository.Object,
            _mockUserIntroduceRepository.Object,
            _mockUserRightsRepository.Object,
            _mockUserWalletRepository.Object,
            _mockMemberSalesProfileRepository.Object,
            _mockUserSalesInformationRepository.Object,
            _mockLogger.Object);
    }

    private void SetupAuthenticatedUser(int userId)
    {
        var claims = new List<Claim>
        {
            new("userId", userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    [Fact]
    public async Task GetMe_ShouldReturnUserData_WhenUserExists()
    {
        // Arrange
        const int userId = 1;
        SetupAuthenticatedUser(userId);

        var user = new User
        {
            UserId = userId,
            UserName = "測試使用者",
            UserAccount = "testuser",
            UserPassword = "hashedpassword",
            UserRights = new UserRights
            {
                UserId = userId,
                UserStatus = true,
                ShoppingPermission = true,
                MessagePermission = true,
                SalesAuthority = false
            },
            Wallet = new UserWallet
            {
                UserId = userId,
                UserPoint = 1000,
                CouponNumber = null
            }
        };

        _mockUserRepository.Setup(r => r.GetCompleteUserDataAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetMe();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UserCompleteResponse>(okResult.Value);
        Assert.Equal(userId, response.UserId);
        Assert.Equal("測試使用者", response.UserName);
        Assert.Equal("testuser", response.UserAccount);
        Assert.NotNull(response.UserRights);
        Assert.NotNull(response.Wallet);
        Assert.Equal(1000, response.Wallet.UserPoint);
    }

    [Fact]
    public async Task GetMe_ShouldReturnUnauthorized_WhenNoUserIdClaim()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        // Act
        var result = await _controller.GetMe();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal("無效的使用者認證", unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetMe_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        const int userId = 1;
        SetupAuthenticatedUser(userId);

        _mockUserRepository.Setup(r => r.GetCompleteUserDataAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.GetMe();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("使用者不存在", notFoundResult.Value);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnPagedUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                UserId = 1,
                UserName = "使用者1",
                UserAccount = "user001",
                UserPassword = "hash",
                UserRights = new UserRights { UserStatus = true, ShoppingPermission = true, MessagePermission = true, SalesAuthority = false },
                Wallet = new UserWallet { UserPoint = 100 }
            },
            new()
            {
                UserId = 2,
                UserName = "使用者2",
                UserAccount = "user002",
                UserPassword = "hash",
                UserRights = new UserRights { UserStatus = true, ShoppingPermission = true, MessagePermission = true, SalesAuthority = true },
                Wallet = new UserWallet { UserPoint = 200 }
            }
        };

        _mockUserRepository.Setup(r => r.GetUsersAsync(0, 20, null))
            .ReturnsAsync(users);
        _mockUserRepository.Setup(r => r.GetUsersCountAsync(null))
            .ReturnsAsync(2);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UsersListResponse>(okResult.Value);
        Assert.Equal(2, response.TotalCount);
        Assert.Equal(1, response.CurrentPage);
        Assert.Equal(20, response.PageSize);
        Assert.Equal(1, response.TotalPages);
        Assert.Equal(2, response.Users.Count());
    }

    [Fact]
    public async Task UpdateMyIntroduce_ShouldCreateNewIntroduce_WhenNotExists()
    {
        // Arrange
        const int userId = 1;
        SetupAuthenticatedUser(userId);

        var request = new UpdateUserIntroduceRequest
        {
            UserNickName = "新暱稱",
            Gender = "M",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "test@example.com",
            Address = "台北市信義區",
            DateOfBirth = new DateTime(1990, 1, 1),
            UserIntroduceText = "這是我的自我介紹"
        };

        _mockUserIntroduceRepository.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync((UserIntroduce?)null);

        _mockUserIntroduceRepository.Setup(r => r.IsNicknameExistsAsync(request.UserNickName, userId))
            .ReturnsAsync(false);
        _mockUserIntroduceRepository.Setup(r => r.IsIdNumberExistsAsync(request.IdNumber, userId))
            .ReturnsAsync(false);
        _mockUserIntroduceRepository.Setup(r => r.IsCellphoneExistsAsync(request.Cellphone, userId))
            .ReturnsAsync(false);
        _mockUserIntroduceRepository.Setup(r => r.IsEmailExistsAsync(request.Email, userId))
            .ReturnsAsync(false);

        _mockUserIntroduceRepository.Setup(r => r.CreateAsync(It.IsAny<UserIntroduce>()))
            .ReturnsAsync((UserIntroduce userIntroduce) => userIntroduce);

        // Act
        var result = await _controller.UpdateMyIntroduce(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UserIntroduceResponse>(okResult.Value);
        Assert.Equal("新暱稱", response.UserNickName);
        Assert.Equal("M", response.Gender);
        Assert.Equal("test@example.com", response.Email);

        _mockUserIntroduceRepository.Verify(r => r.CreateAsync(It.IsAny<UserIntroduce>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMyIntroduce_ShouldReturnBadRequest_WhenNicknameExists()
    {
        // Arrange
        const int userId = 1;
        SetupAuthenticatedUser(userId);

        var request = new UpdateUserIntroduceRequest
        {
            UserNickName = "已存在的暱稱",
            Gender = "M",
            IdNumber = "A123456789",
            Cellphone = "0912345678",
            Email = "test@example.com",
            Address = "台北市信義區",
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        _mockUserIntroduceRepository.Setup(r => r.IsNicknameExistsAsync(request.UserNickName, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateMyIntroduce(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("暱稱已被使用", badRequestResult.Value);
    }

    [Fact]
    public async Task ApplySales_ShouldCreateSalesProfile_WhenNotExists()
    {
        // Arrange
        const int userId = 1;
        SetupAuthenticatedUser(userId);

        var request = new ApplySalesRequest
        {
            BankCode = 700,
            BankAccountNumber = "123456789012",
            AccountCoverPhotoBase64 = null
        };

        _mockMemberSalesProfileRepository.Setup(r => r.ExistsAsync(userId))
            .ReturnsAsync(false);

        _mockMemberSalesProfileRepository.Setup(r => r.CreateAsync(It.IsAny<MemberSalesProfile>()))
            .ReturnsAsync((MemberSalesProfile profile) => profile);

        _mockUserSalesInformationRepository.Setup(r => r.CreateAsync(It.IsAny<UserSalesInformation>()))
            .ReturnsAsync((UserSalesInformation info) => info);

        // Act
        var result = await _controller.ApplySales(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<MemberSalesProfileResponse>(okResult.Value);
        Assert.Equal(700, response.BankCode);
        Assert.Equal("123456789012", response.BankAccountNumber);
        Assert.False(response.HasAccountCoverPhoto);

        _mockMemberSalesProfileRepository.Verify(r => r.CreateAsync(It.IsAny<MemberSalesProfile>()), Times.Once);
        _mockUserSalesInformationRepository.Verify(r => r.CreateAsync(It.IsAny<UserSalesInformation>()), Times.Once);
    }

    [Fact]
    public async Task ApplySales_ShouldReturnBadRequest_WhenAlreadyApplied()
    {
        // Arrange
        const int userId = 1;
        SetupAuthenticatedUser(userId);

        var request = new ApplySalesRequest
        {
            BankCode = 700,
            BankAccountNumber = "123456789012"
        };

        _mockMemberSalesProfileRepository.Setup(r => r.ExistsAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.ApplySales(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("您已經申請過銷售功能", badRequestResult.Value);
    }

    [Fact]
    public async Task GetMyWallet_ShouldReturnWalletInfo()
    {
        // Arrange
        const int userId = 1;
        SetupAuthenticatedUser(userId);

        var wallet = new UserWallet
        {
            UserId = userId,
            UserPoint = 5000,
            CouponNumber = "COUPON123"
        };

        _mockUserWalletRepository.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(wallet);

        // Act
        var result = await _controller.GetMyWallet();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UserWalletResponse>(okResult.Value);
        Assert.Equal(5000, response.UserPoint);
        Assert.Equal("COUPON123", response.CouponNumber);
    }

    [Fact]
    public async Task GetMySalesWallet_ShouldReturnSalesWalletInfo()
    {
        // Arrange
        const int userId = 1;
        SetupAuthenticatedUser(userId);

        var salesInfo = new UserSalesInformation
        {
            UserId = userId,
            UserSalesWallet = 25000
        };

        _mockUserSalesInformationRepository.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(salesInfo);

        // Act
        var result = await _controller.GetMySalesWallet();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<UserSalesInformationResponse>(okResult.Value);
        Assert.Equal(25000, response.UserSalesWallet);
    }

    [Fact]
    public async Task GetMySalesWallet_ShouldReturnNotFound_WhenNotExists()
    {
        // Arrange
        const int userId = 1;
        SetupAuthenticatedUser(userId);

        _mockUserSalesInformationRepository.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync((UserSalesInformation?)null);

        // Act
        var result = await _controller.GetMySalesWallet();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("銷售錢包資料不存在，請先申請銷售功能", notFoundResult.Value);
    }
}
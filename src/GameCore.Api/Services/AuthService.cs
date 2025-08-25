using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace GameCore.Api.Services;

/// <summary>
/// 認證服務，處理用戶註冊、登入和個人資料管理
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserWalletRepository _walletRepository;
    private readonly IUserIntroduceRepository _userIntroduceRepository;
    private readonly IUserRightsRepository _userRightsRepository;
    private readonly IMemberSalesProfileRepository _memberSalesProfileRepository;
    private readonly IUserSalesInformationRepository _userSalesInformationRepository;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IUserWalletRepository walletRepository,
        IUserIntroduceRepository userIntroduceRepository,
        IUserRightsRepository userRightsRepository,
        IMemberSalesProfileRepository memberSalesProfileRepository,
        IUserSalesInformationRepository userSalesInformationRepository,
        JwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
        _userIntroduceRepository = userIntroduceRepository;
        _userRightsRepository = userRightsRepository;
        _memberSalesProfileRepository = memberSalesProfileRepository;
        _userSalesInformationRepository = userSalesInformationRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// 用戶註冊
    /// </summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        _logger.LogInformation("開始處理用戶註冊請求: {UserAccount}", request.User_Account);

        try
        {
            // 檢查帳號是否已存在
            if (await _userRepository.ExistsByAccountAsync(request.User_Account))
            {
                _logger.LogWarning("註冊失敗：帳號已存在: {UserAccount}", request.User_Account);
                return new AuthResponseDto { Success = false, Message = "帳號已存在" };
            }

            // 檢查郵箱是否已存在
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                _logger.LogWarning("註冊失敗：郵箱已被註冊: {Email}", request.Email);
                return new AuthResponseDto { Success = false, Message = "郵箱已被註冊" };
            }

            // 建立新用戶
            var user = new User
            {
                User_name = request.User_name.Trim(),
                User_Account = request.User_Account.Trim(),
                User_Password = BC.HashPassword(request.User_Password),
                Email = request.Email.Trim().ToLowerInvariant(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsEmailVerified = false
            };

            var createdUser = await _userRepository.AddAsync(user);

            // 建立用戶錢包
            var wallet = new UserWallet
            {
                User_Id = createdUser.User_ID,
                User_Point = 0, // 初始點數 0
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _walletRepository.AddAsync(wallet);

            // 建立用戶權限 (預設允許留言和購物，不允許銷售)
            var userRights = new UserRights
            {
                User_Id = createdUser.User_ID,
                User_Status = true,
                ShoppingPermission = true,
                MessagePermission = true,
                SalesAuthority = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRightsRepository.AddAsync(userRights);

            // 建立用戶介紹 (基本資料)
            var userIntroduce = new UserIntroduce
            {
                User_ID = createdUser.User_ID,
                User_NickName = request.User_name,
                Gender = "未指定",
                IdNumber = "",
                Cellphone = "",
                Email = request.Email,
                Address = "",
                DateOfBirth = DateTime.Today.AddYears(-18), // 預設 18 歲
                Create_Account = DateTime.UtcNow
            };

            await _userIntroduceRepository.AddAsync(userIntroduce);

            // 建立銷售錢包 (初始餘額 0)
            var salesWallet = new UserSalesInformation
            {
                User_Id = createdUser.User_ID,
                UserSales_Wallet = 0.00m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userSalesInformationRepository.AddAsync(salesWallet);

            // 生成 JWT Token
            var token = _jwtService.GenerateToken(createdUser.User_ID, createdUser.User_Account, createdUser.Email);

            _logger.LogInformation("用戶註冊成功: {UserId} {UserAccount}", createdUser.User_ID, createdUser.User_Account);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                User = new UserProfileDto
                {
                    User_ID = createdUser.User_ID,
                    User_name = createdUser.User_name,
                    User_Account = createdUser.User_Account,
                    Email = createdUser.Email,
                    User_Point = wallet.User_Point,
                    CreatedAt = createdUser.CreatedAt,
                    LastLoginAt = createdUser.LastLoginAt,
                    IsActive = createdUser.IsActive,
                    IsEmailVerified = createdUser.IsEmailVerified
                },
                Message = "註冊成功"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "註冊過程中發生未預期的錯誤: {UserAccount}", request.User_Account);
            return new AuthResponseDto { Success = false, Message = "註冊失敗，請稍後再試" };
        }
    }

    /// <summary>
    /// 用戶登入
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        _logger.LogInformation("開始處理用戶登入請求: {UserAccount}", request.User_Account);

        try
        {
            // 查找用戶
            var user = await _userRepository.GetByAccountAsync(request.User_Account.Trim());
            if (user == null)
            {
                _logger.LogWarning("登入失敗：用戶不存在: {UserAccount}", request.User_Account);
                return new AuthResponseDto { Success = false, Message = "帳號或密碼錯誤" };
            }

            // 驗證密碼
            if (!BC.Verify(request.User_Password, user.User_Password))
            {
                _logger.LogWarning("登入失敗：密碼錯誤: {UserAccount}", request.User_Account);
                return new AuthResponseDto { Success = false, Message = "帳號或密碼錯誤" };
            }

            // 檢查用戶是否啟用
            if (!user.IsActive)
            {
                _logger.LogWarning("登入失敗：帳戶已被停用: {UserId}", user.User_ID);
                return new AuthResponseDto { Success = false, Message = "帳戶已被停用" };
            }

            // 更新最後登入時間
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // 獲取錢包餘額
            var wallet = await _walletRepository.GetByUserIdAsync(user.User_ID);
            var balance = wallet?.User_Point ?? 0;

            // 生成 JWT Token
            var token = _jwtService.GenerateToken(user.User_ID, user.User_Account, user.Email);

            _logger.LogInformation("用戶登入成功: {UserId} {UserAccount}", user.User_ID, user.User_Account);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                User = new UserProfileDto
                {
                    User_ID = user.User_ID,
                    User_name = user.User_name,
                    User_Account = user.User_Account,
                    Email = user.Email,
                    User_Point = balance,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    IsActive = user.IsActive,
                    IsEmailVerified = user.IsEmailVerified
                },
                Message = "登入成功"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入過程中發生未預期的錯誤: {UserAccount}", request.User_Account);
            return new AuthResponseDto { Success = false, Message = "登入失敗，請稍後再試" };
        }
    }

    /// <summary>
    /// 取得用戶資料
    /// </summary>
    public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
    {
        _logger.LogDebug("獲取用戶資料: {UserId}", userId);

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("用戶不存在: {UserId}", userId);
                return null;
            }

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            var balance = wallet?.User_Point ?? 0;

            return new UserProfileDto
            {
                User_ID = user.User_ID,
                User_name = user.User_name,
                User_Account = user.User_Account,
                Email = user.Email,
                User_Point = balance,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive,
                IsEmailVerified = user.IsEmailVerified
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶資料時發生錯誤: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// 取得完整用戶資料
    /// </summary>
    public async Task<CompleteUserProfileDto?> GetCompleteUserProfileAsync(int userId)
    {
        _logger.LogDebug("獲取完整用戶資料: {UserId}", userId);

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("用戶不存在: {UserId}", userId);
                return null;
            }

            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            var userIntroduce = await _userIntroduceRepository.GetByUserIdAsync(userId);
            var userRights = await _userRightsRepository.GetByUserIdAsync(userId);
            var salesPermission = await _memberSalesProfileRepository.GetByUserIdAsync(userId);
            var salesWallet = await _userSalesInformationRepository.GetByUserIdAsync(userId);

            return new CompleteUserProfileDto
            {
                BasicInfo = new UserProfileDto
                {
                    User_ID = user.User_ID,
                    User_name = user.User_name,
                    User_Account = user.User_Account,
                    Email = user.Email,
                    User_Point = wallet?.User_Point ?? 0,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    IsActive = user.IsActive,
                    IsEmailVerified = user.IsEmailVerified
                },
                Introduce = userIntroduce != null ? new UserIntroduceDto
                {
                    User_ID = userIntroduce.User_ID,
                    User_NickName = userIntroduce.User_NickName,
                    Gender = userIntroduce.Gender,
                    IdNumber = userIntroduce.IdNumber,
                    Cellphone = userIntroduce.Cellphone,
                    Email = userIntroduce.Email,
                    Address = userIntroduce.Address,
                    DateOfBirth = userIntroduce.DateOfBirth,
                    Create_Account = userIntroduce.Create_Account,
                    User_Introduce = userIntroduce.User_Introduce
                } : null,
                Rights = userRights != null ? new UserRightsDto
                {
                    User_Id = userRights.User_Id,
                    User_Status = userRights.User_Status,
                    ShoppingPermission = userRights.ShoppingPermission,
                    MessagePermission = userRights.MessagePermission,
                    SalesAuthority = userRights.SalesAuthority
                } : null,
                Wallet = wallet != null ? new WalletBalanceDto
                {
                    User_Id = wallet.User_Id,
                    User_Point = wallet.User_Point,
                    UpdatedAt = wallet.UpdatedAt
                } : null,
                SalesPermission = salesPermission != null ? new SalesPermissionResponseDto
                {
                    User_Id = salesPermission.User_Id,
                    Status = salesPermission.Status,
                    AppliedAt = salesPermission.AppliedAt,
                    ReviewedAt = salesPermission.ReviewedAt,
                    ReviewNotes = salesPermission.ReviewNotes
                } : null,
                SalesWallet = salesWallet != null ? new SalesWalletDto
                {
                    User_Id = salesWallet.User_Id,
                    UserSales_Wallet = salesWallet.UserSales_Wallet,
                    UpdatedAt = salesWallet.UpdatedAt
                } : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取完整用戶資料時發生錯誤: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// 驗證 JWT Token
    /// </summary>
    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            return _jwtService.ValidateToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "驗證 Token 時發生錯誤");
            return false;
        }
    }

    /// <summary>
    /// 從 Token 取得用戶 ID
    /// </summary>
    public async Task<int?> GetUserIdFromTokenAsync(string token)
    {
        try
        {
            return _jwtService.GetUserIdFromToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "從 Token 取得用戶 ID 時發生錯誤");
            return null;
        }
    }
}

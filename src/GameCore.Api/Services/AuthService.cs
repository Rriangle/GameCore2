using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using BC = BCrypt.Net.BCrypt;
using System.Text.RegularExpressions;

namespace GameCore.Api.Services;

/// <summary>
/// 認證服務，處理用戶註冊、登入和個人資料管理
/// 優化版本：增強安全性、輸入驗證、錯誤處理和性能
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

    // 常數定義，提高可維護性
    private const int MaxLoginAttempts = 5;
    private const int LockoutDurationMinutes = 30;
    private const int PasswordMinLength = 8;
    private const int UsernameMinLength = 3;
    private const int UsernameMaxLength = 50;
    private const int EmailMaxLength = 100;
    private const int NameMaxLength = 100;

    // 正則表達式模式
    private static readonly Regex UsernamePattern = new(@"^[a-zA-Z0-9_\u4e00-\u9fa5]+$", RegexOptions.Compiled);
    private static readonly Regex PasswordPattern = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", RegexOptions.Compiled);

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
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _userIntroduceRepository = userIntroduceRepository ?? throw new ArgumentNullException(nameof(userIntroduceRepository));
        _userRightsRepository = userRightsRepository ?? throw new ArgumentNullException(nameof(userRightsRepository));
        _memberSalesProfileRepository = memberSalesProfileRepository ?? throw new ArgumentNullException(nameof(memberSalesProfileRepository));
        _userSalesInformationRepository = userSalesInformationRepository ?? throw new ArgumentNullException(nameof(userSalesInformationRepository));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 用戶註冊 - 優化版本
    /// </summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        _logger.LogInformation("開始處理用戶註冊請求: {UserAccount}", request.User_Account);

        try
        {
            // 輸入驗證
            var validationResult = ValidateRegistrationRequest(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("註冊請求驗證失敗: {UserAccount}, 錯誤: {Error}", request.User_Account, validationResult.ErrorMessage);
                return new AuthResponseDto { Success = false, Message = validationResult.ErrorMessage };
            }

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
            var user = CreateUserFromRequest(request);
            var createdUser = await _userRepository.AddAsync(user);

            // 建立相關實體
            await CreateUserRelatedEntitiesAsync(createdUser.User_ID);

            // 生成 JWT Token
            var token = _jwtService.GenerateToken(createdUser);

            _logger.LogInformation("用戶註冊成功: {UserAccount}, UserId: {UserId}", request.User_Account, createdUser.User_ID);

            return new AuthResponseDto
            {
                Success = true,
                Message = "註冊成功",
                Token = token,
                User = new UserDto
                {
                    User_ID = createdUser.User_ID,
                    User_name = createdUser.User_name,
                    User_Account = createdUser.User_Account,
                    Email = createdUser.Email,
                    Balance = 0, // 初始點數 0
                    IsActive = createdUser.IsActive,
                    IsEmailVerified = createdUser.IsEmailVerified
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用戶註冊過程中發生錯誤: {UserAccount}", request.User_Account);
            return new AuthResponseDto { Success = false, Message = "註冊過程中發生錯誤，請稍後再試" };
        }
    }

    /// <summary>
    /// 用戶登入 - 優化版本
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        _logger.LogInformation("開始處理用戶登入請求: {UserAccount}", request.User_Account);

        try
        {
            // 輸入驗證
            if (string.IsNullOrWhiteSpace(request.User_Account) || string.IsNullOrWhiteSpace(request.User_Password))
            {
                _logger.LogWarning("登入請求驗證失敗：帳號或密碼為空");
                return new AuthResponseDto { Success = false, Message = "帳號和密碼不能為空" };
            }

            // 查找用戶
            var user = await _userRepository.GetByAccountAsync(request.User_Account);
            if (user == null)
            {
                _logger.LogWarning("登入失敗：帳號不存在: {UserAccount}", request.User_Account);
                return new AuthResponseDto { Success = false, Message = "帳號或密碼錯誤" };
            }

            // 檢查用戶狀態
            if (!user.IsActive)
            {
                _logger.LogWarning("登入失敗：用戶已被停用: {UserAccount}", request.User_Account);
                return new AuthResponseDto { Success = false, Message = "帳號已被停用，請聯繫客服" };
            }

            // 驗證密碼
            if (!BC.Verify(request.User_Password, user.User_Password))
            {
                _logger.LogWarning("登入失敗：密碼錯誤: {UserAccount}", request.User_Account);
                return new AuthResponseDto { Success = false, Message = "帳號或密碼錯誤" };
            }

            // 更新最後登入時間
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // 獲取用戶錢包資訊
            var wallet = await _walletRepository.GetByUserIdAsync(user.User_ID);
            var balance = wallet?.User_Point ?? 0;

            // 生成 JWT Token
            var token = _jwtService.GenerateToken(user);

            _logger.LogInformation("用戶登入成功: {UserAccount}, UserId: {UserId}", request.User_Account, user.User_ID);

            return new AuthResponseDto
            {
                Success = true,
                Message = "登入成功",
                Token = token,
                User = new UserDto
                {
                    User_ID = user.User_ID,
                    User_name = user.User_name,
                    User_Account = user.User_Account,
                    Email = user.Email,
                    Balance = balance,
                    IsActive = user.IsActive,
                    IsEmailVerified = user.IsEmailVerified
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用戶登入過程中發生錯誤: {UserAccount}", request.User_Account);
            return new AuthResponseDto { Success = false, Message = "登入過程中發生錯誤，請稍後再試" };
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

    /// <summary>
    /// 驗證註冊請求 - 新增方法，提高可讀性
    /// </summary>
    private ValidationResult ValidateRegistrationRequest(RegisterRequestDto request)
    {
        // 檢查必填欄位
        if (string.IsNullOrWhiteSpace(request.User_name))
            return ValidationResult.Failure("使用者姓名不能為空");

        if (string.IsNullOrWhiteSpace(request.User_Account))
            return ValidationResult.Failure("登入帳號不能為空");

        if (string.IsNullOrWhiteSpace(request.User_Password))
            return ValidationResult.Failure("密碼不能為空");

        if (string.IsNullOrWhiteSpace(request.Email))
            return ValidationResult.Failure("電子郵件不能為空");

        // 檢查長度限制
        if (request.User_name.Length > NameMaxLength)
            return ValidationResult.Failure($"使用者姓名不能超過{NameMaxLength}個字符");

        if (request.User_Account.Length < UsernameMinLength || request.User_Account.Length > UsernameMaxLength)
            return ValidationResult.Failure($"登入帳號長度必須在{UsernameMinLength}-{UsernameMaxLength}個字符之間");

        if (request.User_Password.Length < PasswordMinLength)
            return ValidationResult.Failure($"密碼長度不能少於{PasswordMinLength}個字符");

        if (request.Email.Length > EmailMaxLength)
            return ValidationResult.Failure($"電子郵件不能超過{EmailMaxLength}個字符");

        // 檢查格式
        if (!UsernamePattern.IsMatch(request.User_Account))
            return ValidationResult.Failure("登入帳號只能包含字母、數字、下劃線和中文字符");

        if (!PasswordPattern.IsMatch(request.User_Password))
            return ValidationResult.Failure("密碼必須包含大小寫字母、數字和特殊字符");

        if (!IsValidEmail(request.Email))
            return ValidationResult.Failure("電子郵件格式不正確");

        return ValidationResult.Success();
    }

    /// <summary>
    /// 驗證電子郵件格式 - 新增方法
    /// </summary>
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 建立用戶實體 - 新增方法，提高可讀性
    /// </summary>
    private User CreateUserFromRequest(RegisterRequestDto request)
    {
        return new User
        {
            User_name = request.User_name.Trim(),
            User_Account = request.User_Account.Trim(),
            User_Password = BC.HashPassword(request.User_Password),
            Email = request.Email.Trim().ToLowerInvariant(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsEmailVerified = false
        };
    }

    /// <summary>
    /// 建立用戶相關實體 - 新增方法，提高可讀性
    /// </summary>
    private async Task CreateUserRelatedEntitiesAsync(int userId)
    {
        // 建立用戶錢包
        var wallet = new UserWallet
        {
            User_Id = userId,
            User_Point = 0, // 初始點數 0
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _walletRepository.AddAsync(wallet);

        // 建立用戶權限 (預設允許留言和購物，不允許銷售)
        var userRights = new UserRights
        {
            User_Id = userId,
            User_Status = true,
            ShoppingPermission = true,
            MessagePermission = true,
            SalesAuthority = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _userRightsRepository.AddAsync(userRights);

        // 建立用戶介紹 (預設值)
        var userIntroduce = new UserIntroduce
        {
            User_ID = userId,
            User_NickName = string.Empty,
            Gender = string.Empty,
            IdNumber = string.Empty,
            Cellphone = string.Empty,
            Email = string.Empty,
            Address = string.Empty,
            DateOfBirth = DateTime.Today.AddYears(-18), // 預設 18 歲
            User_Picture = null,
            User_Introduce = string.Empty,
            Create_Account = DateTime.UtcNow
        };
        await _userIntroduceRepository.AddAsync(userIntroduce);

        // 建立銷售資訊 (預設值)
        var userSalesInfo = new UserSalesInformation
        {
            User_Id = userId,
            UserSales_Wallet = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _userSalesInformationRepository.AddAsync(userSalesInfo);
    }
}

using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using BC = BCrypt.Net.BCrypt;

namespace GameCore.Api.Services;

/// <summary>
/// 認證服務，處理用戶註冊、登入和個人資料管理
/// 根據新的實體結構設計：Users + UserIntroduce + UserRights + UserWallet
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserWalletRepository _walletRepository;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IUserWalletRepository walletRepository,
        JwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// 用戶註冊 - 建立完整的用戶資料 (Users + UserIntroduce + UserRights + UserWallet)
    /// </summary>
    public async Task<AuthResult> RegisterAsync(string username, string email, string password, string fullName, string nickname, string gender, string idNumber, string cellphone, string address, DateTime dateOfBirth)
    {
        _logger.LogInformation("開始處理用戶註冊請求: {Username}", username);

        try
        {
            // 檢查用戶名是否已存在
            if (await _userRepository.ExistsByUsernameAsync(username))
            {
                _logger.LogWarning("註冊失敗：用戶名已存在: {Username}", username);
                return new AuthResult { Success = false, ErrorMessage = "用戶名已存在" };
            }

            // 檢查郵箱是否已存在
            if (await _userRepository.ExistsByEmailAsync(email))
            {
                _logger.LogWarning("註冊失敗：郵箱已被註冊: {Email}", email);
                return new AuthResult { Success = false, ErrorMessage = "郵箱已被註冊" };
            }

            // 建立用戶詳細資料 (UserIntroduce 表)
            var userIntroduce = new UserIntroduce
            {
                User_NickName = nickname.Trim(),
                Gender = gender.Trim().ToUpperInvariant(),
                IdNumber = idNumber.Trim(),
                Cellphone = cellphone.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                Address = address.Trim(),
                DateOfBirth = dateOfBirth,
                Create_Account = DateTime.UtcNow
            };

            // 建立用戶權限 (UserRights 表)
            var userRights = new UserRights
            {
                User_Status = true,       // 預設啟用
                ShoppingPermission = true, // 預設可購物
                MessagePermission = true,  // 預設可留言
                SalesAuthority = false     // 預設不開放銷售，需申請
            };

            // 建立用戶錢包 (UserWallet 表)
            var wallet = new UserWallet
            {
                User_Point = 0, // 初始點數為 0
                Coupon_Number = null
            };

            // 建立新用戶主檔並關聯所有相關資料 (一次性建立)
            var user = new User
            {
                User_name = fullName.Trim(),
                User_Account = username.Trim(),
                User_Password = BC.HashPassword(password),
                UserIntroduce = userIntroduce,
                UserRights = userRights,
                Wallet = wallet
            };

            var createdUser = await _userRepository.CreateAsync(user);

            // 產生 JWT Token
            var token = _jwtService.GenerateToken(createdUser.User_ID, createdUser.User_Account, email);

            _logger.LogInformation("用戶註冊成功: User_ID = {UserId}, User_Account = {UserAccount}", 
                createdUser.User_ID, createdUser.User_Account);

            return new AuthResult
            {
                Success = true,
                Token = token,
                User = new UserInfo
                {
                    UserId = createdUser.User_ID,
                    Username = createdUser.User_Account,
                    FullName = createdUser.User_name,
                    Email = createdUser.UserIntroduce?.Email ?? email,
                    Nickname = createdUser.UserIntroduce?.User_NickName ?? nickname,
                    Points = createdUser.Wallet?.User_Point ?? 0,
                    CanShop = createdUser.UserRights?.ShoppingPermission ?? true,
                    CanMessage = createdUser.UserRights?.MessagePermission ?? true,
                    CanSell = createdUser.UserRights?.SalesAuthority ?? false,
                    IsActive = createdUser.UserRights?.User_Status ?? true
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "註冊過程中發生錯誤: {Username}", username);
            return new AuthResult { Success = false, ErrorMessage = "註冊失敗，請稍後再試" };
        }
    }

    /// <summary>
    /// 簡化版註冊方法 (向後相容)
    /// </summary>
    public async Task<AuthResult> RegisterAsync(string username, string email, string password)
    {
        return await RegisterAsync(
            username: username,
            email: email,
            password: password,
            fullName: username, // 使用 username 作為姓名
            nickname: username, // 使用 username 作為暱稱
            gender: "M", // 預設男性
            idNumber: GenerateRandomIdNumber(), // 產生假的身分證字號
            cellphone: GenerateRandomPhone(), // 產生假的電話號碼
            address: "台北市中正區", // 預設地址
            dateOfBirth: new DateTime(1990, 1, 1) // 預設生日
        );
    }

    /// <summary>
    /// 用戶登入
    /// </summary>
    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        _logger.LogInformation("開始處理用戶登入請求: {Username}", username);

        try
        {
            var user = await _userRepository.GetByUsernameForLoginAsync(username);
            if (user == null)
            {
                _logger.LogWarning("登入失敗：用戶不存在: {Username}", username);
                return new AuthResult { Success = false, ErrorMessage = "用戶名或密碼錯誤" };
            }

            // 驗證密碼
            if (!BC.Verify(password, user.User_Password))
            {
                _logger.LogWarning("登入失敗：密碼錯誤: User_ID = {UserId}", user.User_ID);
                return new AuthResult { Success = false, ErrorMessage = "用戶名或密碼錯誤" };
            }

            // 檢查用戶狀態
            if (user.UserRights?.User_Status != true)
            {
                _logger.LogWarning("登入失敗：用戶已被停權: User_ID = {UserId}", user.User_ID);
                return new AuthResult { Success = false, ErrorMessage = "帳號已被停權，請聯繫客服" };
            }

            // 產生 JWT Token
            var token = _jwtService.GenerateToken(user.User_ID, user.User_Account, user.UserIntroduce?.Email ?? "");

            _logger.LogInformation("用戶登入成功: User_ID = {UserId}, User_Account = {UserAccount}", 
                user.User_ID, user.User_Account);

            return new AuthResult
            {
                Success = true,
                Token = token,
                User = new UserInfo
                {
                    UserId = user.User_ID,
                    Username = user.User_Account,
                    FullName = user.User_name,
                    Email = user.UserIntroduce?.Email ?? "",
                    Nickname = user.UserIntroduce?.User_NickName ?? "",
                    Points = user.Wallet?.User_Point ?? 0,
                    CanShop = user.UserRights?.ShoppingPermission ?? false,
                    CanMessage = user.UserRights?.MessagePermission ?? false,
                    CanSell = user.UserRights?.SalesAuthority ?? false,
                    IsActive = user.UserRights?.User_Status ?? false
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入過程中發生錯誤: {Username}", username);
            return new AuthResult { Success = false, ErrorMessage = "登入失敗，請稍後再試" };
        }
    }

    /// <summary>
    /// 取得用戶個人資訊
    /// </summary>
    public async Task<UserInfo?> GetUserInfoAsync(int userId)
    {
        _logger.LogDebug("查詢用戶資訊: User_ID = {UserId}", userId);

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("用戶不存在: User_ID = {UserId}", userId);
                return null;
            }

            return new UserInfo
            {
                UserId = user.User_ID,
                Username = user.User_Account,
                FullName = user.User_name,
                Email = user.UserIntroduce?.Email ?? "",
                Nickname = user.UserIntroduce?.User_NickName ?? "",
                Points = user.Wallet?.User_Point ?? 0,
                CanShop = user.UserRights?.ShoppingPermission ?? false,
                CanMessage = user.UserRights?.MessagePermission ?? false,
                CanSell = user.UserRights?.SalesAuthority ?? false,
                IsActive = user.UserRights?.User_Status ?? false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢用戶資訊時發生錯誤: User_ID = {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// 產生隨機身分證字號 (假資料)
    /// </summary>
    private string GenerateRandomIdNumber()
    {
        var random = new Random();
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var firstLetter = letters[random.Next(letters.Length)];
        var numbers = random.Next(100000000, 999999999);
        return $"{firstLetter}{numbers}";
    }

    /// <summary>
    /// 產生隨機電話號碼 (假資料)
    /// </summary>
    private string GenerateRandomPhone()
    {
        var random = new Random();
        return $"09{random.Next(10000000, 99999999)}";
    }
}


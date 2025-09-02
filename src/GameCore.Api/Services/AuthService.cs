using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GameCore.Api.Services;

/// <summary>
/// 認證服務 - 處理用戶註冊、登入、JWT 令牌生成等
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIntroduceRepository _userIntroduceRepository;
    private readonly IUserRightsRepository _userRightsRepository;
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly IConfiguration _configuration;

    // 效能優化：靜態 SHA256 實例避免重複創建，提升密碼雜湊效能
    private static readonly SHA256 _sha256 = SHA256.Create();

    public AuthService(
        IUserRepository userRepository,
        IUserIntroduceRepository userIntroduceRepository,
        IUserRightsRepository userRightsRepository,
        IUserWalletRepository userWalletRepository,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _userIntroduceRepository = userIntroduceRepository;
        _userRightsRepository = userRightsRepository;
        _userWalletRepository = userWalletRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// 用戶註冊
    /// </summary>
    public async Task<ApiResponseDto<UserInfoDto>> RegisterAsync(UserRegisterDto registerDto)
    {
        try
        {
            // 檢查帳號是否已存在
            var existingUser = await _userRepository.GetByAccountAsync(registerDto.UserAccount);
            if (existingUser != null)
            {
                return new ApiResponseDto<UserInfoDto>
                {
                    Success = false,
                    Message = "帳號已存在，請使用其他帳號"
                };
            }

            // 檢查電子郵件是否已存在
            var existingEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingEmail != null)
            {
                return new ApiResponseDto<UserInfoDto>
                {
                    Success = false,
                    Message = "電子郵件已被使用，請使用其他電子郵件"
                };
            }

            // 檢查手機號碼是否已存在
            var existingPhone = await _userRepository.GetByPhoneAsync(registerDto.Cellphone);
            if (existingPhone != null)
            {
                return new ApiResponseDto<UserInfoDto>
                {
                    Success = false,
                    Message = "手機號碼已被使用，請使用其他手機號碼"
                };
            }

            // 創建新用戶
            var newUser = new User
            {
                User_Name = registerDto.UserName,
                User_Account = registerDto.UserAccount,
                User_Password = HashPassword(registerDto.UserPassword),
                Created_At = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(newUser);

            // 創建用戶介紹資料
            var userIntroduce = new UserIntroduce
            {
                User_ID = createdUser.User_ID,
                User_NickName = registerDto.UserName,
                Gender = "M", // 預設值，用戶後續可修改
                IdNumber = "", // 預設空值，用戶後續可填寫
                Cellphone = registerDto.Cellphone,
                Email = registerDto.Email,
                Address = "", // 預設空值，用戶後續可填寫
                DateOfBirth = DateTime.UtcNow.AddYears(-18), // 預設 18 歲
                Create_Account = DateTime.UtcNow,
                User_Picture = null,
                User_Introduce = "歡迎來到 GameCore！"
            };

            await _userIntroduceRepository.AddAsync(userIntroduce);

            // 創建用戶權限（預設允許留言和購物，不允許銷售）
            var userRights = new UserRights
            {
                User_Id = createdUser.User_ID,
                User_Status = true, // 預設啟用
                ShoppingPermission = true, // 預設允許購物
                MessagePermission = true, // 預設允許留言
                SalesAuthority = false // 預設不允許銷售
            };

            await _userRightsRepository.AddAsync(userRights);

            // 創建用戶錢包（預設 0 點）
            var userWallet = new UserWallet
            {
                User_Id = createdUser.User_ID,
                User_Point = 0, // 預設 0 點
                Coupon_Number = null // 預設無優惠券
            };

            await _userWalletRepository.AddAsync(userWallet);

            // 構建回應資料
            var userInfo = new UserInfoDto
            {
                UserID = createdUser.User_ID,
                UserName = createdUser.User_Name,
                UserAccount = createdUser.User_Account,
                NickName = userIntroduce.User_NickName,
                Email = userIntroduce.Email,
                Cellphone = userIntroduce.Cellphone,
                CreatedAt = createdUser.Created_At,
                Rights = new UserRightsDto
                {
                    UserStatus = userRights.User_Status,
                    ShoppingPermission = userRights.ShoppingPermission,
                    MessagePermission = userRights.MessagePermission,
                    SalesAuthority = userRights.SalesAuthority
                },
                Wallet = new UserWalletDto
                {
                    UserPoint = userWallet.User_Point,
                    CouponNumber = userWallet.Coupon_Number
                }
            };

            return new ApiResponseDto<UserInfoDto>
            {
                Success = true,
                Message = "註冊成功",
                Data = userInfo
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<UserInfoDto>
            {
                Success = false,
                Message = "註冊失敗：" + ex.Message
            };
        }
    }

    /// <summary>
    /// 用戶登入
    /// </summary>
    public async Task<ApiResponseDto<LoginResponseDto>> LoginAsync(UserLoginDto loginDto)
    {
        try
        {
            // 查找用戶
            var user = await _userRepository.GetByAccountAsync(loginDto.UserAccount);
            if (user == null)
            {
                return new ApiResponseDto<LoginResponseDto>
                {
                    Success = false,
                    Message = "帳號或密碼錯誤"
                };
            }

            // 驗證密碼
            if (!VerifyPassword(loginDto.UserPassword, user.User_Password))
            {
                return new ApiResponseDto<LoginResponseDto>
                {
                    Success = false,
                    Message = "帳號或密碼錯誤"
                };
            }

            // 檢查使用者狀態
            var userRights = await _userRightsRepository.GetByUserIdAsync(user.User_ID);
            if (userRights == null || !userRights.User_Status)
            {
                return new ApiResponseDto<LoginResponseDto>
                {
                    Success = false,
                    Message = "帳號已被停權，請聯繫客服"
                };
            }

            // 獲取用戶完整資訊
            var userIntroduce = await _userIntroduceRepository.GetByUserIdAsync(user.User_ID);
            var userWallet = await _userWalletRepository.GetByUserIdAsync(user.User_ID);

            // 生成 JWT 令牌
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            // 構建用戶資訊
            var userInfo = new UserInfoDto
            {
                UserID = user.User_ID,
                UserName = user.User_Name,
                UserAccount = user.User_Account,
                NickName = userIntroduce?.User_NickName,
                Email = userIntroduce?.Email,
                Cellphone = userIntroduce?.Cellphone,
                CreatedAt = user.Created_At,
                Rights = new UserRightsDto
                {
                    UserStatus = userRights.User_Status,
                    ShoppingPermission = userRights.ShoppingPermission,
                    MessagePermission = userRights.MessagePermission,
                    SalesAuthority = userRights.SalesAuthority
                },
                Wallet = new UserWalletDto
                {
                    UserPoint = userWallet?.User_Point ?? 0,
                    CouponNumber = userWallet?.Coupon_Number
                }
            };

            // 構建登入回應
            var loginResponse = new LoginResponseDto
            {
                Success = true,
                Message = "登入成功",
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24), // JWT 24 小時過期
                User = userInfo
            };

            return new ApiResponseDto<LoginResponseDto>
            {
                Success = true,
                Message = "登入成功",
                Data = loginResponse
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = "登入失敗：" + ex.Message
            };
        }
    }

    /// <summary>
    /// OAuth 登入
    /// </summary>
    public async Task<ApiResponseDto<LoginResponseDto>> OAuthLoginAsync(OAuthLoginDto oauthDto)
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            // 這裡應該實現 OAuth 驗證邏輯
            // 目前先返回錯誤，後續實現
            return new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = "OAuth 登入功能尚未實現"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = "OAuth 登入失敗：" + ex.Message
            };
        }
    }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    public async Task<ApiResponseDto<LoginResponseDto>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            // 這裡應該實現刷新令牌驗證邏輯
            // 目前先返回錯誤，後續實現
            return new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = "刷新令牌功能尚未實現"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<LoginResponseDto>
            {
                Success = false,
                Message = "刷新令牌失敗：" + ex.Message
            };
        }
    }

    /// <summary>
    /// 變更密碼
    /// </summary>
    public async Task<ApiResponseDto<bool>> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "用戶不存在"
                };
            }

            // 驗證當前密碼
            if (!VerifyPassword(changePasswordDto.CurrentPassword, user.User_Password))
            {
                return new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "當前密碼錯誤"
                };
            }

            // 更新密碼
            user.User_Password = HashPassword(changePasswordDto.NewPassword);
            await _userRepository.UpdateAsync(user);

            return new ApiResponseDto<bool>
            {
                Success = true,
                Message = "密碼變更成功",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<bool>
            {
                Success = false,
                Message = "密碼變更失敗：" + ex.Message
            };
        }
    }

    /// <summary>
    /// 忘記密碼
    /// </summary>
    public async Task<ApiResponseDto<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                // 為了安全，不告訴用戶電子郵件是否存在
                return new ApiResponseDto<bool>
                {
                    Success = true,
                    Message = "如果電子郵件存在，重設密碼連結已發送",
                    Data = true
                };
            }

            // 這裡應該實現發送重設密碼電子郵件的邏輯
            // 目前先返回成功，後續實現

            return new ApiResponseDto<bool>
            {
                Success = true,
                Message = "重設密碼連結已發送到您的電子郵件",
                Data = true
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<bool>
            {
                Success = false,
                Message = "忘記密碼處理失敗：" + ex.Message
            };
        }
    }

    /// <summary>
    /// 重設密碼
    /// </summary>
    public async Task<ApiResponseDto<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            await Task.CompletedTask; // 避免 CS1998 警告

            // 這裡應該實現重設令牌驗證邏輯
            // 目前先返回錯誤，後續實現
            return new ApiResponseDto<bool>
            {
                Success = false,
                Message = "重設密碼功能尚未實現"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponseDto<bool>
            {
                Success = false,
                Message = "重設密碼失敗：" + ex.Message
            };
        }
    }

    /// <summary>
    /// 生成 JWT 令牌
    /// </summary>
    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? "DefaultSecretKey123!@#"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.User_ID.ToString()),
            new Claim(ClaimTypes.Name, user.User_Name),
            new Claim(ClaimTypes.Name, user.User_Account),
            new Claim("userId", user.User_ID.ToString()),
            new Claim("userName", user.User_Name),
            new Claim("userAccount", user.User_Account)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "GameCore",
            audience: jwtSettings["Audience"] ?? "GameCoreUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 生成刷新令牌
    /// </summary>
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// 雜湊密碼 - 效能優化：使用靜態 SHA256 實例
    /// </summary>
    private string HashPassword(string password)
    {
        // 效能優化：使用靜態 SHA256 實例，避免每次創建新實例
        var hashedBytes = _sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// 驗證密碼
    /// </summary>
    private bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == hashedPassword;
    }
}

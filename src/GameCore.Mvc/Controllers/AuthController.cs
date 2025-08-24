using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Shared.DTOs;
using GameCore.Domain.Interfaces;
using System.Security.Claims;

namespace GameCore.Mvc.Controllers;

/// <summary>
/// 身份驗證控制器 - 處理註冊、登入、OAuth 等功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IUserService userService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 使用者註冊
    /// </summary>
    /// <param name="request">註冊請求資料</param>
    /// <returns>註冊結果</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            // 檢查資料驗證
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "資料驗證失敗", 
                    errors = ModelState.SelectMany(x => x.Value!.Errors).Select(x => x.ErrorMessage) 
                });
            }

            // 檢查帳號是否已存在
            if (await _userService.IsAccountExistsAsync(request.User_Account))
            {
                return BadRequest(new { success = false, message = "此帳號已被使用" });
            }

            // 檢查暱稱是否已存在
            if (await _userService.IsNickNameExistsAsync(request.User_NickName))
            {
                return BadRequest(new { success = false, message = "此暱稱已被使用" });
            }

            // 檢查身分證字號是否已存在
            if (await _userService.IsIdNumberExistsAsync(request.IdNumber))
            {
                return BadRequest(new { success = false, message = "此身分證字號已被使用" });
            }

            // 檢查手機號碼是否已存在
            if (await _userService.IsCellphoneExistsAsync(request.Cellphone))
            {
                return BadRequest(new { success = false, message = "此手機號碼已被使用" });
            }

            // 檢查電子郵件是否已存在
            if (await _userService.IsEmailExistsAsync(request.Email))
            {
                return BadRequest(new { success = false, message = "此電子郵件已被使用" });
            }

            // 執行註冊
            var result = await _authService.RegisterAsync(request);

            if (result.Success)
            {
                _logger.LogInformation("使用者註冊成功: {UserAccount}", request.User_Account);
                return Ok(new { 
                    success = true, 
                    message = result.Message, 
                    data = result.Response 
                });
            }
            else
            {
                return BadRequest(new { success = false, message = result.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "註冊過程中發生錯誤: {UserAccount}", request.User_Account);
            return StatusCode(500, new { success = false, message = "註冊過程中發生錯誤，請稍後再試" });
        }
    }

    /// <summary>
    /// 使用者登入
    /// </summary>
    /// <param name="request">登入請求資料</param>
    /// <returns>登入結果</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "資料驗證失敗", 
                    errors = ModelState.SelectMany(x => x.Value!.Errors).Select(x => x.ErrorMessage) 
                });
            }

            var result = await _authService.LoginAsync(request);

            if (result.Success)
            {
                _logger.LogInformation("使用者登入成功: {UserAccount}", request.User_Account);
                return Ok(new { 
                    success = true, 
                    message = result.Message, 
                    data = result.Response 
                });
            }
            else
            {
                _logger.LogWarning("使用者登入失敗: {UserAccount}", request.User_Account);
                return Unauthorized(new { success = false, message = result.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登入過程中發生錯誤: {UserAccount}", request.User_Account);
            return StatusCode(500, new { success = false, message = "登入過程中發生錯誤，請稍後再試" });
        }
    }

    /// <summary>
    /// OAuth 登入
    /// </summary>
    /// <param name="request">OAuth 登入請求資料</param>
    /// <returns>登入結果</returns>
    [HttpPost("oauth")]
    [AllowAnonymous]
    public async Task<IActionResult> OAuthLogin([FromBody] OAuthLoginRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "資料驗證失敗", 
                    errors = ModelState.SelectMany(x => x.Value!.Errors).Select(x => x.ErrorMessage) 
                });
            }

            var result = await _authService.OAuthLoginAsync(request);

            if (result.Success)
            {
                _logger.LogInformation("OAuth 登入成功: {Provider}", request.Provider);
                return Ok(new { 
                    success = true, 
                    message = result.Message, 
                    data = result.Response 
                });
            }
            else
            {
                return Unauthorized(new { success = false, message = result.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OAuth 登入過程中發生錯誤: {Provider}", request.Provider);
            return StatusCode(500, new { success = false, message = "OAuth 登入過程中發生錯誤，請稍後再試" });
        }
    }

    /// <summary>
    /// 重新整理存取權杖
    /// </summary>
    /// <param name="refreshToken">重新整理權杖</param>
    /// <returns>新的存取權杖</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest(new { success = false, message = "重新整理權杖不能為空" });
            }

            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (result.Success)
            {
                return Ok(new { 
                    success = true, 
                    message = result.Message, 
                    data = result.Response 
                });
            }
            else
            {
                return Unauthorized(new { success = false, message = result.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重新整理權杖過程中發生錯誤");
            return StatusCode(500, new { success = false, message = "重新整理權杖過程中發生錯誤，請稍後再試" });
        }
    }

    /// <summary>
    /// 登出
    /// </summary>
    /// <returns>登出結果</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                var result = await _authService.LogoutAsync(userId);
                
                if (result)
                {
                    _logger.LogInformation("使用者登出成功: {UserId}", userId);
                    return Ok(new { success = true, message = "登出成功" });
                }
            }

            return BadRequest(new { success = false, message = "登出失敗" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "登出過程中發生錯誤");
            return StatusCode(500, new { success = false, message = "登出過程中發生錯誤，請稍後再試" });
        }
    }

    /// <summary>
    /// 取得目前使用者資料
    /// </summary>
    /// <returns>使用者資料</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                var userProfile = await _userService.GetUserProfileAsync(userId);
                
                if (userProfile != null)
                {
                    return Ok(new { success = true, data = userProfile });
                }
            }

            return NotFound(new { success = false, message = "找不到使用者資料" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得使用者資料過程中發生錯誤");
            return StatusCode(500, new { success = false, message = "取得使用者資料過程中發生錯誤，請稍後再試" });
        }
    }

    /// <summary>
    /// 更新個人資料
    /// </summary>
    /// <param name="request">更新請求資料</param>
    /// <returns>更新結果</returns>
    [HttpPut("profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "資料驗證失敗", 
                    errors = ModelState.SelectMany(x => x.Value!.Errors).Select(x => x.ErrorMessage) 
                });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                var result = await _userService.UpdateProfileAsync(userId, request);
                
                if (result.Success)
                {
                    _logger.LogInformation("使用者更新個人資料成功: {UserId}", userId);
                    return Ok(new { success = true, message = result.Message });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.Message });
                }
            }

            return Unauthorized(new { success = false, message = "無效的使用者" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新個人資料過程中發生錯誤");
            return StatusCode(500, new { success = false, message = "更新個人資料過程中發生錯誤，請稍後再試" });
        }
    }

    /// <summary>
    /// 修改密碼
    /// </summary>
    /// <param name="request">修改密碼請求資料</param>
    /// <returns>修改結果</returns>
    [HttpPut("password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "資料驗證失敗", 
                    errors = ModelState.SelectMany(x => x.Value!.Errors).Select(x => x.ErrorMessage) 
                });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                var result = await _userService.ChangePasswordAsync(userId, request);
                
                if (result.Success)
                {
                    _logger.LogInformation("使用者修改密碼成功: {UserId}", userId);
                    return Ok(new { success = true, message = result.Message });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.Message });
                }
            }

            return Unauthorized(new { success = false, message = "無效的使用者" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "修改密碼過程中發生錯誤");
            return StatusCode(500, new { success = false, message = "修改密碼過程中發生錯誤，請稍後再試" });
        }
    }

    /// <summary>
    /// 上傳頭像圖片
    /// </summary>
    /// <param name="file">圖片檔案</param>
    /// <returns>上傳結果</returns>
    [HttpPost("avatar")]
    [Authorize]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "請選擇要上傳的圖片檔案" });
            }

            // 檢查檔案大小 (限制 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { success = false, message = "圖片檔案大小不能超過 5MB" });
            }

            // 檢查檔案類型
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                return BadRequest(new { success = false, message = "只支援 JPEG、PNG、WebP 格式的圖片" });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var imageData = memoryStream.ToArray();

                var result = await _userService.UploadAvatarAsync(userId, imageData);
                
                if (result.Success)
                {
                    _logger.LogInformation("使用者上傳頭像成功: {UserId}", userId);
                    return Ok(new { success = true, message = result.Message });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.Message });
                }
            }

            return Unauthorized(new { success = false, message = "無效的使用者" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "上傳頭像過程中發生錯誤");
            return StatusCode(500, new { success = false, message = "上傳頭像過程中發生錯誤，請稍後再試" });
        }
    }
}
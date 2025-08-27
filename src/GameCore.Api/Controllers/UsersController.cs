using GameCore.Api.DTOs;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameCore.Api.Controllers;

/// <summary>
/// 使用者管理 API 控制器
/// 提供使用者基本資料的 CRUD 操作
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIntroduceRepository _userIntroduceRepository;
    private readonly IUserRightsRepository _userRightsRepository;
    private readonly IUserWalletRepository _userWalletRepository;
    private readonly IMemberSalesProfileRepository _memberSalesProfileRepository;
    private readonly IUserSalesInformationRepository _userSalesInformationRepository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserRepository userRepository,
        IUserIntroduceRepository userIntroduceRepository,
        IUserRightsRepository userRightsRepository,
        IUserWalletRepository userWalletRepository,
        IMemberSalesProfileRepository memberSalesProfileRepository,
        IUserSalesInformationRepository userSalesInformationRepository,
        ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _userIntroduceRepository = userIntroduceRepository;
        _userRightsRepository = userRightsRepository;
        _userWalletRepository = userWalletRepository;
        _memberSalesProfileRepository = memberSalesProfileRepository;
        _userSalesInformationRepository = userSalesInformationRepository;
        _logger = logger;
    }

    /// <summary>
    /// 取得目前登入使用者的完整資料
    /// </summary>
    /// <returns>使用者完整資料</returns>
    [HttpGet("me")]
    public async Task<ActionResult<UserCompleteResponse>> GetMe()
    {
        try
        {
            // 從 JWT Token 中取得使用者 ID
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("無效的使用者認證");
            }

            var user = await _userRepository.GetCompleteUserDataAsync(userId);
            if (user == null)
            {
                return NotFound("使用者不存在");
            }

            var response = MapToUserCompleteResponse(user);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得使用者資料時發生錯誤");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    /// <summary>
    /// 根據使用者 ID 取得使用者完整資料
    /// </summary>
    /// <param name="id">使用者 ID</param>
    /// <returns>使用者完整資料</returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserCompleteResponse>> GetUser(int id)
    {
        try
        {
            var user = await _userRepository.GetCompleteUserDataAsync(id);
            if (user == null)
            {
                return NotFound("使用者不存在");
            }

            var response = MapToUserCompleteResponse(user);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得使用者資料時發生錯誤: UserId = {UserId}", id);
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    /// <summary>
    /// 取得使用者列表（分頁）
    /// </summary>
    /// <param name="page">頁碼（從1開始）</param>
    /// <param name="pageSize">每頁筆數</param>
    /// <param name="search">搜尋關鍵字</param>
    /// <returns>使用者列表</returns>
    [HttpGet]
    public async Task<ActionResult<UsersListResponse>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var skip = (page - 1) * pageSize;
            var users = await _userRepository.GetUsersAsync(skip, pageSize, search);
            var totalCount = await _userRepository.GetUsersCountAsync(search);

            var userItems = users.Select(MapToUserListItemResponse).ToList();

            var response = new UsersListResponse
            {
                Users = userItems,
                TotalCount = totalCount,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得使用者列表時發生錯誤");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    /// <summary>
    /// 更新使用者介紹資料
    /// </summary>
    /// <param name="request">更新請求</param>
    /// <returns>更新後的使用者介紹資料</returns>
    [HttpPut("me/introduce")]
    public async Task<ActionResult<UserIntroduceResponse>> UpdateMyIntroduce(UpdateUserIntroduceRequest request)
    {
        try
        {
            // 從 JWT Token 中取得使用者 ID
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("無效的使用者認證");
            }

            // 檢查各種唯一性約束
            if (await _userIntroduceRepository.IsNicknameExistsAsync(request.UserNickName, userId))
            {
                return BadRequest("暱稱已被使用");
            }

            if (await _userIntroduceRepository.IsIdNumberExistsAsync(request.IdNumber, userId))
            {
                return BadRequest("身分證字號已被使用");
            }

            if (await _userIntroduceRepository.IsCellphoneExistsAsync(request.Cellphone, userId))
            {
                return BadRequest("電話號碼已被使用");
            }

            if (await _userIntroduceRepository.IsEmailExistsAsync(request.Email, userId))
            {
                return BadRequest("電子郵件已被使用");
            }

            // 取得或創建使用者介紹資料
            var userIntroduce = await _userIntroduceRepository.GetByUserIdAsync(userId);
            
            if (userIntroduce == null)
            {
                // 創建新的介紹資料
                userIntroduce = new UserIntroduce
                {
                    UserId = userId,
                    UserNickName = request.UserNickName,
                    Gender = request.Gender,
                    IdNumber = request.IdNumber,
                    Cellphone = request.Cellphone,
                    Email = request.Email,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth,
                    CreateAccount = DateTime.UtcNow,
                    UserIntroduceText = request.UserIntroduceText
                };
                
                await _userIntroduceRepository.CreateAsync(userIntroduce);
            }
            else
            {
                // 更新現有資料
                userIntroduce.UserNickName = request.UserNickName;
                userIntroduce.Gender = request.Gender;
                userIntroduce.IdNumber = request.IdNumber;
                userIntroduce.Cellphone = request.Cellphone;
                userIntroduce.Email = request.Email;
                userIntroduce.Address = request.Address;
                userIntroduce.DateOfBirth = request.DateOfBirth;
                userIntroduce.UserIntroduceText = request.UserIntroduceText;
                
                await _userIntroduceRepository.UpdateAsync(userIntroduce);
            }

            var response = MapToUserIntroduceResponse(userIntroduce);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新使用者介紹資料時發生錯誤");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    /// <summary>
    /// 申請銷售功能
    /// </summary>
    /// <param name="request">申請請求</param>
    /// <returns>申請結果</returns>
    [HttpPost("me/apply-sales")]
    public async Task<ActionResult<MemberSalesProfileResponse>> ApplySales(ApplySalesRequest request)
    {
        try
        {
            // 從 JWT Token 中取得使用者 ID
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("無效的使用者認證");
            }

            // 檢查是否已經申請過
            if (await _memberSalesProfileRepository.ExistsAsync(userId))
            {
                return BadRequest("您已經申請過銷售功能");
            }

            // 處理圖片資料
            byte[]? photoData = null;
            if (!string.IsNullOrEmpty(request.AccountCoverPhotoBase64))
            {
                try
                {
                    photoData = Convert.FromBase64String(request.AccountCoverPhotoBase64);
                }
                catch
                {
                    return BadRequest("圖片格式不正確");
                }
            }

            // 創建銷售資料
            var salesProfile = new MemberSalesProfile
            {
                UserId = userId,
                BankCode = request.BankCode,
                BankAccountNumber = request.BankAccountNumber,
                AccountCoverPhoto = photoData
            };

            await _memberSalesProfileRepository.CreateAsync(salesProfile);

            // 同時創建銷售資訊記錄
            var salesInfo = new UserSalesInformation
            {
                UserId = userId,
                UserSalesWallet = 0
            };

            await _userSalesInformationRepository.CreateAsync(salesInfo);

            // 注意：這裡不自動開啟銷售權限，需要管理員審核後才開啟
            _logger.LogInformation("使用者申請銷售功能: UserId = {UserId}", userId);

            var response = new MemberSalesProfileResponse
            {
                BankCode = salesProfile.BankCode,
                BankAccountNumber = salesProfile.BankAccountNumber,
                HasAccountCoverPhoto = salesProfile.AccountCoverPhoto != null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "申請銷售功能時發生錯誤");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    /// <summary>
    /// 取得我的錢包資訊
    /// </summary>
    /// <returns>錢包資訊</returns>
    [HttpGet("me/wallet")]
    public async Task<ActionResult<UserWalletResponse>> GetMyWallet()
    {
        try
        {
            // 從 JWT Token 中取得使用者 ID
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("無效的使用者認證");
            }

            var wallet = await _userWalletRepository.GetByUserIdAsync(userId);
            if (wallet == null)
            {
                return NotFound("錢包資料不存在");
            }

            var response = new UserWalletResponse
            {
                UserPoint = wallet.UserPoint,
                CouponNumber = wallet.CouponNumber
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得錢包資訊時發生錯誤");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    /// <summary>
    /// 取得我的銷售錢包資訊
    /// </summary>
    /// <returns>銷售錢包資訊</returns>
    [HttpGet("me/sales-wallet")]
    public async Task<ActionResult<UserSalesInformationResponse>> GetMySalesWallet()
    {
        try
        {
            // 從 JWT Token 中取得使用者 ID
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("無效的使用者認證");
            }

            var salesInfo = await _userSalesInformationRepository.GetByUserIdAsync(userId);
            if (salesInfo == null)
            {
                return NotFound("銷售錢包資料不存在，請先申請銷售功能");
            }

            var response = new UserSalesInformationResponse
            {
                UserSalesWallet = salesInfo.UserSalesWallet
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得銷售錢包資訊時發生錯誤");
            return StatusCode(500, "伺服器內部錯誤");
        }
    }

    #region Private Methods

    /// <summary>
    /// 將使用者實體轉換為完整回應 DTO
    /// </summary>
    private static UserCompleteResponse MapToUserCompleteResponse(User user)
    {
        return new UserCompleteResponse
        {
            UserId = user.UserId,
            UserName = user.UserName,
            UserAccount = user.UserAccount,
            UserIntroduce = user.UserIntroduce != null ? MapToUserIntroduceResponse(user.UserIntroduce) : null,
            UserRights = user.UserRights != null ? MapToUserRightsResponse(user.UserRights) : null,
            Wallet = user.Wallet != null ? new UserWalletResponse 
            { 
                UserPoint = user.Wallet.UserPoint, 
                CouponNumber = user.Wallet.CouponNumber 
            } : null,
            MemberSalesProfile = user.MemberSalesProfile != null ? new MemberSalesProfileResponse
            {
                BankCode = user.MemberSalesProfile.BankCode,
                BankAccountNumber = user.MemberSalesProfile.BankAccountNumber,
                HasAccountCoverPhoto = user.MemberSalesProfile.AccountCoverPhoto != null
            } : null,
            UserSalesInformation = user.UserSalesInformation != null ? new UserSalesInformationResponse
            {
                UserSalesWallet = user.UserSalesInformation.UserSalesWallet
            } : null
        };
    }

    /// <summary>
    /// 將使用者介紹實體轉換為回應 DTO
    /// </summary>
    private static UserIntroduceResponse MapToUserIntroduceResponse(UserIntroduce userIntroduce)
    {
        return new UserIntroduceResponse
        {
            UserNickName = userIntroduce.UserNickName,
            Gender = userIntroduce.Gender,
            IdNumber = userIntroduce.IdNumber,
            Cellphone = userIntroduce.Cellphone,
            Email = userIntroduce.Email,
            Address = userIntroduce.Address,
            DateOfBirth = userIntroduce.DateOfBirth,
            CreateAccount = userIntroduce.CreateAccount,
            UserIntroduceText = userIntroduce.UserIntroduceText,
            HasUserPicture = userIntroduce.UserPicture != null
        };
    }

    /// <summary>
    /// 將使用者權限實體轉換為回應 DTO
    /// </summary>
    private static UserRightsResponse MapToUserRightsResponse(UserRights userRights)
    {
        return new UserRightsResponse
        {
            UserStatus = userRights.UserStatus,
            ShoppingPermission = userRights.ShoppingPermission,
            MessagePermission = userRights.MessagePermission,
            SalesAuthority = userRights.SalesAuthority
        };
    }

    /// <summary>
    /// 將使用者實體轉換為列表項目回應 DTO
    /// </summary>
    private static UserListItemResponse MapToUserListItemResponse(User user)
    {
        return new UserListItemResponse
        {
            UserId = user.UserId,
            UserName = user.UserName,
            UserAccount = user.UserAccount,
            UserNickName = user.UserIntroduce?.UserNickName,
            UserStatus = user.UserRights?.UserStatus ?? true,
            ShoppingPermission = user.UserRights?.ShoppingPermission ?? true,
            MessagePermission = user.UserRights?.MessagePermission ?? true,
            SalesAuthority = user.UserRights?.SalesAuthority ?? false,
            UserPoint = user.Wallet?.UserPoint ?? 0,
            HasSalesProfile = user.MemberSalesProfile != null
        };
    }

    #endregion
}
using System.ComponentModel.DataAnnotations;

namespace GameCore.Api.DTOs;

/// <summary>
/// 使用者註冊請求 DTO
/// </summary>
public class RegisterUserRequest
{
    /// <summary>
    /// 使用者姓名
    /// </summary>
    [Required(ErrorMessage = "使用者姓名為必填")]
    [StringLength(100, ErrorMessage = "使用者姓名長度不能超過100字元")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號
    /// </summary>
    [Required(ErrorMessage = "登入帳號為必填")]
    [StringLength(100, ErrorMessage = "登入帳號長度不能超過100字元")]
    public string UserAccount { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required(ErrorMessage = "密碼為必填")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "密碼長度必須在6-255字元之間")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 確認密碼
    /// </summary>
    [Required(ErrorMessage = "確認密碼為必填")]
    [Compare("Password", ErrorMessage = "密碼與確認密碼不一致")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// 使用者登入請求 DTO
/// </summary>
public class LoginUserRequest
{
    /// <summary>
    /// 登入帳號
    /// </summary>
    [Required(ErrorMessage = "登入帳號為必填")]
    public string UserAccount { get; set; } = string.Empty;

    /// <summary>
    /// 密碼
    /// </summary>
    [Required(ErrorMessage = "密碼為必填")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 使用者介紹資料更新請求 DTO
/// </summary>
public class UpdateUserIntroduceRequest
{
    /// <summary>
    /// 使用者暱稱
    /// </summary>
    [Required(ErrorMessage = "使用者暱稱為必填")]
    [StringLength(100, ErrorMessage = "暱稱長度不能超過100字元")]
    public string UserNickName { get; set; } = string.Empty;

    /// <summary>
    /// 性別 (M/F)
    /// </summary>
    [Required(ErrorMessage = "性別為必填")]
    [RegularExpression("^[MF]$", ErrorMessage = "性別只能是 M 或 F")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// 身分證字號
    /// </summary>
    [Required(ErrorMessage = "身分證字號為必填")]
    [StringLength(20, ErrorMessage = "身分證字號長度不能超過20字元")]
    public string IdNumber { get; set; } = string.Empty;

    /// <summary>
    /// 聯繫電話
    /// </summary>
    [Required(ErrorMessage = "聯繫電話為必填")]
    [StringLength(20, ErrorMessage = "電話長度不能超過20字元")]
    public string Cellphone { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    [Required(ErrorMessage = "電子郵件為必填")]
    [EmailAddress(ErrorMessage = "電子郵件格式不正確")]
    [StringLength(100, ErrorMessage = "Email長度不能超過100字元")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    [Required(ErrorMessage = "地址為必填")]
    [StringLength(200, ErrorMessage = "地址長度不能超過200字元")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 出生年月日
    /// </summary>
    [Required(ErrorMessage = "出生年月日為必填")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// 使用者自介
    /// </summary>
    [StringLength(200, ErrorMessage = "自介長度不能超過200字元")]
    public string? UserIntroduceText { get; set; }
}

/// <summary>
/// 銷售功能申請請求 DTO
/// </summary>
public class ApplySalesRequest
{
    /// <summary>
    /// 銀行代號
    /// </summary>
    [Required(ErrorMessage = "銀行代號為必填")]
    public int BankCode { get; set; }

    /// <summary>
    /// 銀行帳號
    /// </summary>
    [Required(ErrorMessage = "銀行帳號為必填")]
    [StringLength(50, ErrorMessage = "銀行帳號長度不能超過50字元")]
    public string BankAccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// 帳戶封面照片 (Base64)
    /// </summary>
    public string? AccountCoverPhotoBase64 { get; set; }
}

/// <summary>
/// 使用者基本資訊回應 DTO
/// </summary>
public class UserBasicResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserAccount { get; set; } = string.Empty;
}

/// <summary>
/// 使用者完整資訊回應 DTO
/// </summary>
public class UserCompleteResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserAccount { get; set; } = string.Empty;
    
    // 使用者介紹資料
    public UserIntroduceResponse? UserIntroduce { get; set; }
    
    // 使用者權限
    public UserRightsResponse? UserRights { get; set; }
    
    // 使用者錢包
    public UserWalletResponse? Wallet { get; set; }
    
    // 銷售資料 (如果有)
    public MemberSalesProfileResponse? MemberSalesProfile { get; set; }
    
    // 銷售資訊 (如果有)
    public UserSalesInformationResponse? UserSalesInformation { get; set; }
}

/// <summary>
/// 使用者介紹資訊回應 DTO
/// </summary>
public class UserIntroduceResponse
{
    public string UserNickName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string Cellphone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime CreateAccount { get; set; }
    public string? UserIntroduceText { get; set; }
    public bool HasUserPicture { get; set; }  // 是否有頭像，不直接傳送圖片資料
}

/// <summary>
/// 使用者權限回應 DTO
/// </summary>
public class UserRightsResponse
{
    public bool UserStatus { get; set; }
    public bool ShoppingPermission { get; set; }
    public bool MessagePermission { get; set; }
    public bool SalesAuthority { get; set; }
}

/// <summary>
/// 使用者錢包回應 DTO
/// </summary>
public class UserWalletResponse
{
    public int UserPoint { get; set; }
    public string? CouponNumber { get; set; }
}

/// <summary>
/// 會員銷售資料回應 DTO
/// </summary>
public class MemberSalesProfileResponse
{
    public int? BankCode { get; set; }
    public string? BankAccountNumber { get; set; }
    public bool HasAccountCoverPhoto { get; set; }  // 是否有封面照片
}

/// <summary>
/// 使用者銷售資訊回應 DTO
/// </summary>
public class UserSalesInformationResponse
{
    public int UserSalesWallet { get; set; }
}

/// <summary>
/// 登入回應 DTO
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserBasicResponse User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// 使用者列表查詢回應 DTO
/// </summary>
public class UsersListResponse
{
    public IEnumerable<UserListItemResponse> Users { get; set; } = new List<UserListItemResponse>();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// 使用者列表項目回應 DTO
/// </summary>
public class UserListItemResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserAccount { get; set; } = string.Empty;
    public string? UserNickName { get; set; }
    public bool UserStatus { get; set; }
    public bool ShoppingPermission { get; set; }
    public bool MessagePermission { get; set; }
    public bool SalesAuthority { get; set; }
    public int UserPoint { get; set; }
    public bool HasSalesProfile { get; set; }
}
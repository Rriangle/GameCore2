using System.ComponentModel.DataAnnotations;

namespace GameCore.Api.Models.Auth;

/// <summary>
/// 用戶註冊請求模型
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// 使用者姓名 (必填)
    /// </summary>
    [Required(ErrorMessage = "使用者姓名為必填項目")]
    [StringLength(100, ErrorMessage = "使用者姓名不能超過100個字元")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 登入帳號 (必填，唯一)
    /// </summary>
    [Required(ErrorMessage = "登入帳號為必填項目")]
    [StringLength(100, ErrorMessage = "登入帳號不能超過100個字元")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "登入帳號只能包含英文字母、數字和底線")]
    public string UserAccount { get; set; } = string.Empty;

    /// <summary>
    /// 使用者密碼 (必填)
    /// </summary>
    [Required(ErrorMessage = "密碼為必填項目")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "密碼長度必須在8-255個字元之間")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
        ErrorMessage = "密碼必須包含至少一個大寫字母、一個小寫字母、一個數字和一個特殊字元")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 確認密碼 (必填)
    /// </summary>
    [Required(ErrorMessage = "確認密碼為必填項目")]
    [Compare("Password", ErrorMessage = "密碼與確認密碼不符")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件 (必填，唯一)
    /// </summary>
    [Required(ErrorMessage = "電子郵件為必填項目")]
    [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
    [StringLength(100, ErrorMessage = "電子郵件不能超過100個字元")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 使用者暱稱 (必填，唯一)
    /// </summary>
    [Required(ErrorMessage = "使用者暱稱為必填項目")]
    [StringLength(100, ErrorMessage = "使用者暱稱不能超過100個字元")]
    public string UserNickName { get; set; } = string.Empty;

    /// <summary>
    /// 性別 (必填)
    /// </summary>
    [Required(ErrorMessage = "性別為必填項目")]
    [RegularExpression(@"^[MF]$", ErrorMessage = "性別必須是 M 或 F")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// 身分證字號 (必填，唯一)
    /// </summary>
    [Required(ErrorMessage = "身分證字號為必填項目")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "身分證字號必須是10個字元")]
    [RegularExpression(@"^[A-Z][12]\d{8}$", ErrorMessage = "請輸入有效的身分證字號")]
    public string IdNumber { get; set; } = string.Empty;

    /// <summary>
    /// 聯繫電話 (必填，唯一)
    /// </summary>
    [Required(ErrorMessage = "聯繫電話為必填項目")]
    [StringLength(20, ErrorMessage = "聯繫電話不能超過20個字元")]
    [RegularExpression(@"^09\d{8}$", ErrorMessage = "請輸入有效的手機號碼")]
    public string Cellphone { get; set; } = string.Empty;

    /// <summary>
    /// 地址 (必填)
    /// </summary>
    [Required(ErrorMessage = "地址為必填項目")]
    [StringLength(500, ErrorMessage = "地址不能超過500個字元")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 出生年月日 (必填)
    /// </summary>
    [Required(ErrorMessage = "出生年月日為必填項目")]
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// 使用者自介 (可選)
    /// </summary>
    [StringLength(200, ErrorMessage = "使用者自介不能超過200個字元")]
    public string? UserIntroduce { get; set; }
} 
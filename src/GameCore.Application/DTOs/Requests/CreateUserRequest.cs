using System.ComponentModel.DataAnnotations;

namespace GameCore.Application.DTOs.Requests
{
    /// <summary>
    /// 建立使用者請求
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// 使用者名稱
        /// </summary>
        [Required(ErrorMessage = "使用者名稱不能為空")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "使用者名稱長度必須在 3-50 字元之間")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "使用者名稱只能包含字母、數字和底線")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 電子郵件
        /// </summary>
        [Required(ErrorMessage = "電子郵件不能為空")]
        [EmailAddress(ErrorMessage = "電子郵件格式不正確")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 密碼
        /// </summary>
        [Required(ErrorMessage = "密碼不能為空")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度必須在 6-100 字元之間")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 確認密碼
        /// </summary>
        [Required(ErrorMessage = "確認密碼不能為空")]
        [Compare("Password", ErrorMessage = "密碼和確認密碼不匹配")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// 暱稱
        /// </summary>
        [StringLength(50, ErrorMessage = "暱稱長度不能超過 50 字元")]
        public string? Nickname { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// 性別
        /// </summary>
        public string? Gender { get; set; }
    }
} 
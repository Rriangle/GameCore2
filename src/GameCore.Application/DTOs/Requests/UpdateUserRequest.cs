using System.ComponentModel.DataAnnotations;

namespace GameCore.Application.DTOs.Requests
{
    /// <summary>
    /// 更新使用者請求
    /// </summary>
    public class UpdateUserRequest
    {
        /// <summary>
        /// 暱稱
        /// </summary>
        [StringLength(50, ErrorMessage = "暱稱長度不能超過 50 字元")]
        public string? Nickname { get; set; }

        /// <summary>
        /// 電子郵件
        /// </summary>
        [EmailAddress(ErrorMessage = "電子郵件格式不正確")]
        public string? Email { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// 性別
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// 個人簡介
        /// </summary>
        [StringLength(500, ErrorMessage = "個人簡介長度不能超過 500 字元")]
        public string? Bio { get; set; }

        /// <summary>
        /// 頭像 URL
        /// </summary>
        [Url(ErrorMessage = "頭像 URL 格式不正確")]
        public string? AvatarUrl { get; set; }
    }
} 
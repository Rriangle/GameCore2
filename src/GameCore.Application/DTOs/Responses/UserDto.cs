namespace GameCore.Application.DTOs.Responses
{
    /// <summary>
    /// 使用者資料回應
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 使用者識別碼
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 電子郵件
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 暱稱
        /// </summary>
        public string? Nickname { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// 性別
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// 使用者狀態
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 註冊時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 最後登入時間
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// 使用者角色
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
    }
} 
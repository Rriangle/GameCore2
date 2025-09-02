namespace GameCore.Mvc.Models
{
    /// <summary>
    /// 錯誤視圖模型
    /// 用於錯誤頁面顯示錯誤信息
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// 請求 ID
        /// 用於追蹤和除錯錯誤
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// 是否顯示請求 ID
        /// 在開發環境中顯示，生產環境中隱藏
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
} 
namespace GameCore.Mvc.Models
{
    /// <summary>
    /// 麵包屑導航視圖模型
    /// 用於麵包屑導航顯示
    /// </summary>
    public class BreadcrumbViewModel
    {
        /// <summary>
        /// 麵包屑文字
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 麵包屑連結
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 是否為當前頁面
        /// </summary>
        public bool IsActive { get; set; }
    }
} 
using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces
{
    /// <summary>
    /// 洞察服務接口
    /// </summary>
    public interface IInsightService
    {
        /// <summary>
        /// 獲取洞察貼文列表
        /// </summary>
        Task<IEnumerable<Post>> GetInsightsAsync(int? gameId = null, string? status = null, bool? pinned = null);
        
        /// <summary>
        /// 獲取洞察貼文詳情
        /// </summary>
        Task<Post?> GetInsightAsync(int postId);
        
        /// <summary>
        /// 創建洞察貼文
        /// </summary>
        Task<Post> CreateInsightAsync(Post post);
        
        /// <summary>
        /// 更新洞察貼文
        /// </summary>
        Task<Post> UpdateInsightAsync(int postId, Post post);
        
        /// <summary>
        /// 刪除洞察貼文
        /// </summary>
        Task<bool> DeleteInsightAsync(int postId);
        
        /// <summary>
        /// 發布洞察貼文
        /// </summary>
        Task<Post> PublishInsightAsync(int postId);
        
        /// <summary>
        /// 設置/取消置頂
        /// </summary>
        Task<Post> TogglePinAsync(int postId);
        
        /// <summary>
        /// 獲取熱門洞察貼文
        /// </summary>
        Task<IEnumerable<Post>> GetPopularInsightsAsync(int limit = 10);
        
        /// <summary>
        /// 獲取遊戲相關洞察貼文
        /// </summary>
        Task<IEnumerable<Post>> GetGameInsightsAsync(int gameId, int limit = 20);
    }
}
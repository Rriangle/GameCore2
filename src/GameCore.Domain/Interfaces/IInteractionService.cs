using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces
{
    /// <summary>
    /// 互動服務接口
    /// </summary>
    public interface IInteractionService
    {
        /// <summary>
        /// 添加反應（讚、表情等）
        /// </summary>
        Task<Reaction> AddReactionAsync(int userId, string targetType, long targetId, string kind);
        
        /// <summary>
        /// 移除反應
        /// </summary>
        Task<bool> RemoveReactionAsync(int userId, string targetType, long targetId, string kind);
        
        /// <summary>
        /// 獲取目標的反應統計
        /// </summary>
        Task<object> GetReactionStatsAsync(string targetType, long targetId);
        
        /// <summary>
        /// 檢查用戶是否已反應
        /// </summary>
        Task<bool> HasUserReactedAsync(int userId, string targetType, long targetId, string kind);
        
        /// <summary>
        /// 添加收藏
        /// </summary>
        Task<Bookmark> AddBookmarkAsync(int userId, string targetType, long targetId);
        
        /// <summary>
        /// 移除收藏
        /// </summary>
        Task<bool> RemoveBookmarkAsync(int userId, string targetType, long targetId);
        
        /// <summary>
        /// 獲取用戶收藏列表
        /// </summary>
        Task<IEnumerable<Bookmark>> GetUserBookmarksAsync(int userId, string? targetType = null);
        
        /// <summary>
        /// 檢查用戶是否已收藏
        /// </summary>
        Task<bool> HasUserBookmarkedAsync(int userId, string targetType, long targetId);
        
        /// <summary>
        /// 獲取收藏統計
        /// </summary>
        Task<int> GetBookmarkCountAsync(string targetType, long targetId);
        
        /// <summary>
        /// 獲取用戶反應列表
        /// </summary>
        Task<IEnumerable<Reaction>> GetUserReactionsAsync(int userId, string? targetType = null);
    }
}
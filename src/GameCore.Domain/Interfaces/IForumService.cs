using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces
{
    /// <summary>
    /// 論壇服務接口
    /// </summary>
    public interface IForumService
    {
        /// <summary>
        /// 獲取所有論壇
        /// </summary>
        Task<IEnumerable<Forum>> GetAllForumsAsync();
        
        /// <summary>
        /// 獲取論壇詳情
        /// </summary>
        Task<Forum?> GetForumAsync(int forumId);
        
        /// <summary>
        /// 獲取論壇主題列表
        /// </summary>
        Task<IEnumerable<Thread>> GetForumThreadsAsync(int forumId, int page = 1, int pageSize = 20);
        
        /// <summary>
        /// 獲取主題詳情
        /// </summary>
        Task<Thread?> GetThreadAsync(long threadId);
        
        /// <summary>
        /// 創建主題
        /// </summary>
        Task<Thread> CreateThreadAsync(Thread thread);
        
        /// <summary>
        /// 更新主題
        /// </summary>
        Task<Thread> UpdateThreadAsync(long threadId, Thread thread);
        
        /// <summary>
        /// 刪除主題
        /// </summary>
        Task<bool> DeleteThreadAsync(long threadId);
        
        /// <summary>
        /// 獲取主題回覆
        /// </summary>
        Task<IEnumerable<ThreadPost>> GetThreadPostsAsync(long threadId, int page = 1, int pageSize = 20);
        
        /// <summary>
        /// 創建回覆
        /// </summary>
        Task<ThreadPost> CreatePostAsync(ThreadPost post);
        
        /// <summary>
        /// 更新回覆
        /// </summary>
        Task<ThreadPost> UpdatePostAsync(long postId, ThreadPost post);
        
        /// <summary>
        /// 刪除回覆
        /// </summary>
        Task<bool> DeletePostAsync(long postId);
        
        /// <summary>
        /// 獲取熱門主題
        /// </summary>
        Task<IEnumerable<Thread>> GetPopularThreadsAsync(int limit = 10);
        
        /// <summary>
        /// 搜索主題和回覆
        /// </summary>
        Task<IEnumerable<object>> SearchAsync(string keyword, int page = 1, int pageSize = 20);
    }
}
using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 論壇服務介面，定義論壇、討論串、回覆、反應與收藏相關的業務邏輯
/// </summary>
public interface IForumsService
{
    /// <summary>
    /// 取得所有論壇列表
    /// </summary>
    /// <returns>論壇列表</returns>
    Task<List<ForumDto>> GetForumsAsync();

    /// <summary>
    /// 取得論壇詳細資訊
    /// </summary>
    /// <param name="forumId">論壇編號</param>
    /// <returns>論壇詳細資訊，不存在則返回 null</returns>
    Task<ForumDetailDto?> GetForumDetailAsync(int forumId);

    /// <summary>
    /// 取得論壇的討論串列表 (分頁查詢)
    /// </summary>
    /// <param name="forumId">論壇編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁討論串列表</returns>
    Task<PagedThreadsDto> GetForumThreadsAsync(int forumId, ThreadQueryDto query);

    /// <summary>
    /// 建立新討論串
    /// </summary>
    /// <param name="forumId">論壇編號</param>
    /// <param name="authorId">作者會員編號</param>
    /// <param name="request">建立討論串請求</param>
    /// <returns>建立的討論串資料</returns>
    Task<ThreadDto> CreateThreadAsync(int forumId, int authorId, CreateThreadRequestDto request);

    /// <summary>
    /// 取得討論串詳細資訊
    /// </summary>
    /// <param name="threadId">討論串編號</param>
    /// <returns>討論串詳細資訊，不存在則返回 null</returns>
    Task<ThreadDetailDto?> GetThreadDetailAsync(long threadId);

    /// <summary>
    /// 取得討論串的回覆列表 (分頁查詢)
    /// </summary>
    /// <param name="threadId">討論串編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁回覆列表</returns>
    Task<PagedThreadPostsDto> GetThreadPostsAsync(long threadId, ThreadPostQueryDto query);

    /// <summary>
    /// 建立回覆
    /// </summary>
    /// <param name="threadId">討論串編號</param>
    /// <param name="authorId">回覆者會員編號</param>
    /// <param name="request">建立回覆請求</param>
    /// <returns>建立的回覆資料</returns>
    Task<ThreadPostDto> CreateThreadPostAsync(long threadId, int authorId, CreateThreadPostRequestDto request);

    /// <summary>
    /// 更新回覆內容
    /// </summary>
    /// <param name="postId">回覆編號</param>
    /// <param name="authorId">回覆者會員編號 (用於驗證權限)</param>
    /// <param name="request">更新回覆請求</param>
    /// <returns>更新後的回覆資料，不存在或無權限則返回 null</returns>
    Task<ThreadPostDto?> UpdateThreadPostAsync(long postId, int authorId, UpdateThreadPostRequestDto request);

    /// <summary>
    /// 刪除回覆
    /// </summary>
    /// <param name="postId">回覆編號</param>
    /// <param name="authorId">回覆者會員編號 (用於驗證權限)</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteThreadPostAsync(long postId, int authorId);

    /// <summary>
    /// 新增反應 (讚/emoji 等)
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="request">反應請求</param>
    /// <returns>反應結果</returns>
    Task<ReactionResultDto> AddReactionAsync(int userId, AddReactionRequestDto request);

    /// <summary>
    /// 移除反應
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="request">移除反應請求</param>
    /// <returns>是否移除成功</returns>
    Task<bool> RemoveReactionAsync(int userId, RemoveReactionRequestDto request);

    /// <summary>
    /// 取得目標的反應統計
    /// </summary>
    /// <param name="targetType">目標類型</param>
    /// <param name="targetId">目標編號</param>
    /// <returns>反應統計列表</returns>
    Task<List<ReactionStatsDto>> GetReactionStatsAsync(string targetType, long targetId);

    /// <summary>
    /// 新增收藏
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="request">收藏請求</param>
    /// <returns>收藏結果</returns>
    Task<BookmarkResultDto> AddBookmarkAsync(int userId, AddBookmarkRequestDto request);

    /// <summary>
    /// 移除收藏
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="request">移除收藏請求</param>
    /// <returns>是否移除成功</returns>
    Task<bool> RemoveBookmarkAsync(int userId, RemoveBookmarkRequestDto request);

    /// <summary>
    /// 取得使用者的收藏列表 (分頁查詢)
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁收藏列表</returns>
    Task<PagedBookmarksDto> GetUserBookmarksAsync(int userId, BookmarkQueryDto query);

    /// <summary>
    /// 搜尋討論串
    /// </summary>
    /// <param name="query">搜尋條件</param>
    /// <returns>分頁搜尋結果</returns>
    Task<PagedThreadsDto> SearchThreadsAsync(ThreadSearchQueryDto query);

    /// <summary>
    /// 取得熱門討論串
    /// </summary>
    /// <param name="forumId">論壇編號 (可選)</param>
    /// <param name="limit">取得數量</param>
    /// <returns>熱門討論串列表</returns>
    Task<List<ThreadDto>> GetPopularThreadsAsync(int? forumId = null, int limit = 10);

    /// <summary>
    /// 取得最新討論串
    /// </summary>
    /// <param name="forumId">論壇編號 (可選)</param>
    /// <param name="limit">取得數量</param>
    /// <returns>最新討論串列表</returns>
    Task<List<ThreadDto>> GetLatestThreadsAsync(int? forumId = null, int limit = 10);
}
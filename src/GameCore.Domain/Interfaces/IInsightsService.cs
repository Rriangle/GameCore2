using GameCore.Shared.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 洞察服務介面，定義洞察貼文、數據快照相關的業務邏輯
/// </summary>
public interface IInsightsService
{
    /// <summary>
    /// 取得洞察貼文列表 (分頁查詢)
    /// </summary>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁洞察貼文列表</returns>
    Task<PagedPostsDto> GetInsightPostsAsync(PostQueryDto query);

    /// <summary>
    /// 取得洞察貼文詳情
    /// </summary>
    /// <param name="postId">貼文編號</param>
    /// <returns>洞察貼文詳情，不存在則返回 null</returns>
    Task<PostDetailDto?> GetInsightPostDetailAsync(int postId);

    /// <summary>
    /// 建立洞察貼文
    /// </summary>
    /// <param name="authorId">作者編號</param>
    /// <param name="request">建立貼文請求</param>
    /// <returns>建立的貼文資料</returns>
    Task<PostDto> CreateInsightPostAsync(int authorId, CreatePostRequestDto request);

    /// <summary>
    /// 更新洞察貼文
    /// </summary>
    /// <param name="postId">貼文編號</param>
    /// <param name="authorId">作者編號 (用於權限驗證)</param>
    /// <param name="request">更新貼文請求</param>
    /// <returns>更新後的貼文資料，不存在或無權限則返回 null</returns>
    Task<PostDto?> UpdateInsightPostAsync(int postId, int authorId, UpdatePostRequestDto request);

    /// <summary>
    /// 刪除洞察貼文
    /// </summary>
    /// <param name="postId">貼文編號</param>
    /// <param name="authorId">作者編號 (用於權限驗證)</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> DeleteInsightPostAsync(int postId, int authorId);

    /// <summary>
    /// 發佈洞察貼文 (從草稿到發佈狀態)
    /// </summary>
    /// <param name="postId">貼文編號</param>
    /// <param name="authorId">作者編號 (用於權限驗證)</param>
    /// <returns>發佈後的貼文資料，不存在或無權限則返回 null</returns>
    Task<PostDto?> PublishInsightPostAsync(int postId, int authorId);

    /// <summary>
    /// 取得貼文的指標快照
    /// </summary>
    /// <param name="postId">貼文編號</param>
    /// <returns>指標快照，不存在則返回 null</returns>
    Task<PostMetricSnapshotDto?> GetPostMetricSnapshotAsync(int postId);

    /// <summary>
    /// 建立貼文指標快照 (發佈時自動建立)
    /// </summary>
    /// <param name="postId">貼文編號</param>
    /// <param name="gameId">遊戲編號</param>
    /// <param name="date">快照日期</param>
    /// <returns>建立的指標快照</returns>
    Task<PostMetricSnapshotDto> CreatePostMetricSnapshotAsync(int postId, int gameId, DateOnly date);

    /// <summary>
    /// 取得貼文的引用來源
    /// </summary>
    /// <param name="postId">貼文編號</param>
    /// <returns>引用來源列表</returns>
    Task<List<PostSourceDto>> GetPostSourcesAsync(int postId);

    /// <summary>
    /// 新增貼文引用來源
    /// </summary>
    /// <param name="postId">貼文編號</param>
    /// <param name="request">新增來源請求</param>
    /// <returns>新增的來源資料</returns>
    Task<PostSourceDto> AddPostSourceAsync(int postId, AddPostSourceRequestDto request);

    /// <summary>
    /// 移除貼文引用來源
    /// </summary>
    /// <param name="sourceId">來源編號</param>
    /// <param name="authorId">作者編號 (用於權限驗證)</param>
    /// <returns>是否移除成功</returns>
    Task<bool> RemovePostSourceAsync(long sourceId, int authorId);

    /// <summary>
    /// 搜尋洞察貼文
    /// </summary>
    /// <param name="query">搜尋條件</param>
    /// <returns>分頁搜尋結果</returns>
    Task<PagedPostsDto> SearchInsightPostsAsync(PostSearchQueryDto query);

    /// <summary>
    /// 取得熱門洞察貼文
    /// </summary>
    /// <param name="gameId">遊戲編號 (可選)</param>
    /// <param name="limit">取得數量</param>
    /// <returns>熱門洞察貼文列表</returns>
    Task<List<PostDto>> GetPopularInsightPostsAsync(int? gameId = null, int limit = 10);

    /// <summary>
    /// 取得最新洞察貼文
    /// </summary>
    /// <param name="gameId">遊戲編號 (可選)</param>
    /// <param name="limit">取得數量</param>
    /// <returns>最新洞察貼文列表</returns>
    Task<List<PostDto>> GetLatestInsightPostsAsync(int? gameId = null, int limit = 10);

    /// <summary>
    /// 取得置頂洞察貼文
    /// </summary>
    /// <param name="gameId">遊戲編號 (可選)</param>
    /// <returns>置頂洞察貼文列表</returns>
    Task<List<PostDto>> GetPinnedInsightPostsAsync(int? gameId = null);

    /// <summary>
    /// 設定貼文置頂狀態 (管理員功能)
    /// </summary>
    /// <param name="postId">貼文編號</param>
    /// <param name="pinned">是否置頂</param>
    /// <param name="adminId">管理員編號</param>
    /// <returns>是否設定成功</returns>
    Task<bool> SetPostPinnedStatusAsync(int postId, bool pinned, int adminId);

    /// <summary>
    /// 取得作者的洞察貼文列表
    /// </summary>
    /// <param name="authorId">作者編號</param>
    /// <param name="query">查詢條件</param>
    /// <returns>分頁作者貼文列表</returns>
    Task<PagedPostsDto> GetAuthorInsightPostsAsync(int authorId, PostQueryDto query);

    /// <summary>
    /// 取得遊戲相關的洞察貼文統計
    /// </summary>
    /// <param name="gameId">遊戲編號</param>
    /// <returns>洞察貼文統計</returns>
    Task<GameInsightStatsDto> GetGameInsightStatsAsync(int gameId);

    /// <summary>
    /// 取得洞察貼文趨勢分析
    /// </summary>
    /// <param name="query">分析查詢條件</param>
    /// <returns>趨勢分析結果</returns>
    Task<PostTrendAnalysisDto> GetPostTrendAnalysisAsync(PostTrendQueryDto query);
}
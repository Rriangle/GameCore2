using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

/// <summary>
/// 論壇服務
/// 處理論壇主題、回覆、反應和書籤功能
/// </summary>
public class ForumService
{
    private readonly IForumRepository _forumRepository;
    private readonly IForumThreadRepository _threadRepository;
    private readonly IThreadPostRepository _postRepository;
    private readonly IReactionRepository _reactionRepository;
    private readonly IBookmarkRepository _bookmarkRepository;
    private readonly IUserRepository _userRepository;

    public ForumService(
        IForumRepository forumRepository,
        IForumThreadRepository threadRepository,
        IThreadPostRepository postRepository,
        IReactionRepository reactionRepository,
        IBookmarkRepository bookmarkRepository,
        IUserRepository userRepository)
    {
        _forumRepository = forumRepository;
        _threadRepository = threadRepository;
        _postRepository = postRepository;
        _reactionRepository = reactionRepository;
        _bookmarkRepository = bookmarkRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// 取得論壇分類列表
    /// </summary>
    /// <returns>論壇分類列表</returns>
    public async Task<List<ForumCategoryDto>> GetForumCategoriesAsync()
    {
        var forums = await _forumRepository.GetQueryable()
            .OrderBy(f => f.ForumID)
            .ToListAsync();

        var result = new List<ForumCategoryDto>();
        foreach (var forum in forums)
        {
            var threadCount = await _threadRepository.GetQueryable()
                .Where(t => t.ForumID == forum.ForumID && t.Status == "normal")
                .CountAsync();

            var postCount = await _postRepository.GetQueryable()
                .Where(p => p.ThreadID == forum.ForumID)
                .CountAsync();

            var lastActivity = await GetLastActivityAsync(forum.ForumID);

            result.Add(new ForumCategoryDto
            {
                ForumID = forum.ForumID,
                ForumName = forum.Name,
                Description = forum.Description ?? "",
                ParentForumID = 0, // 暫時設為 0，因為實體中沒有這個屬性
                DisplayOrder = forum.ForumID, // 暫時使用 ForumID 作為排序
                IsActive = true, // 暫時設為 true，因為實體中沒有這個屬性
                ThreadCount = threadCount,
                PostCount = postCount,
                LastActivity = lastActivity,
                SubForums = new List<ForumCategoryDto>()
            });
        }

        // 建立階層結構
        return BuildForumHierarchy(result);
    }

    /// <summary>
    /// 取得論壇主題列表
    /// </summary>
    /// <param name="searchParams">搜尋參數</param>
    /// <returns>分頁的主題列表</returns>
    public async Task<PagedResult<ForumThreadDto>> GetThreadsAsync(ForumSearchDto searchParams)
    {
        var query = _threadRepository.GetQueryable()
            .Where(t => t.Status == "normal")
            .AsQueryable();

        // 套用搜尋條件
        if (searchParams.ForumID.HasValue)
        {
            query = query.Where(t => t.ForumID == searchParams.ForumID.Value);
        }

        if (searchParams.AuthorUserID.HasValue)
        {
            query = query.Where(t => t.AuthorUserID == searchParams.AuthorUserID.Value);
        }

        if (!string.IsNullOrEmpty(searchParams.Status))
        {
            query = query.Where(t => t.Status == searchParams.Status);
        }

        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query = query.Where(t => t.Title.Contains(searchParams.SearchTerm));
        }

        if (searchParams.FromDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= searchParams.FromDate.Value);
        }

        if (searchParams.ToDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= searchParams.ToDate.Value);
        }

        // 套用排序
        query = searchParams.SortBy switch
        {
            "updated" => searchParams.SortOrder == "asc" 
                ? query.OrderBy(t => t.UpdatedAt) 
                : query.OrderByDescending(t => t.UpdatedAt),
            _ => searchParams.SortOrder == "asc" 
                ? query.OrderBy(t => t.CreatedAt) 
                : query.OrderByDescending(t => t.CreatedAt)
        };

        var totalCount = await query.CountAsync();
        var threads = await query
            .Skip((searchParams.Page - 1) * searchParams.PageSize)
            .Take(searchParams.PageSize)
            .ToListAsync();

        var threadDtos = threads.Select(t => new ForumThreadDto
        {
            ThreadID = t.ThreadID,
            ForumID = t.ForumID,
            ForumName = "", // 暫時設為空字串，需要從 Forum 表查詢
            AuthorUserID = t.AuthorUserID,
            AuthorName = "", // 暫時設為空字串，需要從 User 表查詢
            Title = t.Title,
            Status = t.Status,
            IsPinned = false, // 暫時設為 false，因為實體中沒有這個屬性
            IsLocked = false, // 暫時設為 false，因為實體中沒有這個屬性
            ViewCount = 0, // 暫時設為 0，因為實體中沒有這個屬性
            ReplyCount = 0, // 暫時設為 0，因為實體中沒有這個屬性
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            LastReplyAt = null, // 暫時設為 null，因為實體中沒有這個屬性
            LastReplyBy = "" // 暫時設為空字串，因為實體中沒有這個屬性
        }).ToList();

        return new PagedResult<ForumThreadDto>
        {
            Data = threadDtos,
            Total = totalCount,
            Page = searchParams.Page,
            PageSize = searchParams.PageSize
        };
    }

    /// <summary>
    /// 建立主題
    /// </summary>
    /// <param name="authorUserId">作者用戶ID</param>
    /// <param name="threadDto">主題資訊</param>
    /// <returns>建立結果</returns>
    public async Task<ServiceResult<ForumThreadDto>> CreateThreadAsync(int authorUserId, CreateThreadDto threadDto)
    {
        // 檢查論壇是否存在
        var forum = await _forumRepository.GetByIdAsync(threadDto.ForumID);
        if (forum == null)
        {
            return ServiceResult<ForumThreadDto>.CreateFailure("論壇不存在或已關閉");
        }

        // 建立主題
        var thread = new ForumThread
        {
            ForumID = threadDto.ForumID,
            AuthorUserID = authorUserId,
            Title = threadDto.Title,
            Status = "normal",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdThread = await _threadRepository.AddAsync(thread);
        if (createdThread == null)
        {
            return ServiceResult<ForumThreadDto>.CreateFailure("主題建立失敗");
        }

        // 建立第一個回覆
        var firstPost = new ThreadPost
        {
            ThreadID = createdThread.ThreadID,
            AuthorUserID = authorUserId,
            ContentMD = threadDto.Content,
            Status = "normal",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _postRepository.AddAsync(firstPost);

        // 取得作者資訊
        var author = await _userRepository.GetByIdAsync(authorUserId);

        var result = new ForumThreadDto
        {
            ThreadID = createdThread.ThreadID,
            ForumID = createdThread.ForumID,
            ForumName = forum.Name,
            AuthorUserID = createdThread.AuthorUserID,
            AuthorName = author?.User_Name ?? "未知用戶",
            Title = createdThread.Title,
            Status = createdThread.Status,
            IsPinned = false, // 預設值
            IsLocked = false, // 預設值
            ViewCount = 0, // 預設值
            ReplyCount = 0, // 預設值
            CreatedAt = createdThread.CreatedAt,
            UpdatedAt = createdThread.UpdatedAt
        };

        return ServiceResult<ForumThreadDto>.CreateSuccess(result);
    }

    /// <summary>
    /// 取得主題詳細資訊
    /// </summary>
    /// <param name="threadId">主題ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>主題詳細資訊</returns>
    public async Task<ServiceResult<ThreadDetailDto>> GetThreadDetailAsync(long threadId, int page = 1, int pageSize = 20)
    {
        var thread = await _threadRepository.GetQueryable()
            .Where(t => t.ThreadID == threadId && t.Status == "normal")
            .FirstOrDefaultAsync();

        if (thread == null)
        {
            return ServiceResult<ThreadDetailDto>.CreateFailure("主題不存在或已刪除");
        }

        // 取得回覆列表
        var postsQuery = _postRepository.GetQueryable()
            .Where(p => p.ThreadID == threadId)
            .OrderBy(p => p.CreatedAt);

        var totalPosts = await postsQuery.CountAsync();
        var posts = await postsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var postDtos = posts.Select(p => new ForumPostDto
        {
            PostID = p.ID,
            ThreadID = p.ThreadID,
            ThreadTitle = thread.Title,
            AuthorUserID = p.AuthorUserID,
            AuthorName = "", // 暫時設為空字串，需要從 User 表查詢
            Content = p.ContentMD,
            ContentType = "markdown", // 暫時設為 markdown，因為實體中沒有這個屬性
            IsEdited = false, // 暫時設為 false，因為實體中沒有這個屬性
            EditedAt = null, // 暫時設為 null，因為實體中沒有這個屬性
            EditReason = "", // 暫時設為空字串，因為實體中沒有這個屬性
            LikeCount = 0, // 暫時設為 0，因為實體中沒有這個屬性
            DislikeCount = 0, // 暫時設為 0，因為實體中沒有這個屬性
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Reactions = new List<PostReactionDto>() // 暫時設為空列表，因為實體中沒有這個屬性
        }).ToList();

        var threadDto = new ForumThreadDto
        {
            ThreadID = thread.ThreadID,
            ForumID = thread.ForumID,
            ForumName = "", // 暫時設為空字串，需要從 Forum 表查詢
            AuthorUserID = thread.AuthorUserID,
            AuthorName = "", // 暫時設為空字串，需要從 User 表查詢
            Title = thread.Title,
            Status = thread.Status,
            IsPinned = false, // 暫時設為 false，因為實體中沒有這個屬性
            IsLocked = false, // 暫時設為 false，因為實體中沒有這個屬性
            ViewCount = 0, // 暫時設為 0，因為實體中沒有這個屬性
            ReplyCount = totalPosts, // 使用實際的回覆數量
            CreatedAt = thread.CreatedAt,
            UpdatedAt = thread.UpdatedAt,
            LastReplyAt = posts.Any() ? posts.Last().CreatedAt : null, // 使用最後一個回覆的時間
            LastReplyBy = "" // 暫時設為空字串，需要從 User 表查詢
        };

        var result = new ThreadDetailDto
        {
            Thread = threadDto,
            Posts = postDtos,
            TotalPosts = totalPosts,
            Page = page,
            PageSize = pageSize
        };

        return ServiceResult<ThreadDetailDto>.CreateSuccess(result);
    }

    /// <summary>
    /// 新增回覆
    /// </summary>
    /// <param name="authorUserId">作者ID</param>
    /// <param name="postDto">回覆資訊</param>
    /// <returns>建立結果</returns>
    public async Task<ServiceResult<ForumPostDto>> CreatePostAsync(int authorUserId, CreatePostDto postDto)
    {
        // 檢查主題是否存在
        var thread = await _threadRepository.GetByIdAsync(postDto.ThreadID);
        if (thread == null || thread.Status != "normal")
        {
            return ServiceResult<ForumPostDto>.CreateFailure("主題不存在、已刪除或已鎖定");
        }

        // 建立回覆
        var post = new ThreadPost
        {
            ThreadID = postDto.ThreadID,
            AuthorUserID = authorUserId,
            ContentMD = postDto.Content,
            ParentPostID = postDto.ReplyToPostID,
            Status = "normal",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _postRepository.AddAsync(post);
        if (result == null)
        {
            return ServiceResult<ForumPostDto>.CreateFailure("回覆建立失敗");
        }

        // 更新主題的最後回覆時間
        thread.UpdatedAt = DateTime.UtcNow;
        await _threadRepository.UpdateAsync(thread);

        var forumPostDto = new ForumPostDto
        {
            PostID = post.ID,
            ThreadID = post.ThreadID,
            ThreadTitle = thread.Title,
            AuthorUserID = post.AuthorUserID,
            AuthorName = "", // 暫時設為空字串，需要從 User 表查詢
            Content = post.ContentMD,
            ContentType = "markdown", // 暫時設為 markdown，因為實體中沒有這個屬性
            IsEdited = false, // 暫時設為 false，因為實體中沒有這個屬性
            EditedAt = null, // 暫時設為 null，因為實體中沒有這個屬性
            EditReason = "", // 暫時設為空字串，因為實體中沒有這個屬性
            LikeCount = 0, // 暫時設為 0，因為實體中沒有這個屬性
            DislikeCount = 0, // 暫時設為 0，因為實體中沒有這個屬性
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            Reactions = new List<PostReactionDto>() // 暫時設為空列表，因為實體中沒有這個屬性
        };

        return ServiceResult<ForumPostDto>.CreateSuccess(forumPostDto);
    }

    /// <summary>
    /// 新增反應
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="reactionDto">反應資訊</param>
    /// <returns>新增結果</returns>
    public async Task<ServiceResult<bool>> AddReactionAsync(int userId, AddReactionDto reactionDto)
    {
        // 檢查是否已有相同反應
        var existingReaction = await _reactionRepository.GetQueryable()
            .Where(r => r.TargetID == reactionDto.PostID && r.UserID == userId && r.TargetType == "post")
            .FirstOrDefaultAsync();

        if (existingReaction != null)
        {
            // 如果反應類型相同，則移除反應
            if (existingReaction.Kind == reactionDto.ReactionType)
            {
                await _reactionRepository.DeleteAsync(existingReaction.ID);
                await UpdatePostReactionCountsAsync(reactionDto.PostID, reactionDto.ReactionType, -1);
                return ServiceResult<bool>.CreateSuccess(true);
            }
            else
            {
                // 更新反應類型
                existingReaction.Kind = reactionDto.ReactionType;
                await _reactionRepository.UpdateAsync(existingReaction);
                return ServiceResult<bool>.CreateSuccess(true);
            }
        }

        // 新增反應
        var reaction = new Reaction
        {
            UserID = userId,
            TargetType = "post",
            TargetID = reactionDto.PostID,
            Kind = reactionDto.ReactionType,
            CreatedAt = DateTime.UtcNow
        };

        var createdReaction = await _reactionRepository.AddAsync(reaction);
        if (createdReaction == null)
        {
            return ServiceResult<bool>.CreateFailure("反應新增失敗");
        }

        await UpdatePostReactionCountsAsync(reactionDto.PostID, reactionDto.ReactionType, 1);
        return ServiceResult<bool>.CreateSuccess(true);
    }

    /// <summary>
    /// 新增書籤
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="bookmarkDto">書籤資訊</param>
    /// <returns>新增結果</returns>
    public async Task<ServiceResult<bool>> AddBookmarkAsync(int userId, AddBookmarkDto bookmarkDto)
    {
        // 檢查是否已有書籤
        var existingBookmark = await _bookmarkRepository.GetQueryable()
            .Where(b => b.UserID == userId && b.TargetType == bookmarkDto.ContentType && b.TargetID == bookmarkDto.ContentID)
            .FirstOrDefaultAsync();

        if (existingBookmark != null)
        {
            return ServiceResult<bool>.CreateFailure("已存在相同書籤");
        }

        // 新增書籤
        var bookmark = new Bookmark
        {
            UserID = userId,
            TargetType = bookmarkDto.ContentType,
            TargetID = bookmarkDto.ContentID,
            CreatedAt = DateTime.UtcNow
        };

        var createdBookmark = await _bookmarkRepository.AddAsync(bookmark);
        return createdBookmark != null 
            ? ServiceResult<bool>.CreateSuccess(true) 
            : ServiceResult<bool>.CreateFailure("書籤新增失敗");
    }

    /// <summary>
    /// 取得使用者書籤列表
    /// </summary>
    /// <param name="userId">使用者ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>分頁的書籤列表</returns>
    public async Task<PagedResult<BookmarkDto>> GetUserBookmarksAsync(int userId, int page = 1, int pageSize = 20)
    {
        var query = _bookmarkRepository.GetQueryable()
            .Where(b => b.UserID == userId)
            .OrderByDescending(b => b.CreatedAt);

        var totalCount = await query.CountAsync();
        var bookmarks = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var bookmarkDtos = bookmarks.Select(b => new BookmarkDto
        {
            BookmarkID = b.ID,
            UserID = b.UserID,
            ContentType = b.TargetType,
            ContentID = b.TargetID,
            ContentTitle = "", // 需要從對應的內容表查詢
            ContentPreview = "", // 需要從對應的內容表查詢
            CreatedAt = b.CreatedAt
        }).ToList();

        return new PagedResult<BookmarkDto>
        {
            Data = bookmarkDtos,
            Total = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// 取得論壇統計資訊
    /// </summary>
    /// <param name="forumId">論壇ID</param>
    /// <returns>統計資訊</returns>
    public async Task<ForumStatsDto> GetForumStatsAsync(int forumId)
    {
        var forum = await _forumRepository.GetByIdAsync(forumId);
        if (forum == null)
        {
            throw new ArgumentException("論壇不存在");
        }

        var totalThreads = await _threadRepository.GetQueryable()
            .Where(t => t.ForumID == forumId)
            .CountAsync();

        var totalPosts = await _postRepository.GetQueryable()
            .Where(p => p.ThreadID == forumId)
            .CountAsync();

        var totalUsers = await _threadRepository.GetQueryable()
            .Where(t => t.ForumID == forumId)
            .Select(t => t.AuthorUserID)
            .Distinct()
            .CountAsync();

        var lastActivity = await _postRepository.GetQueryable()
            .Where(p => p.ThreadID == forumId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => p.CreatedAt)
            .FirstOrDefaultAsync();

        var recentThreads = await _threadRepository.GetQueryable()
            .Where(t => t.ForumID == forumId)
            .OrderByDescending(t => t.UpdatedAt)
            .Take(5)
            .ToListAsync();

        var recentPosts = await _postRepository.GetQueryable()
            .Where(p => p.ThreadID == forumId)
            .OrderByDescending(p => p.CreatedAt)
            .Take(5)
            .ToListAsync();

        var threadDtos = recentThreads.Select(t => new ForumThreadDto
        {
            ThreadID = t.ThreadID,
            ForumID = t.ForumID,
            ForumName = forum.Name,
            AuthorUserID = t.AuthorUserID,
            AuthorName = "", // 暫時設為空字串，需要額外查詢
            Title = t.Title,
            Status = t.Status,
            IsPinned = false, // 暫時設為 false，因為實體中沒有這個屬性
            IsLocked = false, // 暫時設為 false，因為實體中沒有這個屬性
            ViewCount = 0, // 暫時設為 0，因為實體中沒有這個屬性
            ReplyCount = 0, // 暫時設為 0，因為實體中沒有這個屬性
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            LastReplyAt = null, // 暫時設為 null，因為實體中沒有這個屬性
            LastReplyBy = "" // 暫時設為空字串，因為實體中沒有這個屬性
        }).ToList();

        var postDtos = recentPosts.Select(p => new ForumPostDto
        {
            PostID = p.ID,
            ThreadID = p.ThreadID,
            ThreadTitle = "", // 需要額外查詢
            AuthorUserID = p.AuthorUserID,
            AuthorName = "", // 暫時設為空字串，需要額外查詢
            Content = p.ContentMD,
            ContentType = "markdown", // 暫時設為 markdown，因為實體中沒有這個屬性
            IsEdited = false, // 暫時設為 false，因為實體中沒有這個屬性
            EditedAt = null, // 暫時設為 null，因為實體中沒有這個屬性
            EditReason = "", // 暫時設為空字串，因為實體中沒有這個屬性
            LikeCount = 0, // 暫時設為 0，因為實體中沒有這個屬性
            DislikeCount = 0, // 暫時設為 0，因為實體中沒有這個屬性
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            Reactions = new List<PostReactionDto>() // 暫時設為空列表，因為實體中沒有這個屬性
        }).ToList();

        return new ForumStatsDto
        {
            ForumID = forumId,
            ForumName = forum.Name,
            TotalThreads = totalThreads,
            TotalPosts = totalPosts,
            TotalUsers = totalUsers,
            LastActivity = lastActivity,
            RecentThreads = threadDtos,
            RecentPosts = postDtos
        };
    }

    /// <summary>
    /// 建立論壇階層結構
    /// </summary>
    /// <param name="forums">論壇列表</param>
    /// <returns>階層結構</returns>
    private List<ForumCategoryDto> BuildForumHierarchy(List<ForumCategoryDto> forums)
    {
        var forumDict = forums.ToDictionary(f => f.ForumID);
        var rootForums = new List<ForumCategoryDto>();

        foreach (var forum in forums)
        {
            if (forum.ParentForumID == 0)
            {
                rootForums.Add(forum);
            }
            else if (forumDict.ContainsKey(forum.ParentForumID))
            {
                forumDict[forum.ParentForumID].SubForums.Add(forum);
            }
        }

        return rootForums;
    }

    /// <summary>
    /// 取得最後活動時間
    /// </summary>
    /// <param name="forumId">論壇ID</param>
    /// <returns>最後活動時間</returns>
    private async Task<DateTime> GetLastActivityAsync(int forumId)
    {
        var lastThread = await _threadRepository.GetQueryable()
            .Where(t => t.ForumID == forumId)
            .OrderByDescending(t => t.UpdatedAt)
            .FirstOrDefaultAsync();

        var lastPost = await _postRepository.GetQueryable()
            .Where(p => p.ThreadID == forumId)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();

        if (lastThread == null && lastPost == null)
            return DateTime.UtcNow;

        var threadTime = lastThread?.UpdatedAt ?? DateTime.MinValue;
        var postTime = lastPost?.CreatedAt ?? DateTime.MinValue;

        return threadTime > postTime ? threadTime : postTime;
    }

    /// <summary>
    /// 更新回覆反應計數
    /// </summary>
    /// <param name="postId">回覆ID</param>
    /// <param name="reactionType">反應類型</param>
    /// <param name="change">變化量</param>
    private async Task UpdatePostReactionCountsAsync(long postId, string reactionType, int change)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post == null) return;

        // 簡化版本：只更新 UpdatedAt 時間，不更新反應計數
        // 因為 ThreadPost 實體中沒有 Like_Count 和 Dislike_Count 屬性
        post.UpdatedAt = DateTime.UtcNow;
        await _postRepository.UpdateAsync(post);
    }
}

/// <summary>
/// 主題詳細資訊 DTO
/// </summary>
public class ThreadDetailDto
{
    public ForumThreadDto Thread { get; set; } = new ForumThreadDto();
    public List<ForumPostDto> Posts { get; set; } = new List<ForumPostDto>();
    public int TotalPosts { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
} 
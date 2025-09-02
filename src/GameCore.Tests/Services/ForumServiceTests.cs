using GameCore.Api.Services;
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// 論壇服務測試
/// </summary>
public class ForumServiceTests
{
    private readonly Mock<IForumRepository> _mockForumRepository;
    private readonly Mock<IForumThreadRepository> _mockThreadRepository;
    private readonly Mock<IThreadPostRepository> _mockPostRepository;
    private readonly Mock<IReactionRepository> _mockReactionRepository;
    private readonly Mock<IBookmarkRepository> _mockBookmarkRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly ForumService _forumService;

    public ForumServiceTests()
    {
        _mockForumRepository = new Mock<IForumRepository>();
        _mockThreadRepository = new Mock<IForumThreadRepository>();
        _mockPostRepository = new Mock<IThreadPostRepository>();
        _mockReactionRepository = new Mock<IReactionRepository>();
        _mockBookmarkRepository = new Mock<IBookmarkRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _forumService = new ForumService(_mockForumRepository.Object, _mockThreadRepository.Object, _mockPostRepository.Object, _mockReactionRepository.Object, _mockBookmarkRepository.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task GetForumCategoriesAsync_WithValidForums_ReturnsCategoryList()
    {
        // Arrange
        var forums = new List<Forum>
        {
            new Forum
            {
                ForumID = 1,
                Forum_Name = "一般討論",
                Description = "一般性討論區",
                Parent_Forum_ID = 0,
                Display_Order = 1,
                Is_Active = true
            },
            new Forum
            {
                ForumID = 2,
                Forum_Name = "遊戲討論",
                Description = "遊戲相關討論",
                Parent_Forum_ID = 0,
                Display_Order = 2,
                Is_Active = true
            }
        };

        var mockQueryable = forums.AsQueryable();
        _mockForumRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        var threadsQueryable = new List<ForumThread>().AsQueryable();
        _mockThreadRepository.Setup(x => x.GetQueryable()).Returns(threadsQueryable);

        var postsQueryable = new List<ThreadPost>().AsQueryable();
        _mockPostRepository.Setup(x => x.GetQueryable()).Returns(postsQueryable);

        // Act
        var result = await _forumService.GetForumCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("一般討論", result[0].ForumName);
        Assert.Equal("遊戲討論", result[1].ForumName);
        Assert.Equal(1, result[0].DisplayOrder);
        Assert.Equal(2, result[1].DisplayOrder);
    }

    [Fact]
    public async Task GetThreadsAsync_WithValidParameters_ReturnsPagedResult()
    {
        // Arrange
        var searchParams = new ForumSearchDto
        {
            ForumID = 1,
            Page = 1,
            PageSize = 20,
            SortBy = "created",
            SortOrder = "desc"
        };

        var threads = new List<ForumThread>
        {
            new ForumThread
            {
                ThreadID = 1,
                ForumID = 1,
                AuthorUserID = 1,
                Title = "測試主題1",
                Status = "normal",
                Is_Pinned = false,
                Is_Locked = false,
                View_Count = 100,
                Reply_Count = 5,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                Forum = new Forum { Forum_Name = "一般討論" },
                Author = new User { User_Name = "用戶1" }
            },
            new ForumThread
            {
                ThreadID = 2,
                ForumID = 1,
                AuthorUserID = 2,
                Title = "測試主題2",
                Status = "normal",
                Is_Pinned = false,
                Is_Locked = false,
                View_Count = 80,
                Reply_Count = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2),
                Forum = new Forum { Forum_Name = "一般討論" },
                Author = new User { User_Name = "用戶2" }
            }
        };

        var mockQueryable = threads.AsQueryable();
        _mockThreadRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _forumService.GetThreadsAsync(searchParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("測試主題1", result.Data[0].Title);
        Assert.Equal("測試主題2", result.Data[1].Title);
    }

    [Fact]
    public async Task CreateThreadAsync_WithValidData_CreatesThreadSuccessfully()
    {
        // Arrange
        var authorUserId = 1;
        var threadDto = new CreateThreadDto
        {
            ForumID = 1,
            Title = "新主題",
            Content = "這是新主題的內容",
            Tags = new List<string> { "標籤1", "標籤2" }
        };

        var forum = new Forum
        {
            ForumID = 1,
            Forum_Name = "一般討論",
            Is_Active = true
        };

        var author = new User
        {
            User_ID = authorUserId,
            User_Name = "用戶1"
        };

        var createdThread = new ForumThread
        {
            ThreadID = 1,
            ForumID = 1,
            AuthorUserID = authorUserId,
            Title = "新主題",
            Status = "normal",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdPost = new ThreadPost
        {
            PostID = 1,
            ThreadID = 1,
            AuthorUserID = authorUserId,
            Content = "這是新主題的內容",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockForumRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(forum);
        _mockThreadRepository.Setup(x => x.AddAsync(It.IsAny<ForumThread>())).ReturnsAsync(createdThread);
        _mockPostRepository.Setup(x => x.AddAsync(It.IsAny<ThreadPost>())).ReturnsAsync(createdPost);

        // Act
        var result = await _forumService.CreateThreadAsync(authorUserId, threadDto);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.ThreadID);
        Assert.Equal(1, result.Data.ForumID);
        Assert.Equal("一般討論", result.Data.ForumName);
        Assert.Equal(authorUserId, result.Data.AuthorUserID);
        Assert.Equal("用戶1", result.Data.AuthorName);
        Assert.Equal("新主題", result.Data.Title);
        Assert.Equal("normal", result.Data.Status);
    }

    [Fact]
    public async Task CreateThreadAsync_WithInactiveForum_ReturnsFailure()
    {
        // Arrange
        var authorUserId = 1;
        var threadDto = new CreateThreadDto
        {
            ForumID = 1,
            Title = "新主題",
            Content = "這是新主題的內容"
        };

        var forum = new Forum
        {
            ForumID = 1,
            Forum_Name = "一般討論",
            Is_Active = false
        };

        _mockForumRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(forum);

        // Act
        var result = await _forumService.CreateThreadAsync(authorUserId, threadDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("論壇不存在或已關閉", result.Message);
    }

    [Fact]
    public async Task GetThreadDetailAsync_WithValidThreadId_ReturnsThreadDetail()
    {
        // Arrange
        var threadId = 1L;
        var page = 1;
        var pageSize = 20;

        var thread = new ForumThread
        {
            ThreadID = threadId,
            ForumID = 1,
            AuthorUserID = 1,
            Title = "測試主題",
            Status = "normal",
            Is_Pinned = false,
            Is_Locked = false,
            View_Count = 100,
            Reply_Count = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            Forum = new Forum { Forum_Name = "一般討論" },
            Author = new User { User_Name = "用戶1" }
        };

        var posts = new List<ThreadPost>
        {
            new ThreadPost
            {
                PostID = 1,
                ThreadID = threadId,
                AuthorUserID = 1,
                Content = "這是主題內容",
                Content_Type = "text",
                Is_Edited = false,
                Like_Count = 10,
                Dislike_Count = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                Author = new User { User_Name = "用戶1" },
                Reactions = new List<Reaction>()
            }
        };

        var mockThreadQueryable = new List<ForumThread> { thread }.AsQueryable();
        _mockThreadRepository.Setup(x => x.GetQueryable()).Returns(mockThreadQueryable);

        var mockPostQueryable = posts.AsQueryable();
        _mockPostRepository.Setup(x => x.GetQueryable()).Returns(mockPostQueryable);

        _mockThreadRepository.Setup(x => x.UpdateAsync(It.IsAny<ForumThread>())).ReturnsAsync(true);

        // Act
        var result = await _forumService.GetThreadDetailAsync(threadId, page, pageSize);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(threadId, result.Data.Thread.ThreadID);
        Assert.Equal("測試主題", result.Data.Thread.Title);
        Assert.Equal("一般討論", result.Data.Thread.ForumName);
        Assert.Equal(1, result.Data.Thread.AuthorUserID);
        Assert.Equal("用戶1", result.Data.Thread.AuthorName);
        Assert.Equal(1, result.Data.Posts.Count);
        Assert.Equal("這是主題內容", result.Data.Posts[0].Content);
    }

    [Fact]
    public async Task CreatePostAsync_WithValidData_CreatesPostSuccessfully()
    {
        // Arrange
        var authorUserId = 1;
        var postDto = new CreatePostDto
        {
            ThreadID = 1,
            Content = "這是回覆內容",
            ContentType = "text"
        };

        var thread = new ForumThread
        {
            ThreadID = 1,
            Status = "normal",
            Is_Locked = false,
            Reply_Count = 5,
            Last_Reply_At = DateTime.UtcNow.AddDays(-1),
            Last_Reply_By = "1"
        };

        var createdPost = new ThreadPost
        {
            PostID = 1,
            ThreadID = 1,
            AuthorUserID = authorUserId,
            Content = "這是回覆內容",
            Content_Type = "text",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var author = new User
        {
            User_ID = authorUserId,
            User_Name = "用戶1"
        };

        _mockThreadRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(thread);
        _mockPostRepository.Setup(x => x.AddAsync(It.IsAny<ThreadPost>())).ReturnsAsync(createdPost);
        _mockThreadRepository.Setup(x => x.UpdateAsync(It.IsAny<ForumThread>())).ReturnsAsync(true);

        // Act
        var result = await _forumService.CreatePostAsync(authorUserId, postDto);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.PostID);
        Assert.Equal(1, result.Data.ThreadID);
        Assert.Equal(authorUserId, result.Data.AuthorUserID);
        Assert.Equal("用戶1", result.Data.AuthorName);
        Assert.Equal("這是回覆內容", result.Data.Content);
        Assert.Equal("text", result.Data.ContentType);
    }

    [Fact]
    public async Task CreatePostAsync_WithLockedThread_ReturnsFailure()
    {
        // Arrange
        var authorUserId = 1;
        var postDto = new CreatePostDto
        {
            ThreadID = 1,
            Content = "這是回覆內容"
        };

        var thread = new ForumThread
        {
            ThreadID = 1,
            Status = "normal",
            Is_Locked = true
        };

        _mockThreadRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(thread);

        // Act
        var result = await _forumService.CreatePostAsync(authorUserId, postDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("主題不存在、已刪除或已鎖定", result.Message);
    }

    [Fact]
    public async Task AddReactionAsync_WithNewReaction_AddsReactionSuccessfully()
    {
        // Arrange
        var userId = 1;
        var reactionDto = new AddReactionDto
        {
            PostID = 1,
            ReactionType = "like"
        };

        var post = new ThreadPost
        {
            PostID = 1,
            Like_Count = 10,
            Dislike_Count = 0
        };

        var createdReaction = new Reaction
        {
            ReactionID = 1,
            PostID = 1,
            UserID = userId,
            Reaction_Type = "like",
            CreatedAt = DateTime.UtcNow
        };

        var mockQueryable = new List<Reaction>().AsQueryable();
        _mockReactionRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);
        _mockReactionRepository.Setup(x => x.AddAsync(It.IsAny<Reaction>())).ReturnsAsync(createdReaction);
        _mockPostRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(post);
        _mockPostRepository.Setup(x => x.UpdateAsync(It.IsAny<ThreadPost>())).ReturnsAsync(true);

        // Act
        var result = await _forumService.AddReactionAsync(userId, reactionDto);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task AddBookmarkAsync_WithValidData_AddsBookmarkSuccessfully()
    {
        // Arrange
        var userId = 1;
        var bookmarkDto = new AddBookmarkDto
        {
            ContentType = "thread",
            ContentID = 1
        };

        var createdBookmark = new Bookmark
        {
            BookmarkID = 1,
            UserID = userId,
            Content_Type = "thread",
            Content_ID = 1,
            Content_Title = "測試主題",
            Content_Preview = "測試主題",
            CreatedAt = DateTime.UtcNow
        };

        var mockQueryable = new List<Bookmark>().AsQueryable();
        _mockBookmarkRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);
        _mockBookmarkRepository.Setup(x => x.AddAsync(It.IsAny<Bookmark>())).ReturnsAsync(createdBookmark);

        // Act
        var result = await _forumService.AddBookmarkAsync(userId, bookmarkDto);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task GetUserBookmarksAsync_WithValidParameters_ReturnsPagedResult()
    {
        // Arrange
        var userId = 1;
        var page = 1;
        var pageSize = 20;

        var bookmarks = new List<Bookmark>
        {
            new Bookmark
            {
                BookmarkID = 1,
                UserID = userId,
                Content_Type = "thread",
                Content_ID = 1,
                Content_Title = "測試主題1",
                Content_Preview = "測試主題1",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Bookmark
            {
                BookmarkID = 2,
                UserID = userId,
                Content_Type = "post",
                Content_ID = 1,
                Content_Title = "回覆 #1",
                Content_Preview = "這是回覆內容",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        var mockQueryable = bookmarks.AsQueryable();
        _mockBookmarkRepository.Setup(x => x.GetQueryable()).Returns(mockQueryable);

        // Act
        var result = await _forumService.GetUserBookmarksAsync(userId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("測試主題1", result.Data[0].ContentTitle);
        Assert.Equal("回覆 #1", result.Data[1].ContentTitle);
    }

    [Fact]
    public async Task GetForumStatsAsync_WithValidForumId_ReturnsForumStats()
    {
        // Arrange
        var forumId = 1;
        var forum = new Forum
        {
            ForumID = forumId,
            Forum_Name = "一般討論"
        };

        var threads = new List<ForumThread>
        {
            new ForumThread { ThreadID = 1, ForumID = forumId, Status = "normal" },
            new ForumThread { ThreadID = 2, ForumID = forumId, Status = "normal" }
        };

        var posts = new List<ThreadPost>
        {
            new ThreadPost { PostID = 1, ThreadID = 1 },
            new ThreadPost { PostID = 2, ThreadID = 1 },
            new ThreadPost { PostID = 3, ThreadID = 2 }
        };

        var mockThreadQueryable = threads.AsQueryable();
        _mockThreadRepository.Setup(x => x.GetQueryable()).Returns(mockThreadQueryable);

        var mockPostQueryable = posts.AsQueryable();
        _mockPostRepository.Setup(x => x.GetQueryable()).Returns(mockPostQueryable);

        _mockForumRepository.Setup(x => x.GetByIdAsync(forumId)).ReturnsAsync(forum);

        // Act
        var result = await _forumService.GetForumStatsAsync(forumId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(forumId, result.ForumID);
        Assert.Equal("一般討論", result.ForumName);
        Assert.Equal(2, result.TotalThreads);
        Assert.Equal(3, result.TotalPosts);
        Assert.Equal(2, result.TotalUsers);
    }
} 
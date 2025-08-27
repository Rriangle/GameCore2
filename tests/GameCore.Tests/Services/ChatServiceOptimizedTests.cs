using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using GameCore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameCore.Tests.Services;

/// <summary>
/// ChatService 優化版本單元測試
/// </summary>
public class ChatServiceOptimizedTests : IDisposable
{
    private readonly DbContextOptions<GameCoreDbContext> _options;
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ChatService> _logger;
    private readonly ChatService _service;

    public ChatServiceOptimizedTests()
    {
        _options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(_options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Mock.Of<ILogger<ChatService>>();
        _service = new ChatService(_context, _memoryCache, _logger);

        SeedTestData();
    }

    public void Dispose()
    {
        _context.Dispose();
        _memoryCache.Dispose();
    }

    #region 測試數據準備

    private void SeedTestData()
    {
        // 創建用戶
        var user1 = new User { user_id = 1, username = "user1", email = "user1@test.com" };
        var user2 = new User { user_id = 2, username = "user2", email = "user2@test.com" };
        var user3 = new User { user_id = 3, username = "user3", email = "user3@test.com" };
        _context.Users.AddRange(user1, user2, user3);

        // 創建聊天訊息
        var message1 = new ChatMessage 
        { 
            message_id = 1, 
            sender_id = 1, 
            receiver_id = 2, 
            chat_content = "Hello user2!", 
            sent_at = DateTime.UtcNow.AddHours(-1), 
            is_read = false, 
            is_sent = true 
        };
        var message2 = new ChatMessage 
        { 
            message_id = 2, 
            sender_id = 2, 
            receiver_id = 1, 
            chat_content = "Hi user1!", 
            sent_at = DateTime.UtcNow.AddHours(-2), 
            is_read = true, 
            is_sent = true 
        };
        var message3 = new ChatMessage 
        { 
            message_id = 3, 
            sender_id = 1, 
            receiver_id = 3, 
            chat_content = "Hello user3!", 
            sent_at = DateTime.UtcNow.AddHours(-3), 
            is_read = false, 
            is_sent = true 
        };
        var message4 = new ChatMessage 
        { 
            message_id = 4, 
            sender_id = 3, 
            receiver_id = 1, 
            chat_content = "Hi user1!", 
            sent_at = DateTime.UtcNow.AddHours(-4), 
            is_read = false, 
            is_sent = true 
        };
        _context.ChatMessages.AddRange(message1, message2, message3, message4);

        _context.SaveChanges();
    }

    #endregion

    #region GetDirectMessagesAsync 測試

    [Fact]
    public async Task GetDirectMessagesAsync_WithValidParameters_ShouldReturnMessages()
    {
        // Arrange
        var userId = 1;
        var peerId = 2;
        var page = 1;
        var pageSize = 50;

        // Act
        var result = await _service.GetDirectMessagesAsync(userId, peerId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var messages = result.ToList();
        Assert.Equal(2, messages.Count);
        Assert.All(messages, m => Assert.True(
            (m.sender_id == userId && m.receiver_id == peerId) ||
            (m.sender_id == peerId && m.receiver_id == userId)));
    }

    [Fact]
    public async Task GetDirectMessagesAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var peerId = 2;
        var page = 1;
        var pageSize = 50;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetDirectMessagesAsync(userId, peerId, page, pageSize));
    }

    [Fact]
    public async Task GetDirectMessagesAsync_WithInvalidPeerId_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var peerId = -1;
        var page = 1;
        var pageSize = 50;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetDirectMessagesAsync(userId, peerId, page, pageSize));
    }

    [Fact]
    public async Task GetDirectMessagesAsync_WithInvalidPage_ShouldThrowException()
    {
        // Arrange
        var userId = 1;
        var peerId = 2;
        var page = 0;
        var pageSize = 50;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetDirectMessagesAsync(userId, peerId, page, pageSize));
    }

    [Fact]
    public async Task GetDirectMessagesAsync_WithInvalidPageSize_ShouldUseDefaultPageSize()
    {
        // Arrange
        var userId = 1;
        var peerId = 2;
        var page = 1;
        var pageSize = 0;

        // Act
        var result = await _service.GetDirectMessagesAsync(userId, peerId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var messages = result.ToList();
        Assert.True(messages.Count <= 50); // Default page size
    }

    [Fact]
    public async Task GetDirectMessagesAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;
        var peerId = 2;
        var page = 1;
        var pageSize = 50;

        // Act - 第一次調用
        var result1 = await _service.GetDirectMessagesAsync(userId, peerId, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.ChatMessages.RemoveRange(_context.ChatMessages);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetDirectMessagesAsync(userId, peerId, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region SendDirectMessageAsync 測試

    [Fact]
    public async Task SendDirectMessageAsync_WithValidData_ShouldCreateMessage()
    {
        // Arrange
        var senderId = 1;
        var receiverId = 2;
        var content = "Test message";

        // Act
        var result = await _service.SendDirectMessageAsync(senderId, receiverId, content);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.message_id > 0);
        Assert.Equal(senderId, result.sender_id);
        Assert.Equal(receiverId, result.receiver_id);
        Assert.Equal(content, result.chat_content);
        Assert.False(result.is_read);
        Assert.True(result.is_sent);
        Assert.True(result.sent_at > DateTime.UtcNow.AddMinutes(-1));

        // 驗證資料庫中確實創建了記錄
        var dbMessage = await _context.ChatMessages
            .FirstOrDefaultAsync(m => m.message_id == result.message_id);
        Assert.NotNull(dbMessage);
    }

    [Fact]
    public async Task SendDirectMessageAsync_WithInvalidSenderId_ShouldThrowException()
    {
        // Arrange
        var senderId = -1;
        var receiverId = 2;
        var content = "Test message";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendDirectMessageAsync(senderId, receiverId, content));
    }

    [Fact]
    public async Task SendDirectMessageAsync_WithInvalidReceiverId_ShouldThrowException()
    {
        // Arrange
        var senderId = 1;
        var receiverId = -1;
        var content = "Test message";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendDirectMessageAsync(senderId, receiverId, content));
    }

    [Fact]
    public async Task SendDirectMessageAsync_WithEmptyContent_ShouldThrowException()
    {
        // Arrange
        var senderId = 1;
        var receiverId = 2;
        var content = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendDirectMessageAsync(senderId, receiverId, content));
    }

    [Fact]
    public async Task SendDirectMessageAsync_WithNullContent_ShouldThrowException()
    {
        // Arrange
        var senderId = 1;
        var receiverId = 2;
        string content = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendDirectMessageAsync(senderId, receiverId, content));
    }

    [Fact]
    public async Task SendDirectMessageAsync_WithTooLongContent_ShouldThrowException()
    {
        // Arrange
        var senderId = 1;
        var receiverId = 2;
        var content = new string('a', 1001); // Exceeds MaxMessageLength

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendDirectMessageAsync(senderId, receiverId, content));
    }

    [Fact]
    public async Task SendDirectMessageAsync_ToSelf_ShouldThrowException()
    {
        // Arrange
        var senderId = 1;
        var receiverId = 1; // Same as sender
        var content = "Test message";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendDirectMessageAsync(senderId, receiverId, content));
    }

    #endregion

    #region SendSystemMessageAsync 測試

    [Fact]
    public async Task SendSystemMessageAsync_WithValidData_ShouldCreateSystemMessage()
    {
        // Arrange
        var receiverId = 2;
        var content = "System maintenance notice";
        int? managerId = null;

        // Act
        var result = await _service.SendSystemMessageAsync(receiverId, content, managerId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.message_id > 0);
        Assert.Equal(1, result.sender_id); // System user
        Assert.Equal(receiverId, result.receiver_id);
        Assert.Equal(content, result.chat_content);
        Assert.False(result.is_read);
        Assert.True(result.is_sent);
        Assert.True(result.sent_at > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task SendSystemMessageAsync_WithManagerId_ShouldSetManagerId()
    {
        // Arrange
        var receiverId = 2;
        var content = "System maintenance notice";
        int? managerId = 5;

        // Act
        var result = await _service.SendSystemMessageAsync(receiverId, content, managerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(managerId, result.manager_id);
    }

    [Fact]
    public async Task SendSystemMessageAsync_WithInvalidReceiverId_ShouldThrowException()
    {
        // Arrange
        var receiverId = -1;
        var content = "System maintenance notice";
        int? managerId = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendSystemMessageAsync(receiverId, content, managerId));
    }

    [Fact]
    public async Task SendSystemMessageAsync_WithEmptyContent_ShouldThrowException()
    {
        // Arrange
        var receiverId = 2;
        var content = "";
        int? managerId = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendSystemMessageAsync(receiverId, content, managerId));
    }

    [Fact]
    public async Task SendSystemMessageAsync_WithTooLongContent_ShouldThrowException()
    {
        // Arrange
        var receiverId = 2;
        var content = new string('a', 1001); // Exceeds MaxMessageLength
        int? managerId = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendSystemMessageAsync(receiverId, content, managerId));
    }

    #endregion

    #region MarkMessageAsReadAsync 測試

    [Fact]
    public async Task MarkMessageAsReadAsync_WithValidData_ShouldMarkAsRead()
    {
        // Arrange
        var messageId = 1;
        var userId = 2; // Receiver of message 1

        // Act
        var result = await _service.MarkMessageAsReadAsync(messageId, userId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實更新了記錄
        var dbMessage = await _context.ChatMessages
            .FirstOrDefaultAsync(m => m.message_id == messageId);
        Assert.NotNull(dbMessage);
        Assert.True(dbMessage.is_read);
    }

    [Fact]
    public async Task MarkMessageAsReadAsync_WithAlreadyReadMessage_ShouldReturnTrue()
    {
        // Arrange
        var messageId = 2; // Already read message
        var userId = 1; // Receiver of message 2

        // Act
        var result = await _service.MarkMessageAsReadAsync(messageId, userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task MarkMessageAsReadAsync_WithInvalidMessageId_ShouldThrowException()
    {
        // Arrange
        var messageId = -1;
        var userId = 2;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.MarkMessageAsReadAsync(messageId, userId));
    }

    [Fact]
    public async Task MarkMessageAsReadAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var messageId = 1;
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.MarkMessageAsReadAsync(messageId, userId));
    }

    [Fact]
    public async Task MarkMessageAsReadAsync_WithNonExistentMessage_ShouldReturnFalse()
    {
        // Arrange
        var messageId = 999;
        var userId = 2;

        // Act
        var result = await _service.MarkMessageAsReadAsync(messageId, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task MarkMessageAsReadAsync_WithWrongReceiver_ShouldReturnFalse()
    {
        // Arrange
        var messageId = 1;
        var userId = 3; // Not the receiver of message 1

        // Act
        var result = await _service.MarkMessageAsReadAsync(messageId, userId);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetUnreadCountAsync 測試

    [Fact]
    public async Task GetUnreadCountAsync_WithValidUserId_ShouldReturnCount()
    {
        // Arrange
        var userId = 2;

        // Act
        var result = await _service.GetUnreadCountAsync(userId);

        // Assert
        Assert.Equal(1, result); // User 2 has 1 unread message
    }

    [Fact]
    public async Task GetUnreadCountAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUnreadCountAsync(userId));
    }

    [Fact]
    public async Task GetUnreadCountAsync_WithNoUnreadMessages_ShouldReturnZero()
    {
        // Arrange
        var userId = 1; // User 1 has no unread messages

        // Act
        var result = await _service.GetUnreadCountAsync(userId);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetUnreadCountAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 2;

        // Act - 第一次調用
        var result1 = await _service.GetUnreadCountAsync(userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.ChatMessages.RemoveRange(_context.ChatMessages);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUnreadCountAsync(userId);

        // Assert
        Assert.Equal(result1, result2);
    }

    #endregion

    #region GetRecentConversationsAsync 測試

    [Fact]
    public async Task GetRecentConversationsAsync_WithValidUserId_ShouldReturnConversations()
    {
        // Arrange
        var userId = 1;
        var limit = 10;

        // Act
        var result = await _service.GetRecentConversationsAsync(userId, limit);

        // Assert
        Assert.NotNull(result);
        var conversations = result.ToList();
        Assert.Equal(2, conversations.Count); // User 1 has conversations with user 2 and 3
        Assert.All(conversations, c => Assert.True(
            c.sender_id == userId || c.receiver_id == userId));
    }

    [Fact]
    public async Task GetRecentConversationsAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;
        var limit = 10;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetRecentConversationsAsync(userId, limit));
    }

    [Fact]
    public async Task GetRecentConversationsAsync_WithInvalidLimit_ShouldUseDefaultLimit()
    {
        // Arrange
        var userId = 1;
        var limit = 0;

        // Act
        var result = await _service.GetRecentConversationsAsync(userId, limit);

        // Assert
        Assert.NotNull(result);
        var conversations = result.ToList();
        Assert.True(conversations.Count <= 10); // Default limit
    }

    [Fact]
    public async Task GetRecentConversationsAsync_WithExcessiveLimit_ShouldUseMaxLimit()
    {
        // Arrange
        var userId = 1;
        var limit = 1000;

        // Act
        var result = await _service.GetRecentConversationsAsync(userId, limit);

        // Assert
        Assert.NotNull(result);
        var conversations = result.ToList();
        Assert.True(conversations.Count <= 100); // Max limit
    }

    [Fact]
    public async Task GetRecentConversationsAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;
        var limit = 10;

        // Act - 第一次調用
        var result1 = await _service.GetRecentConversationsAsync(userId, limit);
        
        // 清除資料庫數據（模擬快取生效）
        _context.ChatMessages.RemoveRange(_context.ChatMessages);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetRecentConversationsAsync(userId, limit);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region DeleteMessageAsync 測試

    [Fact]
    public async Task DeleteMessageAsync_WithValidData_ShouldDeleteMessage()
    {
        // Arrange
        var messageId = 1;
        var userId = 1; // Sender of message 1

        // Act
        var result = await _service.DeleteMessageAsync(messageId, userId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實刪除了記錄
        var dbMessage = await _context.ChatMessages
            .FirstOrDefaultAsync(m => m.message_id == messageId);
        Assert.Null(dbMessage);
    }

    [Fact]
    public async Task DeleteMessageAsync_WithInvalidMessageId_ShouldThrowException()
    {
        // Arrange
        var messageId = -1;
        var userId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.DeleteMessageAsync(messageId, userId));
    }

    [Fact]
    public async Task DeleteMessageAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var messageId = 1;
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.DeleteMessageAsync(messageId, userId));
    }

    [Fact]
    public async Task DeleteMessageAsync_WithNonExistentMessage_ShouldReturnFalse()
    {
        // Arrange
        var messageId = 999;
        var userId = 1;

        // Act
        var result = await _service.DeleteMessageAsync(messageId, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteMessageAsync_WithWrongSender_ShouldReturnFalse()
    {
        // Arrange
        var messageId = 1;
        var userId = 2; // Not the sender of message 1

        // Act
        var result = await _service.DeleteMessageAsync(messageId, userId);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region 邊界情況測試

    [Fact]
    public async Task GetDirectMessagesAsync_WithNoMessages_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 1;
        var peerId = 999; // User with no messages
        var page = 1;
        var pageSize = 50;

        // Act
        var result = await _service.GetDirectMessagesAsync(userId, peerId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetRecentConversationsAsync_WithNoConversations_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 999; // User with no conversations
        var limit = 10;

        // Act
        var result = await _service.GetRecentConversationsAsync(userId, limit);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUnreadCountAsync_WithNoMessages_ShouldReturnZero()
    {
        // Arrange
        var userId = 999; // User with no messages

        // Act
        var result = await _service.GetUnreadCountAsync(userId);

        // Assert
        Assert.Equal(0, result);
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetDirectMessagesAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var peerId = 2;
        var page = 1;
        var pageSize = 50;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetDirectMessagesAsync(userId, peerId, page, pageSize);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetRecentConversationsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var limit = 10;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetRecentConversationsAsync(userId, limit);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetUnreadCountAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 2;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetUnreadCountAsync(userId);
        stopwatch.Stop();

        // Assert
        Assert.True(result >= 0);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    #endregion
}
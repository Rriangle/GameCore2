using Xunit;
using Moq;
using GameCore.Api.Services;
using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;

namespace GameCore.Tests.Services;

/// <summary>
/// 聊天服務測試
/// </summary>
public class ChatServiceTests
{
    private readonly Mock<IChatMessageRepository> _mockChatMessageRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly ChatService _chatService;

    public ChatServiceTests()
    {
        _mockChatMessageRepository = new Mock<IChatMessageRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _chatService = new ChatService(
            _mockChatMessageRepository.Object,
            _mockUserRepository.Object,
            Mock.Of<ILogger<ChatService>>());
    }

    /// <summary>
    /// 測試取得聊天記錄
    /// </summary>
    [Fact]
    public async Task GetChatHistoryAsync_ShouldReturnChatHistory()
    {
        // Arrange
        var userId = 1;
        var otherUserId = 2;
        var messages = new List<ChatMessage>
        {
            new ChatMessage
            {
                MessageId = 1,
                SenderId = 1,
                ReceiverId = 2,
                ChatContent = "你好",
                SentAt = DateTime.UtcNow,
                IsRead = true,
                IsSent = true
            },
            new ChatMessage
            {
                MessageId = 2,
                SenderId = 2,
                ReceiverId = 1,
                ChatContent = "你好啊",
                SentAt = DateTime.UtcNow.AddMinutes(1),
                IsRead = false,
                IsSent = true
            }
        };

        var sender1 = new User { UserId = 1, UserName = "用戶1" };
        var sender2 = new User { UserId = 2, UserName = "用戶2" };

        _mockChatMessageRepository.Setup(x => x.GetChatHistoryAsync(userId, otherUserId, 1, 50))
            .ReturnsAsync(messages);
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(sender1);
        _mockUserRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(sender2);

        // Act
        var result = await _chatService.GetChatHistoryAsync(userId, otherUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        var firstMessage = result.First();
        Assert.Equal("你好", firstMessage.Content);
        Assert.Equal("用戶1", firstMessage.SenderName);
    }

    /// <summary>
    /// 測試發送聊天訊息
    /// </summary>
    [Fact]
    public async Task SendMessageAsync_ShouldSendMessage()
    {
        // Arrange
        var senderId = 1;
        var request = new SendChatMessageRequest
        {
            ReceiverId = 2,
            Content = "測試訊息"
        };

        var sender = new User { UserId = 1, UserName = "發送者" };
        var receiver = new User { UserId = 2, UserName = "接收者" };
        var createdMessage = new ChatMessage
        {
            MessageId = 1,
            SenderId = senderId,
            ReceiverId = 2,
            ChatContent = "測試訊息",
            SentAt = DateTime.UtcNow,
            IsRead = false,
            IsSent = true
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(senderId))
            .ReturnsAsync(sender);
        _mockUserRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(receiver);
        _mockChatMessageRepository.Setup(x => x.AddAsync(It.IsAny<ChatMessage>()))
            .ReturnsAsync(createdMessage);

        // Act
        var result = await _chatService.SendMessageAsync(senderId, request);

        // Assert
        Assert.True(result);
        _mockChatMessageRepository.Verify(x => x.AddAsync(It.IsAny<ChatMessage>()), Times.Once);
    }

    /// <summary>
    /// 測試發送聊天訊息 - 發送者不存在
    /// </summary>
    [Fact]
    public async Task SendMessageAsync_WithInvalidSender_ShouldReturnFalse()
    {
        // Arrange
        var senderId = 999;
        var request = new SendChatMessageRequest
        {
            ReceiverId = 2,
            Content = "測試訊息"
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(senderId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _chatService.SendMessageAsync(senderId, request);

        // Assert
        Assert.False(result);
        _mockChatMessageRepository.Verify(x => x.AddAsync(It.IsAny<ChatMessage>()), Times.Never);
    }

    /// <summary>
    /// 測試發送聊天訊息 - 接收者不存在
    /// </summary>
    [Fact]
    public async Task SendMessageAsync_WithInvalidReceiver_ShouldReturnFalse()
    {
        // Arrange
        var senderId = 1;
        var request = new SendChatMessageRequest
        {
            ReceiverId = 999,
            Content = "測試訊息"
        };

        var sender = new User { UserId = 1, UserName = "發送者" };

        _mockUserRepository.Setup(x => x.GetByIdAsync(senderId))
            .ReturnsAsync(sender);
        _mockUserRepository.Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _chatService.SendMessageAsync(senderId, request);

        // Assert
        Assert.False(result);
        _mockChatMessageRepository.Verify(x => x.AddAsync(It.IsAny<ChatMessage>()), Times.Never);
    }

    /// <summary>
    /// 測試標記訊息為已讀
    /// </summary>
    [Fact]
    public async Task MarkAsReadAsync_ShouldMarkAsRead()
    {
        // Arrange
        var messageId = 1;
        var userId = 1;

        _mockChatMessageRepository.Setup(x => x.MarkAsReadAsync(messageId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _chatService.MarkAsReadAsync(messageId, userId);

        // Assert
        Assert.True(result);
        _mockChatMessageRepository.Verify(x => x.MarkAsReadAsync(messageId, userId), Times.Once);
    }

    /// <summary>
    /// 測試標記與特定用戶的所有訊息為已讀
    /// </summary>
    [Fact]
    public async Task MarkAllAsReadAsync_ShouldMarkAllAsRead()
    {
        // Arrange
        var userId = 1;
        var otherUserId = 2;
        var expectedCount = 3;

        _mockChatMessageRepository.Setup(x => x.MarkAllAsReadAsync(userId, otherUserId))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _chatService.MarkAllAsReadAsync(userId, otherUserId);

        // Assert
        Assert.Equal(expectedCount, result);
        _mockChatMessageRepository.Verify(x => x.MarkAllAsReadAsync(userId, otherUserId), Times.Once);
    }

    /// <summary>
    /// 測試取得未讀訊息數量
    /// </summary>
    [Fact]
    public async Task GetUnreadCountAsync_ShouldReturnUnreadCount()
    {
        // Arrange
        var userId = 1;
        var expectedCount = 5;

        _mockChatMessageRepository.Setup(x => x.GetUnreadCountAsync(userId))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _chatService.GetUnreadCountAsync(userId);

        // Assert
        Assert.Equal(expectedCount, result);
        _mockChatMessageRepository.Verify(x => x.GetUnreadCountAsync(userId), Times.Once);
    }

    /// <summary>
    /// 測試取得聊天對象列表
    /// </summary>
    [Fact]
    public async Task GetChatPartnersAsync_ShouldReturnChatPartners()
    {
        // Arrange
        var userId = 1;
        var partners = new List<ChatMessage>
        {
            new ChatMessage
            {
                MessageId = 1,
                SenderId = 2,
                ReceiverId = 1,
                ChatContent = "最新訊息",
                SentAt = DateTime.UtcNow,
                IsRead = false,
                IsSent = true
            }
        };

        var partner = new User { UserId = 2, UserName = "聊天對象" };

        _mockChatMessageRepository.Setup(x => x.GetChatPartnersAsync(userId))
            .ReturnsAsync(partners);
        _mockUserRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(partner);

        // Act
        var result = await _chatService.GetChatPartnersAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var firstPartner = result.First();
        Assert.Equal("最新訊息", firstPartner.Content);
        Assert.Equal("聊天對象", firstPartner.SenderName);
    }

    /// <summary>
    /// 測試取得最新訊息
    /// </summary>
    [Fact]
    public async Task GetLatestMessageAsync_ShouldReturnLatestMessage()
    {
        // Arrange
        var userId = 1;
        var otherUserId = 2;
        var latestMessage = new ChatMessage
        {
            MessageId = 1,
            SenderId = 2,
            ReceiverId = 1,
            ChatContent = "最新訊息",
            SentAt = DateTime.UtcNow,
            IsRead = false,
            IsSent = true
        };

        var sender = new User { UserId = 2, UserName = "發送者" };

        _mockChatMessageRepository.Setup(x => x.GetLatestMessageAsync(userId, otherUserId))
            .ReturnsAsync(latestMessage);
        _mockUserRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(sender);

        // Act
        var result = await _chatService.GetLatestMessageAsync(userId, otherUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("最新訊息", result.Content);
        Assert.Equal("發送者", result.SenderName);
    }

    /// <summary>
    /// 測試搜尋聊天訊息
    /// </summary>
    [Fact]
    public async Task SearchMessagesAsync_ShouldReturnSearchResults()
    {
        // Arrange
        var userId = 1;
        var keyword = "測試";
        var messages = new List<ChatMessage>
        {
            new ChatMessage
            {
                MessageId = 1,
                SenderId = 1,
                ReceiverId = 2,
                ChatContent = "這是一個測試訊息",
                SentAt = DateTime.UtcNow,
                IsRead = true,
                IsSent = true
            }
        };

        var sender = new User { UserId = 1, UserName = "發送者" };

        _mockChatMessageRepository.Setup(x => x.SearchMessagesAsync(userId, keyword, 1, 20))
            .ReturnsAsync(messages);
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(sender);

        // Act
        var result = await _chatService.SearchMessagesAsync(userId, keyword);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        var firstMessage = result.First();
        Assert.Equal("這是一個測試訊息", firstMessage.Content);
        Assert.Equal("發送者", firstMessage.SenderName);
    }

    /// <summary>
    /// 測試取得聊天統計
    /// </summary>
    [Fact]
    public async Task GetChatStatsAsync_ShouldReturnChatStats()
    {
        // Arrange
        var userId = 1;
        var unreadCount = 3;
        var partners = new List<ChatMessage>
        {
            new ChatMessage { SentAt = DateTime.UtcNow },
            new ChatMessage { SentAt = DateTime.UtcNow.AddDays(-1) }
        };

        _mockChatMessageRepository.Setup(x => x.GetUnreadCountAsync(userId))
            .ReturnsAsync(unreadCount);
        _mockChatMessageRepository.Setup(x => x.GetChatPartnersAsync(userId))
            .ReturnsAsync(partners);

        // Act
        var result = await _chatService.GetChatStatsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(unreadCount, result.UnreadCount);
        Assert.Equal(2, result.ChatPartnerCount);
        Assert.Equal(1, result.TodayMessageCount);
    }
} 
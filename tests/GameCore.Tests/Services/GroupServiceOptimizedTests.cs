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
/// GroupService 優化版本單元測試
/// </summary>
public class GroupServiceOptimizedTests : IDisposable
{
    private readonly DbContextOptions<GameCoreDbContext> _options;
    private readonly GameCoreDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<GroupService> _logger;
    private readonly GroupService _service;

    public GroupServiceOptimizedTests()
    {
        _options = new DbContextOptionsBuilder<GameCoreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GameCoreDbContext(_options);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = Mock.Of<ILogger<GroupService>>();
        _service = new GroupService(_context, _memoryCache, _logger);

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
        var user4 = new User { user_id = 4, username = "user4", email = "user4@test.com" };
        var user5 = new User { user_id = 5, username = "user5", email = "user5@test.com" };
        _context.Users.AddRange(user1, user2, user3, user4, user5);

        // 創建群組
        var group1 = new Group 
        { 
            group_id = 1, 
            group_name = "Test Group 1", 
            description = "A test group", 
            created_by = 1, 
            created_at = DateTime.UtcNow.AddDays(-1), 
            is_active = true 
        };
        var group2 = new Group 
        { 
            group_id = 2, 
            group_name = "Test Group 2", 
            description = "Another test group", 
            created_by = 2, 
            created_at = DateTime.UtcNow.AddDays(-2), 
            is_active = true 
        };
        _context.Groups.AddRange(group1, group2);

        // 創建群組成員
        var member1 = new GroupMember 
        { 
            member_id = 1, 
            group_id = 1, 
            user_id = 1, 
            joined_at = DateTime.UtcNow.AddDays(-1), 
            role = "admin", 
            is_active = true 
        };
        var member2 = new GroupMember 
        { 
            member_id = 2, 
            group_id = 1, 
            user_id = 2, 
            joined_at = DateTime.UtcNow.AddDays(-1), 
            role = "member", 
            is_active = true 
        };
        var member3 = new GroupMember 
        { 
            member_id = 3, 
            group_id = 1, 
            user_id = 3, 
            joined_at = DateTime.UtcNow.AddDays(-1), 
            role = "member", 
            is_active = true 
        };
        var member4 = new GroupMember 
        { 
            member_id = 4, 
            group_id = 2, 
            user_id = 2, 
            joined_at = DateTime.UtcNow.AddDays(-2), 
            role = "admin", 
            is_active = true 
        };
        var member5 = new GroupMember 
        { 
            member_id = 5, 
            group_id = 2, 
            user_id = 4, 
            joined_at = DateTime.UtcNow.AddDays(-2), 
            role = "member", 
            is_active = true 
        };
        _context.GroupMembers.AddRange(member1, member2, member3, member4, member5);

        // 創建群組聊天訊息
        var message1 = new GroupChat 
        { 
            message_id = 1, 
            group_id = 1, 
            sender_id = 1, 
            message_content = "Hello everyone!", 
            sent_at = DateTime.UtcNow.AddHours(-1), 
            is_sent = true 
        };
        var message2 = new GroupChat 
        { 
            message_id = 2, 
            group_id = 1, 
            sender_id = 2, 
            message_content = "Hi there!", 
            sent_at = DateTime.UtcNow.AddHours(-2), 
            is_sent = true 
        };
        var message3 = new GroupChat 
        { 
            message_id = 3, 
            group_id = 2, 
            sender_id = 2, 
            message_content = "Group 2 message", 
            sent_at = DateTime.UtcNow.AddHours(-3), 
            is_sent = true 
        };
        _context.GroupChats.AddRange(message1, message2, message3);

        // 創建群組封鎖
        var block1 = new GroupBlock 
        { 
            block_id = 1, 
            group_id = 1, 
            blocked_user_id = 4, 
            blocked_by = 1, 
            blocked_at = DateTime.UtcNow.AddDays(-1), 
            reason = "Spam", 
            is_active = true 
        };
        var block2 = new GroupBlock 
        { 
            block_id = 2, 
            group_id = 2, 
            blocked_user_id = 5, 
            blocked_by = 2, 
            blocked_at = DateTime.UtcNow.AddDays(-2), 
            reason = "Inappropriate content", 
            is_active = true 
        };
        _context.GroupBlocks.AddRange(block1, block2);

        _context.SaveChanges();
    }

    #endregion

    #region GetUserGroupsAsync 測試

    [Fact]
    public async Task GetUserGroupsAsync_WithValidUserId_ShouldReturnGroups()
    {
        // Arrange
        var userId = 1;

        // Act
        var result = await _service.GetUserGroupsAsync(userId);

        // Assert
        Assert.NotNull(result);
        var groups = result.ToList();
        Assert.Equal(1, groups.Count);
        Assert.Equal(1, groups[0].group_id);
        Assert.Equal("Test Group 1", groups[0].group_name);
    }

    [Fact]
    public async Task GetUserGroupsAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetUserGroupsAsync(userId));
    }

    [Fact]
    public async Task GetUserGroupsAsync_WithNoGroups_ShouldReturnEmpty()
    {
        // Arrange
        var userId = 5; // User with no groups

        // Act
        var result = await _service.GetUserGroupsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserGroupsAsync_ShouldUseCache()
    {
        // Arrange
        var userId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetUserGroupsAsync(userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.GroupMembers.RemoveRange(_context.GroupMembers);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetUserGroupsAsync(userId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region GetGroupAsync 測試

    [Fact]
    public async Task GetGroupAsync_WithValidGroupId_ShouldReturnGroup()
    {
        // Arrange
        var groupId = 1;

        // Act
        var result = await _service.GetGroupAsync(groupId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(groupId, result.group_id);
        Assert.Equal("Test Group 1", result.group_name);
        Assert.Equal("A test group", result.description);
        Assert.Equal(1, result.created_by);
    }

    [Fact]
    public async Task GetGroupAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var groupId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetGroupAsync(groupId));
    }

    [Fact]
    public async Task GetGroupAsync_WithNonExistentGroup_ShouldReturnNull()
    {
        // Arrange
        var groupId = 999;

        // Act
        var result = await _service.GetGroupAsync(groupId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetGroupAsync_ShouldUseCache()
    {
        // Arrange
        var groupId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetGroupAsync(groupId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.Groups.RemoveRange(_context.Groups);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetGroupAsync(groupId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.group_id, result2.group_id);
    }

    #endregion

    #region GetGroupMembersAsync 測試

    [Fact]
    public async Task GetGroupMembersAsync_WithValidGroupId_ShouldReturnMembers()
    {
        // Arrange
        var groupId = 1;
        var page = 1;
        var pageSize = 50;

        // Act
        var result = await _service.GetGroupMembersAsync(groupId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var members = result.ToList();
        Assert.Equal(3, members.Count);
        Assert.All(members, m => Assert.Equal(groupId, m.group_id));
    }

    [Fact]
    public async Task GetGroupMembersAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var groupId = -1;
        var page = 1;
        var pageSize = 50;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetGroupMembersAsync(groupId, page, pageSize));
    }

    [Fact]
    public async Task GetGroupMembersAsync_WithInvalidPage_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var page = 0;
        var pageSize = 50;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetGroupMembersAsync(groupId, page, pageSize));
    }

    [Fact]
    public async Task GetGroupMembersAsync_WithInvalidPageSize_ShouldUseDefaultPageSize()
    {
        // Arrange
        var groupId = 1;
        var page = 1;
        var pageSize = 0;

        // Act
        var result = await _service.GetGroupMembersAsync(groupId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var members = result.ToList();
        Assert.True(members.Count <= 50); // Default page size
    }

    [Fact]
    public async Task GetGroupMembersAsync_ShouldUseCache()
    {
        // Arrange
        var groupId = 1;
        var page = 1;
        var pageSize = 50;

        // Act - 第一次調用
        var result1 = await _service.GetGroupMembersAsync(groupId, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.GroupMembers.RemoveRange(_context.GroupMembers);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetGroupMembersAsync(groupId, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region GetGroupMessagesAsync 測試

    [Fact]
    public async Task GetGroupMessagesAsync_WithValidParameters_ShouldReturnMessages()
    {
        // Arrange
        var groupId = 1;
        var page = 1;
        var pageSize = 50;

        // Act
        var result = await _service.GetGroupMessagesAsync(groupId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var messages = result.ToList();
        Assert.Equal(2, messages.Count);
        Assert.All(messages, m => Assert.Equal(groupId, m.group_id));
    }

    [Fact]
    public async Task GetGroupMessagesAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var groupId = -1;
        var page = 1;
        var pageSize = 50;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetGroupMessagesAsync(groupId, page, pageSize));
    }

    [Fact]
    public async Task GetGroupMessagesAsync_WithInvalidPage_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var page = 0;
        var pageSize = 50;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetGroupMessagesAsync(groupId, page, pageSize));
    }

    [Fact]
    public async Task GetGroupMessagesAsync_WithInvalidPageSize_ShouldUseDefaultPageSize()
    {
        // Arrange
        var groupId = 1;
        var page = 1;
        var pageSize = 0;

        // Act
        var result = await _service.GetGroupMessagesAsync(groupId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        var messages = result.ToList();
        Assert.True(messages.Count <= 50); // Default page size
    }

    [Fact]
    public async Task GetGroupMessagesAsync_ShouldUseCache()
    {
        // Arrange
        var groupId = 1;
        var page = 1;
        var pageSize = 50;

        // Act - 第一次調用
        var result1 = await _service.GetGroupMessagesAsync(groupId, page, pageSize);
        
        // 清除資料庫數據（模擬快取生效）
        _context.GroupChats.RemoveRange(_context.GroupChats);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetGroupMessagesAsync(groupId, page, pageSize);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region CreateGroupAsync 測試

    [Fact]
    public async Task CreateGroupAsync_WithValidData_ShouldCreateGroup()
    {
        // Arrange
        var groupName = "New Test Group";
        var description = "A new test group";
        var createdBy = 1;

        // Act
        var result = await _service.CreateGroupAsync(groupName, description, createdBy);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.group_id > 0);
        Assert.Equal(groupName, result.group_name);
        Assert.Equal(description, result.description);
        Assert.Equal(createdBy, result.created_by);
        Assert.True(result.is_active);
        Assert.True(result.created_at > DateTime.UtcNow.AddMinutes(-1));

        // 驗證資料庫中確實創建了記錄
        var dbGroup = await _context.Groups
            .FirstOrDefaultAsync(g => g.group_id == result.group_id);
        Assert.NotNull(dbGroup);

        // 驗證創建者自動成為管理員成員
        var dbMember = await _context.GroupMembers
            .FirstOrDefaultAsync(m => m.group_id == result.group_id && m.user_id == createdBy);
        Assert.NotNull(dbMember);
        Assert.Equal("admin", dbMember.role);
    }

    [Fact]
    public async Task CreateGroupAsync_WithEmptyGroupName_ShouldThrowException()
    {
        // Arrange
        var groupName = "";
        var description = "A new test group";
        var createdBy = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateGroupAsync(groupName, description, createdBy));
    }

    [Fact]
    public async Task CreateGroupAsync_WithNullGroupName_ShouldThrowException()
    {
        // Arrange
        string groupName = null!;
        var description = "A new test group";
        var createdBy = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateGroupAsync(groupName, description, createdBy));
    }

    [Fact]
    public async Task CreateGroupAsync_WithTooLongGroupName_ShouldThrowException()
    {
        // Arrange
        var groupName = new string('a', 101); // Exceeds MaxGroupNameLength
        var description = "A new test group";
        var createdBy = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateGroupAsync(groupName, description, createdBy));
    }

    [Fact]
    public async Task CreateGroupAsync_WithInvalidCreatedBy_ShouldThrowException()
    {
        // Arrange
        var groupName = "New Test Group";
        var description = "A new test group";
        var createdBy = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateGroupAsync(groupName, description, createdBy));
    }

    #endregion

    #region SendGroupMessageAsync 測試

    [Fact]
    public async Task SendGroupMessageAsync_WithValidData_ShouldSendMessage()
    {
        // Arrange
        var groupId = 1;
        var senderId = 1;
        var content = "Hello group!";

        // Act
        var result = await _service.SendGroupMessageAsync(groupId, senderId, content);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.message_id > 0);
        Assert.Equal(groupId, result.group_id);
        Assert.Equal(senderId, result.sender_id);
        Assert.Equal(content, result.message_content);
        Assert.True(result.is_sent);
        Assert.True(result.sent_at > DateTime.UtcNow.AddMinutes(-1));

        // 驗證資料庫中確實創建了記錄
        var dbMessage = await _context.GroupChats
            .FirstOrDefaultAsync(m => m.message_id == result.message_id);
        Assert.NotNull(dbMessage);
    }

    [Fact]
    public async Task SendGroupMessageAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var groupId = -1;
        var senderId = 1;
        var content = "Hello group!";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendGroupMessageAsync(groupId, senderId, content));
    }

    [Fact]
    public async Task SendGroupMessageAsync_WithInvalidSenderId_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var senderId = -1;
        var content = "Hello group!";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendGroupMessageAsync(groupId, senderId, content));
    }

    [Fact]
    public async Task SendGroupMessageAsync_WithEmptyContent_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var senderId = 1;
        var content = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendGroupMessageAsync(groupId, senderId, content));
    }

    [Fact]
    public async Task SendGroupMessageAsync_WithTooLongContent_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var senderId = 1;
        var content = new string('a', 1001); // Exceeds MaxMessageLength

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.SendGroupMessageAsync(groupId, senderId, content));
    }

    [Fact]
    public async Task SendGroupMessageAsync_WithNonMemberSender_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var senderId = 5; // Not a member of group 1
        var content = "Hello group!";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.SendGroupMessageAsync(groupId, senderId, content));
    }

    #endregion

    #region AddMemberAsync 測試

    [Fact]
    public async Task AddMemberAsync_WithValidData_ShouldAddMember()
    {
        // Arrange
        var groupId = 1;
        var userId = 4;
        var adminId = 1;

        // Act
        var result = await _service.AddMemberAsync(groupId, userId, adminId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實創建了記錄
        var dbMember = await _context.GroupMembers
            .FirstOrDefaultAsync(m => m.group_id == groupId && m.user_id == userId);
        Assert.NotNull(dbMember);
        Assert.Equal("member", dbMember.role);
        Assert.True(dbMember.is_active);
    }

    [Fact]
    public async Task AddMemberAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var groupId = -1;
        var userId = 4;
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddMemberAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task AddMemberAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = -1;
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddMemberAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task AddMemberAsync_WithInvalidAdminId_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = 4;
        var adminId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.AddMemberAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task AddMemberAsync_WithNonAdminUser_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = 4;
        var adminId = 2; // Not an admin of group 1

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.AddMemberAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task AddMemberAsync_WithAlreadyMember_ShouldReturnFalse()
    {
        // Arrange
        var groupId = 1;
        var userId = 2; // Already a member of group 1
        var adminId = 1;

        // Act
        var result = await _service.AddMemberAsync(groupId, userId, adminId);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region RemoveMemberAsync 測試

    [Fact]
    public async Task RemoveMemberAsync_WithValidData_ShouldRemoveMember()
    {
        // Arrange
        var groupId = 1;
        var userId = 3;
        var adminId = 1;

        // Act
        var result = await _service.RemoveMemberAsync(groupId, userId, adminId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實更新了記錄
        var dbMember = await _context.GroupMembers
            .FirstOrDefaultAsync(m => m.group_id == groupId && m.user_id == userId);
        Assert.NotNull(dbMember);
        Assert.False(dbMember.is_active);
    }

    [Fact]
    public async Task RemoveMemberAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var groupId = -1;
        var userId = 3;
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.RemoveMemberAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task RemoveMemberAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = -1;
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.RemoveMemberAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task RemoveMemberAsync_WithInvalidAdminId_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = 3;
        var adminId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.RemoveMemberAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task RemoveMemberAsync_WithNonAdminUser_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = 3;
        var adminId = 2; // Not an admin of group 1

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.RemoveMemberAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task RemoveMemberAsync_WithNonMember_ShouldReturnFalse()
    {
        // Arrange
        var groupId = 1;
        var userId = 5; // Not a member of group 1
        var adminId = 1;

        // Act
        var result = await _service.RemoveMemberAsync(groupId, userId, adminId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveMemberAsync_WithAdminSelf_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = 1; // Admin trying to remove themselves
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.RemoveMemberAsync(groupId, userId, adminId));
    }

    #endregion

    #region BlockUserAsync 測試

    [Fact]
    public async Task BlockUserAsync_WithValidData_ShouldBlockUser()
    {
        // Arrange
        var groupId = 1;
        var userId = 5;
        var adminId = 1;
        var reason = "Inappropriate behavior";

        // Act
        var result = await _service.BlockUserAsync(groupId, userId, adminId, reason);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實創建了記錄
        var dbBlock = await _context.GroupBlocks
            .FirstOrDefaultAsync(b => b.group_id == groupId && b.blocked_user_id == userId);
        Assert.NotNull(dbBlock);
        Assert.Equal(reason, dbBlock.reason);
        Assert.Equal(adminId, dbBlock.blocked_by);
        Assert.True(dbBlock.is_active);
    }

    [Fact]
    public async Task BlockUserAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var groupId = -1;
        var userId = 5;
        var adminId = 1;
        var reason = "Inappropriate behavior";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.BlockUserAsync(groupId, userId, adminId, reason));
    }

    [Fact]
    public async Task BlockUserAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = -1;
        var adminId = 1;
        var reason = "Inappropriate behavior";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.BlockUserAsync(groupId, userId, adminId, reason));
    }

    [Fact]
    public async Task BlockUserAsync_WithInvalidAdminId_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = 5;
        var adminId = -1;
        var reason = "Inappropriate behavior";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.BlockUserAsync(groupId, userId, adminId, reason));
    }

    [Fact]
    public async Task BlockUserAsync_WithNonAdminUser_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = 5;
        var adminId = 2; // Not an admin of group 1
        var reason = "Inappropriate behavior";

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.BlockUserAsync(groupId, userId, adminId, reason));
    }

    [Fact]
    public async Task BlockUserAsync_WithAlreadyBlockedUser_ShouldReturnFalse()
    {
        // Arrange
        var groupId = 1;
        var userId = 4; // Already blocked in group 1
        var adminId = 1;
        var reason = "Inappropriate behavior";

        // Act
        var result = await _service.BlockUserAsync(groupId, userId, adminId, reason);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region UnblockUserAsync 測試

    [Fact]
    public async Task UnblockUserAsync_WithValidData_ShouldUnblockUser()
    {
        // Arrange
        var groupId = 1;
        var userId = 4;
        var adminId = 1;

        // Act
        var result = await _service.UnblockUserAsync(groupId, userId, adminId);

        // Assert
        Assert.True(result);

        // 驗證資料庫中確實更新了記錄
        var dbBlock = await _context.GroupBlocks
            .FirstOrDefaultAsync(b => b.group_id == groupId && b.blocked_user_id == userId);
        Assert.NotNull(dbBlock);
        Assert.False(dbBlock.is_active);
    }

    [Fact]
    public async Task UnblockUserAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var groupId = -1;
        var userId = 4;
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.UnblockUserAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task UnblockUserAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = -1;
        var adminId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.UnblockUserAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task UnblockUserAsync_WithInvalidAdminId_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = 4;
        var adminId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.UnblockUserAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task UnblockUserAsync_WithNonAdminUser_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = 4;
        var adminId = 2; // Not an admin of group 1

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.UnblockUserAsync(groupId, userId, adminId));
    }

    [Fact]
    public async Task UnblockUserAsync_WithNotBlockedUser_ShouldReturnFalse()
    {
        // Arrange
        var groupId = 1;
        var userId = 5; // Not blocked in group 1
        var adminId = 1;

        // Act
        var result = await _service.UnblockUserAsync(groupId, userId, adminId);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsUserBlockedAsync 測試

    [Fact]
    public async Task IsUserBlockedAsync_WithBlockedUser_ShouldReturnTrue()
    {
        // Arrange
        var groupId = 1;
        var userId = 4; // Blocked in group 1

        // Act
        var result = await _service.IsUserBlockedAsync(groupId, userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsUserBlockedAsync_WithNotBlockedUser_ShouldReturnFalse()
    {
        // Arrange
        var groupId = 1;
        var userId = 5; // Not blocked in group 1

        // Act
        var result = await _service.IsUserBlockedAsync(groupId, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsUserBlockedAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var groupId = -1;
        var userId = 4;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.IsUserBlockedAsync(groupId, userId));
    }

    [Fact]
    public async Task IsUserBlockedAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.IsUserBlockedAsync(groupId, userId));
    }

    [Fact]
    public async Task IsUserBlockedAsync_ShouldUseCache()
    {
        // Arrange
        var groupId = 1;
        var userId = 4;

        // Act - 第一次調用
        var result1 = await _service.IsUserBlockedAsync(groupId, userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.GroupBlocks.RemoveRange(_context.GroupBlocks);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.IsUserBlockedAsync(groupId, userId);

        // Assert
        Assert.Equal(result1, result2);
    }

    #endregion

    #region IsUserAdminAsync 測試

    [Fact]
    public async Task IsUserAdminAsync_WithAdminUser_ShouldReturnTrue()
    {
        // Arrange
        var groupId = 1;
        var userId = 1; // Admin of group 1

        // Act
        var result = await _service.IsUserAdminAsync(groupId, userId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsUserAdminAsync_WithNonAdminUser_ShouldReturnFalse()
    {
        // Arrange
        var groupId = 1;
        var userId = 2; // Member but not admin of group 1

        // Act
        var result = await _service.IsUserAdminAsync(groupId, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsUserAdminAsync_WithNonMemberUser_ShouldReturnFalse()
    {
        // Arrange
        var groupId = 1;
        var userId = 5; // Not a member of group 1

        // Act
        var result = await _service.IsUserAdminAsync(groupId, userId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsUserAdminAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var groupId = -1;
        var userId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.IsUserAdminAsync(groupId, userId));
    }

    [Fact]
    public async Task IsUserAdminAsync_WithInvalidUserId_ShouldThrowException()
    {
        // Arrange
        var groupId = 1;
        var userId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.IsUserAdminAsync(groupId, userId));
    }

    [Fact]
    public async Task IsUserAdminAsync_ShouldUseCache()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;

        // Act - 第一次調用
        var result1 = await _service.IsUserAdminAsync(groupId, userId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.GroupMembers.RemoveRange(_context.GroupMembers);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.IsUserAdminAsync(groupId, userId);

        // Assert
        Assert.Equal(result1, result2);
    }

    #endregion

    #region GetGroupBlocksAsync 測試

    [Fact]
    public async Task GetGroupBlocksAsync_WithValidGroupId_ShouldReturnBlocks()
    {
        // Arrange
        var groupId = 1;

        // Act
        var result = await _service.GetGroupBlocksAsync(groupId);

        // Assert
        Assert.NotNull(result);
        var blocks = result.ToList();
        Assert.Equal(1, blocks.Count);
        Assert.All(blocks, b => Assert.Equal(groupId, b.group_id));
    }

    [Fact]
    public async Task GetGroupBlocksAsync_WithInvalidGroupId_ShouldThrowException()
    {
        // Arrange
        var groupId = -1;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.GetGroupBlocksAsync(groupId));
    }

    [Fact]
    public async Task GetGroupBlocksAsync_WithNoBlocks_ShouldReturnEmpty()
    {
        // Arrange
        var groupId = 999; // Group with no blocks

        // Act
        var result = await _service.GetGroupBlocksAsync(groupId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGroupBlocksAsync_ShouldUseCache()
    {
        // Arrange
        var groupId = 1;

        // Act - 第一次調用
        var result1 = await _service.GetGroupBlocksAsync(groupId);
        
        // 清除資料庫數據（模擬快取生效）
        _context.GroupBlocks.RemoveRange(_context.GroupBlocks);
        _context.SaveChanges();

        // Act - 第二次調用（應該從快取獲取）
        var result2 = await _service.GetGroupBlocksAsync(groupId);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Count(), result2.Count());
    }

    #endregion

    #region 邊界情況測試

    [Fact]
    public async Task GetGroupMembersAsync_WithNoMembers_ShouldReturnEmpty()
    {
        // Arrange
        var groupId = 999; // Group with no members
        var page = 1;
        var pageSize = 50;

        // Act
        var result = await _service.GetGroupMembersAsync(groupId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetGroupMessagesAsync_WithNoMessages_ShouldReturnEmpty()
    {
        // Arrange
        var groupId = 999; // Group with no messages
        var page = 1;
        var pageSize = 50;

        // Act
        var result = await _service.GetGroupMessagesAsync(groupId, page, pageSize);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region 性能測試

    [Fact]
    public async Task GetUserGroupsAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var userId = 1;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetUserGroupsAsync(userId);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetGroupMembersAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var groupId = 1;
        var page = 1;
        var pageSize = 50;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetGroupMembersAsync(groupId, page, pageSize);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task GetGroupMessagesAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var groupId = 1;
        var page = 1;
        var pageSize = 50;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.GetGroupMessagesAsync(groupId, page, pageSize);
        stopwatch.Stop();

        // Assert
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task IsUserBlockedAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var groupId = 1;
        var userId = 4;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.IsUserBlockedAsync(groupId, userId);
        stopwatch.Stop();

        // Assert
        Assert.True(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    [Fact]
    public async Task IsUserAdminAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _service.IsUserAdminAsync(groupId, userId);
        stopwatch.Stop();

        // Assert
        Assert.True(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000); // 應該在1秒內完成
    }

    #endregion
}
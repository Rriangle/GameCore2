using Xunit;
using Moq;
using GameCore.Api.Services;
using GameCore.Domain.Interfaces;
using GameCore.Domain.Entities;
using GameCore.Shared.DTOs;

namespace GameCore.Tests.Services;

/// <summary>
/// 群組服務測試
/// </summary>
public class GroupServiceTests
{
    private readonly Mock<IGroupRepository> _mockGroupRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly GroupService _groupService;

    public GroupServiceTests()
    {
        _mockGroupRepository = new Mock<IGroupRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _groupService = new GroupService(
            _mockGroupRepository.Object,
            _mockUserRepository.Object,
            Mock.Of<ILogger<GroupService>>());
    }

    /// <summary>
    /// 測試建立群組
    /// </summary>
    [Fact]
    public async Task CreateGroupAsync_ShouldCreateGroup()
    {
        // Arrange
        var creatorId = 1;
        var request = new CreateGroupRequest
        {
            GroupName = "測試群組",
            InitialMemberIds = new List<int> { 2, 3 }
        };

        var creator = new User { UserId = 1, UserName = "建立者" };
        var createdGroup = new Group
        {
            GroupId = 1,
            GroupName = "測試群組",
            CreatedBy = creatorId,
            CreatedAt = DateTime.UtcNow
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(creatorId))
            .ReturnsAsync(creator);
        _mockGroupRepository.Setup(x => x.AddAsync(It.IsAny<Group>()))
            .ReturnsAsync(createdGroup);
        _mockGroupRepository.Setup(x => x.AddMemberAsync(It.IsAny<GroupMember>()))
            .ReturnsAsync(new GroupMember());

        // Act
        var result = await _groupService.CreateGroupAsync(creatorId, request);

        // Assert
        Assert.Equal(1, result);
        _mockGroupRepository.Verify(x => x.AddAsync(It.IsAny<Group>()), Times.Once);
        _mockGroupRepository.Verify(x => x.AddMemberAsync(It.IsAny<GroupMember>()), Times.Exactly(3)); // 建立者 + 2個初始成員
    }

    /// <summary>
    /// 測試建立群組 - 建立者不存在
    /// </summary>
    [Fact]
    public async Task CreateGroupAsync_WithInvalidCreator_ShouldThrowException()
    {
        // Arrange
        var creatorId = 999;
        var request = new CreateGroupRequest
        {
            GroupName = "測試群組",
            InitialMemberIds = new List<int>()
        };

        _mockUserRepository.Setup(x => x.GetByIdAsync(creatorId))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _groupService.CreateGroupAsync(creatorId, request));
        _mockGroupRepository.Verify(x => x.AddAsync(It.IsAny<Group>()), Times.Never);
    }

    /// <summary>
    /// 測試取得群組詳情
    /// </summary>
    [Fact]
    public async Task GetGroupAsync_ShouldReturnGroupDetails()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;
        var group = new Group
        {
            GroupId = 1,
            GroupName = "測試群組",
            CreatedBy = 1,
            CreatedAt = DateTime.UtcNow
        };

        var creator = new User { UserId = 1, UserName = "建立者" };
        var members = new List<GroupMember>
        {
            new GroupMember { UserId = 1, IsAdmin = true },
            new GroupMember { UserId = 2, IsAdmin = false }
        };

        _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId))
            .ReturnsAsync(group);
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(creator);
        _mockGroupRepository.Setup(x => x.GetMembersAsync(groupId))
            .ReturnsAsync(members);
        _mockGroupRepository.Setup(x => x.IsMemberAsync(groupId, userId))
            .ReturnsAsync(true);
        _mockGroupRepository.Setup(x => x.IsAdminAsync(groupId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _groupService.GetGroupAsync(groupId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("測試群組", result.GroupName);
        Assert.Equal("建立者", result.CreatedByName);
        Assert.Equal(2, result.MemberCount);
        Assert.True(result.IsAdmin);
        Assert.True(result.IsMember);
    }

    /// <summary>
    /// 測試取得群組詳情 - 群組不存在
    /// </summary>
    [Fact]
    public async Task GetGroupAsync_WithInvalidGroup_ShouldReturnNull()
    {
        // Arrange
        var groupId = 999;
        var userId = 1;

        _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId))
            .ReturnsAsync((Group?)null);

        // Act
        var result = await _groupService.GetGroupAsync(groupId, userId);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// 測試取得用戶群組列表
    /// </summary>
    [Fact]
    public async Task GetUserGroupsAsync_ShouldReturnUserGroups()
    {
        // Arrange
        var userId = 1;
        var groups = new List<Group>
        {
            new Group
            {
                GroupId = 1,
                GroupName = "群組1",
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow
            },
            new Group
            {
                GroupId = 2,
                GroupName = "群組2",
                CreatedBy = 2,
                CreatedAt = DateTime.UtcNow
            }
        };

        var creator1 = new User { UserId = 1, UserName = "建立者1" };
        var creator2 = new User { UserId = 2, UserName = "建立者2" };
        var members1 = new List<GroupMember> { new GroupMember { UserId = 1 } };
        var members2 = new List<GroupMember> { new GroupMember { UserId = 1 } };

        _mockGroupRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(groups);
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(creator1);
        _mockUserRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(creator2);
        _mockGroupRepository.Setup(x => x.GetMembersAsync(1))
            .ReturnsAsync(members1);
        _mockGroupRepository.Setup(x => x.GetMembersAsync(2))
            .ReturnsAsync(members2);
        _mockGroupRepository.Setup(x => x.IsAdminAsync(1, userId))
            .ReturnsAsync(true);
        _mockGroupRepository.Setup(x => x.IsAdminAsync(2, userId))
            .ReturnsAsync(false);

        // Act
        var result = await _groupService.GetUserGroupsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        var firstGroup = result.First();
        Assert.Equal("群組1", firstGroup.GroupName);
        Assert.True(firstGroup.IsAdmin);
        Assert.True(firstGroup.IsMember);
    }

    /// <summary>
    /// 測試加入群組
    /// </summary>
    [Fact]
    public async Task JoinGroupAsync_ShouldJoinGroup()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;
        var group = new Group { GroupId = 1, GroupName = "測試群組" };

        _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId))
            .ReturnsAsync(group);
        _mockGroupRepository.Setup(x => x.IsMemberAsync(groupId, userId))
            .ReturnsAsync(false);
        _mockGroupRepository.Setup(x => x.IsBlockedAsync(groupId, userId))
            .ReturnsAsync(false);
        _mockGroupRepository.Setup(x => x.AddMemberAsync(It.IsAny<GroupMember>()))
            .ReturnsAsync(new GroupMember());

        // Act
        var result = await _groupService.JoinGroupAsync(groupId, userId);

        // Assert
        Assert.True(result);
        _mockGroupRepository.Verify(x => x.AddMemberAsync(It.IsAny<GroupMember>()), Times.Once);
    }

    /// <summary>
    /// 測試加入群組 - 已經是成員
    /// </summary>
    [Fact]
    public async Task JoinGroupAsync_WhenAlreadyMember_ShouldReturnFalse()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;
        var group = new Group { GroupId = 1, GroupName = "測試群組" };

        _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId))
            .ReturnsAsync(group);
        _mockGroupRepository.Setup(x => x.IsMemberAsync(groupId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _groupService.JoinGroupAsync(groupId, userId);

        // Assert
        Assert.False(result);
        _mockGroupRepository.Verify(x => x.AddMemberAsync(It.IsAny<GroupMember>()), Times.Never);
    }

    /// <summary>
    /// 測試退出群組
    /// </summary>
    [Fact]
    public async Task LeaveGroupAsync_ShouldLeaveGroup()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;
        var group = new Group { GroupId = 1, GroupName = "測試群組", CreatedBy = 2 };

        _mockGroupRepository.Setup(x => x.IsMemberAsync(groupId, userId))
            .ReturnsAsync(true);
        _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId))
            .ReturnsAsync(group);
        _mockGroupRepository.Setup(x => x.RemoveMemberAsync(groupId, userId))
            .ReturnsAsync(true);

        // Act
        var result = await _groupService.LeaveGroupAsync(groupId, userId);

        // Assert
        Assert.True(result);
        _mockGroupRepository.Verify(x => x.RemoveMemberAsync(groupId, userId), Times.Once);
    }

    /// <summary>
    /// 測試退出群組 - 不是成員
    /// </summary>
    [Fact]
    public async Task LeaveGroupAsync_WhenNotMember_ShouldReturnFalse()
    {
        // Arrange
        var groupId = 1;
        var userId = 1;

        _mockGroupRepository.Setup(x => x.IsMemberAsync(groupId, userId))
            .ReturnsAsync(false);

        // Act
        var result = await _groupService.LeaveGroupAsync(groupId, userId);

        // Assert
        Assert.False(result);
        _mockGroupRepository.Verify(x => x.RemoveMemberAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    /// <summary>
    /// 測試發送群組訊息
    /// </summary>
    [Fact]
    public async Task SendGroupMessageAsync_ShouldSendMessage()
    {
        // Arrange
        var groupId = 1;
        var senderId = 1;
        var request = new SendGroupMessageRequest
        {
            GroupId = 1,
            Content = "測試群組訊息"
        };

        var group = new Group { GroupId = 1, GroupName = "測試群組" };

        _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId))
            .ReturnsAsync(group);
        _mockGroupRepository.Setup(x => x.IsMemberAsync(groupId, senderId))
            .ReturnsAsync(true);
        _mockGroupRepository.Setup(x => x.IsBlockedAsync(groupId, senderId))
            .ReturnsAsync(false);
        _mockGroupRepository.Setup(x => x.AddChatMessageAsync(It.IsAny<GroupChat>()))
            .ReturnsAsync(new GroupChat());

        // Act
        var result = await _groupService.SendGroupMessageAsync(groupId, senderId, request);

        // Assert
        Assert.True(result);
        _mockGroupRepository.Verify(x => x.AddChatMessageAsync(It.IsAny<GroupChat>()), Times.Once);
    }

    /// <summary>
    /// 測試發送群組訊息 - 不是成員
    /// </summary>
    [Fact]
    public async Task SendGroupMessageAsync_WhenNotMember_ShouldReturnFalse()
    {
        // Arrange
        var groupId = 1;
        var senderId = 1;
        var request = new SendGroupMessageRequest
        {
            GroupId = 1,
            Content = "測試群組訊息"
        };

        var group = new Group { GroupId = 1, GroupName = "測試群組" };

        _mockGroupRepository.Setup(x => x.GetByIdAsync(groupId))
            .ReturnsAsync(group);
        _mockGroupRepository.Setup(x => x.IsMemberAsync(groupId, senderId))
            .ReturnsAsync(false);

        // Act
        var result = await _groupService.SendGroupMessageAsync(groupId, senderId, request);

        // Assert
        Assert.False(result);
        _mockGroupRepository.Verify(x => x.AddChatMessageAsync(It.IsAny<GroupChat>()), Times.Never);
    }

    /// <summary>
    /// 測試封鎖用戶
    /// </summary>
    [Fact]
    public async Task BlockUserAsync_ShouldBlockUser()
    {
        // Arrange
        var groupId = 1;
        var blockerId = 1;
        var request = new BlockUserRequest
        {
            GroupId = 1,
            UserId = 2
        };

        _mockGroupRepository.Setup(x => x.IsAdminAsync(groupId, blockerId))
            .ReturnsAsync(true);
        _mockGroupRepository.Setup(x => x.IsMemberAsync(groupId, request.UserId))
            .ReturnsAsync(true);
        _mockGroupRepository.Setup(x => x.IsBlockedAsync(groupId, request.UserId))
            .ReturnsAsync(false);
        _mockGroupRepository.Setup(x => x.AddBlockAsync(It.IsAny<GroupBlock>()))
            .ReturnsAsync(new GroupBlock());

        // Act
        var result = await _groupService.BlockUserAsync(groupId, blockerId, request);

        // Assert
        Assert.True(result);
        _mockGroupRepository.Verify(x => x.AddBlockAsync(It.IsAny<GroupBlock>()), Times.Once);
    }

    /// <summary>
    /// 測試封鎖用戶 - 不是管理員
    /// </summary>
    [Fact]
    public async Task BlockUserAsync_WhenNotAdmin_ShouldReturnFalse()
    {
        // Arrange
        var groupId = 1;
        var blockerId = 1;
        var request = new BlockUserRequest
        {
            GroupId = 1,
            UserId = 2
        };

        _mockGroupRepository.Setup(x => x.IsAdminAsync(groupId, blockerId))
            .ReturnsAsync(false);

        // Act
        var result = await _groupService.BlockUserAsync(groupId, blockerId, request);

        // Assert
        Assert.False(result);
        _mockGroupRepository.Verify(x => x.AddBlockAsync(It.IsAny<GroupBlock>()), Times.Never);
    }

    /// <summary>
    /// 測試搜尋群組
    /// </summary>
    [Fact]
    public async Task SearchGroupsAsync_ShouldReturnSearchResults()
    {
        // Arrange
        var keyword = "測試";
        var groups = new List<Group>
        {
            new Group
            {
                GroupId = 1,
                GroupName = "測試群組1",
                CreatedBy = 1,
                CreatedAt = DateTime.UtcNow
            },
            new Group
            {
                GroupId = 2,
                GroupName = "測試群組2",
                CreatedBy = 2,
                CreatedAt = DateTime.UtcNow
            }
        };

        var creator1 = new User { UserId = 1, UserName = "建立者1" };
        var creator2 = new User { UserId = 2, UserName = "建立者2" };

        _mockGroupRepository.Setup(x => x.SearchGroupsAsync(keyword, 1, 20))
            .ReturnsAsync(groups);
        _mockUserRepository.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(creator1);
        _mockUserRepository.Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(creator2);

        // Act
        var result = await _groupService.SearchGroupsAsync(keyword);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        var firstGroup = result.First();
        Assert.Equal("測試群組1", firstGroup.GroupName);
        Assert.Equal("建立者1", firstGroup.CreatedByName);
    }

    /// <summary>
    /// 測試取得群組統計
    /// </summary>
    [Fact]
    public async Task GetGroupStatsAsync_ShouldReturnGroupStats()
    {
        // Arrange
        var userId = 1;
        var groups = new List<Group>
        {
            new Group { GroupId = 1, CreatedBy = 1 },
            new Group { GroupId = 2, CreatedBy = 2 }
        };

        _mockGroupRepository.Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(groups);
        _mockGroupRepository.Setup(x => x.IsAdminAsync(1, userId))
            .ReturnsAsync(true);
        _mockGroupRepository.Setup(x => x.IsAdminAsync(2, userId))
            .ReturnsAsync(false);

        // Act
        var result = await _groupService.GetGroupStatsAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.JoinedGroupCount);
        Assert.Equal(1, result.AdminGroupCount);
        Assert.Equal(2, result.TotalGroupCount);
    }
} 
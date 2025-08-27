using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

public class GroupService : IGroupService
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<GroupService> _logger;

    public GroupService(GameCoreDbContext context, ILogger<GroupService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<GroupDto>> GetGroupsAsync()
    {
        try
        {
            var groups = await _context.Groups
                .Include(g => g.Owner)
                .Include(g => g.Members.Where(m => m.IsActive))
                .Where(g => g.IsActive)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            return groups.Select(g => new GroupDto
            {
                GroupId = g.GroupId,
                OwnerId = g.OwnerId,
                OwnerName = g.Owner.Username,
                Name = g.Name,
                Description = g.Description,
                AvatarUrl = g.AvatarUrl,
                IsActive = g.IsActive,
                MemberCount = g.Members.Count(m => m.IsActive),
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting groups");
            return Enumerable.Empty<GroupDto>();
        }
    }

    public async Task<GroupDto?> GetGroupAsync(int groupId)
    {
        try
        {
            var group = await _context.Groups
                .Include(g => g.Owner)
                .Include(g => g.Members.Where(m => m.IsActive))
                .FirstOrDefaultAsync(g => g.GroupId == groupId && g.IsActive);

            if (group == null) return null;

            return new GroupDto
            {
                GroupId = group.GroupId,
                OwnerId = group.OwnerId,
                OwnerName = group.Owner.Username,
                Name = group.Name,
                Description = group.Description,
                AvatarUrl = group.AvatarUrl,
                IsActive = group.IsActive,
                MemberCount = group.Members.Count(m => m.IsActive),
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting group {GroupId}", groupId);
            return null;
        }
    }

    public async Task<GroupResult> CreateGroupAsync(int userId, string name, string description, string? avatarUrl)
    {
        try
        {
            var group = new Group
            {
                OwnerId = userId,
                Name = name,
                Description = description,
                AvatarUrl = avatarUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            // Add owner as first member
            var member = new GroupMember
            {
                GroupId = group.GroupId,
                UserId = userId,
                Role = "owner",
                IsActive = true,
                JoinedAt = DateTime.UtcNow
            };

            _context.GroupMembers.Add(member);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Group created: User {UserId}, Name {Name}", userId, name);

            return new GroupResult
            {
                Success = true,
                Message = "群組創建成功",
                Group = await GetGroupAsync(group.GroupId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating group for user {UserId}", userId);
            return new GroupResult
            {
                Success = false,
                Message = "群組創建失敗"
            };
        }
    }

    public async Task<GroupResult> UpdateGroupAsync(int groupId, int userId, string name, string description, string? avatarUrl)
    {
        try
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.GroupId == groupId && g.OwnerId == userId);

            if (group == null)
            {
                return new GroupResult
                {
                    Success = false,
                    Message = "群組不存在或無權限修改"
                };
            }

            group.Name = name;
            group.Description = description;
            group.AvatarUrl = avatarUrl;
            group.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Group updated: Group {GroupId}, User {UserId}", groupId, userId);

            return new GroupResult
            {
                Success = true,
                Message = "群組更新成功",
                Group = await GetGroupAsync(groupId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating group {GroupId} for user {UserId}", groupId, userId);
            return new GroupResult
            {
                Success = false,
                Message = "群組更新失敗"
            };
        }
    }

    public async Task<GroupResult> DeleteGroupAsync(int groupId, int userId)
    {
        try
        {
            var group = await _context.Groups
                .FirstOrDefaultAsync(g => g.GroupId == groupId && g.OwnerId == userId);

            if (group == null)
            {
                return new GroupResult
                {
                    Success = false,
                    Message = "群組不存在或無權限刪除"
                };
            }

            group.IsActive = false;
            group.UpdatedAt = DateTime.UtcNow;

            // Deactivate all members
            var members = await _context.GroupMembers
                .Where(m => m.GroupId == groupId && m.IsActive)
                .ToListAsync();

            foreach (var member in members)
            {
                member.IsActive = false;
                member.LeftAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Group deleted: Group {GroupId}, User {UserId}", groupId, userId);

            return new GroupResult
            {
                Success = true,
                Message = "群組刪除成功"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting group {GroupId} for user {UserId}", groupId, userId);
            return new GroupResult
            {
                Success = false,
                Message = "群組刪除失敗"
            };
        }
    }

    public async Task<GroupMemberResult> AddMemberAsync(int groupId, int userId, int memberId, string role)
    {
        try
        {
            // Check if user has permission to add members
            var isOwner = await IsUserOwnerAsync(groupId, userId);
            if (!isOwner)
            {
                return new GroupMemberResult
                {
                    Success = false,
                    Message = "無權限添加成員"
                };
            }

            // Check if member already exists
            var existingMember = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == memberId);

            if (existingMember != null)
            {
                if (existingMember.IsActive)
                {
                    return new GroupMemberResult
                    {
                        Success = false,
                        Message = "用戶已經是群組成員"
                    };
                }
                else
                {
                    // Reactivate member
                    existingMember.IsActive = true;
                    existingMember.Role = role;
                    existingMember.JoinedAt = DateTime.UtcNow;
                    existingMember.LeftAt = null;
                }
            }
            else
            {
                var member = new GroupMember
                {
                    GroupId = groupId,
                    UserId = memberId,
                    Role = role,
                    IsActive = true,
                    JoinedAt = DateTime.UtcNow
                };

                _context.GroupMembers.Add(member);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Member added to group: Group {GroupId}, Member {MemberId}, Role {Role}", 
                groupId, memberId, role);

            return new GroupMemberResult
            {
                Success = true,
                Message = "成員添加成功",
                Member = await GetGroupMemberAsync(groupId, memberId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding member to group {GroupId}", groupId);
            return new GroupMemberResult
            {
                Success = false,
                Message = "成員添加失敗"
            };
        }
    }

    public async Task<GroupMemberResult> RemoveMemberAsync(int groupId, int userId, int memberId)
    {
        try
        {
            // Check if user has permission to remove members
            var isOwner = await IsUserOwnerAsync(groupId, userId);
            if (!isOwner && userId != memberId)
            {
                return new GroupMemberResult
                {
                    Success = false,
                    Message = "無權限移除成員"
                };
            }

            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == memberId && m.IsActive);

            if (member == null)
            {
                return new GroupMemberResult
                {
                    Success = false,
                    Message = "成員不存在"
                };
            }

            member.IsActive = false;
            member.LeftAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Member removed from group: Group {GroupId}, Member {MemberId}", 
                groupId, memberId);

            return new GroupMemberResult
            {
                Success = true,
                Message = "成員移除成功"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing member from group {GroupId}", groupId);
            return new GroupMemberResult
            {
                Success = false,
                Message = "成員移除失敗"
            };
        }
    }

    public async Task<GroupMemberResult> UpdateMemberRoleAsync(int groupId, int userId, int memberId, string role)
    {
        try
        {
            // Check if user has permission to update roles
            var isOwner = await IsUserOwnerAsync(groupId, userId);
            if (!isOwner)
            {
                return new GroupMemberResult
                {
                    Success = false,
                    Message = "無權限更新成員角色"
                };
            }

            var member = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == memberId && m.IsActive);

            if (member == null)
            {
                return new GroupMemberResult
                {
                    Success = false,
                    Message = "成員不存在"
                };
            }

            member.Role = role;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Member role updated: Group {GroupId}, Member {MemberId}, Role {Role}", 
                groupId, memberId, role);

            return new GroupMemberResult
            {
                Success = true,
                Message = "成員角色更新成功",
                Member = await GetGroupMemberAsync(groupId, memberId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating member role in group {GroupId}", groupId);
            return new GroupMemberResult
            {
                Success = false,
                Message = "成員角色更新失敗"
            };
        }
    }

    public async Task<IEnumerable<GroupMemberDto>> GetGroupMembersAsync(int groupId)
    {
        try
        {
            var members = await _context.GroupMembers
                .Include(m => m.User)
                .Where(m => m.GroupId == groupId && m.IsActive)
                .OrderBy(m => m.JoinedAt)
                .ToListAsync();

            return members.Select(m => new GroupMemberDto
            {
                MemberId = m.MemberId,
                GroupId = m.GroupId,
                UserId = m.UserId,
                Username = m.User.Username,
                Role = m.Role,
                IsActive = m.IsActive,
                JoinedAt = m.JoinedAt,
                LeftAt = m.LeftAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting group members for group {GroupId}", groupId);
            return Enumerable.Empty<GroupMemberDto>();
        }
    }

    public async Task<GroupConversationResult> SendMessageAsync(int groupId, int userId, string message, string messageType)
    {
        try
        {
            // Check if user is member of the group
            var isMember = await IsUserMemberAsync(groupId, userId);
            if (!isMember)
            {
                return new GroupConversationResult
                {
                    Success = false,
                    Message = "您不是群組成員"
                };
            }

            var conversation = new GroupConversation
            {
                GroupId = groupId,
                UserId = userId,
                Message = message,
                MessageType = messageType,
                CreatedAt = DateTime.UtcNow
            };

            _context.GroupConversations.Add(conversation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Message sent to group: Group {GroupId}, User {UserId}", groupId, userId);

            return new GroupConversationResult
            {
                Success = true,
                Message = "訊息發送成功",
                Message = new GroupConversationDto
                {
                    ConversationId = conversation.ConversationId,
                    GroupId = conversation.GroupId,
                    UserId = conversation.UserId,
                    Username = "當前用戶", // Will be populated when retrieved
                    Message = conversation.Message,
                    MessageType = conversation.MessageType,
                    CreatedAt = conversation.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message to group {GroupId}", groupId);
            return new GroupConversationResult
            {
                Success = false,
                Message = "訊息發送失敗"
            };
        }
    }

    public async Task<IEnumerable<GroupConversationDto>> GetGroupMessagesAsync(int groupId, int? limit = null)
    {
        try
        {
            var query = _context.GroupConversations
                .Include(c => c.User)
                .Where(c => c.GroupId == groupId)
                .OrderByDescending(c => c.CreatedAt);

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            var messages = await query.ToListAsync();

            return messages.Select(m => new GroupConversationDto
            {
                ConversationId = m.ConversationId,
                GroupId = m.GroupId,
                UserId = m.UserId,
                Username = m.User.Username,
                Message = m.Message,
                MessageType = m.MessageType,
                CreatedAt = m.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting group messages for group {GroupId}", groupId);
            return Enumerable.Empty<GroupConversationDto>();
        }
    }

    public async Task<IEnumerable<GroupDto>> GetUserGroupsAsync(int userId)
    {
        try
        {
            var groups = await _context.GroupMembers
                .Include(m => m.Group)
                .ThenInclude(g => g.Owner)
                .Include(m => m.Group.Members.Where(mem => mem.IsActive))
                .Where(m => m.UserId == userId && m.IsActive && m.Group.IsActive)
                .OrderByDescending(m => m.Group.CreatedAt)
                .Select(m => m.Group)
                .ToListAsync();

            return groups.Select(g => new GroupDto
            {
                GroupId = g.GroupId,
                OwnerId = g.OwnerId,
                OwnerName = g.Owner.Username,
                Name = g.Name,
                Description = g.Description,
                AvatarUrl = g.AvatarUrl,
                IsActive = g.IsActive,
                MemberCount = g.Members.Count(m => m.IsActive),
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user groups for user {UserId}", userId);
            return Enumerable.Empty<GroupDto>();
        }
    }

    public async Task<bool> IsUserMemberAsync(int groupId, int userId)
    {
        try
        {
            return await _context.GroupMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && m.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} is member of group {GroupId}", userId, groupId);
            return false;
        }
    }

    public async Task<bool> IsUserOwnerAsync(int groupId, int userId)
    {
        try
        {
            return await _context.Groups
                .AnyAsync(g => g.GroupId == groupId && g.OwnerId == userId && g.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} is owner of group {GroupId}", userId, groupId);
            return false;
        }
    }

    private async Task<GroupMemberDto?> GetGroupMemberAsync(int groupId, int memberId)
    {
        try
        {
            var member = await _context.GroupMembers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == memberId && m.IsActive);

            if (member == null) return null;

            return new GroupMemberDto
            {
                MemberId = member.MemberId,
                GroupId = member.GroupId,
                UserId = member.UserId,
                Username = member.User.Username,
                Role = member.Role,
                IsActive = member.IsActive,
                JoinedAt = member.JoinedAt,
                LeftAt = member.LeftAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting group member {MemberId} from group {GroupId}", memberId, groupId);
            return null;
        }
    }
}
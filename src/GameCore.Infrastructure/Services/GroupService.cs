using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services
{
    /// <summary>
    /// 群組服務實現
    /// </summary>
    public class GroupService : IGroupService
    {
        private readonly GameCoreDbContext _context;
        private readonly ILogger<GroupService> _logger;

        public GroupService(GameCoreDbContext context, ILogger<GroupService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 獲取用戶的群組列表
        /// </summary>
        public async Task<IEnumerable<Group>> GetUserGroupsAsync(int userId)
        {
            try
            {
                return await _context.GroupMembers
                    .Include(gm => gm.Group)
                        .ThenInclude(g => g.CreatedByUser)
                    .Where(gm => gm.user_id == userId)
                    .Select(gm => gm.Group)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶 {UserId} 的群組列表時發生錯誤", userId);
                throw;
            }
        }

        /// <summary>
        /// 獲取群組詳情
        /// </summary>
        public async Task<Group?> GetGroupAsync(int groupId)
        {
            try
            {
                return await _context.Groups
                    .Include(g => g.CreatedByUser)
                    .Include(g => g.Members)
                        .ThenInclude(gm => gm.User)
                    .Include(g => g.ChatMessages.OrderByDescending(gc => gc.sent_at).Take(10))
                        .ThenInclude(gc => gc.Sender)
                    .Include(g => g.Blocks)
                        .ThenInclude(gb => gb.BlockedUser)
                    .FirstOrDefaultAsync(g => g.group_id == groupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取群組 {GroupId} 詳情時發生錯誤", groupId);
                throw;
            }
        }

        /// <summary>
        /// 創建群組
        /// </summary>
        public async Task<Group> CreateGroupAsync(string groupName, int createdBy)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var group = new Group
                    {
                        group_name = groupName,
                        created_by = createdBy,
                        created_at = DateTime.UtcNow
                    };

                    _context.Groups.Add(group);
                    await _context.SaveChangesAsync();

                    // 創建者自動成為群組成員和管理員
                    var member = new GroupMember
                    {
                        group_id = group.group_id,
                        user_id = createdBy,
                        joined_at = DateTime.UtcNow,
                        is_admin = true
                    };

                    _context.GroupMembers.Add(member);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return group;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建群組時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 加入群組
        /// </summary>
        public async Task<bool> JoinGroupAsync(int groupId, int userId)
        {
            try
            {
                // 檢查是否已經被封鎖
                var isBlocked = await IsUserBlockedAsync(groupId, userId);
                if (isBlocked)
                    return false;

                // 檢查是否已經是成員
                var existingMember = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                if (existingMember != null)
                    return true;

                var member = new GroupMember
                {
                    group_id = groupId,
                    user_id = userId,
                    joined_at = DateTime.UtcNow,
                    is_admin = false
                };

                _context.GroupMembers.Add(member);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用戶 {UserId} 加入群組 {GroupId} 時發生錯誤", userId, groupId);
                throw;
            }
        }

        /// <summary>
        /// 離開群組
        /// </summary>
        public async Task<bool> LeaveGroupAsync(int groupId, int userId)
        {
            try
            {
                var member = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                if (member == null)
                    return false;

                _context.GroupMembers.Remove(member);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用戶 {UserId} 離開群組 {GroupId} 時發生錯誤", userId, groupId);
                throw;
            }
        }

        /// <summary>
        /// 移除群組成員
        /// </summary>
        public async Task<bool> RemoveMemberAsync(int groupId, int userId, int adminUserId)
        {
            try
            {
                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, adminUserId);
                if (!isAdmin)
                    return false;

                var member = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                if (member == null)
                    return false;

                _context.GroupMembers.Remove(member);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除群組 {GroupId} 成員 {UserId} 時發生錯誤", groupId, userId);
                throw;
            }
        }

        /// <summary>
        /// 設置成員為管理員
        /// </summary>
        public async Task<bool> SetMemberAsAdminAsync(int groupId, int userId, int adminUserId)
        {
            try
            {
                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, adminUserId);
                if (!isAdmin)
                    return false;

                var member = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                if (member == null)
                    return false;

                member.is_admin = true;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "設置群組 {GroupId} 成員 {UserId} 為管理員時發生錯誤", groupId, userId);
                throw;
            }
        }

        /// <summary>
        /// 移除管理員權限
        /// </summary>
        public async Task<bool> RemoveAdminAsync(int groupId, int userId, int adminUserId)
        {
            try
            {
                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, adminUserId);
                if (!isAdmin)
                    return false;

                var member = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                if (member == null)
                    return false;

                member.is_admin = false;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除群組 {GroupId} 成員 {UserId} 管理員權限時發生錯誤", groupId, userId);
                throw;
            }
        }

        /// <summary>
        /// 獲取群組成員列表
        /// </summary>
        public async Task<IEnumerable<GroupMember>> GetGroupMembersAsync(int groupId)
        {
            try
            {
                return await _context.GroupMembers
                    .Include(gm => gm.User)
                    .Where(gm => gm.group_id == groupId)
                    .OrderByDescending(gm => gm.is_admin)
                    .ThenBy(gm => gm.joined_at)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取群組 {GroupId} 成員列表時發生錯誤", groupId);
                throw;
            }
        }

        /// <summary>
        /// 獲取群組訊息列表
        /// </summary>
        public async Task<IEnumerable<GroupChat>> GetGroupMessagesAsync(int groupId, int page = 1, int pageSize = 50)
        {
            try
            {
                return await _context.GroupChats
                    .Include(gc => gc.Sender)
                    .Where(gc => gc.group_id == groupId)
                    .OrderByDescending(gc => gc.sent_at)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .OrderBy(gc => gc.sent_at)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取群組 {GroupId} 訊息列表時發生錯誤", groupId);
                throw;
            }
        }

        /// <summary>
        /// 發送群組訊息
        /// </summary>
        public async Task<GroupChat> SendGroupMessageAsync(int groupId, int senderId, string content)
        {
            try
            {
                // 檢查發送者是否被封鎖
                var isBlocked = await IsUserBlockedAsync(groupId, senderId);
                if (isBlocked)
                    throw new InvalidOperationException("用戶已被群組封鎖，無法發送訊息");

                var message = new GroupChat
                {
                    group_id = groupId,
                    sender_id = senderId,
                    group_chat_content = content,
                    sent_at = DateTime.UtcNow,
                    is_sent = true
                };

                _context.GroupChats.Add(message);
                await _context.SaveChangesAsync();

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送群組 {GroupId} 訊息時發生錯誤", groupId);
                throw;
            }
        }

        /// <summary>
        /// 封鎖用戶
        /// </summary>
        public async Task<bool> BlockUserAsync(int groupId, int userId, int blockedBy)
        {
            try
            {
                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, blockedBy);
                if (!isAdmin)
                    return false;

                // 檢查是否已經被封鎖
                var existingBlock = await _context.GroupBlocks
                    .FirstOrDefaultAsync(gb => gb.group_id == groupId && gb.user_id == userId);

                if (existingBlock != null)
                    return true;

                var block = new GroupBlock
                {
                    group_id = groupId,
                    user_id = userId,
                    blocked_by = blockedBy,
                    created_at = DateTime.UtcNow
                };

                _context.GroupBlocks.Add(block);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "封鎖群組 {GroupId} 用戶 {UserId} 時發生錯誤", groupId, userId);
                throw;
            }
        }

        /// <summary>
        /// 解除封鎖用戶
        /// </summary>
        public async Task<bool> UnblockUserAsync(int groupId, int userId, int adminUserId)
        {
            try
            {
                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, adminUserId);
                if (!isAdmin)
                    return false;

                var block = await _context.GroupBlocks
                    .FirstOrDefaultAsync(gb => gb.group_id == groupId && gb.user_id == userId);

                if (block == null)
                    return false;

                _context.GroupBlocks.Remove(block);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解除封鎖群組 {GroupId} 用戶 {UserId} 時發生錯誤", groupId, userId);
                throw;
            }
        }

        /// <summary>
        /// 獲取群組封鎖列表
        /// </summary>
        public async Task<IEnumerable<GroupBlock>> GetGroupBlocksAsync(int groupId)
        {
            try
            {
                return await _context.GroupBlocks
                    .Include(gb => gb.BlockedUser)
                    .Include(gb => gb.BlockedByUser)
                    .Where(gb => gb.group_id == groupId)
                    .OrderByDescending(gb => gb.created_at)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取群組 {GroupId} 封鎖列表時發生錯誤", groupId);
                throw;
            }
        }

        /// <summary>
        /// 檢查用戶是否被封鎖
        /// </summary>
        public async Task<bool> IsUserBlockedAsync(int groupId, int userId)
        {
            try
            {
                return await _context.GroupBlocks
                    .AnyAsync(gb => gb.group_id == groupId && gb.user_id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查用戶 {UserId} 是否被群組 {GroupId} 封鎖時發生錯誤", userId, groupId);
                throw;
            }
        }

        /// <summary>
        /// 檢查用戶是否為管理員
        /// </summary>
        public async Task<bool> IsUserAdminAsync(int groupId, int userId)
        {
            try
            {
                var member = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                return member?.is_admin ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查用戶 {UserId} 是否為群組 {GroupId} 管理員時發生錯誤", userId, groupId);
                throw;
            }
        }

        /// <summary>
        /// 刪除群組
        /// </summary>
        public async Task<bool> DeleteGroupAsync(int groupId, int adminUserId)
        {
            try
            {
                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, adminUserId);
                if (!isAdmin)
                    return false;

                var group = await _context.Groups
                    .Include(g => g.Members)
                    .Include(g => g.ChatMessages)
                    .Include(g => g.Blocks)
                    .FirstOrDefaultAsync(g => g.group_id == groupId);

                if (group == null)
                    return false;

                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除群組 {GroupId} 時發生錯誤", groupId);
                throw;
            }
        }
    }
}
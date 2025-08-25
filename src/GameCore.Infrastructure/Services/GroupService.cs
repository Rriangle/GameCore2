using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services
{
    /// <summary>
    /// 群組服務實現 - 優化版本
    /// 增強性能、快取、輸入驗證、錯誤處理和可維護性
    /// </summary>
    public class GroupService : IGroupService
    {
        private readonly GameCoreDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<GroupService> _logger;

        // 常數定義，提高可維護性
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 50;
        private const int MaxGroupNameLength = 100;
        private const int MaxMessageLength = 1000;
        private const int CacheExpirationMinutes = 15;
        private const string UserGroupsCacheKey = "UserGroups_{0}";
        private const string GroupCacheKey = "Group_{0}";
        private const string GroupMembersCacheKey = "GroupMembers_{0}";
        private const string GroupMessagesCacheKey = "GroupMessages_{0}_{1}_{2}";
        private const string GroupBlocksCacheKey = "GroupBlocks_{0}";
        private const string UserAdminCacheKey = "UserAdmin_{0}_{1}";
        private const string UserBlockedCacheKey = "UserBlocked_{0}_{1}";

        public GroupService(
            GameCoreDbContext context,
            IMemoryCache memoryCache,
            ILogger<GroupService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 獲取用戶的群組列表
        /// </summary>
        public async Task<IEnumerable<Group>> GetUserGroupsAsync(int userId)
        {
            try
            {
                // 輸入驗證
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));

                // 快取鍵
                var cacheKey = string.Format(UserGroupsCacheKey, userId);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<Group> cachedGroups))
                {
                    _logger.LogDebug("從快取獲取用戶 {UserId} 的群組列表", userId);
                    return cachedGroups;
                }

                var groups = await _context.GroupMembers
                    .Include(gm => gm.Group)
                        .ThenInclude(g => g.CreatedByUser)
                    .Where(gm => gm.user_id == userId)
                    .Select(gm => gm.Group)
                    .AsNoTracking()
                    .ToListAsync();

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, groups, cacheOptions);

                _logger.LogDebug("獲取用戶 {UserId} 的群組列表，共 {Count} 個", userId, groups.Count);
                return groups;
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));

                // 快取鍵
                var cacheKey = string.Format(GroupCacheKey, groupId);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out Group cachedGroup))
                {
                    _logger.LogDebug("從快取獲取群組 {GroupId} 詳情", groupId);
                    return cachedGroup;
                }

                var group = await _context.Groups
                    .Include(g => g.CreatedByUser)
                    .Include(g => g.Members)
                        .ThenInclude(gm => gm.User)
                    .Include(g => g.ChatMessages.OrderByDescending(gc => gc.sent_at).Take(10))
                        .ThenInclude(gc => gc.Sender)
                    .Include(g => g.Blocks)
                        .ThenInclude(gb => gb.BlockedUser)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.group_id == groupId);

                if (group != null)
                {
                    // 設置快取
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                    _memoryCache.Set(cacheKey, group, cacheOptions);
                }

                _logger.LogDebug("獲取群組 {GroupId} 詳情，結果: {Result}", groupId, group != null ? "找到" : "未找到");
                return group;
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
                // 輸入驗證
                if (string.IsNullOrWhiteSpace(groupName))
                    throw new ArgumentException("群組名稱不能為空", nameof(groupName));
                
                if (groupName.Length > MaxGroupNameLength)
                    throw new ArgumentException($"群組名稱長度不能超過 {MaxGroupNameLength} 個字符", nameof(groupName));
                
                if (createdBy <= 0)
                    throw new ArgumentException("創建者ID必須為正整數", nameof(createdBy));

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

                    // 清除相關快取
                    ClearUserGroupsCache(createdBy);

                    _logger.LogInformation("用戶 {CreatedBy} 成功創建群組 {GroupName}，群組ID: {GroupId}", 
                        createdBy, groupName, group.group_id);
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));

                // 檢查是否已經被封鎖
                var isBlocked = await IsUserBlockedAsync(groupId, userId);
                if (isBlocked)
                {
                    _logger.LogWarning("用戶 {UserId} 嘗試加入已被封鎖的群組 {GroupId}", userId, groupId);
                    return false;
                }

                // 檢查是否已經是成員
                var existingMember = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                if (existingMember != null)
                {
                    _logger.LogDebug("用戶 {UserId} 已經是群組 {GroupId} 的成員", userId, groupId);
                    return true;
                }

                var member = new GroupMember
                {
                    group_id = groupId,
                    user_id = userId,
                    joined_at = DateTime.UtcNow,
                    is_admin = false
                };

                _context.GroupMembers.Add(member);
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearUserGroupsCache(userId);
                ClearGroupMembersCache(groupId);
                ClearGroupCache(groupId);

                _logger.LogInformation("用戶 {UserId} 成功加入群組 {GroupId}", userId, groupId);
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));

                var member = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                if (member == null)
                {
                    _logger.LogWarning("用戶 {UserId} 嘗試離開不存在的群組 {GroupId}", userId, groupId);
                    return false;
                }

                _context.GroupMembers.Remove(member);
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearUserGroupsCache(userId);
                ClearGroupMembersCache(groupId);
                ClearGroupCache(groupId);
                ClearUserAdminCache(groupId, userId);

                _logger.LogInformation("用戶 {UserId} 成功離開群組 {GroupId}", userId, groupId);
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));
                
                if (adminUserId <= 0)
                    throw new ArgumentException("管理員用戶ID必須為正整數", nameof(adminUserId));

                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, adminUserId);
                if (!isAdmin)
                {
                    _logger.LogWarning("非管理員用戶 {AdminUserId} 嘗試移除群組 {GroupId} 的成員 {UserId}", adminUserId, groupId, userId);
                    return false;
                }

                var member = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                if (member == null)
                {
                    _logger.LogWarning("嘗試移除不存在的群組 {GroupId} 成員 {UserId}", groupId, userId);
                    return false;
                }

                _context.GroupMembers.Remove(member);
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearUserGroupsCache(userId);
                ClearGroupMembersCache(groupId);
                ClearGroupCache(groupId);
                ClearUserAdminCache(groupId, userId);

                _logger.LogInformation("管理員 {AdminUserId} 成功移除群組 {GroupId} 的成員 {UserId}", adminUserId, groupId, userId);
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));
                
                if (adminUserId <= 0)
                    throw new ArgumentException("管理員用戶ID必須為正整數", nameof(adminUserId));

                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, adminUserId);
                if (!isAdmin)
                {
                    _logger.LogWarning("非管理員用戶 {AdminUserId} 嘗試設置群組 {GroupId} 的成員 {UserId} 為管理員", adminUserId, groupId, userId);
                    return false;
                }

                var member = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                if (member == null)
                {
                    _logger.LogWarning("嘗試設置不存在的群組 {GroupId} 成員 {UserId} 為管理員", groupId, userId);
                    return false;
                }

                if (member.is_admin)
                {
                    _logger.LogDebug("群組 {GroupId} 的成員 {UserId} 已經是管理員", groupId, userId);
                    return true;
                }

                member.is_admin = true;
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearGroupMembersCache(groupId);
                ClearGroupCache(groupId);
                ClearUserAdminCache(groupId, userId);

                _logger.LogInformation("管理員 {AdminUserId} 成功設置群組 {GroupId} 的成員 {UserId} 為管理員", adminUserId, groupId, userId);
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));
                
                if (adminUserId <= 0)
                    throw new ArgumentException("管理員用戶ID必須為正整數", nameof(adminUserId));

                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, adminUserId);
                if (!isAdmin)
                {
                    _logger.LogWarning("非管理員用戶 {AdminUserId} 嘗試移除群組 {GroupId} 的成員 {UserId} 管理員權限", adminUserId, groupId, userId);
                    return false;
                }

                var member = await _context.GroupMembers
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                if (member == null)
                {
                    _logger.LogWarning("嘗試移除不存在的群組 {GroupId} 成員 {UserId} 管理員權限", groupId, userId);
                    return false;
                }

                if (!member.is_admin)
                {
                    _logger.LogDebug("群組 {GroupId} 的成員 {UserId} 不是管理員", groupId, userId);
                    return true;
                }

                member.is_admin = false;
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearGroupMembersCache(groupId);
                ClearGroupCache(groupId);
                ClearUserAdminCache(groupId, userId);

                _logger.LogInformation("管理員 {AdminUserId} 成功移除群組 {GroupId} 的成員 {UserId} 管理員權限", adminUserId, groupId, userId);
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));

                // 快取鍵
                var cacheKey = string.Format(GroupMembersCacheKey, groupId);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<GroupMember> cachedMembers))
                {
                    _logger.LogDebug("從快取獲取群組 {GroupId} 的成員列表", groupId);
                    return cachedMembers;
                }

                var members = await _context.GroupMembers
                    .Include(gm => gm.User)
                    .Where(gm => gm.group_id == groupId)
                    .OrderByDescending(gm => gm.is_admin)
                    .ThenBy(gm => gm.joined_at)
                    .AsNoTracking()
                    .ToListAsync();

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, members, cacheOptions);

                _logger.LogDebug("獲取群組 {GroupId} 的成員列表，共 {Count} 個", groupId, members.Count);
                return members;
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (page <= 0)
                    throw new ArgumentException("頁碼必須為正整數", nameof(page));
                
                if (pageSize <= 0 || pageSize > MaxPageSize)
                    pageSize = DefaultPageSize;

                // 快取鍵
                var cacheKey = string.Format(GroupMessagesCacheKey, groupId, page, pageSize);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<GroupChat> cachedMessages))
                {
                    _logger.LogDebug("從快取獲取群組 {GroupId} 的訊息列表，頁面 {Page}", groupId, page);
                    return cachedMessages;
                }

                var messages = await _context.GroupChats
                    .Include(gc => gc.Sender)
                    .Where(gc => gc.group_id == groupId)
                    .OrderByDescending(gc => gc.sent_at)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .OrderBy(gc => gc.sent_at)
                    .AsNoTracking()
                    .ToListAsync();

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, messages, cacheOptions);

                _logger.LogDebug("獲取群組 {GroupId} 的訊息列表，頁面 {Page}，共 {Count} 條", groupId, page, messages.Count);
                return messages;
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (senderId <= 0)
                    throw new ArgumentException("發送者ID必須為正整數", nameof(senderId));
                
                if (string.IsNullOrWhiteSpace(content))
                    throw new ArgumentException("訊息內容不能為空", nameof(content));
                
                if (content.Length > MaxMessageLength)
                    throw new ArgumentException($"訊息內容長度不能超過 {MaxMessageLength} 個字符", nameof(content));

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

                // 清除相關快取
                ClearGroupMessagesCache(groupId);
                ClearGroupCache(groupId);

                _logger.LogInformation("用戶 {SenderId} 成功在群組 {GroupId} 發送訊息，訊息ID: {MessageId}", 
                    senderId, groupId, message.message_id);
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));
                
                if (blockedBy <= 0)
                    throw new ArgumentException("封鎖者ID必須為正整數", nameof(blockedBy));

                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, blockedBy);
                if (!isAdmin)
                {
                    _logger.LogWarning("非管理員用戶 {BlockedBy} 嘗試封鎖群組 {GroupId} 的用戶 {UserId}", blockedBy, groupId, userId);
                    return false;
                }

                // 檢查是否已經被封鎖
                var existingBlock = await _context.GroupBlocks
                    .FirstOrDefaultAsync(gb => gb.group_id == groupId && gb.user_id == userId);

                if (existingBlock != null)
                {
                    _logger.LogDebug("用戶 {UserId} 已經被群組 {GroupId} 封鎖", userId, groupId);
                    return true;
                }

                var block = new GroupBlock
                {
                    group_id = groupId,
                    user_id = userId,
                    blocked_by = blockedBy,
                    created_at = DateTime.UtcNow
                };

                _context.GroupBlocks.Add(block);
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearGroupBlocksCache(groupId);
                ClearUserBlockedCache(groupId, userId);
                ClearGroupCache(groupId);

                _logger.LogInformation("管理員 {BlockedBy} 成功封鎖群組 {GroupId} 的用戶 {UserId}", blockedBy, groupId, userId);
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));
                
                if (adminUserId <= 0)
                    throw new ArgumentException("管理員用戶ID必須為正整數", nameof(adminUserId));

                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, adminUserId);
                if (!isAdmin)
                {
                    _logger.LogWarning("非管理員用戶 {AdminUserId} 嘗試解除封鎖群組 {GroupId} 的用戶 {UserId}", adminUserId, groupId, userId);
                    return false;
                }

                var block = await _context.GroupBlocks
                    .FirstOrDefaultAsync(gb => gb.group_id == groupId && gb.user_id == userId);

                if (block == null)
                {
                    _logger.LogWarning("嘗試解除封鎖不存在的群組 {GroupId} 用戶 {UserId} 封鎖", groupId, userId);
                    return false;
                }

                _context.GroupBlocks.Remove(block);
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearGroupBlocksCache(groupId);
                ClearUserBlockedCache(groupId, userId);
                ClearGroupCache(groupId);

                _logger.LogInformation("管理員 {AdminUserId} 成功解除封鎖群組 {GroupId} 的用戶 {UserId}", adminUserId, groupId, userId);
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));

                // 快取鍵
                var cacheKey = string.Format(GroupBlocksCacheKey, groupId);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<GroupBlock> cachedBlocks))
                {
                    _logger.LogDebug("從快取獲取群組 {GroupId} 的封鎖列表", groupId);
                    return cachedBlocks;
                }

                var blocks = await _context.GroupBlocks
                    .Include(gb => gb.BlockedUser)
                    .Include(gb => gb.BlockedByUser)
                    .Where(gb => gb.group_id == groupId)
                    .OrderByDescending(gb => gb.created_at)
                    .AsNoTracking()
                    .ToListAsync();

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, blocks, cacheOptions);

                _logger.LogDebug("獲取群組 {GroupId} 的封鎖列表，共 {Count} 個", groupId, blocks.Count);
                return blocks;
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));

                // 快取鍵
                var cacheKey = string.Format(UserBlockedCacheKey, groupId, userId);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out bool cachedResult))
                {
                    _logger.LogDebug("從快取獲取用戶 {UserId} 在群組 {GroupId} 的封鎖狀態: {Result}", userId, groupId, cachedResult);
                    return cachedResult;
                }

                var result = await _context.GroupBlocks
                    .AsNoTracking()
                    .AnyAsync(gb => gb.group_id == groupId && gb.user_id == userId);

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, result, cacheOptions);

                _logger.LogDebug("用戶 {UserId} 在群組 {GroupId} 的封鎖狀態: {Result}", userId, groupId, result);
                return result;
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (userId <= 0)
                    throw new ArgumentException("用戶ID必須為正整數", nameof(userId));

                // 快取鍵
                var cacheKey = string.Format(UserAdminCacheKey, groupId, userId);

                // 嘗試從快取獲取
                if (_memoryCache.TryGetValue(cacheKey, out bool cachedResult))
                {
                    _logger.LogDebug("從快取獲取用戶 {UserId} 在群組 {GroupId} 的管理員狀態: {Result}", userId, groupId, cachedResult);
                    return cachedResult;
                }

                var member = await _context.GroupMembers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(gm => gm.group_id == groupId && gm.user_id == userId);

                var result = member?.is_admin ?? false;

                // 設置快取
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                _memoryCache.Set(cacheKey, result, cacheOptions);

                _logger.LogDebug("用戶 {UserId} 在群組 {GroupId} 的管理員狀態: {Result}", userId, groupId, result);
                return result;
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
                // 輸入驗證
                if (groupId <= 0)
                    throw new ArgumentException("群組ID必須為正整數", nameof(groupId));
                
                if (adminUserId <= 0)
                    throw new ArgumentException("管理員用戶ID必須為正整數", nameof(adminUserId));

                // 檢查操作者是否為管理員
                var isAdmin = await IsUserAdminAsync(groupId, adminUserId);
                if (!isAdmin)
                {
                    _logger.LogWarning("非管理員用戶 {AdminUserId} 嘗試刪除群組 {GroupId}", adminUserId, groupId);
                    return false;
                }

                var group = await _context.Groups
                    .Include(g => g.Members)
                    .Include(g => g.ChatMessages)
                    .Include(g => g.Blocks)
                    .FirstOrDefaultAsync(g => g.group_id == groupId);

                if (group == null)
                {
                    _logger.LogWarning("嘗試刪除不存在的群組 {GroupId}", groupId);
                    return false;
                }

                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();

                // 清除相關快取
                ClearGroupCache(groupId);
                ClearGroupMembersCache(groupId);
                ClearGroupMessagesCache(groupId);
                ClearGroupBlocksCache(groupId);

                _logger.LogInformation("管理員 {AdminUserId} 成功刪除群組 {GroupId}", adminUserId, groupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除群組 {GroupId} 時發生錯誤", groupId);
                throw;
            }
        }

        #region 快取管理

        /// <summary>
        /// 清除用戶群組快取
        /// </summary>
        private void ClearUserGroupsCache(int userId)
        {
            var cacheKey = string.Format(UserGroupsCacheKey, userId);
            _memoryCache.Remove(cacheKey);
            _logger.LogDebug("清除用戶 {UserId} 的群組快取", userId);
        }

        /// <summary>
        /// 清除群組快取
        /// </summary>
        private void ClearGroupCache(int groupId)
        {
            var cacheKey = string.Format(GroupCacheKey, groupId);
            _memoryCache.Remove(cacheKey);
            _logger.LogDebug("清除群組 {GroupId} 的快取", groupId);
        }

        /// <summary>
        /// 清除群組成員快取
        /// </summary>
        private void ClearGroupMembersCache(int groupId)
        {
            var cacheKey = string.Format(GroupMembersCacheKey, groupId);
            _memoryCache.Remove(cacheKey);
            _logger.LogDebug("清除群組 {GroupId} 的成員快取", groupId);
        }

        /// <summary>
        /// 清除群組訊息快取
        /// </summary>
        private void ClearGroupMessagesCache(int groupId)
        {
            // 清除所有頁面的快取
            for (int page = 1; page <= 5; page++)
            {
                for (int pageSize = 10; pageSize <= MaxPageSize; pageSize += 10)
                {
                    var cacheKey = string.Format(GroupMessagesCacheKey, groupId, page, pageSize);
                    _memoryCache.Remove(cacheKey);
                }
            }
            _logger.LogDebug("清除群組 {GroupId} 的訊息快取", groupId);
        }

        /// <summary>
        /// 清除群組封鎖快取
        /// </summary>
        private void ClearGroupBlocksCache(int groupId)
        {
            var cacheKey = string.Format(GroupBlocksCacheKey, groupId);
            _memoryCache.Remove(cacheKey);
            _logger.LogDebug("清除群組 {GroupId} 的封鎖快取", groupId);
        }

        /// <summary>
        /// 清除用戶管理員狀態快取
        /// </summary>
        private void ClearUserAdminCache(int groupId, int userId)
        {
            var cacheKey = string.Format(UserAdminCacheKey, groupId, userId);
            _memoryCache.Remove(cacheKey);
            _logger.LogDebug("清除用戶 {UserId} 在群組 {GroupId} 的管理員狀態快取", userId, groupId);
        }

        /// <summary>
        /// 清除用戶封鎖狀態快取
        /// </summary>
        private void ClearUserBlockedCache(int groupId, int userId)
        {
            var cacheKey = string.Format(UserBlockedCacheKey, groupId, userId);
            _memoryCache.Remove(cacheKey);
            _logger.LogDebug("清除用戶 {UserId} 在群組 {GroupId} 的封鎖狀態快取", userId, groupId);
        }

        #endregion
    }
}
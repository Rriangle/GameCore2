using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Logging;
using GameCore.Domain.Entities;

namespace GameCore.Api.Services;

/// <summary>
/// 群組服務
/// 處理群組相關業務邏輯
/// </summary>
public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GroupService> _logger;

    public GroupService(
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        ILogger<GroupService> logger)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// 建立群組
    /// </summary>
    /// <param name="request">建立群組請求</param>
    /// <returns>群組資訊</returns>
    public async Task<ServiceResult<GroupDTO>> CreateGroupAsync(CreateGroupRequest request)
    {
        try
        {
            // 驗證創建者是否存在
            var creator = await _userRepository.GetByIdAsync(request.CreatedBy);
            if (creator == null)
            {
                return ServiceResult<GroupDTO>.CreateFailure("創建者不存在");
            }

            // 建立群組
            var group = new Group
            {
                GroupName = request.GroupName,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            var groupId = await _groupRepository.AddAsync(group);

            // 將創建者加入群組
            var member = new GroupMember
            {
                GroupID = group.GroupID,
                UserID = request.CreatedBy,
                JoinedAt = DateTime.UtcNow,
                IsAdmin = true
            };

            await _groupRepository.AddMemberAsync(member);

            var groupDto = new GroupDTO
            {
                GroupId = group.GroupID,
                GroupName = group.GroupName,
                CreatedBy = group.CreatedBy,
                CreatedAt = group.CreatedAt,
                MemberCount = 1
            };

            return ServiceResult<GroupDTO>.CreateSuccess(groupDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立群組時發生錯誤");
            return ServiceResult<GroupDTO>.CreateFailure("建立群組失敗");
        }
    }

    /// <summary>
    /// 加入群組
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>操作結果</returns>
    public async Task<ServiceResult<bool>> JoinGroupAsync(int groupId, int userId)
    {
        try
        {
            // 驗證群組是否存在
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return ServiceResult<bool>.CreateFailure("群組不存在");
            }

            // 驗證用戶是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult<bool>.CreateFailure("用戶不存在");
            }

            // 檢查用戶是否已被封鎖
            var isBlocked = await _groupRepository.IsUserBlockedAsync(groupId, userId);
            if (isBlocked)
            {
                return ServiceResult<bool>.CreateFailure("用戶已被群組封鎖");
            }

            // 加入群組
            var member = new GroupMember
            {
                GroupID = groupId,
                UserID = userId,
                JoinedAt = DateTime.UtcNow,
                IsAdmin = false
            };

            await _groupRepository.AddMemberAsync(member);

            return ServiceResult<bool>.CreateSuccess(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加入群組時發生錯誤");
            return ServiceResult<bool>.CreateFailure("加入群組失敗");
        }
    }

    /// <summary>
    /// 退出群組
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">用戶ID</param>
    /// <returns>操作結果</returns>
    public async Task<ServiceResult<bool>> LeaveGroupAsync(int groupId, int userId)
    {
        try
        {
            // 驗證群組是否存在
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return ServiceResult<bool>.CreateFailure("群組不存在");
            }

            // 檢查用戶是否為群組成員
            var member = await _groupRepository.GetMemberAsync(groupId, userId);
            if (member == null)
            {
                return ServiceResult<bool>.CreateFailure("用戶不是群組成員");
            }

            // 退出群組
            await _groupRepository.RemoveMemberAsync(groupId, userId);

            return ServiceResult<bool>.CreateSuccess(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "退出群組時發生錯誤");
            return ServiceResult<bool>.CreateFailure("退出群組失敗");
        }
    }

    /// <summary>
    /// 發送群組訊息
    /// </summary>
    /// <param name="request">發送訊息請求</param>
    /// <returns>訊息資訊</returns>
    public async Task<ServiceResult<GroupChatDTO>> SendGroupMessageAsync(SendGroupMessageRequest request)
    {
        try
        {
            // 驗證群組是否存在
            var group = await _groupRepository.GetByIdAsync(request.GroupId);
            if (group == null)
            {
                return ServiceResult<GroupChatDTO>.CreateFailure("群組不存在");
            }

            // 檢查發送者是否為群組成員
            var member = await _groupRepository.GetMemberAsync(request.GroupId, request.SenderId);
            if (member == null)
            {
                return ServiceResult<GroupChatDTO>.CreateFailure("發送者不是群組成員");
            }

            // 檢查發送者是否被封鎖
            var isBlocked = await _groupRepository.IsUserBlockedAsync(request.GroupId, request.SenderId);
            if (isBlocked)
            {
                return ServiceResult<GroupChatDTO>.CreateFailure("發送者已被群組封鎖");
            }

            // 建立群組聊天訊息
            var groupChat = new GroupChat
            {
                GroupID = request.GroupId,
                SenderID = request.SenderId,
                GroupChatContent = request.Content,
                SentAt = DateTime.UtcNow,
                IsSent = true
            };

            var messageId = await _groupRepository.AddMessageAsync(groupChat);

            var groupChatDto = new GroupChatDTO
            {
                GroupChatID = groupChat.GroupChatID,
                GroupID = groupChat.GroupID,
                SenderID = groupChat.SenderID,
                GroupChatContent = groupChat.GroupChatContent,
                SentAt = groupChat.SentAt,
                IsSent = groupChat.IsSent
            };

            return ServiceResult<GroupChatDTO>.CreateSuccess(groupChatDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發送群組訊息時發生錯誤");
            return ServiceResult<GroupChatDTO>.CreateFailure("發送群組訊息失敗");
        }
    }

    /// <summary>
    /// 封鎖群組成員
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">被封鎖用戶ID</param>
    /// <param name="blockedBy">封鎖者ID</param>
    /// <returns>操作結果</returns>
    public async Task<ServiceResult<bool>> BlockGroupMemberAsync(int groupId, int userId, int blockedBy)
    {
        try
        {
            // 驗證群組是否存在
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return ServiceResult<bool>.CreateFailure("群組不存在");
            }

            // 檢查封鎖者是否為群組成員
            var blocker = await _groupRepository.GetMemberAsync(groupId, blockedBy);
            if (blocker == null)
            {
                return ServiceResult<bool>.CreateFailure("封鎖者不是群組成員");
            }

            // 建立封鎖記錄
            var groupBlock = new GroupBlock
            {
                GroupID = groupId,
                UserID = userId,
                BlockedBy = blockedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _groupRepository.AddBlockAsync(groupBlock);

            return ServiceResult<bool>.CreateSuccess(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "封鎖群組成員時發生錯誤");
            return ServiceResult<bool>.CreateFailure("封鎖群組成員失敗");
        }
    }

    /// <summary>
    /// 解除封鎖群組成員
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">被解除封鎖用戶ID</param>
    /// <param name="unblockedBy">解除封鎖者ID</param>
    /// <returns>操作結果</returns>
    public async Task<ServiceResult<bool>> UnblockGroupMemberAsync(int groupId, int userId, int unblockedBy)
    {
        try
        {
            // 驗證群組是否存在
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return ServiceResult<bool>.CreateFailure("群組不存在");
            }

            // 檢查解除封鎖者是否為群組成員
            var unblocker = await _groupRepository.GetMemberAsync(groupId, unblockedBy);
            if (unblocker == null)
            {
                return ServiceResult<bool>.CreateFailure("解除封鎖者不是群組成員");
            }

            // 移除封鎖記錄
            await _groupRepository.RemoveBlockAsync(groupId, userId);

            return ServiceResult<bool>.CreateSuccess(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解除封鎖群組成員時發生錯誤");
            return ServiceResult<bool>.CreateFailure("解除封鎖群組成員失敗");
        }
    }

    /// <summary>
    /// 取得群組資訊
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>群組資訊</returns>
    public async Task<ServiceResult<GroupDTO>> GetGroupAsync(int groupId)
    {
        try
        {
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return ServiceResult<GroupDTO>.CreateFailure("群組不存在");
            }

            var memberCount = await _groupRepository.GetMemberCountAsync(groupId);

            var groupDto = new GroupDTO
            {
                GroupId = group.GroupID,
                GroupName = group.GroupName,
                CreatedBy = group.CreatedBy,
                CreatedAt = group.CreatedAt,
                MemberCount = memberCount,
                MessageCount = 0 // 暫時設為0，因為實體中沒有這個屬性
            };

            return ServiceResult<GroupDTO>.CreateSuccess(groupDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得群組資訊時發生錯誤");
            return ServiceResult<GroupDTO>.CreateFailure("取得群組資訊失敗");
        }
    }

    /// <summary>
    /// 取得用戶的群組列表
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>群組列表</returns>
    public async Task<ServiceResult<List<GroupDTO>>> GetUserGroupsAsync(int userId)
    {
        try
        {
            var groups = await _groupRepository.GetUserGroupsAsync(userId);
            var groupDtos = new List<GroupDTO>();

            foreach (var group in groups)
            {
                var memberCount = await _groupRepository.GetMemberCountAsync(group.GroupID);

                groupDtos.Add(new GroupDTO
                {
                    GroupId = group.GroupID,
                    GroupName = group.GroupName,
                    CreatedBy = group.CreatedBy,
                    CreatedAt = group.CreatedAt,
                    MemberCount = memberCount
                });
            }

            return ServiceResult<List<GroupDTO>>.CreateSuccess(groupDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶群組列表時發生錯誤");
            return ServiceResult<List<GroupDTO>>.CreateFailure("取得用戶群組列表失敗");
        }
    }

    /// <summary>
    /// 取得群組成員列表
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>成員列表</returns>
    public async Task<ServiceResult<List<GroupMemberDTO>>> GetGroupMembersAsync(int groupId)
    {
        try
        {
            var members = await _groupRepository.GetGroupMembersAsync(groupId);
            var memberDtos = members.Select(m => new GroupMemberDTO
            {
                GroupId = m.GroupID,
                UserId = m.UserID,
                JoinedAt = m.JoinedAt,
                IsAdmin = m.IsAdmin
            }).ToList();

            return ServiceResult<List<GroupMemberDTO>>.CreateSuccess(memberDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得群組成員列表時發生錯誤");
            return ServiceResult<List<GroupMemberDTO>>.CreateFailure("取得群組成員列表失敗");
        }
    }

    /// <summary>
    /// 取得群組聊天記錄
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>聊天記錄</returns>
    public async Task<ServiceResult<List<GroupChatDTO>>> GetGroupChatHistoryAsync(int groupId, int page = 1, int pageSize = 20)
    {
        try
        {
            var messages = await _groupRepository.GetGroupChatHistoryAsync(groupId, page, pageSize);
            var messageDtos = messages.Select(m => new GroupChatDTO
            {
                GroupChatID = m.GroupChatID,
                GroupID = m.GroupID,
                SenderID = m.SenderID,
                GroupChatContent = m.GroupChatContent,
                SentAt = m.SentAt,
                IsSent = m.IsSent
            }).ToList();

            return ServiceResult<List<GroupChatDTO>>.CreateSuccess(messageDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得群組聊天記錄時發生錯誤");
            return ServiceResult<List<GroupChatDTO>>.CreateFailure("取得群組聊天記錄失敗");
        }
    }

    /// <summary>
    /// 搜尋群組
    /// </summary>
    /// <param name="keyword">搜尋關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>群組列表</returns>
    public async Task<ServiceResult<List<GroupDTO>>> SearchGroupsAsync(string keyword, int page = 1, int pageSize = 20)
    {
        try
        {
            var groups = await _groupRepository.SearchGroupsAsync(keyword, page, pageSize);
            var groupDtos = new List<GroupDTO>();

            foreach (var group in groups)
            {
                var memberCount = await _groupRepository.GetMemberCountAsync(group.GroupID);

                groupDtos.Add(new GroupDTO
                {
                    GroupId = group.GroupID,
                    GroupName = group.GroupName,
                    CreatedBy = group.CreatedBy,
                    CreatedAt = group.CreatedAt,
                    MemberCount = memberCount
                });
            }

            return ServiceResult<List<GroupDTO>>.CreateSuccess(groupDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋群組時發生錯誤");
            return ServiceResult<List<GroupDTO>>.CreateFailure("搜尋群組失敗");
        }
    }

    /// <summary>
    /// 封鎖群組用戶
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">被封鎖用戶ID</param>
    /// <param name="blockedBy">封鎖者ID</param>
    /// <returns>操作結果</returns>
    public async Task<ServiceResult<bool>> BlockUserAsync(int groupId, int userId, int blockedBy)
    {
        try
        {
            // 驗證群組是否存在
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return ServiceResult<bool>.CreateFailure("群組不存在");
            }

            // 檢查是否已被封鎖
            var isBlocked = await _groupRepository.IsUserBlockedAsync(groupId, userId);
            if (isBlocked)
            {
                return ServiceResult<bool>.CreateFailure("用戶已被群組封鎖");
            }

            // 建立封鎖記錄
            var groupBlock = new GroupBlock
            {
                GroupID = groupId,
                UserID = userId,
                BlockedBy = blockedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _groupRepository.AddBlockAsync(groupBlock);

            return ServiceResult<bool>.CreateSuccess(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "封鎖群組用戶時發生錯誤");
            return ServiceResult<bool>.CreateFailure("封鎖群組用戶失敗");
        }
    }

    /// <summary>
    /// 解除群組用戶封鎖
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">被解除封鎖用戶ID</param>
    /// <returns>操作結果</returns>
    public async Task<ServiceResult<bool>> UnblockUserAsync(int groupId, int userId)
    {
        try
        {
            // 驗證群組是否存在
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return ServiceResult<bool>.CreateFailure("群組不存在");
            }

            // 檢查是否已被封鎖
            var isBlocked = await _groupRepository.IsUserBlockedAsync(groupId, userId);
            if (!isBlocked)
            {
                return ServiceResult<bool>.CreateFailure("用戶未被群組封鎖");
            }

            // 移除封鎖記錄
            var result = await _groupRepository.RemoveBlockAsync(groupId, userId);
            if (!result)
            {
                return ServiceResult<bool>.CreateFailure("解除封鎖失敗");
            }

            return ServiceResult<bool>.CreateSuccess(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解除群組用戶封鎖時發生錯誤");
            return ServiceResult<bool>.CreateFailure("解除群組用戶封鎖失敗");
        }
    }

    /// <summary>
    /// 取得群組統計資訊
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>統計資訊</returns>
    public async Task<ServiceResult<GroupStatsDTO>> GetGroupStatsAsync(int groupId)
    {
        try
        {
            var group = await _groupRepository.GetByIdAsync(groupId);
            if (group == null)
            {
                return ServiceResult<GroupStatsDTO>.CreateFailure("群組不存在");
            }

            var memberCount = await _groupRepository.GetMemberCountAsync(groupId);
            var messageCount = await _groupRepository.GetMessageCountAsync(groupId);

            var stats = new GroupStatsDTO
            {
                JoinedGroupCount = memberCount,
                AdminGroupCount = 0, // 暫時設為0，需要實作管理員統計
                TotalGroupCount = 1 // 暫時設為1，需要實作總群組統計
            };

            return ServiceResult<GroupStatsDTO>.CreateSuccess(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得群組統計資訊時發生錯誤");
            return ServiceResult<GroupStatsDTO>.CreateFailure("取得群組統計資訊失敗");
        }
    }

    /// <summary>
    /// 取得用戶群組統計
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>群組統計</returns>
    public async Task<ServiceResult<GroupStatsDTO>> GetUserGroupStatsAsync(int userId)
    {
        try
        {
            // 效能優化：使用資料庫層面計數方法，避免載入完整資料到記憶體
            var joinedGroupCount = await _groupRepository.GetUserJoinedGroupCountAsync(userId);
            var adminGroupCount = await _groupRepository.GetUserAdminGroupCountAsync(userId);
            var totalGroups = await _groupRepository.GetTotalGroupCountAsync();

            var stats = new GroupStatsDTO
            {
                JoinedGroupCount = joinedGroupCount,
                AdminGroupCount = adminGroupCount,
                TotalGroupCount = totalGroups
            };

            return ServiceResult<GroupStatsDTO>.CreateSuccess(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶群組統計時發生錯誤");
            return ServiceResult<GroupStatsDTO>.CreateFailure("取得用戶群組統計失敗");
        }
    }

    /// <summary>
    /// 取得群組聊天訊息
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>聊天訊息列表</returns>
    public async Task<IEnumerable<GroupChatDTO>> GetGroupMessagesAsync(int groupId, int page = 1, int pageSize = 20)
    {
        try
        {
            var messages = await _groupRepository.GetGroupMessagesAsync(groupId, page, pageSize);

            var messageDtos = new List<GroupChatDTO>();

            foreach (var m in messages)
            {
                var sender = await _userRepository.GetByIdAsync(m.SenderID);
                var messageDto = new GroupChatDTO
                {
                    GroupChatID = m.GroupChatID,
                    GroupID = m.GroupID,
                    SenderID = m.SenderID,
                    GroupChatContent = m.GroupChatContent,
                    SentAt = m.SentAt,
                    IsSent = m.IsSent
                };

                if (sender != null)
                {
                    messageDto.SenderName = sender.User_Name;
                }

                messageDtos.Add(messageDto);
            }

            return messageDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得群組聊天訊息時發生錯誤");
            throw;
        }
    }
} 
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Controllers
{
    /// <summary>
    /// 群組控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly ILogger<GroupController> _logger;

        public GroupController(IGroupService groupService, ILogger<GroupController> logger)
        {
            _groupService = groupService;
            _logger = logger;
        }

        /// <summary>
        /// 獲取用戶的群組列表
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<GroupListResponseDto>> GetUserGroups(int userId)
        {
            try
            {
                var groups = await _groupService.GetUserGroupsAsync(userId);

                var response = new GroupListResponseDto
                {
                    Groups = groups.Select(g => new GroupResponseDto
                    {
                        GroupId = g.group_id,
                        GroupName = g.group_name,
                        CreatedByName = g.CreatedByUser?.User_name ?? "未知",
                        CreatedAt = g.created_at,
                        MemberCount = g.Members?.Count ?? 0,
                        IsUserMember = true,
                        IsUserAdmin = g.Members?.FirstOrDefault(m => m.user_id == userId)?.is_admin ?? false,
                        IsUserBlocked = false // 需要額外檢查
                    }).ToList(),
                    TotalCount = groups.Count(),
                    Page = 1,
                    PageSize = groups.Count()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶 {UserId} 的群組列表時發生錯誤", userId);
                return StatusCode(500, new { error = "獲取群組列表失敗" });
            }
        }

        /// <summary>
        /// 獲取群組詳情
        /// </summary>
        [HttpGet("{groupId}")]
        public async Task<ActionResult<GroupDetailResponseDto>> GetGroup(int groupId, [FromQuery] int userId)
        {
            try
            {
                var group = await _groupService.GetGroupAsync(groupId);
                if (group == null)
                    return NotFound(new { error = "群組不存在" });

                var isUserMember = group.Members?.Any(m => m.user_id == userId) ?? false;
                var isUserAdmin = group.Members?.FirstOrDefault(m => m.user_id == userId)?.is_admin ?? false;
                var isUserBlocked = group.Blocks?.Any(b => b.user_id == userId) ?? false;

                var response = new GroupDetailResponseDto
                {
                    GroupId = group.group_id,
                    GroupName = group.group_name,
                    CreatedByName = group.CreatedByUser?.User_name ?? "未知",
                    CreatedAt = group.created_at,
                    MemberCount = group.Members?.Count ?? 0,
                    IsUserMember = isUserMember,
                    IsUserAdmin = isUserAdmin,
                    IsUserBlocked = isUserBlocked,
                    Members = group.Members?.Select(m => new GroupMemberResponseDto
                    {
                        UserId = m.user_id,
                        UserName = m.User?.User_name ?? "未知",
                        NickName = m.User?.NickName,
                        JoinedAt = m.joined_at,
                        IsAdmin = m.is_admin
                    }).ToList() ?? new List<GroupMemberResponseDto>(),
                    RecentMessages = group.ChatMessages?.Select(gc => new GroupChatResponseDto
                    {
                        GroupChatId = gc.group_chat_id,
                        SenderName = gc.Sender?.User_name ?? "未知",
                        Content = gc.group_chat_content,
                        SentAt = gc.sent_at,
                        IsSent = gc.is_sent
                    }).ToList() ?? new List<GroupChatResponseDto>(),
                    BlockedUsers = group.Blocks?.Select(gb => new GroupBlockResponseDto
                    {
                        BlockId = gb.block_id,
                        UserId = gb.user_id,
                        UserName = gb.BlockedUser?.User_name ?? "未知",
                        BlockedByName = gb.BlockedByUser?.User_name ?? "未知",
                        CreatedAt = gb.created_at
                    }).ToList() ?? new List<GroupBlockResponseDto>()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取群組 {GroupId} 詳情時發生錯誤", groupId);
                return StatusCode(500, new { error = "獲取群組詳情失敗" });
            }
        }

        /// <summary>
        /// 創建群組
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<GroupResponseDto>> CreateGroup([FromBody] CreateGroupRequestDto request)
        {
            try
            {
                var group = await _groupService.CreateGroupAsync(request.GroupName, request.CreatedBy);

                var response = new GroupResponseDto
                {
                    GroupId = group.group_id,
                    GroupName = group.group_name,
                    CreatedByName = "創建者",
                    CreatedAt = group.created_at,
                    MemberCount = 1,
                    IsUserMember = true,
                    IsUserAdmin = true,
                    IsUserBlocked = false
                };

                return CreatedAtAction(nameof(GetGroup), new { groupId = group.group_id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建群組時發生錯誤");
                return StatusCode(500, new { error = "創建群組失敗" });
            }
        }

        /// <summary>
        /// 加入群組
        /// </summary>
        [HttpPost("join")]
        public async Task<ActionResult> JoinGroup([FromBody] JoinGroupRequestDto request)
        {
            try
            {
                var success = await _groupService.JoinGroupAsync(request.GroupId, request.UserId);
                if (!success)
                    return BadRequest(new { error = "無法加入群組，可能已被封鎖或已是成員" });

                return Ok(new { message = "成功加入群組" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用戶 {UserId} 加入群組 {GroupId} 時發生錯誤", request.UserId, request.GroupId);
                return StatusCode(500, new { error = "加入群組失敗" });
            }
        }

        /// <summary>
        /// 離開群組
        /// </summary>
        [HttpPost("leave")]
        public async Task<ActionResult> LeaveGroup([FromBody] LeaveGroupRequestDto request)
        {
            try
            {
                var success = await _groupService.LeaveGroupAsync(request.GroupId, request.UserId);
                if (!success)
                    return BadRequest(new { error = "無法離開群組，可能不是成員" });

                return Ok(new { message = "成功離開群組" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用戶 {UserId} 離開群組 {GroupId} 時發生錯誤", request.UserId, request.GroupId);
                return StatusCode(500, new { error = "離開群組失敗" });
            }
        }

        /// <summary>
        /// 移除群組成員
        /// </summary>
        [HttpPost("remove-member")]
        public async Task<ActionResult> RemoveMember([FromBody] GroupManagementRequestDto request)
        {
            try
            {
                var success = await _groupService.RemoveMemberAsync(request.GroupId, request.TargetUserId, request.AdminUserId);
                if (!success)
                    return BadRequest(new { error = "無法移除成員，可能權限不足或成員不存在" });

                return Ok(new { message = "成功移除成員" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除群組 {GroupId} 成員 {TargetUserId} 時發生錯誤", request.GroupId, request.TargetUserId);
                return StatusCode(500, new { error = "移除成員失敗" });
            }
        }

        /// <summary>
        /// 設置成員為管理員
        /// </summary>
        [HttpPost("set-admin")]
        public async Task<ActionResult> SetMemberAsAdmin([FromBody] GroupManagementRequestDto request)
        {
            try
            {
                var success = await _groupService.SetMemberAsAdminAsync(request.GroupId, request.TargetUserId, request.AdminUserId);
                if (!success)
                    return BadRequest(new { error = "無法設置管理員，可能權限不足或成員不存在" });

                return Ok(new { message = "成功設置管理員" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "設置群組 {GroupId} 成員 {TargetUserId} 為管理員時發生錯誤", request.GroupId, request.TargetUserId);
                return StatusCode(500, new { error = "設置管理員失敗" });
            }
        }

        /// <summary>
        /// 移除管理員權限
        /// </summary>
        [HttpPost("remove-admin")]
        public async Task<ActionResult> RemoveAdmin([FromBody] GroupManagementRequestDto request)
        {
            try
            {
                var success = await _groupService.RemoveAdminAsync(request.GroupId, request.TargetUserId, request.AdminUserId);
                if (!success)
                    return BadRequest(new { error = "無法移除管理員權限，可能權限不足或成員不存在" });

                return Ok(new { message = "成功移除管理員權限" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除群組 {GroupId} 成員 {TargetUserId} 管理員權限時發生錯誤", request.GroupId, request.TargetUserId);
                return StatusCode(500, new { error = "移除管理員權限失敗" });
            }
        }

        /// <summary>
        /// 獲取群組成員列表
        /// </summary>
        [HttpGet("{groupId}/members")]
        public async Task<ActionResult<IEnumerable<GroupMemberResponseDto>>> GetGroupMembers(int groupId)
        {
            try
            {
                var members = await _groupService.GetGroupMembersAsync(groupId);

                var response = members.Select(m => new GroupMemberResponseDto
                {
                    UserId = m.user_id,
                    UserName = m.User?.User_name ?? "未知",
                    NickName = m.User?.NickName,
                    JoinedAt = m.joined_at,
                    IsAdmin = m.is_admin
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取群組 {GroupId} 成員列表時發生錯誤", groupId);
                return StatusCode(500, new { error = "獲取成員列表失敗" });
            }
        }

        /// <summary>
        /// 獲取群組訊息列表
        /// </summary>
        [HttpGet("{groupId}/messages")]
        public async Task<ActionResult<GroupMessageListResponseDto>> GetGroupMessages(
            int groupId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var messages = await _groupService.GetGroupMessagesAsync(groupId, page, pageSize);

                var response = new GroupMessageListResponseDto
                {
                    Messages = messages.Select(gc => new GroupChatResponseDto
                    {
                        GroupChatId = gc.group_chat_id,
                        SenderName = gc.Sender?.User_name ?? "未知",
                        Content = gc.group_chat_content,
                        SentAt = gc.sent_at,
                        IsSent = gc.is_sent
                    }).ToList(),
                    TotalCount = messages.Count(),
                    Page = page,
                    PageSize = pageSize
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取群組 {GroupId} 訊息列表時發生錯誤", groupId);
                return StatusCode(500, new { error = "獲取訊息列表失敗" });
            }
        }

        /// <summary>
        /// 發送群組訊息
        /// </summary>
        [HttpPost("{groupId}/messages")]
        public async Task<ActionResult<GroupChatResponseDto>> SendGroupMessage(
            int groupId,
            [FromBody] SendGroupMessageRequestDto request)
        {
            try
            {
                var message = await _groupService.SendGroupMessageAsync(groupId, request.SenderId, request.Content);

                var response = new GroupChatResponseDto
                {
                    GroupChatId = message.group_chat_id,
                    SenderName = "發送者",
                    Content = message.group_chat_content,
                    SentAt = message.sent_at,
                    IsSent = message.is_sent
                };

                return CreatedAtAction(nameof(GetGroupMessages), new { groupId }, response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送群組 {GroupId} 訊息時發生錯誤", groupId);
                return StatusCode(500, new { error = "發送群組訊息失敗" });
            }
        }

        /// <summary>
        /// 封鎖用戶
        /// </summary>
        [HttpPost("{groupId}/block")]
        public async Task<ActionResult> BlockUser(
            int groupId,
            [FromBody] GroupManagementRequestDto request)
        {
            try
            {
                var success = await _groupService.BlockUserAsync(groupId, request.TargetUserId, request.AdminUserId);
                if (!success)
                    return BadRequest(new { error = "無法封鎖用戶，可能權限不足或用戶已被封鎖" });

                return Ok(new { message = "成功封鎖用戶" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "封鎖群組 {GroupId} 用戶 {TargetUserId} 時發生錯誤", groupId, request.TargetUserId);
                return StatusCode(500, new { error = "封鎖用戶失敗" });
            }
        }

        /// <summary>
        /// 解除封鎖用戶
        /// </summary>
        [HttpPost("{groupId}/unblock")]
        public async Task<ActionResult> UnblockUser(
            int groupId,
            [FromBody] GroupManagementRequestDto request)
        {
            try
            {
                var success = await _groupService.UnblockUserAsync(groupId, request.TargetUserId, request.AdminUserId);
                if (!success)
                    return BadRequest(new { error = "無法解除封鎖，可能權限不足或用戶未被封鎖" });

                return Ok(new { message = "成功解除封鎖" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "解除封鎖群組 {GroupId} 用戶 {TargetUserId} 時發生錯誤", groupId, request.TargetUserId);
                return StatusCode(500, new { error = "解除封鎖失敗" });
            }
        }

        /// <summary>
        /// 獲取群組封鎖列表
        /// </summary>
        [HttpGet("{groupId}/blocks")]
        public async Task<ActionResult<IEnumerable<GroupBlockResponseDto>>> GetGroupBlocks(int groupId)
        {
            try
            {
                var blocks = await _groupService.GetGroupBlocksAsync(groupId);

                var response = blocks.Select(gb => new GroupBlockResponseDto
                {
                    BlockId = gb.block_id,
                    UserId = gb.user_id,
                    UserName = gb.BlockedUser?.User_name ?? "未知",
                    BlockedByName = gb.BlockedByUser?.User_name ?? "未知",
                    CreatedAt = gb.created_at
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取群組 {GroupId} 封鎖列表時發生錯誤", groupId);
                return StatusCode(500, new { error = "獲取封鎖列表失敗" });
            }
        }

        /// <summary>
        /// 刪除群組
        /// </summary>
        [HttpDelete("{groupId}")]
        public async Task<ActionResult> DeleteGroup(int groupId, [FromQuery] int adminUserId)
        {
            try
            {
                var success = await _groupService.DeleteGroupAsync(groupId, adminUserId);
                if (!success)
                    return BadRequest(new { error = "無法刪除群組，可能權限不足或群組不存在" });

                return Ok(new { message = "成功刪除群組" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除群組 {GroupId} 時發生錯誤", groupId);
                return StatusCode(500, new { error = "刪除群組失敗" });
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using System.Security.Claims;

namespace GameCore.Mvc.Controllers;

/// <summary>
/// 社交控制器 - 處理私訊、群組、封鎖功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SocialController : ControllerBase
{
    private readonly ISocialService _socialService;
    private readonly ILogger<SocialController> _logger;

    public SocialController(ISocialService socialService, ILogger<SocialController> logger)
    {
        _socialService = socialService;
        _logger = logger;
    }

    // ==================== 私訊功能 ====================

    /// <summary>
    /// 獲取與特定用戶的聊天記錄
    /// </summary>
    [HttpGet("chat")]
    public async Task<ActionResult<PagedChatMessagesDto>> GetChatMessages([FromQuery] int peerId, [FromQuery] ChatQueryDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.GetChatWithUserAsync(userId, peerId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取聊天記錄時發生錯誤: UserId={UserId}, PeerId={PeerId}", GetCurrentUserId(), peerId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 發送私訊
    /// </summary>
    [HttpPost("chat")]
    public async Task<ActionResult<ChatMessageDto>> SendDirectMessage([FromBody] SendDirectMessageRequestDto request)
    {
        try
        {
            var senderId = GetCurrentUserId();
            var result = await _socialService.SendDirectMessageAsync(senderId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發送私訊時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 標記私訊為已讀
    /// </summary>
    [HttpPost("chat/{messageId}/read")]
    public async Task<ActionResult> MarkDirectMessageAsRead(int messageId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.MarkDirectMessageAsReadAsync(userId, messageId);
            if (!result)
                return NotFound(new { message = "訊息不存在或無權限" });

            return Ok(new { message = "訊息已標記為已讀" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "標記私訊已讀時發生錯誤: MessageId={MessageId}", messageId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取聊天聯絡人列表
    /// </summary>
    [HttpGet("contacts")]
    public async Task<ActionResult<List<ChatContactDto>>> GetChatContacts()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.GetChatContactsAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取聊天聯絡人列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取未讀私訊數量
    /// </summary>
    [HttpGet("chat/unread-count")]
    public async Task<ActionResult<int>> GetUnreadDirectMessageCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            var count = await _socialService.GetUnreadDirectMessageCountAsync(userId);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取未讀私訊數量時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    // ==================== 群組管理 ====================

    /// <summary>
    /// 創建群組
    /// </summary>
    [HttpPost("groups")]
    public async Task<ActionResult<GroupDto>> CreateGroup([FromBody] CreateGroupRequestDto request)
    {
        try
        {
            var creatorId = GetCurrentUserId();
            var result = await _socialService.CreateGroupAsync(creatorId, request);
            return CreatedAtAction(nameof(GetGroupDetail), new { groupId = result.GroupId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "創建群組時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取群組詳情
    /// </summary>
    [HttpGet("groups/{groupId}")]
    public async Task<ActionResult<GroupDetailDto>> GetGroupDetail(int groupId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.GetGroupDetailAsync(userId, groupId);
            if (result == null)
                return NotFound(new { message = "群組不存在或無權限訪問" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取群組詳情時發生錯誤: GroupId={GroupId}", groupId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取用戶加入的群組列表
    /// </summary>
    [HttpGet("groups")]
    public async Task<ActionResult<PagedGroupsDto>> GetUserGroups([FromQuery] GroupQueryDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.GetUserGroupsAsync(userId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取用戶群組列表時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 加入群組
    /// </summary>
    [HttpPost("groups/{groupId}/join")]
    public async Task<ActionResult> JoinGroup(int groupId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.JoinGroupAsync(userId, groupId);
            if (!result)
                return BadRequest(new { message = "無法加入群組" });

            return Ok(new { message = "成功加入群組" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加入群組時發生錯誤: GroupId={GroupId}", groupId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 離開群組
    /// </summary>
    [HttpPost("groups/{groupId}/leave")]
    public async Task<ActionResult> LeaveGroup(int groupId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.LeaveGroupAsync(userId, groupId);
            if (!result)
                return BadRequest(new { message = "無法離開群組" });

            return Ok(new { message = "成功離開群組" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "離開群組時發生錯誤: GroupId={GroupId}", groupId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取群組成員列表
    /// </summary>
    [HttpGet("groups/{groupId}/members")]
    public async Task<ActionResult<PagedGroupMembersDto>> GetGroupMembers(int groupId, [FromQuery] GroupMemberQueryDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.GetGroupMembersAsync(userId, groupId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取群組成員列表時發生錯誤: GroupId={GroupId}", groupId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    // ==================== 群組聊天 ====================

    /// <summary>
    /// 獲取群組聊天記錄
    /// </summary>
    [HttpGet("groups/{groupId}/messages")]
    public async Task<ActionResult<PagedGroupChatMessagesDto>> GetGroupMessages(int groupId, [FromQuery] GroupChatQueryDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.GetGroupChatMessagesAsync(userId, groupId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取群組聊天記錄時發生錯誤: GroupId={GroupId}", groupId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 發送群組訊息
    /// </summary>
    [HttpPost("groups/{groupId}/messages")]
    public async Task<ActionResult<GroupChatMessageDto>> SendGroupMessage(int groupId, [FromBody] SendGroupMessageRequestDto request)
    {
        try
        {
            var senderId = GetCurrentUserId();
            var result = await _socialService.SendGroupMessageAsync(senderId, groupId, request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發送群組訊息時發生錯誤: GroupId={GroupId}", groupId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    // ==================== 群組封鎖 ====================

    /// <summary>
    /// 在群組中封鎖用戶
    /// </summary>
    [HttpPost("groups/{groupId}/block/{blockedUserId}")]
    public async Task<ActionResult> BlockUserInGroup(int groupId, int blockedUserId, [FromBody] BlockUserRequestDto? request = null)
    {
        try
        {
            var blockerId = GetCurrentUserId();
            var result = await _socialService.BlockUserInGroupAsync(blockerId, groupId, blockedUserId, request?.Reason);
            if (!result)
                return BadRequest(new { message = "無法封鎖此用戶" });

            return Ok(new { message = "用戶已被封鎖" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "封鎖群組用戶時發生錯誤: GroupId={GroupId}, BlockedUserId={BlockedUserId}", groupId, blockedUserId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 在群組中解除封鎖用戶
    /// </summary>
    [HttpDelete("groups/{groupId}/block/{blockedUserId}")]
    public async Task<ActionResult> UnblockUserInGroup(int groupId, int blockedUserId)
    {
        try
        {
            var unblockerId = GetCurrentUserId();
            var result = await _socialService.UnblockUserInGroupAsync(unblockerId, groupId, blockedUserId);
            if (!result)
                return NotFound(new { message = "封鎖記錄不存在" });

            return Ok(new { message = "已解除封鎖" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解除群組用戶封鎖時發生錯誤: GroupId={GroupId}, BlockedUserId={BlockedUserId}", groupId, blockedUserId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取群組的封鎖用戶列表
    /// </summary>
    [HttpGet("groups/{groupId}/blocks")]
    public async Task<ActionResult<PagedGroupBlocksDto>> GetGroupBlocks(int groupId, [FromQuery] GroupBlockQueryDto query)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.GetGroupBlocksAsync(userId, groupId, query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取群組封鎖列表時發生錯誤: GroupId={GroupId}", groupId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 檢查用戶是否在群組中被封鎖
    /// </summary>
    [HttpGet("groups/{groupId}/blocked")]
    public async Task<ActionResult<bool>> IsUserBlockedInGroup(int groupId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.IsUserBlockedInGroupAsync(userId, groupId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查用戶封鎖狀態時發生錯誤: GroupId={GroupId}", groupId);
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    // ==================== 搜尋與發現 ====================

    /// <summary>
    /// 搜尋群組
    /// </summary>
    [HttpGet("groups/search")]
    public async Task<ActionResult<PagedGroupsDto>> SearchGroups([FromQuery] GroupSearchQueryDto query)
    {
        try
        {
            var result = await _socialService.SearchGroupsAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋群組時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取推薦群組
    /// </summary>
    [HttpGet("groups/recommended")]
    public async Task<ActionResult<List<GroupDto>>> GetRecommendedGroups([FromQuery] int limit = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _socialService.GetRecommendedGroupsAsync(userId, limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取推薦群組時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    /// <summary>
    /// 獲取熱門群組
    /// </summary>
    [HttpGet("groups/popular")]
    [AllowAnonymous]
    public async Task<ActionResult<List<GroupDto>>> GetPopularGroups([FromQuery] int limit = 10)
    {
        try
        {
            var result = await _socialService.GetPopularGroupsAsync(limit);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "獲取熱門群組時發生錯誤");
            return StatusCode(500, new { message = "系統錯誤" });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("無效的用戶身份");
        }
        return userId;
    }
}

/// <summary>
/// 封鎖用戶請求 DTO
/// </summary>
public class BlockUserRequestDto
{
    public string? Reason { get; set; }
}
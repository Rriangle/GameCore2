using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Api.Services;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

/// <summary>
/// 群組控制器
/// 處理群組相關的 API 端點
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly ILogger<GroupController> _logger;

    public GroupController(
        IGroupService groupService,
        ILogger<GroupController> logger)
    {
        _groupService = groupService;
        _logger = logger;
    }

    /// <summary>
    /// 建立群組
    /// </summary>
    /// <param name="request">建立群組請求</param>
    /// <returns>群組ID</returns>
    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "請求資料無效", errors = ModelState });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _groupService.CreateGroupAsync(request);
            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message });
            }
            var groupId = result.Data?.GroupId ?? 0;

            return Ok(new
            {
                success = true,
                message = "建立群組成功",
                data = new { groupId }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立群組時發生錯誤");
            return StatusCode(500, new { success = false, message = "建立群組時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得群組詳情
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>群組詳情</returns>
    [HttpGet("{groupId}")]
    [ResponseCache(Duration = 300)] // 效能優化：5分鐘快取，群組詳情變動頻率中等
    public async Task<IActionResult> GetGroup(int groupId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _groupService.GetGroupAsync(groupId);
            if (!result.Success)
            {
                return NotFound(new { success = false, message = result.Message });
            }
            var group = result.Data;

            return Ok(new
            {
                success = true,
                message = "取得群組詳情成功",
                data = group
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得群組詳情時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得群組詳情時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得用戶加入的群組列表
    /// </summary>
    /// <returns>群組列表</returns>
    [HttpGet("my-groups")]
    public async Task<IActionResult> GetMyGroups()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _groupService.GetUserGroupsAsync(userId.Value);

            return Ok(new
            {
                success = true,
                message = "取得群組列表成功",
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得群組列表時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得群組列表時發生錯誤" });
        }
    }

    /// <summary>
    /// 加入群組
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>操作結果</returns>
    [HttpPost("{groupId}/join")]
    public async Task<IActionResult> JoinGroup(int groupId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _groupService.JoinGroupAsync(groupId, userId.Value);

            if (result.Success)
            {
                return Ok(new { success = true, message = "加入群組成功" });
            }
            else
            {
                return BadRequest(new { success = false, message = result.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加入群組時發生錯誤");
            return StatusCode(500, new { success = false, message = "加入群組時發生錯誤" });
        }
    }

    /// <summary>
    /// 退出群組
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>操作結果</returns>
    [HttpPost("{groupId}/leave")]
    public async Task<IActionResult> LeaveGroup(int groupId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _groupService.LeaveGroupAsync(groupId, userId.Value);

            if (result.Success)
            {
                return Ok(new { success = true, message = "退出群組成功" });
            }
            else
            {
                return BadRequest(new { success = false, message = result.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "退出群組時發生錯誤");
            return StatusCode(500, new { success = false, message = "退出群組時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得群組成員列表
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>成員列表</returns>
    [HttpGet("{groupId}/members")]
    public async Task<IActionResult> GetGroupMembers(int groupId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _groupService.GetGroupMembersAsync(groupId);

            return Ok(new
            {
                success = true,
                message = "取得群組成員列表成功",
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得群組成員列表時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得群組成員列表時發生錯誤" });
        }
    }

    /// <summary>
    /// 發送群組訊息
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="request">發送訊息請求</param>
    /// <returns>操作結果</returns>
    [HttpPost("{groupId}/messages")]
    public async Task<IActionResult> SendGroupMessage(int groupId, [FromBody] SendGroupMessageRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "請求資料無效", errors = ModelState });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            request.GroupId = groupId;
            request.SenderId = userId.Value;
            var result = await _groupService.SendGroupMessageAsync(request);

            if (result.Success)
            {
                return Ok(new { success = true, message = "發送群組訊息成功", data = result.Data });
            }
            else
            {
                return BadRequest(new { success = false, message = result.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發送群組訊息時發生錯誤");
            return StatusCode(500, new { success = false, message = "發送群組訊息時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得群組聊天記錄
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="page">頁碼（預設：1）</param>
    /// <param name="pageSize">頁面大小（預設：50）</param>
    /// <returns>聊天記錄</returns>
    [HttpGet("{groupId}/messages")]
    public async Task<IActionResult> GetGroupChatHistory(
        int groupId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _groupService.GetGroupChatHistoryAsync(groupId, page, pageSize);

            return Ok(new
            {
                success = true,
                message = "取得群組聊天記錄成功",
                data = new
                {
                    messages = result.Data ?? new List<GroupChatDTO>(),
                    pagination = new
                    {
                        page,
                        pageSize,
                        total = result.Data?.Count ?? 0
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得群組聊天記錄時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得群組聊天記錄時發生錯誤" });
        }
    }

    /// <summary>
    /// 封鎖用戶（管理員功能）
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="request">封鎖請求</param>
    /// <returns>操作結果</returns>
    [HttpPost("{groupId}/block")]
    public async Task<IActionResult> BlockUser(int groupId, [FromBody] BlockUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "請求資料無效", errors = ModelState });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _groupService.BlockUserAsync(groupId, request.UserId, userId.Value);

            if (result.Success)
            {
                return Ok(new { success = true, message = "封鎖用戶成功" });
            }
            else
            {
                return BadRequest(new { success = false, message = result.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "封鎖用戶時發生錯誤");
            return StatusCode(500, new { success = false, message = "封鎖用戶時發生錯誤" });
        }
    }

    /// <summary>
    /// 解除封鎖用戶（管理員功能）
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="userId">被解除封鎖的用戶ID</param>
    /// <returns>操作結果</returns>
    [HttpDelete("{groupId}/block/{userId}")]
    public async Task<IActionResult> UnblockUser(int groupId, int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _groupService.UnblockUserAsync(groupId, userId);

            if (result.Success)
            {
                return Ok(new { success = true, message = "解除封鎖用戶成功" });
            }
            else
            {
                return BadRequest(new { success = false, message = result.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解除封鎖用戶時發生錯誤");
            return StatusCode(500, new { success = false, message = "解除封鎖用戶時發生錯誤" });
        }
    }

    /// <summary>
    /// 搜尋群組
    /// </summary>
    /// <param name="keyword">關鍵字</param>
    /// <param name="page">頁碼（預設：1）</param>
    /// <param name="pageSize">頁面大小（預設：20）</param>
    /// <returns>搜尋結果</returns>
    [HttpGet("search")]
    public async Task<IActionResult> SearchGroups(
        [FromQuery] string keyword,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { success = false, message = "關鍵字不能為空" });
            }

            var result = await _groupService.SearchGroupsAsync(keyword, page, pageSize);

            return Ok(new
            {
                success = true,
                message = "搜尋群組成功",
                data = new
                {
                    groups = result.Data ?? new List<GroupDTO>(),
                    keyword,
                    pagination = new
                    {
                        page,
                        pageSize,
                        total = result.Data?.Count ?? 0
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋群組時發生錯誤");
            return StatusCode(500, new { success = false, message = "搜尋群組時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得群組統計
    /// </summary>
    /// <returns>群組統計</returns>
    [HttpGet("stats")]
    public async Task<IActionResult> GetGroupStats()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _groupService.GetGroupStatsAsync(userId.Value);

            return Ok(new
            {
                success = true,
                message = "取得群組統計成功",
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得群組統計時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得群組統計時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得當前用戶ID
    /// </summary>
    /// <returns>用戶ID</returns>
    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }
} 
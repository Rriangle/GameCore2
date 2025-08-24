using GameCore.Api.DTOs;
using GameCore.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

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
    /// 獲取群組列表
    /// </summary>
    /// <returns>群組列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GroupDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetGroups()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取群組列表請求: {CorrelationId}", correlationId);

        var groups = await _groupService.GetGroupsAsync();

        _logger.LogInformation("成功獲取群組列表: {CorrelationId}, Count: {Count}", correlationId, groups.Count());
        return Ok(ApiResponse<IEnumerable<GroupDto>>.SuccessResult(groups));
    }

    /// <summary>
    /// 獲取群組詳情
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>群組詳情</returns>
    [HttpGet("{groupId}")]
    [ProducesResponseType(typeof(ApiResponse<GroupDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetGroup(int groupId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取群組詳情請求: {CorrelationId}, GroupId: {GroupId}", correlationId, groupId);

        var group = await _groupService.GetGroupAsync(groupId);
        if (group == null)
        {
            _logger.LogWarning("群組不存在: {CorrelationId}, GroupId: {GroupId}", correlationId, groupId);
            return NotFound(ApiResponse<object>.ErrorResult("群組不存在"));
        }

        _logger.LogInformation("成功獲取群組詳情: {CorrelationId}, GroupId: {GroupId}", correlationId, groupId);
        return Ok(ApiResponse<GroupDto>.SuccessResult(group));
    }

    /// <summary>
    /// 創建群組
    /// </summary>
    /// <param name="request">群組創建請求</param>
    /// <returns>創建結果</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<GroupResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到創建群組請求: {CorrelationId}, Name: {Name}", correlationId, request.Name);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("創建群組請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _groupService.CreateGroupAsync(userId, request.Name, request.Description, request.AvatarUrl);

        if (result.Success)
        {
            _logger.LogInformation("群組創建成功: {CorrelationId}, UserId: {UserId}, GroupId: {GroupId}", 
                correlationId, userId, result.Group?.GroupId);
            return Ok(ApiResponse<GroupResult>.SuccessResult(result));
        }

        _logger.LogWarning("群組創建失敗: {CorrelationId}, UserId: {UserId}, Message: {Message}", 
            correlationId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "群組創建失敗"));
    }

    /// <summary>
    /// 更新群組
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="request">更新請求</param>
    /// <returns>更新結果</returns>
    [HttpPut("{groupId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<GroupResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] UpdateGroupRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到更新群組請求: {CorrelationId}, GroupId: {GroupId}", correlationId, groupId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("更新群組請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _groupService.UpdateGroupAsync(groupId, userId, request.Name, request.Description, request.AvatarUrl);

        if (result.Success)
        {
            _logger.LogInformation("群組更新成功: {CorrelationId}, GroupId: {GroupId}, UserId: {UserId}", 
                correlationId, groupId, userId);
            return Ok(ApiResponse<GroupResult>.SuccessResult(result));
        }

        _logger.LogWarning("群組更新失敗: {CorrelationId}, GroupId: {GroupId}, UserId: {UserId}, Message: {Message}", 
            correlationId, groupId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "群組更新失敗"));
    }

    /// <summary>
    /// 刪除群組
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>刪除結果</returns>
    [HttpDelete("{groupId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<GroupResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> DeleteGroup(int groupId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到刪除群組請求: {CorrelationId}, GroupId: {GroupId}", correlationId, groupId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _groupService.DeleteGroupAsync(groupId, userId);

        if (result.Success)
        {
            _logger.LogInformation("群組刪除成功: {CorrelationId}, GroupId: {GroupId}, UserId: {UserId}", 
                correlationId, groupId, userId);
            return Ok(ApiResponse<GroupResult>.SuccessResult(result));
        }

        _logger.LogWarning("群組刪除失敗: {CorrelationId}, GroupId: {GroupId}, UserId: {UserId}, Message: {Message}", 
            correlationId, groupId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "群組刪除失敗"));
    }

    /// <summary>
    /// 獲取群組成員
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <returns>成員列表</returns>
    [HttpGet("{groupId}/members")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GroupMemberDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetGroupMembers(int groupId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取群組成員請求: {CorrelationId}, GroupId: {GroupId}", correlationId, groupId);

        var members = await _groupService.GetGroupMembersAsync(groupId);

        _logger.LogInformation("成功獲取群組成員: {CorrelationId}, GroupId: {GroupId}, Count: {Count}", 
            correlationId, groupId, members.Count());
        return Ok(ApiResponse<IEnumerable<GroupMemberDto>>.SuccessResult(members));
    }

    /// <summary>
    /// 添加群組成員
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="request">添加成員請求</param>
    /// <returns>添加結果</returns>
    [HttpPost("{groupId}/members")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<GroupMemberResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> AddMember(int groupId, [FromBody] AddMemberRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到添加群組成員請求: {CorrelationId}, GroupId: {GroupId}, MemberId: {MemberId}", 
            correlationId, groupId, request.MemberId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("添加群組成員請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _groupService.AddMemberAsync(groupId, userId, request.MemberId, request.Role);

        if (result.Success)
        {
            _logger.LogInformation("群組成員添加成功: {CorrelationId}, GroupId: {GroupId}, MemberId: {MemberId}", 
                correlationId, groupId, request.MemberId);
            return Ok(ApiResponse<GroupMemberResult>.SuccessResult(result));
        }

        _logger.LogWarning("群組成員添加失敗: {CorrelationId}, GroupId: {GroupId}, MemberId: {MemberId}, Message: {Message}", 
            correlationId, groupId, request.MemberId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "成員添加失敗"));
    }

    /// <summary>
    /// 移除群組成員
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="memberId">成員ID</param>
    /// <returns>移除結果</returns>
    [HttpDelete("{groupId}/members/{memberId}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<GroupMemberResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> RemoveMember(int groupId, int memberId)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到移除群組成員請求: {CorrelationId}, GroupId: {GroupId}, MemberId: {MemberId}", 
            correlationId, groupId, memberId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _groupService.RemoveMemberAsync(groupId, userId, memberId);

        if (result.Success)
        {
            _logger.LogInformation("群組成員移除成功: {CorrelationId}, GroupId: {GroupId}, MemberId: {MemberId}", 
                correlationId, groupId, memberId);
            return Ok(ApiResponse<GroupMemberResult>.SuccessResult(result));
        }

        _logger.LogWarning("群組成員移除失敗: {CorrelationId}, GroupId: {GroupId}, MemberId: {MemberId}, Message: {Message}", 
            correlationId, groupId, memberId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "成員移除失敗"));
    }

    /// <summary>
    /// 發送群組訊息
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="request">發送訊息請求</param>
    /// <returns>發送結果</returns>
    [HttpPost("{groupId}/messages")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<GroupConversationResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> SendMessage(int groupId, [FromBody] SendMessageRequestDto request)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到發送群組訊息請求: {CorrelationId}, GroupId: {GroupId}", correlationId, groupId);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("發送群組訊息請求驗證失敗: {CorrelationId}, Errors: {Errors}", correlationId, string.Join(", ", errors));
            return BadRequest(ApiResponse<object>.ErrorResult("請求資料無效", errors));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var result = await _groupService.SendMessageAsync(groupId, userId, request.Message, request.MessageType);

        if (result.Success)
        {
            _logger.LogInformation("群組訊息發送成功: {CorrelationId}, GroupId: {GroupId}, UserId: {UserId}", 
                correlationId, groupId, userId);
            return Ok(ApiResponse<GroupConversationResult>.SuccessResult(result));
        }

        _logger.LogWarning("群組訊息發送失敗: {CorrelationId}, GroupId: {GroupId}, UserId: {UserId}, Message: {Message}", 
            correlationId, groupId, userId, result.Message);
        return BadRequest(ApiResponse<object>.ErrorResult(result.Message ?? "訊息發送失敗"));
    }

    /// <summary>
    /// 獲取群組訊息
    /// </summary>
    /// <param name="groupId">群組ID</param>
    /// <param name="limit">訊息數量限制</param>
    /// <returns>訊息列表</returns>
    [HttpGet("{groupId}/messages")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GroupConversationDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetGroupMessages(int groupId, [FromQuery] int? limit)
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取群組訊息請求: {CorrelationId}, GroupId: {GroupId}, Limit: {Limit}", 
            correlationId, groupId, limit);

        var messages = await _groupService.GetGroupMessagesAsync(groupId, limit);

        _logger.LogInformation("成功獲取群組訊息: {CorrelationId}, GroupId: {GroupId}, Count: {Count}", 
            correlationId, groupId, messages.Count());
        return Ok(ApiResponse<IEnumerable<GroupConversationDto>>.SuccessResult(messages));
    }

    /// <summary>
    /// 獲取用戶的群組
    /// </summary>
    /// <returns>用戶群組列表</returns>
    [HttpGet("user")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GroupDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public async Task<IActionResult> GetUserGroups()
    {
        var correlationId = HttpContext.GetCorrelationId();
        _logger.LogInformation("收到獲取用戶群組請求: {CorrelationId}", correlationId);

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("無效的認證資訊: {CorrelationId}", correlationId);
            return Unauthorized(ApiResponse<object>.ErrorResult("無效的認證資訊"));
        }

        var groups = await _groupService.GetUserGroupsAsync(userId);

        _logger.LogInformation("成功獲取用戶群組: {CorrelationId}, UserId: {UserId}, Count: {Count}", 
            correlationId, userId, groups.Count());
        return Ok(ApiResponse<IEnumerable<GroupDto>>.SuccessResult(groups));
    }
}

public class CreateGroupRequestDto
{
    [Required(ErrorMessage = "群組名稱為必填項目")]
    [StringLength(100, ErrorMessage = "群組名稱長度不能超過 100 個字元")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "群組描述為必填項目")]
    [StringLength(500, ErrorMessage = "群組描述長度不能超過 500 個字元")]
    public string Description { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "頭像URL長度不能超過 200 個字元")]
    public string? AvatarUrl { get; set; }
}

public class UpdateGroupRequestDto
{
    [Required(ErrorMessage = "群組名稱為必填項目")]
    [StringLength(100, ErrorMessage = "群組名稱長度不能超過 100 個字元")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "群組描述為必填項目")]
    [StringLength(500, ErrorMessage = "群組描述長度不能超過 500 個字元")]
    public string Description { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "頭像URL長度不能超過 200 個字元")]
    public string? AvatarUrl { get; set; }
}

public class AddMemberRequestDto
{
    [Required(ErrorMessage = "成員ID為必填項目")]
    public int MemberId { get; set; }

    [Required(ErrorMessage = "成員角色為必填項目")]
    [StringLength(20, ErrorMessage = "成員角色長度不能超過 20 個字元")]
    public string Role { get; set; } = string.Empty;
}

public class SendMessageRequestDto
{
    [Required(ErrorMessage = "訊息內容為必填項目")]
    public string Message { get; set; } = string.Empty;

    [Required(ErrorMessage = "訊息類型為必填項目")]
    [StringLength(20, ErrorMessage = "訊息類型長度不能超過 20 個字元")]
    public string MessageType { get; set; } = "text";
}
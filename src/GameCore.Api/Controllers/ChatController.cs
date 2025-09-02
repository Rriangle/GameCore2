using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameCore.Api.Services;
using GameCore.Shared.DTOs;
using System.Security.Claims;

namespace GameCore.Api.Controllers;

/// <summary>
/// 聊天控制器
/// 處理私訊相關的 API 端點
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IChatService chatService,
        ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    /// <summary>
    /// 取得與特定用戶的聊天記錄
    /// </summary>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <param name="page">頁碼（預設：1）</param>
    /// <param name="pageSize">頁面大小（預設：50）</param>
    /// <returns>聊天記錄</returns>
    [HttpGet("history/{otherUserId}")]
    public async Task<IActionResult> GetChatHistory(
        int otherUserId,
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

            var messages = await _chatService.GetChatHistoryAsync(userId.Value, otherUserId, page, pageSize);

            return Ok(new
            {
                success = true,
                message = "取得聊天記錄成功",
                data = new
                {
                    messages,
                    pagination = new
                    {
                        page,
                        pageSize,
                        total = messages.Count()
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得聊天記錄時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得聊天記錄時發生錯誤" });
        }
    }

    /// <summary>
    /// 發送聊天訊息
    /// </summary>
    /// <param name="request">發送訊息請求</param>
    /// <returns>操作結果</returns>
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendChatMessageRequest request)
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

            var result = await _chatService.SendMessageAsync(userId.Value, request);

            if (result)
            {
                return Ok(new { success = true, message = "發送訊息成功" });
            }
            else
            {
                return BadRequest(new { success = false, message = "發送訊息失敗" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "發送聊天訊息時發生錯誤");
            return StatusCode(500, new { success = false, message = "發送訊息時發生錯誤" });
        }
    }

    /// <summary>
    /// 標記訊息為已讀
    /// </summary>
    /// <param name="messageId">訊息ID</param>
    /// <returns>操作結果</returns>
    [HttpPost("messages/{messageId}/read")]
    public async Task<IActionResult> MarkAsRead(int messageId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var result = await _chatService.MarkAsReadAsync(messageId, userId.Value);

            if (result)
            {
                return Ok(new { success = true, message = "標記已讀成功" });
            }
            else
            {
                return BadRequest(new { success = false, message = "標記已讀失敗" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "標記訊息為已讀時發生錯誤");
            return StatusCode(500, new { success = false, message = "標記已讀時發生錯誤" });
        }
    }

    /// <summary>
    /// 標記與特定用戶的所有訊息為已讀
    /// </summary>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <returns>操作結果</returns>
    [HttpPost("read-all/{otherUserId}")]
    public async Task<IActionResult> MarkAllAsRead(int otherUserId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var count = await _chatService.MarkAllAsReadAsync(userId.Value, otherUserId);

            return Ok(new
            {
                success = true,
                message = $"標記 {count} 個訊息為已讀成功",
                data = new { count }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "標記所有訊息為已讀時發生錯誤");
            return StatusCode(500, new { success = false, message = "標記所有訊息為已讀時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得未讀訊息數量
    /// </summary>
    /// <returns>未讀數量</returns>
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var count = await _chatService.GetUnreadCountAsync(userId.Value);

            return Ok(new
            {
                success = true,
                message = "取得未讀訊息數量成功",
                data = new { unreadCount = count }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得未讀訊息數量時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得未讀訊息數量時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得聊天對象列表
    /// </summary>
    /// <returns>聊天對象列表</returns>
    [HttpGet("partners")]
    public async Task<IActionResult> GetChatPartners()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var partners = await _chatService.GetChatPartnersAsync(userId.Value);

            return Ok(new
            {
                success = true,
                message = "取得聊天對象列表成功",
                data = partners
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得聊天對象列表時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得聊天對象列表時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得與特定用戶的最新訊息
    /// </summary>
    /// <param name="otherUserId">對方用戶ID</param>
    /// <returns>最新訊息</returns>
    [HttpGet("latest/{otherUserId}")]
    public async Task<IActionResult> GetLatestMessage(int otherUserId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var message = await _chatService.GetLatestMessageAsync(userId.Value, otherUserId);

            return Ok(new
            {
                success = true,
                message = "取得最新訊息成功",
                data = message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得最新訊息時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得最新訊息時發生錯誤" });
        }
    }

    /// <summary>
    /// 搜尋聊天訊息
    /// </summary>
    /// <param name="keyword">關鍵字</param>
    /// <param name="page">頁碼（預設：1）</param>
    /// <param name="pageSize">頁面大小（預設：20）</param>
    /// <returns>搜尋結果</returns>
    [HttpGet("search")]
    public async Task<IActionResult> SearchMessages(
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

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var messages = await _chatService.SearchMessagesAsync(userId.Value, keyword, page, pageSize);

            return Ok(new
            {
                success = true,
                message = "搜尋訊息成功",
                data = new
                {
                    messages,
                    keyword,
                    pagination = new
                    {
                        page,
                        pageSize,
                        total = messages.Count()
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋聊天訊息時發生錯誤");
            return StatusCode(500, new { success = false, message = "搜尋訊息時發生錯誤" });
        }
    }

    /// <summary>
    /// 取得聊天統計
    /// </summary>
    /// <returns>聊天統計</returns>
    [HttpGet("stats")]
    public async Task<IActionResult> GetChatStats()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "未授權的存取" });
            }

            var stats = await _chatService.GetChatStatsAsync(userId.Value);

            return Ok(new
            {
                success = true,
                message = "取得聊天統計成功",
                data = stats
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得聊天統計時發生錯誤");
            return StatusCode(500, new { success = false, message = "取得聊天統計時發生錯誤" });
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
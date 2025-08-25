using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Controllers
{
    /// <summary>
    /// 聊天控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// 獲取私訊對話
        /// </summary>
        [HttpGet("direct")]
        public async Task<ActionResult<ChatConversationResponseDto>> GetDirectMessages(
            [FromQuery] int userId,
            [FromQuery] int peerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var messages = await _chatService.GetDirectMessagesAsync(userId, peerId, page, pageSize);
                var unreadCount = await _chatService.GetUnreadCountAsync(userId);

                var response = new ChatConversationResponseDto
                {
                    Messages = messages.Select(m => new ChatMessageResponseDto
                    {
                        MessageId = m.message_id,
                        SenderName = m.Sender?.User_name ?? "未知",
                        ManagerName = m.Manager?.Manager_Name,
                        ReceiverId = m.receiver_id ?? 0,
                        Content = m.chat_content,
                        SentAt = m.sent_at,
                        IsRead = m.is_read,
                        IsSent = m.is_sent,
                        IsSystemMessage = m.manager_id.HasValue
                    }).ToList(),
                    TotalCount = messages.Count(),
                    Page = page,
                    PageSize = pageSize,
                    UnreadCount = unreadCount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶 {UserId} 與 {PeerId} 的私訊時發生錯誤", userId, peerId);
                return StatusCode(500, new { error = "獲取私訊失敗" });
            }
        }

        /// <summary>
        /// 發送私訊
        /// </summary>
        [HttpPost("direct")]
        public async Task<ActionResult<ChatMessageResponseDto>> SendDirectMessage([FromBody] SendMessageRequestDto request, [FromQuery] int senderId)
        {
            try
            {
                var message = await _chatService.SendDirectMessageAsync(senderId, request.ReceiverId, request.Content);

                var response = new ChatMessageResponseDto
                {
                    MessageId = message.message_id,
                    SenderName = "發送者",
                    ReceiverId = message.receiver_id ?? 0,
                    Content = message.chat_content,
                    SentAt = message.sent_at,
                    IsRead = message.is_read,
                    IsSent = message.is_sent,
                    IsSystemMessage = false
                };

                return CreatedAtAction(nameof(GetDirectMessages), new { userId = senderId, peerId = request.ReceiverId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送私訊時發生錯誤");
                return StatusCode(500, new { error = "發送私訊失敗" });
            }
        }

        /// <summary>
        /// 發送系統訊息
        /// </summary>
        [HttpPost("system")]
        public async Task<ActionResult<ChatMessageResponseDto>> SendSystemMessage([FromBody] SystemMessageRequestDto request)
        {
            try
            {
                var message = await _chatService.SendSystemMessageAsync(request.ReceiverId, request.Content, request.ManagerId);

                var response = new ChatMessageResponseDto
                {
                    MessageId = message.message_id,
                    SenderName = "系統",
                    ManagerName = request.ManagerId.HasValue ? "管理員" : null,
                    ReceiverId = message.receiver_id ?? 0,
                    Content = message.chat_content,
                    SentAt = message.sent_at,
                    IsRead = message.is_read,
                    IsSent = message.is_sent,
                    IsSystemMessage = true
                };

                return CreatedAtAction(nameof(GetDirectMessages), new { userId = 1, peerId = request.ReceiverId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送系統訊息時發生錯誤");
                return StatusCode(500, new { error = "發送系統訊息失敗" });
            }
        }

        /// <summary>
        /// 標記訊息為已讀
        /// </summary>
        [HttpPost("{messageId}/read")]
        public async Task<ActionResult> MarkAsRead(int messageId, [FromBody] MarkMessageReadRequestDto request)
        {
            try
            {
                if (request.MessageId != messageId)
                    return BadRequest(new { error = "訊息ID不匹配" });

                var success = await _chatService.MarkMessageAsReadAsync(messageId, request.UserId);
                if (!success)
                    return NotFound(new { error = "訊息或用戶不存在" });

                return Ok(new { message = "訊息已標記為已讀" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "標記訊息 {MessageId} 為已讀時發生錯誤", messageId);
                return StatusCode(500, new { error = "標記已讀失敗" });
            }
        }

        /// <summary>
        /// 獲取未讀訊息數量
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount([FromQuery] int userId)
        {
            try
            {
                var count = await _chatService.GetUnreadCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶 {UserId} 未讀訊息數量時發生錯誤", userId);
                return StatusCode(500, new { error = "獲取未讀數量失敗" });
            }
        }

        /// <summary>
        /// 獲取最近對話
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<RecentConversationResponseDto>>> GetRecentConversations(
            [FromQuery] int userId,
            [FromQuery] int limit = 10)
        {
            try
            {
                var conversations = await _chatService.GetRecentConversationsAsync(userId, limit);

                var response = conversations.Select(m => new RecentConversationResponseDto
                {
                    PeerId = m.sender_id == userId ? (m.receiver_id ?? 0) : m.sender_id,
                    PeerName = m.Sender?.User_name ?? "未知",
                    LastMessage = m.chat_content,
                    LastMessageTime = m.sent_at,
                    UnreadCount = 0, // 這裡需要額外計算
                    IsSystemMessage = m.manager_id.HasValue
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取用戶 {UserId} 最近對話時發生錯誤", userId);
                return StatusCode(500, new { error = "獲取最近對話失敗" });
            }
        }

        /// <summary>
        /// 刪除訊息
        /// </summary>
        [HttpDelete("{messageId}")]
        public async Task<ActionResult> DeleteMessage(int messageId, [FromQuery] int userId)
        {
            try
            {
                var success = await _chatService.DeleteMessageAsync(messageId, userId);
                if (!success)
                    return NotFound(new { error = "訊息或用戶不存在" });

                return Ok(new { message = "訊息已刪除" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除訊息 {MessageId} 時發生錯誤", messageId);
                return StatusCode(500, new { error = "刪除訊息失敗" });
            }
        }
    }
}
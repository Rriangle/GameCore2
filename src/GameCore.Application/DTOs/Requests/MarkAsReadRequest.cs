namespace GameCore.Application.DTOs.Requests;

/// <summary>
/// 標記已讀請求 DTO
/// </summary>
public class MarkAsReadRequest
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// 訊息 ID 列表
    /// </summary>
    public List<int> MessageIds { get; set; } = new();
    
    /// <summary>
    /// 聊天類型 (private/group)
    /// </summary>
    public string ChatType { get; set; } = string.Empty;
    
    /// <summary>
    /// 聊天對象 ID (使用者 ID 或群組 ID)
    /// </summary>
    public int ChatTargetId { get; set; }
} 
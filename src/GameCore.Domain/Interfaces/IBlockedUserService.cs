namespace GameCore.Domain.Interfaces;

public interface IBlockedUserService
{
    Task<IEnumerable<BlockedUserDto>> GetBlockedUsersAsync(int userId);
    Task<IEnumerable<BlockedUserDto>> GetBlockedByUsersAsync(int userId);
    Task<BlockedUserResult> BlockUserAsync(int blockerId, int blockedId, string reason);
    Task<BlockedUserResult> UnblockUserAsync(int blockerId, int blockedId);
    Task<bool> IsUserBlockedAsync(int blockerId, int blockedId);
    Task<bool> IsUserBlockedByAsync(int userId, int blockerId);
    Task<BlockedUserDto?> GetBlockedUserAsync(int blockerId, int blockedId);
}

public class BlockedUserDto
{
    public int BlockId { get; set; }
    public int BlockerId { get; set; }
    public string BlockerName { get; set; } = string.Empty;
    public int BlockedId { get; set; }
    public string BlockedName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class BlockedUserResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public BlockedUserDto? BlockedUser { get; set; }
}
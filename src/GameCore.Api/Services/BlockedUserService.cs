using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Api.Services;

public class BlockedUserService : IBlockedUserService
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<BlockedUserService> _logger;

    public BlockedUserService(GameCoreDbContext context, ILogger<BlockedUserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<BlockedUserDto>> GetBlockedUsersAsync(int userId)
    {
        try
        {
            var blockedUsers = await _context.BlockedUsers
                .Include(b => b.Blocked)
                .Where(b => b.BlockerId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return blockedUsers.Select(b => new BlockedUserDto
            {
                BlockId = b.BlockId,
                BlockerId = b.BlockerId,
                BlockerName = "當前用戶",
                BlockedId = b.BlockedId,
                BlockedName = b.Blocked.Username,
                Reason = b.Reason,
                CreatedAt = b.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting blocked users for user {UserId}", userId);
            return Enumerable.Empty<BlockedUserDto>();
        }
    }

    public async Task<IEnumerable<BlockedUserDto>> GetBlockedByUsersAsync(int userId)
    {
        try
        {
            var blockedByUsers = await _context.BlockedUsers
                .Include(b => b.Blocker)
                .Where(b => b.BlockedId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return blockedByUsers.Select(b => new BlockedUserDto
            {
                BlockId = b.BlockId,
                BlockerId = b.BlockerId,
                BlockerName = b.Blocker.Username,
                BlockedId = b.BlockedId,
                BlockedName = "當前用戶",
                Reason = b.Reason,
                CreatedAt = b.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting blocked by users for user {UserId}", userId);
            return Enumerable.Empty<BlockedUserDto>();
        }
    }

    public async Task<BlockedUserResult> BlockUserAsync(int blockerId, int blockedId, string reason)
    {
        try
        {
            // Check if already blocked
            var existingBlock = await _context.BlockedUsers
                .FirstOrDefaultAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId);

            if (existingBlock != null)
            {
                return new BlockedUserResult
                {
                    Success = false,
                    Message = "用戶已經被封鎖"
                };
            }

            // Check if trying to block self
            if (blockerId == blockedId)
            {
                return new BlockedUserResult
                {
                    Success = false,
                    Message = "不能封鎖自己"
                };
            }

            var blockedUser = new BlockedUser
            {
                BlockerId = blockerId,
                BlockedId = blockedId,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            };

            _context.BlockedUsers.Add(blockedUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User blocked: Blocker {BlockerId}, Blocked {BlockedId}, Reason {Reason}", 
                blockerId, blockedId, reason);

            return new BlockedUserResult
            {
                Success = true,
                Message = "用戶封鎖成功",
                BlockedUser = await GetBlockedUserAsync(blockerId, blockedId)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error blocking user {BlockedId} by user {BlockerId}", blockedId, blockerId);
            return new BlockedUserResult
            {
                Success = false,
                Message = "用戶封鎖失敗"
            };
        }
    }

    public async Task<BlockedUserResult> UnblockUserAsync(int blockerId, int blockedId)
    {
        try
        {
            var blockedUser = await _context.BlockedUsers
                .FirstOrDefaultAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId);

            if (blockedUser == null)
            {
                return new BlockedUserResult
                {
                    Success = false,
                    Message = "用戶未被封鎖"
                };
            }

            _context.BlockedUsers.Remove(blockedUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User unblocked: Blocker {BlockerId}, Blocked {BlockedId}", blockerId, blockedId);

            return new BlockedUserResult
            {
                Success = true,
                Message = "用戶解除封鎖成功"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unblocking user {BlockedId} by user {BlockerId}", blockedId, blockerId);
            return new BlockedUserResult
            {
                Success = false,
                Message = "用戶解除封鎖失敗"
            };
        }
    }

    public async Task<bool> IsUserBlockedAsync(int blockerId, int blockedId)
    {
        try
        {
            return await _context.BlockedUsers
                .AnyAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {BlockedId} is blocked by user {BlockerId}", blockedId, blockerId);
            return false;
        }
    }

    public async Task<bool> IsUserBlockedByAsync(int userId, int blockerId)
    {
        try
        {
            return await _context.BlockedUsers
                .AnyAsync(b => b.BlockerId == blockerId && b.BlockedId == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} is blocked by user {BlockerId}", userId, blockerId);
            return false;
        }
    }

    public async Task<BlockedUserDto?> GetBlockedUserAsync(int blockerId, int blockedId)
    {
        try
        {
            var blockedUser = await _context.BlockedUsers
                .Include(b => b.Blocked)
                .FirstOrDefaultAsync(b => b.BlockerId == blockerId && b.BlockedId == blockedId);

            if (blockedUser == null) return null;

            return new BlockedUserDto
            {
                BlockId = blockedUser.BlockId,
                BlockerId = blockedUser.BlockerId,
                BlockerName = "當前用戶",
                BlockedId = blockedUser.BlockedId,
                BlockedName = blockedUser.Blocked.Username,
                Reason = blockedUser.Reason,
                CreatedAt = blockedUser.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting blocked user {BlockedId} by user {BlockerId}", blockedId, blockerId);
            return null;
        }
    }
}
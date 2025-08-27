using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 用戶資料存取層，處理用戶相關的資料庫操作
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(GameCoreDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        _logger.LogDebug("查詢用戶資料: UserId = {UserId}", userId);

        return await _context.Users
            .Include(u => u.Wallet)
            .AsNoTracking() // 改善效能，避免不必要的變更追蹤
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        _logger.LogDebug("查詢用戶資料: Username = {Username}", username);

        return await _context.Users
            .Include(u => u.Wallet)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        _logger.LogDebug("查詢用戶資料: Email = {Email}", email);

        return await _context.Users
            .Include(u => u.Wallet)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateAsync(User user)
    {
        _logger.LogInformation("建立新用戶: Username = {Username}, Email = {Email}", user.Username, user.Email);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("用戶建立成功: UserId = {UserId}", user.UserId);
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _logger.LogInformation("更新用戶資料: UserId = {UserId}", user.UserId);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("用戶資料更新成功: UserId = {UserId}", user.UserId);
        return user;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        _logger.LogDebug("檢查用戶名是否存在: {Username}", username);

        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        _logger.LogDebug("檢查郵箱是否存在: {Email}", email);

        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email);
    }

    /// <summary>
    /// 根據用戶名查詢用戶（不包含錢包資料，用於登入驗證）
    /// </summary>
    public async Task<User?> GetByUsernameForLoginAsync(string username)
    {
        _logger.LogDebug("查詢用戶資料（登入用）: Username = {Username}", username);

        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        _logger.LogDebug("查詢用戶資料: UsernameOrEmail = {UsernameOrEmail}", usernameOrEmail);

        return await _context.Users
            .Include(u => u.Wallet)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        _logger.LogDebug("根據 Refresh Token 查詢用戶");

        return await _context.Users
            .Include(u => u.Wallet)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);
    }

    public async Task<User> AddAsync(User user)
    {
        _logger.LogInformation("新增用戶: Username = {Username}, Email = {Email}", user.Username, user.Email);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("用戶新增成功: UserId = {UserId}", user.UserId);
        return user;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiryDate)
    {
        _logger.LogDebug("更新用戶 Refresh Token: UserId = {UserId}", userId);

        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiryDate;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearRefreshTokenAsync(int userId)
    {
        _logger.LogDebug("清除用戶 Refresh Token: UserId = {UserId}", userId);

        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _context.SaveChangesAsync();
        }
    }
}

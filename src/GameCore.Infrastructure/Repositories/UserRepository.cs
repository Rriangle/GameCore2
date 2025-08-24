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
        _logger.LogDebug("查詢用戶資料: User_ID = {UserId}", userId);
        
        return await _context.Users
            .Include(u => u.UserWallet)
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .AsNoTracking() // 改善效能，避免不必要的變更追蹤
            .FirstOrDefaultAsync(u => u.User_ID == userId);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        _logger.LogDebug("查詢用戶資料: User_name = {Username}", username);
        
        return await _context.Users
            .Include(u => u.UserWallet)
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.User_name == username);
    }

    public async Task<User?> GetByAccountAsync(string account)
    {
        _logger.LogDebug("查詢用戶資料: User_Account = {Account}", account);
        
        return await _context.Users
            .Include(u => u.UserWallet)
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.User_Account == account);
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeUserId = null)
    {
        var query = _context.Users.AsQueryable();
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.User_ID != excludeUserId.Value);
        }
        
        return !await query.AnyAsync(u => u.User_name == username);
    }

    public async Task<bool> IsAccountUniqueAsync(string account, int? excludeUserId = null)
    {
        var query = _context.Users.AsQueryable();
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.User_ID != excludeUserId.Value);
        }
        
        return !await query.AnyAsync(u => u.User_Account == account);
    }

    public async Task<User> CreateAsync(User user)
    {
        _logger.LogDebug("創建新用戶: User_name = {Username}", user.User_name);
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _logger.LogDebug("更新用戶資料: User_ID = {UserId}", user.User_ID);
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int userId)
    {
        _logger.LogDebug("刪除用戶: User_ID = {UserId}", userId);
        
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return false;
        }
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> ValidateCredentialsAsync(string account, string passwordHash)
    {
        _logger.LogDebug("驗證用戶憑證: User_Account = {Account}", account);
        
        return await _context.Users
            .Include(u => u.UserWallet)
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .FirstOrDefaultAsync(u => u.User_Account == account && u.User_Password == passwordHash);
    }
}

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
            .Include(u => u.Wallet)
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .AsNoTracking() // 改善效能，避免不必要的變更追蹤
            .FirstOrDefaultAsync(u => u.User_ID == userId);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        _logger.LogDebug("查詢用戶資料: User_Account = {Username}", username);
        
        return await _context.Users
            .Include(u => u.Wallet)
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.User_Account == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        _logger.LogDebug("查詢用戶資料: Email = {Email}", email);
        
        return await _context.Users
            .Include(u => u.Wallet)
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .AsNoTracking()
            .Where(u => u.UserIntroduce != null && u.UserIntroduce.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        _logger.LogInformation("建立新用戶: User_Account = {UserAccount}, User_name = {UserName}", user.User_Account, user.User_name);
        
        // 開始資料庫交易，確保所有相關資料一致性
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // 先新增主要用戶資料
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // 如果有相關的 UserIntroduce，一併建立
            if (user.UserIntroduce != null)
            {
                user.UserIntroduce.User_ID = user.User_ID;
                _context.UserIntroduces.Add(user.UserIntroduce);
            }
            
            // 如果有相關的 UserRights，一併建立
            if (user.UserRights != null)
            {
                user.UserRights.User_Id = user.User_ID;
                _context.UserRights.Add(user.UserRights);
            }
            
            // 如果有相關的 UserWallet，一併建立
            if (user.Wallet != null)
            {
                user.Wallet.User_Id = user.User_ID;
                _context.UserWallets.Add(user.Wallet);
            }
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            _logger.LogInformation("用戶建立成功: User_ID = {UserId}", user.User_ID);
            return user;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "建立用戶時發生錯誤: {UserAccount}", user.User_Account);
            throw;
        }
    }

    public async Task<User> UpdateAsync(User user)
    {
        _logger.LogInformation("更新用戶資料: User_ID = {UserId}", user.User_ID);
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("用戶資料更新成功: User_ID = {UserId}", user.User_ID);
        return user;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        _logger.LogDebug("檢查用戶名是否存在: {Username}", username);
        
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.User_Account == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        _logger.LogDebug("檢查郵箱是否存在: {Email}", email);
        
        return await _context.Users
            .Include(u => u.UserIntroduce)
            .AsNoTracking()
            .AnyAsync(u => u.UserIntroduce != null && u.UserIntroduce.Email == email);
    }

    /// <summary>
    /// 根據用戶名查詢用戶（不包含錢包資料，用於登入驗證）
    /// </summary>
    public async Task<User?> GetByUsernameForLoginAsync(string username)
    {
        _logger.LogDebug("查詢用戶資料（登入用）: User_Account = {Username}", username);
        
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.User_Account == username);
    }
}

using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 使用者權限資料存取實作
/// </summary>
public class UserRightsRepository : IUserRightsRepository
{
    private readonly GameCoreDbContext _context;

    public UserRightsRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據使用者ID取得使用者權限
    /// </summary>
    public async Task<UserRights?> GetByUserIdAsync(int userId)
    {
        return await _context.UserRights
            .Include(ur => ur.User)
            .FirstOrDefaultAsync(ur => ur.UserId == userId);
    }

    /// <summary>
    /// 創建使用者權限
    /// </summary>
    public async Task<UserRights> CreateAsync(UserRights userRights)
    {
        _context.UserRights.Add(userRights);
        await _context.SaveChangesAsync();
        return userRights;
    }

    /// <summary>
    /// 更新使用者權限
    /// </summary>
    public async Task<UserRights> UpdateAsync(UserRights userRights)
    {
        _context.UserRights.Update(userRights);
        await _context.SaveChangesAsync();
        return userRights;
    }

    /// <summary>
    /// 刪除使用者權限
    /// </summary>
    public async Task<bool> DeleteAsync(int userId)
    {
        var userRights = await _context.UserRights
            .FirstOrDefaultAsync(ur => ur.UserId == userId);
        
        if (userRights == null)
            return false;

        _context.UserRights.Remove(userRights);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 批量取得多個使用者的權限
    /// </summary>
    public async Task<IEnumerable<UserRights>> GetByUserIdsAsync(IEnumerable<int> userIds)
    {
        return await _context.UserRights
            .Where(ur => userIds.Contains(ur.UserId))
            .ToListAsync();
    }

    /// <summary>
    /// 更新使用者狀態
    /// </summary>
    public async Task<bool> UpdateUserStatusAsync(int userId, bool isActive)
    {
        var userRights = await _context.UserRights
            .FirstOrDefaultAsync(ur => ur.UserId == userId);
        
        if (userRights == null)
            return false;

        userRights.UserStatus = isActive;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 更新購物權限
    /// </summary>
    public async Task<bool> UpdateShoppingPermissionAsync(int userId, bool canShopping)
    {
        var userRights = await _context.UserRights
            .FirstOrDefaultAsync(ur => ur.UserId == userId);
        
        if (userRights == null)
            return false;

        userRights.ShoppingPermission = canShopping;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 更新留言權限
    /// </summary>
    public async Task<bool> UpdateMessagePermissionAsync(int userId, bool canMessage)
    {
        var userRights = await _context.UserRights
            .FirstOrDefaultAsync(ur => ur.UserId == userId);
        
        if (userRights == null)
            return false;

        userRights.MessagePermission = canMessage;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 更新銷售權限
    /// </summary>
    public async Task<bool> UpdateSalesAuthorityAsync(int userId, bool canSales)
    {
        var userRights = await _context.UserRights
            .FirstOrDefaultAsync(ur => ur.UserId == userId);
        
        if (userRights == null)
            return false;

        userRights.SalesAuthority = canSales;
        await _context.SaveChangesAsync();
        return true;
    }
}
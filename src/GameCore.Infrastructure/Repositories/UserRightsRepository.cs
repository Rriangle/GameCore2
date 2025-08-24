using GameCore.Domain.Entities;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 使用者權限存取層
/// 處理 UserRights 表的資料庫操作
/// </summary>
public class UserRightsRepository
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<UserRightsRepository> _logger;

    public UserRightsRepository(GameCoreDbContext context, ILogger<UserRightsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 根據用戶ID查詢權限設定
    /// </summary>
    public async Task<UserRights?> GetByUserIdAsync(int userId)
    {
        _logger.LogDebug("查詢用戶權限: User_Id = {UserId}", userId);
        
        return await _context.UserRights
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.User_Id == userId);
    }

    /// <summary>
    /// 建立用戶權限
    /// </summary>
    public async Task<UserRights> CreateAsync(UserRights userRights)
    {
        _logger.LogInformation("建立用戶權限: User_Id = {UserId}", userRights.User_Id);
        
        _context.UserRights.Add(userRights);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("用戶權限建立成功: User_Id = {UserId}", userRights.User_Id);
        return userRights;
    }

    /// <summary>
    /// 更新用戶權限
    /// </summary>
    public async Task<UserRights> UpdateAsync(UserRights userRights)
    {
        _logger.LogInformation("更新用戶權限: User_Id = {UserId}", userRights.User_Id);
        
        _context.UserRights.Update(userRights);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("用戶權限更新成功: User_Id = {UserId}", userRights.User_Id);
        return userRights;
    }

    /// <summary>
    /// 啟用或停用用戶
    /// </summary>
    public async Task<bool> SetUserStatusAsync(int userId, bool isActive)
    {
        _logger.LogInformation("設定用戶狀態: User_Id = {UserId}, Status = {Status}", userId, isActive);
        
        var userRights = await _context.UserRights.FirstOrDefaultAsync(r => r.User_Id == userId);
        if (userRights == null)
        {
            _logger.LogWarning("用戶權限不存在: User_Id = {UserId}", userId);
            return false;
        }

        userRights.User_Status = isActive;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("用戶狀態設定成功: User_Id = {UserId}, Status = {Status}", userId, isActive);
        return true;
    }

    /// <summary>
    /// 設定購物權限
    /// </summary>
    public async Task<bool> SetShoppingPermissionAsync(int userId, bool canShop)
    {
        _logger.LogInformation("設定購物權限: User_Id = {UserId}, CanShop = {CanShop}", userId, canShop);
        
        var userRights = await _context.UserRights.FirstOrDefaultAsync(r => r.User_Id == userId);
        if (userRights == null)
        {
            _logger.LogWarning("用戶權限不存在: User_Id = {UserId}", userId);
            return false;
        }

        userRights.ShoppingPermission = canShop;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("購物權限設定成功: User_Id = {UserId}, CanShop = {CanShop}", userId, canShop);
        return true;
    }

    /// <summary>
    /// 設定留言權限
    /// </summary>
    public async Task<bool> SetMessagePermissionAsync(int userId, bool canMessage)
    {
        _logger.LogInformation("設定留言權限: User_Id = {UserId}, CanMessage = {CanMessage}", userId, canMessage);
        
        var userRights = await _context.UserRights.FirstOrDefaultAsync(r => r.User_Id == userId);
        if (userRights == null)
        {
            _logger.LogWarning("用戶權限不存在: User_Id = {UserId}", userId);
            return false;
        }

        userRights.MessagePermission = canMessage;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("留言權限設定成功: User_Id = {UserId}, CanMessage = {CanMessage}", userId, canMessage);
        return true;
    }

    /// <summary>
    /// 設定銷售權限
    /// </summary>
    public async Task<bool> SetSalesAuthorityAsync(int userId, bool canSell)
    {
        _logger.LogInformation("設定銷售權限: User_Id = {UserId}, CanSell = {CanSell}", userId, canSell);
        
        var userRights = await _context.UserRights.FirstOrDefaultAsync(r => r.User_Id == userId);
        if (userRights == null)
        {
            _logger.LogWarning("用戶權限不存在: User_Id = {UserId}", userId);
            return false;
        }

        userRights.SalesAuthority = canSell;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("銷售權限設定成功: User_Id = {UserId}, CanSell = {CanSell}", userId, canSell);
        return true;
    }
}
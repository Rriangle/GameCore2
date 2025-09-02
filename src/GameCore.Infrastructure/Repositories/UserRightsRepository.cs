using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 用戶權限資料存取層
/// 實作 IUserRightsRepository 介面
/// </summary>
public class UserRightsRepository : IUserRightsRepository
{
    private readonly GameCoreDbContext _context;

    public UserRightsRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據用戶ID取得用戶權限
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>用戶權限資料</returns>
    public async Task<UserRights?> GetByUserIdAsync(int userId)
    {
        return await _context.UserRights
            .FirstOrDefaultAsync(ur => ur.User_Id == userId);
    }

    /// <summary>
    /// 新增用戶權限
    /// </summary>
    /// <param name="userRights">用戶權限資料</param>
    /// <returns>新增的用戶權限資料</returns>
    public async Task<UserRights> AddAsync(UserRights userRights)
    {
        _context.UserRights.Add(userRights);
        await _context.SaveChangesAsync();
        return userRights;
    }

    /// <summary>
    /// 更新用戶權限
    /// </summary>
    /// <param name="userRights">用戶權限資料</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateAsync(UserRights userRights)
    {
        _context.UserRights.Update(userRights);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除用戶權限
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(int userId)
    {
        var userRights = await _context.UserRights.FindAsync(userId);
        if (userRights == null)
            return false;

        _context.UserRights.Remove(userRights);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 根據權限狀態取得用戶列表
    /// </summary>
    /// <param name="userStatus">用戶狀態</param>
    /// <returns>用戶權限列表</returns>
    public async Task<IEnumerable<UserRights>> GetByStatusAsync(bool userStatus)
    {
        return await _context.UserRights
            .Where(ur => ur.User_Status == userStatus)
            .ToListAsync();
    }

    /// <summary>
    /// 根據購物權限取得用戶列表
    /// </summary>
    /// <param name="shoppingPermission">購物權限</param>
    /// <returns>用戶權限列表</returns>
    public async Task<IEnumerable<UserRights>> GetByShoppingPermissionAsync(bool shoppingPermission)
    {
        return await _context.UserRights
            .Where(ur => ur.ShoppingPermission == shoppingPermission)
            .ToListAsync();
    }

    /// <summary>
    /// 根據留言權限取得用戶列表
    /// </summary>
    /// <param name="messagePermission">留言權限</param>
    /// <returns>用戶權限列表</returns>
    public async Task<IEnumerable<UserRights>> GetByMessagePermissionAsync(bool messagePermission)
    {
        return await _context.UserRights
            .Where(ur => ur.MessagePermission == messagePermission)
            .ToListAsync();
    }

    /// <summary>
    /// 根據銷售權限取得用戶列表
    /// </summary>
    /// <param name="salesAuthority">銷售權限</param>
    /// <returns>用戶權限列表</returns>
    public async Task<IEnumerable<UserRights>> GetBySalesAuthorityAsync(bool salesAuthority)
    {
        return await _context.UserRights
            .Where(ur => ur.SalesAuthority == salesAuthority)
            .ToListAsync();
    }

    /// <summary>
    /// 取得所有用戶權限
    /// </summary>
    /// <returns>用戶權限列表</returns>
    public async Task<IEnumerable<UserRights>> GetAllAsync()
    {
        return await _context.UserRights.ToListAsync();
    }

    /// <summary>
    /// 分頁取得用戶權限
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>用戶權限列表</returns>
    public async Task<IEnumerable<UserRights>> GetPagedAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        return await _context.UserRights
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// 取得用戶權限總數
    /// </summary>
    /// <returns>用戶權限總數</returns>
    public async Task<int> GetCountAsync()
    {
        return await _context.UserRights.CountAsync();
    }
} 
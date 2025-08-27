using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 使用者基本資料存取實作
/// 處理使用者相關的資料庫操作，嚴格按照規格文件的資料表結構
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

    /// <summary>
    /// 根據使用者ID取得使用者資料（包含所有相關資料）
    /// </summary>
    public async Task<User?> GetByIdAsync(int userId)
    {
        _logger.LogDebug("查詢使用者資料: UserId = {UserId}", userId);
        
        return await _context.Users
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .Include(u => u.Wallet)
            .Include(u => u.MemberSalesProfile)
            .Include(u => u.UserSalesInformation)
            .AsNoTracking() // 改善效能，避免不必要的變更追蹤
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// 根據使用者姓名取得使用者資料
    /// </summary>
    public async Task<User?> GetByUserNameAsync(string userName)
    {
        _logger.LogDebug("查詢使用者資料: UserName = {UserName}", userName);
        
        return await _context.Users
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .Include(u => u.Wallet)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName);
    }

    /// <summary>
    /// 根據登入帳號取得使用者資料
    /// </summary>
    public async Task<User?> GetByUserAccountAsync(string userAccount)
    {
        _logger.LogDebug("查詢使用者資料: UserAccount = {UserAccount}", userAccount);
        
        return await _context.Users
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .Include(u => u.Wallet)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserAccount == userAccount);
    }

    /// <summary>
    /// 創建使用者（包含預設的權限和錢包）
    /// </summary>
    public async Task<User> CreateAsync(User user)
    {
        _logger.LogInformation("建立新使用者: UserName = {UserName}, UserAccount = {UserAccount}", user.UserName, user.UserAccount);
        
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. 創建使用者基本資料
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 2. 創建預設的使用者權限
            var userRights = new UserRights
            {
                UserId = user.UserId,
                UserStatus = true,           // 預設啟用
                ShoppingPermission = true,   // 預設可購物
                MessagePermission = true,    // 預設可留言
                SalesAuthority = false       // 預設不可銷售，需申請開通
            };
            _context.UserRights.Add(userRights);

            // 3. 創建預設的使用者錢包
            var userWallet = new UserWallet
            {
                UserId = user.UserId,
                UserPoint = 100,  // 新用戶贈送100點數
                CouponNumber = null
            };
            _context.UserWallets.Add(userWallet);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            _logger.LogInformation("使用者建立成功: UserId = {UserId}", user.UserId);
            return user;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "建立使用者失敗: UserName = {UserName}", user.UserName);
            throw;
        }
    }

    /// <summary>
    /// 更新使用者資料
    /// </summary>
    public async Task<User> UpdateAsync(User user)
    {
        _logger.LogInformation("更新使用者資料: UserId = {UserId}", user.UserId);
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("使用者資料更新成功: UserId = {UserId}", user.UserId);
        return user;
    }

    /// <summary>
    /// 刪除使用者（硬刪除，會級聯刪除所有相關資料）
    /// </summary>
    public async Task<bool> DeleteAsync(int userId)
    {
        _logger.LogWarning("刪除使用者: UserId = {UserId}", userId);
        
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        _logger.LogWarning("使用者刪除成功: UserId = {UserId}", userId);
        return true;
    }

    /// <summary>
    /// 檢查使用者姓名是否已存在
    /// </summary>
    public async Task<bool> ExistsByUserNameAsync(string userName, int? excludeUserId = null)
    {
        _logger.LogDebug("檢查使用者姓名是否存在: {UserName}", userName);
        
        var query = _context.Users.Where(u => u.UserName == userName);
        
        if (excludeUserId.HasValue)
            query = query.Where(u => u.UserId != excludeUserId.Value);
        
        return await query.AsNoTracking().AnyAsync();
    }

    /// <summary>
    /// 檢查登入帳號是否已存在
    /// </summary>
    public async Task<bool> ExistsByUserAccountAsync(string userAccount, int? excludeUserId = null)
    {
        _logger.LogDebug("檢查登入帳號是否存在: {UserAccount}", userAccount);
        
        var query = _context.Users.Where(u => u.UserAccount == userAccount);
        
        if (excludeUserId.HasValue)
            query = query.Where(u => u.UserId != excludeUserId.Value);
        
        return await query.AsNoTracking().AnyAsync();
    }

    /// <summary>
    /// 取得使用者列表（分頁）
    /// </summary>
    public async Task<IEnumerable<User>> GetUsersAsync(int skip = 0, int take = 100, string? searchKeyword = null)
    {
        _logger.LogDebug("查詢使用者列表: Skip = {Skip}, Take = {Take}, SearchKeyword = {SearchKeyword}", skip, take, searchKeyword);
        
        var query = _context.Users
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .Include(u => u.Wallet)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(u => 
                u.UserName.Contains(searchKeyword) || 
                u.UserAccount.Contains(searchKeyword) ||
                (u.UserIntroduce != null && u.UserIntroduce.UserNickName.Contains(searchKeyword)));
        }

        return await query
            .OrderByDescending(u => u.UserId)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    /// <summary>
    /// 取得使用者總數
    /// </summary>
    public async Task<int> GetUsersCountAsync(string? searchKeyword = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(u => 
                u.UserName.Contains(searchKeyword) || 
                u.UserAccount.Contains(searchKeyword) ||
                (u.UserIntroduce != null && u.UserIntroduce.UserNickName.Contains(searchKeyword)));
        }

        return await query.AsNoTracking().CountAsync();
    }

    /// <summary>
    /// 取得完整的使用者資料（包含所有關聯資料）
    /// </summary>
    public async Task<User?> GetCompleteUserDataAsync(int userId)
    {
        _logger.LogDebug("查詢完整使用者資料: UserId = {UserId}", userId);
        
        return await _context.Users
            .Include(u => u.UserIntroduce)
            .Include(u => u.UserRights)
            .Include(u => u.Wallet)
            .Include(u => u.MemberSalesProfile)
            .Include(u => u.UserSalesInformation)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// 根據登入帳號查詢使用者（用於登入驗證，不包含關聯資料）
    /// </summary>
    public async Task<User?> GetByUserAccountForLoginAsync(string userAccount)
    {
        _logger.LogDebug("查詢使用者資料（登入用）: UserAccount = {UserAccount}", userAccount);
        
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserAccount == userAccount);
    }
}

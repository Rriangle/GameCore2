using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 用戶資料庫操作實作
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
    /// 根據ID取得用戶
    /// </summary>
    /// <param name="id">用戶ID</param>
    /// <returns>用戶實體</returns>
    public async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .Include(u => u.UserSalesInformation)
                .AsNoTracking() // 效能優化：唯讀查詢不需要變更追蹤
                .FirstOrDefaultAsync(u => u.User_ID == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據ID取得用戶時發生錯誤: {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// 根據帳號取得用戶
    /// </summary>
    /// <param name="account">用戶帳號</param>
    /// <returns>用戶實體</returns>
    public async Task<User?> GetByAccountAsync(string account)
    {
        try
        {
            if (string.IsNullOrEmpty(account))
                throw new ArgumentException("帳號不能為空", nameof(account));

            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .Include(u => u.UserSalesInformation)
                .AsNoTracking() // 效能優化：唯讀查詢不需要變更追蹤
                .FirstOrDefaultAsync(u => u.User_Account == account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據帳號取得用戶時發生錯誤: {Account}", account);
            throw;
        }
    }

    /// <summary>
    /// 根據電子郵件取得用戶
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>用戶實體</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("電子郵件不能為空", nameof(email));

            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .Include(u => u.UserSalesInformation)
                .AsNoTracking() // 效能優化：唯讀查詢不需要變更追蹤
                .FirstOrDefaultAsync(u => u.UserIntroduce != null && u.UserIntroduce.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據電子郵件取得用戶時發生錯誤: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// 根據手機號碼取得用戶
    /// </summary>
    /// <param name="phone">手機號碼</param>
    /// <returns>用戶實體</returns>
    public async Task<User?> GetByPhoneAsync(string phone)
    {
        try
        {
            if (string.IsNullOrEmpty(phone))
                throw new ArgumentException("手機號碼不能為空", nameof(phone));

            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .Include(u => u.UserSalesInformation)
                .AsNoTracking() // 效能優化：唯讀查詢不需要變更追蹤
                .FirstOrDefaultAsync(u => u.UserIntroduce != null && u.UserIntroduce.Cellphone == phone);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根據手機號碼取得用戶時發生錯誤: {Phone}", phone);
            throw;
        }
    }

    /// <summary>
    /// 新增用戶
    /// </summary>
    /// <param name="user">用戶實體</param>
    /// <returns>新增的用戶實體</returns>
    public async Task<User> AddAsync(User user)
    {
        try
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.Created_At = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("新增用戶成功: {UserId}, {Account}", user.User_ID, user.User_Account);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增用戶時發生錯誤: {Account}", user?.User_Account);
            throw;
        }
    }

    /// <summary>
    /// 更新用戶
    /// </summary>
    /// <param name="user">用戶實體</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateAsync(User user)
    {
        try
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Update(user);
            var result = await _context.SaveChangesAsync();

            _logger.LogInformation("更新用戶成功: {UserId}, {Account}", user.User_ID, user.User_Account);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用戶時發生錯誤: {UserId}, {Account}", user?.User_ID, user?.User_Account);
            throw;
        }
    }

    /// <summary>
    /// 刪除用戶
    /// </summary>
    /// <param name="id">用戶ID</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation("刪除用戶成功: {UserId}, {Account}", id, user.User_Account);
                return result > 0;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除用戶時發生錯誤: {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// 檢查帳號是否存在
    /// </summary>
    /// <param name="account">用戶帳號</param>
    /// <returns>是否存在</returns>
    public async Task<bool> AccountExistsAsync(string account)
    {
        try
        {
            if (string.IsNullOrEmpty(account))
                return false;

            // 效能優化：使用 AnyAsync 替代 CountAsync > 0，避免完整計數
            return await _context.Users
                .AsNoTracking() // 效能優化：存在性檢查不需要變更追蹤
                .AnyAsync(u => u.User_Account == account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查帳號是否存在時發生錯誤: {Account}", account);
            throw;
        }
    }

    /// <summary>
    /// 檢查電子郵件是否存在
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>是否存在</returns>
    public async Task<bool> EmailExistsAsync(string email)
    {
        try
        {
            if (string.IsNullOrEmpty(email))
                return false;

            // 效能優化：使用 AnyAsync 替代 CountAsync > 0，避免完整計數
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .AsNoTracking() // 效能優化：存在性檢查不需要變更追蹤
                .AnyAsync(u => u.UserIntroduce != null && u.UserIntroduce.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查電子郵件是否存在時發生錯誤: {Email}", email);
            throw;
        }
    }

    /// <summary>
    /// 檢查手機號碼是否存在
    /// </summary>
    /// <param name="phone">手機號碼</param>
    /// <returns>是否存在</returns>
    public async Task<bool> PhoneExistsAsync(string phone)
    {
        try
        {
            if (string.IsNullOrEmpty(phone))
                return false;

            // 效能優化：使用 AnyAsync 替代 CountAsync > 0，避免完整計數
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .AsNoTracking() // 效能優化：存在性檢查不需要變更追蹤
                .AnyAsync(u => u.UserIntroduce != null && u.UserIntroduce.Cellphone == phone);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查手機號碼是否存在時發生錯誤: {Phone}", phone);
            throw;
        }
    }

    /// <summary>
    /// 取得所有用戶
    /// </summary>
    /// <returns>用戶列表</returns>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        try
        {
            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .Include(u => u.UserSalesInformation)
                .AsNoTracking() // 效能優化：列表查詢通常不需要變更追蹤
                .OrderBy(u => u.User_ID)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得所有用戶時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 分頁取得用戶
    /// </summary>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">頁面大小</param>
    /// <returns>用戶列表</returns>
    public async Task<IEnumerable<User>> GetPagedAsync(int page, int pageSize)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.Users
                .Include(u => u.UserIntroduce)
                .Include(u => u.UserSalesInformation)
                .AsNoTracking() // 效能優化：分頁查詢通常不需要變更追蹤
                .OrderBy(u => u.User_ID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分頁取得用戶時發生錯誤: Page={Page}, PageSize={PageSize}", page, pageSize);
            throw;
        }
    }

    /// <summary>
    /// 取得用戶總數
    /// </summary>
    /// <returns>用戶總數</returns>
    public async Task<int> GetCountAsync()
    {
        try
        {
            // 效能優化：計數查詢不需要變更追蹤
            return await _context.Users
                .AsNoTracking() // 效能優化：計數查詢不需要變更追蹤
                .CountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取得用戶總數時發生錯誤");
            throw;
        }
    }

    // 保留原有的其他方法以維持向後相容性
    /// <summary>
    /// 根據條件搜尋用戶
    /// </summary>
    /// <param name="searchTerm">搜尋關鍵字</param>
    /// <param name="page">頁碼</param>
    /// <param name="pageSize">每頁大小</param>
    /// <returns>分頁用戶列表</returns>
    public async Task<List<User>> SearchUsersAsync(string searchTerm, int page = 1, int pageSize = 20)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;

            // 效能優化：使用 AsNoTracking() 提升唯讀查詢效能
            return await _context.Users
                .AsNoTracking() // 效能優化：搜尋查詢通常不需要變更追蹤
                .Where(u => u.User_Account.Contains(searchTerm) || 
                           u.User_Name.Contains(searchTerm))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜尋用戶時發生錯誤: {SearchTerm}, Page={Page}, PageSize={PageSize}", 
                searchTerm, page, pageSize);
            throw;
        }
    }

    /// <summary>
    /// 檢查帳號是否存在（向後相容性方法）
    /// </summary>
    /// <param name="account">帳號</param>
    /// <returns>是否存在</returns>
    public async Task<bool> ExistsAsync(string account)
    {
        return await AccountExistsAsync(account);
    }
} 
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 使用者銷售資訊資料存取實作
/// </summary>
public class UserSalesInformationRepository : IUserSalesInformationRepository
{
    private readonly GameCoreDbContext _context;

    public UserSalesInformationRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據使用者ID取得使用者銷售資訊
    /// </summary>
    public async Task<UserSalesInformation?> GetByUserIdAsync(int userId)
    {
        return await _context.UserSalesInformations
            .Include(usi => usi.User)
            .FirstOrDefaultAsync(usi => usi.UserId == userId);
    }

    /// <summary>
    /// 創建使用者銷售資訊
    /// </summary>
    public async Task<UserSalesInformation> CreateAsync(UserSalesInformation salesInfo)
    {
        _context.UserSalesInformations.Add(salesInfo);
        await _context.SaveChangesAsync();
        return salesInfo;
    }

    /// <summary>
    /// 更新使用者銷售資訊
    /// </summary>
    public async Task<UserSalesInformation> UpdateAsync(UserSalesInformation salesInfo)
    {
        _context.UserSalesInformations.Update(salesInfo);
        await _context.SaveChangesAsync();
        return salesInfo;
    }

    /// <summary>
    /// 刪除使用者銷售資訊
    /// </summary>
    public async Task<bool> DeleteAsync(int userId)
    {
        var salesInfo = await _context.UserSalesInformations
            .FirstOrDefaultAsync(usi => usi.UserId == userId);
        
        if (salesInfo == null)
            return false;

        _context.UserSalesInformations.Remove(salesInfo);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 增加使用者銷售錢包金額
    /// </summary>
    public async Task<int> AddWalletAmountAsync(int userId, int amount)
    {
        var salesInfo = await _context.UserSalesInformations
            .FirstOrDefaultAsync(usi => usi.UserId == userId);
        
        if (salesInfo == null)
        {
            // 如果不存在，創建一個新的銷售資訊記錄
            salesInfo = new UserSalesInformation
            {
                UserId = userId,
                UserSalesWallet = amount
            };
            _context.UserSalesInformations.Add(salesInfo);
        }
        else
        {
            salesInfo.UserSalesWallet += amount;
        }
        
        await _context.SaveChangesAsync();
        return salesInfo.UserSalesWallet;
    }

    /// <summary>
    /// 扣除使用者銷售錢包金額
    /// </summary>
    public async Task<int> DeductWalletAmountAsync(int userId, int amount)
    {
        var salesInfo = await _context.UserSalesInformations
            .FirstOrDefaultAsync(usi => usi.UserId == userId);
        
        if (salesInfo == null || salesInfo.UserSalesWallet < amount)
            return -1; // 餘額不足或記錄不存在

        salesInfo.UserSalesWallet -= amount;
        await _context.SaveChangesAsync();
        return salesInfo.UserSalesWallet;
    }

    /// <summary>
    /// 取得使用者銷售錢包餘額
    /// </summary>
    public async Task<int> GetWalletBalanceAsync(int userId)
    {
        var salesInfo = await _context.UserSalesInformations
            .FirstOrDefaultAsync(usi => usi.UserId == userId);
        
        return salesInfo?.UserSalesWallet ?? 0;
    }

    /// <summary>
    /// 批量取得多個使用者的銷售資訊
    /// </summary>
    public async Task<IEnumerable<UserSalesInformation>> GetByUserIdsAsync(IEnumerable<int> userIds)
    {
        return await _context.UserSalesInformations
            .Where(usi => userIds.Contains(usi.UserId))
            .ToListAsync();
    }
}
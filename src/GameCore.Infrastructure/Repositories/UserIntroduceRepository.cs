using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 使用者介紹資料存取實作
/// </summary>
public class UserIntroduceRepository : IUserIntroduceRepository
{
    private readonly GameCoreDbContext _context;

    public UserIntroduceRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據使用者ID取得使用者介紹資料
    /// </summary>
    public async Task<UserIntroduce?> GetByUserIdAsync(int userId)
    {
        return await _context.UserIntroduces
            .Include(ui => ui.User)
            .FirstOrDefaultAsync(ui => ui.UserId == userId);
    }

    /// <summary>
    /// 創建使用者介紹資料
    /// </summary>
    public async Task<UserIntroduce> CreateAsync(UserIntroduce userIntroduce)
    {
        _context.UserIntroduces.Add(userIntroduce);
        await _context.SaveChangesAsync();
        return userIntroduce;
    }

    /// <summary>
    /// 更新使用者介紹資料
    /// </summary>
    public async Task<UserIntroduce> UpdateAsync(UserIntroduce userIntroduce)
    {
        _context.UserIntroduces.Update(userIntroduce);
        await _context.SaveChangesAsync();
        return userIntroduce;
    }

    /// <summary>
    /// 刪除使用者介紹資料
    /// </summary>
    public async Task<bool> DeleteAsync(int userId)
    {
        var userIntroduce = await _context.UserIntroduces
            .FirstOrDefaultAsync(ui => ui.UserId == userId);
        
        if (userIntroduce == null)
            return false;

        _context.UserIntroduces.Remove(userIntroduce);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 檢查暱稱是否已存在
    /// </summary>
    public async Task<bool> IsNicknameExistsAsync(string nickname, int? excludeUserId = null)
    {
        var query = _context.UserIntroduces.Where(ui => ui.UserNickName == nickname);
        
        if (excludeUserId.HasValue)
            query = query.Where(ui => ui.UserId != excludeUserId.Value);
        
        return await query.AnyAsync();
    }

    /// <summary>
    /// 檢查身分證字號是否已存在
    /// </summary>
    public async Task<bool> IsIdNumberExistsAsync(string idNumber, int? excludeUserId = null)
    {
        var query = _context.UserIntroduces.Where(ui => ui.IdNumber == idNumber);
        
        if (excludeUserId.HasValue)
            query = query.Where(ui => ui.UserId != excludeUserId.Value);
        
        return await query.AnyAsync();
    }

    /// <summary>
    /// 檢查電話是否已存在
    /// </summary>
    public async Task<bool> IsCellphoneExistsAsync(string cellphone, int? excludeUserId = null)
    {
        var query = _context.UserIntroduces.Where(ui => ui.Cellphone == cellphone);
        
        if (excludeUserId.HasValue)
            query = query.Where(ui => ui.UserId != excludeUserId.Value);
        
        return await query.AnyAsync();
    }

    /// <summary>
    /// 檢查Email是否已存在
    /// </summary>
    public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null)
    {
        var query = _context.UserIntroduces.Where(ui => ui.Email == email);
        
        if (excludeUserId.HasValue)
            query = query.Where(ui => ui.UserId != excludeUserId.Value);
        
        return await query.AnyAsync();
    }
}
using GameCore.Domain.Entities;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 使用者詳細資料存取層
/// 處理 UserIntroduce 表的資料庫操作
/// </summary>
public class UserIntroduceRepository
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<UserIntroduceRepository> _logger;

    public UserIntroduceRepository(GameCoreDbContext context, ILogger<UserIntroduceRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 根據用戶ID查詢詳細資料
    /// </summary>
    public async Task<UserIntroduce?> GetByUserIdAsync(int userId)
    {
        _logger.LogDebug("查詢用戶詳細資料: User_ID = {UserId}", userId);
        
        return await _context.UserIntroduces
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.User_ID == userId);
    }

    /// <summary>
    /// 建立用戶詳細資料
    /// </summary>
    public async Task<UserIntroduce> CreateAsync(UserIntroduce userIntroduce)
    {
        _logger.LogInformation("建立用戶詳細資料: User_ID = {UserId}, Email = {Email}", 
            userIntroduce.User_ID, userIntroduce.Email);
        
        _context.UserIntroduces.Add(userIntroduce);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("用戶詳細資料建立成功: User_ID = {UserId}", userIntroduce.User_ID);
        return userIntroduce;
    }

    /// <summary>
    /// 更新用戶詳細資料
    /// </summary>
    public async Task<UserIntroduce> UpdateAsync(UserIntroduce userIntroduce)
    {
        _logger.LogInformation("更新用戶詳細資料: User_ID = {UserId}", userIntroduce.User_ID);
        
        _context.UserIntroduces.Update(userIntroduce);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("用戶詳細資料更新成功: User_ID = {UserId}", userIntroduce.User_ID);
        return userIntroduce;
    }

    /// <summary>
    /// 檢查郵箱是否已存在
    /// </summary>
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        _logger.LogDebug("檢查郵箱是否存在: {Email}", email);
        
        return await _context.UserIntroduces
            .AsNoTracking()
            .AnyAsync(u => u.Email == email);
    }

    /// <summary>
    /// 檢查暱稱是否已存在
    /// </summary>
    public async Task<bool> ExistsByNicknameAsync(string nickname)
    {
        _logger.LogDebug("檢查暱稱是否存在: {Nickname}", nickname);
        
        return await _context.UserIntroduces
            .AsNoTracking()
            .AnyAsync(u => u.User_NickName == nickname);
    }

    /// <summary>
    /// 檢查身分證字號是否已存在
    /// </summary>
    public async Task<bool> ExistsByIdNumberAsync(string idNumber)
    {
        _logger.LogDebug("檢查身分證字號是否存在: {IdNumber}", idNumber);
        
        return await _context.UserIntroduces
            .AsNoTracking()
            .AnyAsync(u => u.IdNumber == idNumber);
    }

    /// <summary>
    /// 檢查電話號碼是否已存在
    /// </summary>
    public async Task<bool> ExistsByCellphoneAsync(string cellphone)
    {
        _logger.LogDebug("檢查電話號碼是否存在: {Cellphone}", cellphone);
        
        return await _context.UserIntroduces
            .AsNoTracking()
            .AnyAsync(u => u.Cellphone == cellphone);
    }
}
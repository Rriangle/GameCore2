using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 用戶介紹資料存取層
/// 實作 IUserIntroduceRepository 介面
/// </summary>
public class UserIntroduceRepository : IUserIntroduceRepository
{
    private readonly GameCoreDbContext _context;

    public UserIntroduceRepository(GameCoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根據用戶ID取得用戶介紹
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>用戶介紹資料</returns>
    public async Task<UserIntroduce?> GetByUserIdAsync(int userId)
    {
        return await _context.UserIntroduces
            .FirstOrDefaultAsync(ui => ui.User_ID == userId);
    }

    /// <summary>
    /// 根據暱稱取得用戶介紹
    /// </summary>
    /// <param name="nickName">暱稱</param>
    /// <returns>用戶介紹資料</returns>
    public async Task<UserIntroduce?> GetByNickNameAsync(string nickName)
    {
        return await _context.UserIntroduces
            .FirstOrDefaultAsync(ui => ui.User_NickName == nickName);
    }

    /// <summary>
    /// 根據身分證字號取得用戶介紹
    /// </summary>
    /// <param name="idNumber">身分證字號</param>
    /// <returns>用戶介紹資料</returns>
    public async Task<UserIntroduce?> GetByIdNumberAsync(string idNumber)
    {
        return await _context.UserIntroduces
            .FirstOrDefaultAsync(ui => ui.IdNumber == idNumber);
    }

    /// <summary>
    /// 根據手機號碼取得用戶介紹
    /// </summary>
    /// <param name="cellphone">手機號碼</param>
    /// <returns>用戶介紹資料</returns>
    public async Task<UserIntroduce?> GetByCellphoneAsync(string cellphone)
    {
        return await _context.UserIntroduces
            .FirstOrDefaultAsync(ui => ui.Cellphone == cellphone);
    }

    /// <summary>
    /// 根據電子郵件取得用戶介紹
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>用戶介紹資料</returns>
    public async Task<UserIntroduce?> GetByEmailAsync(string email)
    {
        return await _context.UserIntroduces
            .FirstOrDefaultAsync(ui => ui.Email == email);
    }

    /// <summary>
    /// 新增用戶介紹
    /// </summary>
    /// <param name="userIntroduce">用戶介紹資料</param>
    /// <returns>新增的用戶介紹資料</returns>
    public async Task<UserIntroduce> AddAsync(UserIntroduce userIntroduce)
    {
        _context.UserIntroduces.Add(userIntroduce);
        await _context.SaveChangesAsync();
        return userIntroduce;
    }

    /// <summary>
    /// 更新用戶介紹
    /// </summary>
    /// <param name="userIntroduce">用戶介紹資料</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateAsync(UserIntroduce userIntroduce)
    {
        _context.UserIntroduces.Update(userIntroduce);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 刪除用戶介紹
    /// </summary>
    /// <param name="userId">用戶ID</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(int userId)
    {
        var userIntroduce = await _context.UserIntroduces.FindAsync(userId);
        if (userIntroduce == null)
            return false;

        _context.UserIntroduces.Remove(userIntroduce);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    /// <summary>
    /// 檢查暱稱是否已存在
    /// </summary>
    /// <param name="nickName">暱稱</param>
    /// <returns>是否存在</returns>
    public async Task<bool> NickNameExistsAsync(string nickName)
    {
        return await _context.UserIntroduces.AnyAsync(ui => ui.User_NickName == nickName);
    }

    /// <summary>
    /// 檢查身分證字號是否已存在
    /// </summary>
    /// <param name="idNumber">身分證字號</param>
    /// <returns>是否存在</returns>
    public async Task<bool> IdNumberExistsAsync(string idNumber)
    {
        return await _context.UserIntroduces.AnyAsync(ui => ui.IdNumber == idNumber);
    }

    /// <summary>
    /// 檢查手機號碼是否已存在
    /// </summary>
    /// <param name="cellphone">手機號碼</param>
    /// <returns>是否存在</returns>
    public async Task<bool> CellphoneExistsAsync(string cellphone)
    {
        return await _context.UserIntroduces.AnyAsync(ui => ui.Cellphone == cellphone);
    }

    /// <summary>
    /// 檢查電子郵件是否已存在
    /// </summary>
    /// <param name="email">電子郵件</param>
    /// <returns>是否存在</returns>
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.UserIntroduces.AnyAsync(ui => ui.Email == email);
    }
} 
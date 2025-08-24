using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Repositories;

/// <summary>
/// 會員銷售資料倉儲實作，處理銷售資料的資料庫操作
/// </summary>
public class MemberSalesProfileRepository : IMemberSalesProfileRepository
{
    private readonly GameCoreDbContext _context;
    private readonly ILogger<MemberSalesProfileRepository> _logger;

    /// <summary>
    /// 建構函式，注入相依服務
    /// </summary>
    /// <param name="context">資料庫上下文</param>
    /// <param name="logger">日誌記錄器</param>
    public MemberSalesProfileRepository(GameCoreDbContext context, ILogger<MemberSalesProfileRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 根據用戶編號取得銷售資料
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>銷售資料，不存在則返回 null</returns>
    public async Task<MemberSalesProfile?> GetByUserIdAsync(int userId)
    {
        try
        {
            _logger.LogInformation("正在查詢用戶 {UserId} 的銷售資料", userId);

            var salesProfile = await _context.MemberSalesProfiles
                .Include(sp => sp.User)
                .FirstOrDefaultAsync(sp => sp.User_Id == userId);

            if (salesProfile == null)
            {
                _logger.LogInformation("用戶 {UserId} 的銷售資料不存在", userId);
            }

            return salesProfile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢用戶 {UserId} 銷售資料時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 建立新銷售資料
    /// </summary>
    /// <param name="salesProfile">銷售資料</param>
    /// <returns>建立的銷售資料</returns>
    public async Task<MemberSalesProfile> CreateAsync(MemberSalesProfile salesProfile)
    {
        try
        {
            _logger.LogInformation("正在建立用戶 {UserId} 的銷售資料", salesProfile.User_Id);

            _context.MemberSalesProfiles.Add(salesProfile);
            await _context.SaveChangesAsync();

            _logger.LogInformation("成功建立用戶 {UserId} 的銷售資料", salesProfile.User_Id);
            return salesProfile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立用戶 {UserId} 銷售資料時發生錯誤", salesProfile.User_Id);
            throw;
        }
    }

    /// <summary>
    /// 更新銷售資料
    /// </summary>
    /// <param name="salesProfile">銷售資料</param>
    /// <returns>更新的銷售資料</returns>
    public async Task<MemberSalesProfile> UpdateAsync(MemberSalesProfile salesProfile)
    {
        try
        {
            _logger.LogInformation("正在更新用戶 {UserId} 的銷售資料", salesProfile.User_Id);

            _context.MemberSalesProfiles.Update(salesProfile);
            await _context.SaveChangesAsync();

            _logger.LogInformation("成功更新用戶 {UserId} 的銷售資料", salesProfile.User_Id);
            return salesProfile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新用戶 {UserId} 銷售資料時發生錯誤", salesProfile.User_Id);
            throw;
        }
    }

    /// <summary>
    /// 刪除銷售資料
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>是否刪除成功</returns>
    public async Task<bool> DeleteAsync(int userId)
    {
        try
        {
            _logger.LogInformation("正在刪除用戶 {UserId} 的銷售資料", userId);

            var salesProfile = await _context.MemberSalesProfiles
                .FirstOrDefaultAsync(sp => sp.User_Id == userId);

            if (salesProfile == null)
            {
                _logger.LogWarning("用戶 {UserId} 的銷售資料不存在，無法刪除", userId);
                return false;
            }

            _context.MemberSalesProfiles.Remove(salesProfile);
            await _context.SaveChangesAsync();

            _logger.LogInformation("成功刪除用戶 {UserId} 的銷售資料", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除用戶 {UserId} 銷售資料時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 檢查銀行帳號是否已存在
    /// </summary>
    /// <param name="bankCode">銀行代號</param>
    /// <param name="bankAccountNumber">銀行帳號</param>
    /// <param name="excludeUserId">排除的用戶編號 (用於更新時檢查)</param>
    /// <returns>是否已存在</returns>
    public async Task<bool> IsBankAccountExistsAsync(int bankCode, string bankAccountNumber, int? excludeUserId = null)
    {
        try
        {
            _logger.LogInformation("檢查銀行帳號是否存在: 銀行代號 {BankCode}, 帳號 {BankAccount}", bankCode, bankAccountNumber);

            var query = _context.MemberSalesProfiles
                .Where(sp => sp.BankCode == bankCode && sp.BankAccountNumber == bankAccountNumber);

            if (excludeUserId.HasValue)
            {
                query = query.Where(sp => sp.User_Id != excludeUserId.Value);
            }

            var exists = await query.AnyAsync();

            _logger.LogInformation("銀行帳號存在檢查結果: {Exists}", exists);
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查銀行帳號是否存在時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 取得所有銷售資料列表 (管理者用)
    /// </summary>
    /// <returns>銷售資料列表</returns>
    public async Task<List<MemberSalesProfile>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("正在查詢所有銷售資料");

            var salesProfiles = await _context.MemberSalesProfiles
                .Include(sp => sp.User)
                .OrderBy(sp => sp.User_Id)
                .ToListAsync();

            _logger.LogInformation("查詢到 {Count} 筆銷售資料", salesProfiles.Count);
            return salesProfiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢所有銷售資料時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 根據銀行代號查詢銷售資料
    /// </summary>
    /// <param name="bankCode">銀行代號</param>
    /// <returns>該銀行的銷售資料列表</returns>
    public async Task<List<MemberSalesProfile>> GetByBankCodeAsync(int bankCode)
    {
        try
        {
            _logger.LogInformation("正在查詢銀行代號 {BankCode} 的銷售資料", bankCode);

            var salesProfiles = await _context.MemberSalesProfiles
                .Include(sp => sp.User)
                .Where(sp => sp.BankCode == bankCode)
                .OrderBy(sp => sp.User_Id)
                .ToListAsync();

            _logger.LogInformation("查詢到銀行代號 {BankCode} 的 {Count} 筆銷售資料", bankCode, salesProfiles.Count);
            return salesProfiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢銀行代號 {BankCode} 銷售資料時發生錯誤", bankCode);
            throw;
        }
    }
}
using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// 銷售服務實作，處理銷售相關的業務邏輯
/// </summary>
public class SalesService : ISalesService
{
    private readonly IMemberSalesProfileRepository _salesProfileRepository;
    private readonly IUserSalesInformationRepository _salesInformationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SalesService> _logger;

    /// <summary>
    /// 建構函式，注入相依服務
    /// </summary>
    /// <param name="salesProfileRepository">銷售資料倉儲</param>
    /// <param name="salesInformationRepository">銷售資訊倉儲</param>
    /// <param name="userRepository">用戶倉儲</param>
    /// <param name="logger">日誌記錄器</param>
    public SalesService(
        IMemberSalesProfileRepository salesProfileRepository,
        IUserSalesInformationRepository salesInformationRepository,
        IUserRepository userRepository,
        ILogger<SalesService> logger)
    {
        _salesProfileRepository = salesProfileRepository;
        _salesInformationRepository = salesInformationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// 申請銷售功能
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="application">申請資料</param>
    /// <returns>是否申請成功</returns>
    public async Task<bool> ApplySalesProfileAsync(int userId, SalesProfileApplicationDto application)
    {
        try
        {
            _logger.LogInformation("用戶 {UserId} 正在申請銷售功能，銀行代號: {BankCode}，帳號: {BankAccount}", 
                userId, application.BankCode, application.BankAccountNumber);

            // 檢查用戶是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("用戶 {UserId} 不存在", userId);
                return false;
            }

            // 檢查是否已有銷售資料
            var existingProfile = await _salesProfileRepository.GetByUserIdAsync(userId);
            if (existingProfile != null)
            {
                _logger.LogWarning("用戶 {UserId} 已有銷售資料，無法重複申請", userId);
                return false;
            }

            // 檢查銀行帳號是否已被使用
            var bankAccountExists = await _salesProfileRepository.IsBankAccountExistsAsync(
                application.BankCode, application.BankAccountNumber);
            if (bankAccountExists)
            {
                _logger.LogWarning("銀行帳號 {BankCode}-{BankAccount} 已被使用", 
                    application.BankCode, application.BankAccountNumber);
                return false;
            }

            // 建立銷售資料
            var salesProfile = new MemberSalesProfile
            {
                User_Id = userId,
                BankCode = application.BankCode,
                BankAccountNumber = application.BankAccountNumber,
                AccountCoverPhoto = !string.IsNullOrEmpty(application.AccountCoverPhotoBase64) 
                    ? Convert.FromBase64String(application.AccountCoverPhotoBase64) 
                    : null
            };

            await _salesProfileRepository.CreateAsync(salesProfile);

            // 建立銷售錢包資訊
            var salesInfo = new UserSalesInformation
            {
                User_Id = userId,
                UserSales_Wallet = 0 // 初始餘額為 0
            };

            await _salesInformationRepository.CreateAsync(salesInfo);

            _logger.LogInformation("用戶 {UserId} 成功申請銷售功能", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用戶 {UserId} 申請銷售功能時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 取得銷售資料
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>銷售資料，無資料時返回 null</returns>
    public async Task<SalesProfileDto?> GetSalesProfileAsync(int userId)
    {
        try
        {
            _logger.LogInformation("正在查詢用戶 {UserId} 的銷售資料", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("用戶 {UserId} 不存在", userId);
                return null;
            }

            var salesProfile = await _salesProfileRepository.GetByUserIdAsync(userId);
            var userRights = user.UserRights;

            var profile = new SalesProfileDto
            {
                User_Id = userId,
                BankCode = salesProfile?.BankCode,
                BankAccountNumberMasked = MaskBankAccountNumber(salesProfile?.BankAccountNumber),
                AccountCoverPhotoBase64 = salesProfile?.AccountCoverPhoto != null 
                    ? Convert.ToBase64String(salesProfile.AccountCoverPhoto) 
                    : null,
                HasSalesAuthority = userRights?.SalesAuthority == true,
                ApplicationStatus = GetApplicationStatus(salesProfile, userRights),
                ApplicationTime = DateTime.UtcNow // TODO: 從實際申請時間取得
            };

            _logger.LogInformation("用戶 {UserId} 的銷售資料查詢完成，銷售權限: {HasSalesAuthority}", 
                userId, profile.HasSalesAuthority);

            return profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢用戶 {UserId} 銷售資料時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 更新銷售資料
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="application">更新資料</param>
    /// <returns>是否更新成功</returns>
    public async Task<bool> UpdateSalesProfileAsync(int userId, SalesProfileApplicationDto application)
    {
        try
        {
            _logger.LogInformation("用戶 {UserId} 正在更新銷售資料", userId);

            var salesProfile = await _salesProfileRepository.GetByUserIdAsync(userId);
            if (salesProfile == null)
            {
                _logger.LogWarning("用戶 {UserId} 的銷售資料不存在，無法更新", userId);
                return false;
            }

            // 檢查新銀行帳號是否已被其他用戶使用
            var bankAccountExists = await _salesProfileRepository.IsBankAccountExistsAsync(
                application.BankCode, application.BankAccountNumber, userId);
            if (bankAccountExists)
            {
                _logger.LogWarning("銀行帳號 {BankCode}-{BankAccount} 已被其他用戶使用", 
                    application.BankCode, application.BankAccountNumber);
                return false;
            }

            // 更新銷售資料
            salesProfile.BankCode = application.BankCode;
            salesProfile.BankAccountNumber = application.BankAccountNumber;
            if (!string.IsNullOrEmpty(application.AccountCoverPhotoBase64))
            {
                salesProfile.AccountCoverPhoto = Convert.FromBase64String(application.AccountCoverPhotoBase64);
            }

            await _salesProfileRepository.UpdateAsync(salesProfile);

            _logger.LogInformation("用戶 {UserId} 成功更新銷售資料", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "用戶 {UserId} 更新銷售資料時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 取得銷售錢包資訊
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>銷售錢包資訊</returns>
    public async Task<SalesWalletDto?> GetSalesWalletAsync(int userId)
    {
        try
        {
            _logger.LogInformation("正在查詢用戶 {UserId} 的銷售錢包", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("用戶 {UserId} 不存在", userId);
                return null;
            }

            var salesInfo = await _salesInformationRepository.GetByUserIdAsync(userId);
            var hasSalesAuthority = user.UserRights?.SalesAuthority == true;

            var wallet = new SalesWalletDto
            {
                User_Id = userId,
                UserSales_Wallet = salesInfo?.UserSales_Wallet ?? 0,
                HasSalesAuthority = hasSalesAuthority
            };

            _logger.LogInformation("用戶 {UserId} 的銷售錢包餘額: {Balance}，銷售權限: {HasSalesAuthority}", 
                userId, wallet.UserSales_Wallet, wallet.HasSalesAuthority);

            return wallet;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢用戶 {UserId} 銷售錢包時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 銷售錢包交易 (增加收入)
    /// </summary>
    /// <param name="transaction">交易資料</param>
    /// <returns>是否交易成功</returns>
    public async Task<bool> AddSalesRevenueAsync(SalesWalletTransactionDto transaction)
    {
        try
        {
            _logger.LogInformation("正在為用戶 {UserId} 增加銷售收入 {Amount}，類型: {Type}", 
                transaction.UserId, transaction.Amount, transaction.Type);

            if (transaction.Amount <= 0)
            {
                _logger.LogWarning("銷售收入金額 {Amount} 必須大於 0", transaction.Amount);
                return false;
            }

            var success = await _salesInformationRepository.AddToSalesWalletAsync(transaction.UserId, transaction.Amount);
            if (success)
            {
                _logger.LogInformation("成功為用戶 {UserId} 增加銷售收入 {Amount}，備註: {Note}", 
                    transaction.UserId, transaction.Amount, transaction.Note);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "為用戶 {UserId} 增加銷售收入時發生錯誤", transaction.UserId);
            throw;
        }
    }

    /// <summary>
    /// 銷售錢包交易 (扣除費用)
    /// </summary>
    /// <param name="transaction">交易資料</param>
    /// <returns>是否交易成功 (餘額不足會返回 false)</returns>
    public async Task<bool> DeductSalesAmountAsync(SalesWalletTransactionDto transaction)
    {
        try
        {
            _logger.LogInformation("正在為用戶 {UserId} 扣除銷售金額 {Amount}，類型: {Type}", 
                transaction.UserId, transaction.Amount, transaction.Type);

            if (transaction.Amount <= 0)
            {
                _logger.LogWarning("銷售扣除金額 {Amount} 必須大於 0", transaction.Amount);
                return false;
            }

            // 檢查餘額是否足夠
            var hasEnough = await HasSufficientSalesBalanceAsync(transaction.UserId, transaction.Amount);
            if (!hasEnough)
            {
                _logger.LogWarning("用戶 {UserId} 的銷售錢包餘額不足，無法扣除 {Amount}", 
                    transaction.UserId, transaction.Amount);
                return false;
            }

            var success = await _salesInformationRepository.DeductFromSalesWalletAsync(transaction.UserId, transaction.Amount);
            if (success)
            {
                _logger.LogInformation("成功為用戶 {UserId} 扣除銷售金額 {Amount}，備註: {Note}", 
                    transaction.UserId, transaction.Amount, transaction.Note);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "為用戶 {UserId} 扣除銷售金額時發生錯誤", transaction.UserId);
            throw;
        }
    }

    /// <summary>
    /// 管理者審核銷售申請
    /// </summary>
    /// <param name="adminUserId">管理者編號</param>
    /// <param name="userId">申請用戶編號</param>
    /// <param name="approved">是否核准</param>
    /// <param name="reason">審核理由</param>
    /// <returns>是否審核成功</returns>
    public async Task<bool> ReviewSalesApplicationAsync(int adminUserId, int userId, bool approved, string reason)
    {
        try
        {
            _logger.LogInformation("管理者 {AdminUserId} 正在審核用戶 {UserId} 的銷售申請，結果: {Approved}，理由: {Reason}", 
                adminUserId, userId, approved, reason);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("申請用戶 {UserId} 不存在", userId);
                return false;
            }

            var salesProfile = await _salesProfileRepository.GetByUserIdAsync(userId);
            if (salesProfile == null)
            {
                _logger.LogWarning("用戶 {UserId} 沒有銷售申請資料", userId);
                return false;
            }

            // 更新用戶權限
            if (user.UserRights != null)
            {
                user.UserRights.SalesAuthority = approved;
                await _userRepository.UpdateAsync(user);
            }

            _logger.LogInformation("管理者 {AdminUserId} 成功審核用戶 {UserId} 的銷售申請，結果: {Approved}", 
                adminUserId, userId, approved);

            // TODO: 發送通知給申請用戶
            // await _notificationService.SendSalesApplicationReviewNotificationAsync(userId, approved, reason);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "管理者 {AdminUserId} 審核用戶 {UserId} 銷售申請時發生錯誤", 
                adminUserId, userId);
            throw;
        }
    }

    /// <summary>
    /// 檢查用戶是否有銷售權限
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <returns>是否有銷售權限</returns>
    public async Task<bool> HasSalesAuthorityAsync(int userId)
    {
        try
        {
            _logger.LogInformation("正在檢查用戶 {UserId} 是否有銷售權限", userId);

            var user = await _userRepository.GetByIdAsync(userId);
            var hasSalesAuthority = user?.UserRights?.SalesAuthority == true;

            _logger.LogInformation("用戶 {UserId} 的銷售權限: {HasSalesAuthority}", userId, hasSalesAuthority);
            return hasSalesAuthority;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查用戶 {UserId} 銷售權限時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 檢查銷售錢包是否有足夠餘額
    /// </summary>
    /// <param name="userId">用戶編號</param>
    /// <param name="requiredAmount">需要的金額</param>
    /// <returns>是否有足夠餘額</returns>
    public async Task<bool> HasSufficientSalesBalanceAsync(int userId, int requiredAmount)
    {
        try
        {
            _logger.LogInformation("正在檢查用戶 {UserId} 銷售錢包是否有足夠餘額 {RequiredAmount}", 
                userId, requiredAmount);

            var currentBalance = await _salesInformationRepository.GetSalesWalletBalanceAsync(userId);
            var hasEnough = currentBalance >= requiredAmount;

            _logger.LogInformation("用戶 {UserId} 目前銷售餘額: {CurrentBalance}，需要金額: {RequiredAmount}，足夠: {HasEnough}", 
                userId, currentBalance, requiredAmount, hasEnough);

            return hasEnough;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "檢查用戶 {UserId} 銷售錢包餘額時發生錯誤", userId);
            throw;
        }
    }

    /// <summary>
    /// 取得待審核的銷售申請列表 (管理者用)
    /// </summary>
    /// <returns>待審核申請列表</returns>
    public async Task<List<SalesProfileDto>> GetPendingApplicationsAsync()
    {
        try
        {
            _logger.LogInformation("正在查詢待審核的銷售申請");

            var allProfiles = await _salesProfileRepository.GetAllAsync();
            var pendingApplications = new List<SalesProfileDto>();

            foreach (var profile in allProfiles)
            {
                var user = await _userRepository.GetByIdAsync(profile.User_Id);
                var hasAuthority = user?.UserRights?.SalesAuthority == true;

                // 有銷售資料但沒有權限的視為待審核
                if (!hasAuthority)
                {
                    pendingApplications.Add(new SalesProfileDto
                    {
                        User_Id = profile.User_Id,
                        BankCode = profile.BankCode,
                        BankAccountNumberMasked = MaskBankAccountNumber(profile.BankAccountNumber),
                        AccountCoverPhotoBase64 = profile.AccountCoverPhoto != null 
                            ? Convert.ToBase64String(profile.AccountCoverPhoto) 
                            : null,
                        HasSalesAuthority = false,
                        ApplicationStatus = "pending",
                        ApplicationTime = DateTime.UtcNow // TODO: 從實際申請時間取得
                    });
                }
            }

            _logger.LogInformation("查詢到 {Count} 筆待審核的銷售申請", pendingApplications.Count);
            return pendingApplications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查詢待審核銷售申請時發生錯誤");
            throw;
        }
    }

    /// <summary>
    /// 遮蔽銀行帳號顯示
    /// </summary>
    /// <param name="bankAccountNumber">銀行帳號</param>
    /// <returns>遮蔽後的帳號</returns>
    private static string? MaskBankAccountNumber(string? bankAccountNumber)
    {
        if (string.IsNullOrEmpty(bankAccountNumber))
            return null;

        if (bankAccountNumber.Length <= 4)
            return bankAccountNumber;

        var visibleLength = 3;
        var maskedLength = bankAccountNumber.Length - visibleLength - 3;
        var masked = new string('*', Math.Max(maskedLength, 3));

        return bankAccountNumber[..visibleLength] + masked + bankAccountNumber[^3..];
    }

    /// <summary>
    /// 取得申請狀態
    /// </summary>
    /// <param name="salesProfile">銷售資料</param>
    /// <param name="userRights">用戶權限</param>
    /// <returns>申請狀態</returns>
    private static string GetApplicationStatus(MemberSalesProfile? salesProfile, UserRights? userRights)
    {
        if (salesProfile == null)
            return "none";

        if (userRights?.SalesAuthority == true)
            return "approved";

        return "pending";
    }
}
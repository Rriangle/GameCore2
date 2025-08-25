using GameCore.Api.DTOs;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// 假資料生成服務接口
/// </summary>
public interface IFakeDataService
{
    /// <summary>
    /// 生成假用戶資料
    /// </summary>
    /// <param name="count">生成數量</param>
    /// <returns>生成的用戶數量</returns>
    Task<int> GenerateFakeUsersAsync(int count);

    /// <summary>
    /// 獲取假資料統計
    /// </summary>
    /// <returns>假資料統計資訊</returns>
    Task<FakeDataStatsDto> GetFakeDataStatsAsync();

    /// <summary>
    /// 清理假資料
    /// </summary>
    /// <returns>清理的資料數量</returns>
    Task<int> CleanupFakeDataAsync();

    #region 假資料生成輔助方法

    /// <summary>
    /// 生成假姓名
    /// </summary>
    /// <returns>假姓名</returns>
    string GenerateFakeName();

    /// <summary>
    /// 生成假帳號
    /// </summary>
    /// <returns>假帳號</returns>
    string GenerateFakeAccount();

    /// <summary>
    /// 生成假電子郵件
    /// </summary>
    /// <returns>假電子郵件</returns>
    string GenerateFakeEmail();

    /// <summary>
    /// 生成假暱稱
    /// </summary>
    /// <returns>假暱稱</returns>
    string GenerateFakeNickname();

    /// <summary>
    /// 生成假身分證號
    /// </summary>
    /// <returns>假身分證號</returns>
    string GenerateFakeIdNumber();

    /// <summary>
    /// 生成假手機號碼
    /// </summary>
    /// <returns>假手機號碼</returns>
    string GenerateFakeCellphone();

    /// <summary>
    /// 生成假地址
    /// </summary>
    /// <returns>假地址</returns>
    string GenerateFakeAddress();

    /// <summary>
    /// 生成假用戶介紹
    /// </summary>
    /// <returns>假用戶介紹</returns>
    string GenerateFakeUserIntroduce();

    /// <summary>
    /// 生成假優惠券號碼
    /// </summary>
    /// <returns>假優惠券號碼</returns>
    string GenerateFakeCouponNumber();

    /// <summary>
    /// 生成假銀行帳號
    /// </summary>
    /// <returns>假銀行帳號</returns>
    string GenerateFakeBankAccountNumber();

    /// <summary>
    /// 生成假審核備註
    /// </summary>
    /// <returns>假審核備註</returns>
    string GenerateFakeReviewNotes();

    #endregion
}
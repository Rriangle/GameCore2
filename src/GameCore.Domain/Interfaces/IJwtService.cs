using System.Security.Claims;

namespace GameCore.Domain.Interfaces;

/// <summary>
/// JWT 權杖服務介面
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// 產生存取權杖
    /// </summary>
    /// <param name="userId">使用者編號</param>
    /// <param name="userAccount">登入帳號</param>
    /// <param name="additionalClaims">額外的聲明</param>
    /// <returns>JWT 權杖</returns>
    string GenerateAccessToken(int userId, string userAccount, IEnumerable<Claim>? additionalClaims = null);

    /// <summary>
    /// 產生重新整理權杖
    /// </summary>
    /// <returns>重新整理權杖</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// 驗證權杖
    /// </summary>
    /// <param name="token">JWT 權杖</param>
    /// <returns>驗證結果與聲明</returns>
    (bool IsValid, ClaimsPrincipal? Principal) ValidateToken(string token);

    /// <summary>
    /// 從權杖中取得使用者編號
    /// </summary>
    /// <param name="token">JWT 權杖</param>
    /// <returns>使用者編號</returns>
    int? GetUserIdFromToken(string token);

    /// <summary>
    /// 檢查權杖是否即將過期
    /// </summary>
    /// <param name="token">JWT 權杖</param>
    /// <param name="minutesBeforeExpiry">過期前幾分鐘</param>
    /// <returns>是否即將過期</returns>
    bool IsTokenExpiringSoon(string token, int minutesBeforeExpiry = 15);
}
using GameCore.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GameCore.Infrastructure.Services;

/// <summary>
/// JWT 權杖服務實作類別，處理權杖的產生、驗證和刷新
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpiryMinutes;
    private readonly int _refreshTokenExpiryDays;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = _configuration["Jwt:SecretKey"] ?? "GameCore_Super_Secret_Key_2024_Must_Be_At_Least_32_Characters_Long";
        _issuer = _configuration["Jwt:Issuer"] ?? "GameCore";
        _audience = _configuration["Jwt:Audience"] ?? "GameCore_Users";
        _accessTokenExpiryMinutes = int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "1440"); // 24小時
        _refreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "30"); // 30天
    }

    /// <summary>
    /// 產生存取權杖
    /// </summary>
    public string GenerateAccessToken(int userId, string userAccount, IEnumerable<Claim>? additionalClaims = null)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, userAccount),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64),
            new Claim("user_id", userId.ToString()),
            new Claim("user_account", userAccount),
            new Claim("token_type", "access")
        };

        // 加入額外的聲明
        if (additionalClaims != null)
        {
            claims.AddRange(additionalClaims);
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 產生重新整理權杖
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        var refreshToken = Convert.ToBase64String(randomBytes);
        
        // 為了驗證目的，我們也將 refresh token 做成 JWT 格式
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, 
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                ClaimValueTypes.Integer64),
            new Claim("token_type", "refresh"),
            new Claim("refresh_id", refreshToken)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 驗證權杖
    /// </summary>
    public (bool IsValid, ClaimsPrincipal? Principal) ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // 不允許時間偏移
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            // 確保是 JWT 權杖
            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return (false, null);
            }

            return (true, principal);
        }
        catch (Exception)
        {
            return (false, null);
        }
    }

    /// <summary>
    /// 從權杖中提取用戶編號
    /// </summary>
    public int? GetUserIdFromToken(string token)
    {
        var (isValid, principal) = ValidateToken(token);
        
        if (!isValid || principal == null)
        {
            return null;
        }

        var userIdClaim = principal.FindFirst("user_id")?.Value ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        
        if (int.TryParse(userIdClaim, out int userId))
        {
            return userId;
        }

        return null;
    }

    /// <summary>
    /// 從權杖中提取用戶帳號
    /// </summary>
    public string? GetUserAccountFromToken(string token)
    {
        var (isValid, principal) = ValidateToken(token);
        
        if (!isValid || principal == null)
        {
            return null;
        }

        return principal.FindFirst("user_account")?.Value ?? principal.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
    }

    /// <summary>
    /// 檢查權杖是否即將過期
    /// </summary>
    public bool IsTokenExpiringSoon(string token, int minutesBeforeExpiry = 15)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            
            var expiryTime = jsonToken.ValidTo;
            var timeRemaining = expiryTime - DateTime.UtcNow;
            
            return timeRemaining.TotalMinutes <= minutesBeforeExpiry;
        }
        catch
        {
            return true; // 如果無法讀取，視為即將過期
        }
    }

    /// <summary>
    /// 取得權杖過期時間
    /// </summary>
    public DateTime? GetTokenExpiry(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            
            return jsonToken.ValidTo;
        }
        catch
        {
            return null;
        }
    }
}
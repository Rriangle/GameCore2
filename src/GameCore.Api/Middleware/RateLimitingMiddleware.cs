using System.Collections.Concurrent;
using System.Net;

namespace GameCore.Api.Middleware;

/// <summary>
/// Rate Limiting 中介軟體，防止暴力破解攻擊
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimitStore;
    private readonly int _maxRequestsPerMinute;
    private readonly int _maxRequestsPerHour;

    public RateLimitingMiddleware(
        RequestDelegate next, 
        ILogger<RateLimitingMiddleware> logger,
        int maxRequestsPerMinute = 60,
        int maxRequestsPerHour = 1000)
    {
        _next = next;
        _logger = logger;
        _rateLimitStore = new ConcurrentDictionary<string, RateLimitInfo>();
        _maxRequestsPerMinute = maxRequestsPerMinute;
        _maxRequestsPerHour = maxRequestsPerHour;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = GetClientIpAddress(context);
        var endpoint = context.Request.Path.Value;

        // 只對認證端點進行 Rate Limiting
        if (IsAuthEndpoint(endpoint))
        {
            if (!IsRateLimitAllowed(clientIp, endpoint))
            {
                _logger.LogWarning("Rate limit exceeded for IP: {ClientIp} on endpoint: {Endpoint}", clientIp, endpoint);
                
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";
                
                var response = new
                {
                    success = false,
                    message = "請求過於頻繁，請稍後再試",
                    retryAfter = GetRetryAfterSeconds(clientIp, endpoint)
                };

                await context.Response.WriteAsJsonAsync(response);
                return;
            }
        }

        await _next(context);
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // 檢查 X-Forwarded-For 標頭（用於代理伺服器）
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // 檢查 X-Real-IP 標頭
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // 使用遠端 IP 地址
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private bool IsAuthEndpoint(string? endpoint)
    {
        return endpoint != null && (
            endpoint.StartsWith("/api/auth/login", StringComparison.OrdinalIgnoreCase) ||
            endpoint.StartsWith("/api/auth/register", StringComparison.OrdinalIgnoreCase)
        );
    }

    private bool IsRateLimitAllowed(string clientIp, string endpoint)
    {
        var key = $"{clientIp}:{endpoint}";
        var now = DateTime.UtcNow;

        var rateLimitInfo = _rateLimitStore.GetOrAdd(key, _ => new RateLimitInfo());

        // 清理過期的請求記錄
        rateLimitInfo.CleanupExpiredRequests(now);

        // 檢查分鐘限制
        if (rateLimitInfo.RequestsPerMinute.Count >= _maxRequestsPerMinute)
        {
            return false;
        }

        // 檢查小時限制
        if (rateLimitInfo.RequestsPerHour.Count >= _maxRequestsPerHour)
        {
            return false;
        }

        // 記錄新請求
        rateLimitInfo.RequestsPerMinute.Add(now);
        rateLimitInfo.RequestsPerHour.Add(now);

        return true;
    }

    private int GetRetryAfterSeconds(string clientIp, string endpoint)
    {
        var key = $"{clientIp}:{endpoint}";
        if (_rateLimitStore.TryGetValue(key, out var rateLimitInfo))
        {
            var oldestMinuteRequest = rateLimitInfo.RequestsPerMinute.FirstOrDefault();
            if (oldestMinuteRequest != default)
            {
                var secondsUntilReset = 60 - (int)(DateTime.UtcNow - oldestMinuteRequest).TotalSeconds;
                return Math.Max(1, secondsUntilReset);
            }
        }
        return 60; // 預設 1 分鐘
    }
}

/// <summary>
/// Rate Limit 資訊
/// </summary>
public class RateLimitInfo
{
    public List<DateTime> RequestsPerMinute { get; set; } = new();
    public List<DateTime> RequestsPerHour { get; set; } = new();

    public void CleanupExpiredRequests(DateTime now)
    {
        // 清理超過 1 分鐘的請求
        RequestsPerMinute.RemoveAll(time => (now - time).TotalMinutes >= 1);
        
        // 清理超過 1 小時的請求
        RequestsPerHour.RemoveAll(time => (now - time).TotalHours >= 1);
    }
}

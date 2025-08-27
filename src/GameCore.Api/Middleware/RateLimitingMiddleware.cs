using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace GameCore.Api.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        public RateLimitingMiddleware(
            RequestDelegate next,
            IMemoryCache cache,
            ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientId(context);
            var endpoint = context.Request.Path.Value;

            // 檢查速率限制
            if (!await CheckRateLimitAsync(clientId, endpoint))
            {
                _logger.LogWarning("速率限制觸發: {ClientId}, {Endpoint}", clientId, endpoint);

                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    error = "請求過於頻繁，請稍後再試",
                    retryAfter = 60
                };

                await context.Response.WriteAsJsonAsync(response);
                return;
            }

            await _next(context);
        }

        private string GetClientId(HttpContext context)
        {
            // 優先使用用戶 ID（如果已認證）
            var userId = context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
                return $"user_{userId}";

            // 否則使用 IP 地址
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return $"ip_{ipAddress}";
        }

        private async Task<bool> CheckRateLimitAsync(string clientId, string? endpoint)
        {
            var cacheKey = $"rate_limit_{clientId}_{endpoint}";
            var limit = GetRateLimit(endpoint);
            var window = TimeSpan.FromMinutes(1);

            var requestCount = await _cache.GetOrCreateAsync(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = window;
                return Task.FromResult(0);
            });

            if (requestCount >= limit)
                return false;

            _cache.Set(cacheKey, requestCount + 1, window);
            return true;
        }

        private int GetRateLimit(string? endpoint)
        {
            // 根據端點類型設定不同的速率限制
            if (endpoint?.StartsWith("/api/auth/") == true)
                return 10; // 認證端點：每分鐘 10 次
            else if (endpoint?.StartsWith("/api/") == true)
                return 100; // 一般 API：每分鐘 100 次
            else
                return 1000; // 其他端點：每分鐘 1000 次
        }
    }

    public static class RateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }
    }
}

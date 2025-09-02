namespace GameCore.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        // 效能優化：只在 Debug 模式下記錄詳細請求日誌，減少生產環境日誌量
        _logger.LogDebug("開始處理請求: {Method} {Path}", requestMethod, requestPath);

        try
        {
            await _next(context);
        }
        finally
        {
            var duration = DateTime.UtcNow - startTime;
            var statusCode = context.Response.StatusCode;

            // 效能優化：根據狀態碼和持續時間調整日誌等級
            // 成功請求使用 Debug 等級，錯誤請求使用 Warning 等級，慢請求使用 Information 等級
            var logLevel = statusCode >= 400 ? LogLevel.Warning : 
                          duration.TotalMilliseconds > 1000 ? LogLevel.Information : 
                          LogLevel.Debug;

            _logger.Log(logLevel,
                "請求完成: {Method} {Path} - {StatusCode} ({Duration}ms)",
                requestMethod, requestPath, statusCode, duration.TotalMilliseconds);
        }
    }
}

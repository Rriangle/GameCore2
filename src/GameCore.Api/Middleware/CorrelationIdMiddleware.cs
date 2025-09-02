namespace GameCore.Api.Middleware;

/// <summary>
/// CorrelationId 中介軟體，用於追蹤請求流程
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        // 設定 CorrelationId 到 HttpContext 項目中
        context.Items["CorrelationId"] = correlationId;

        // 設定 CorrelationId 到回應標頭中
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        // 使用結構化日誌記錄請求開始
        _logger.LogInformation(
            "Request started: {Method} {Path} from {IpAddress} with CorrelationId: {CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            GetClientIpAddress(context),
            correlationId
        );

        try
        {
            await _next(context);
        }
        finally
        {
            // 記錄請求完成
            _logger.LogInformation(
                "Request completed: {Method} {Path} with status {StatusCode} and CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                correlationId
            );
        }
    }

    private string GetOrCreateCorrelationId(HttpContext context)
    {
        // 檢查請求標頭中是否已有 CorrelationId
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var existingCorrelationId))
        {
            return existingCorrelationId.ToString();
        }

        // 生成新的 CorrelationId
        return Guid.NewGuid().ToString();
    }

    private string GetClientIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

/// <summary>
/// CorrelationId 擴展方法
/// </summary>
public static class CorrelationIdExtensions
{
    public static string? GetCorrelationId(this HttpContext context)
    {
        return context.Items["CorrelationId"]?.ToString();
    }
}

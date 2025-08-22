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

        _logger.LogInformation("開始處理請求: {Method} {Path}", requestMethod, requestPath);

        try
        {
            await _next(context);
        }
        finally
        {
            var duration = DateTime.UtcNow - startTime;
            var statusCode = context.Response.StatusCode;

            _logger.LogInformation(
                "請求完成: {Method} {Path} - {StatusCode} ({Duration}ms)",
                requestMethod, requestPath, statusCode, duration.TotalMilliseconds);
        }
    }
}

namespace GameCore.Api.Extensions;

/// <summary>
/// HttpContext 擴展方法
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// 獲取當前請求的 CorrelationId
    /// </summary>
    /// <param name="context">HttpContext</param>
    /// <returns>CorrelationId</returns>
    public static string? GetCorrelationId(this HttpContext context)
    {
        return context.Items["CorrelationId"]?.ToString();
    }
}

using System.Text.Json;
using Microsoft.Extensions.Logging;
using GameCore.Shared.DTOs;

namespace GameCore.Api.Middleware;

/// <summary>
/// 響應包裝中間件
/// 統一包裝所有 API 響應格式
/// </summary>
public class ResponseWrapperMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseWrapperMiddleware> _logger;

    // 效能優化：靜態 JsonSerializerOptions 避免重複創建，提升序列化效能
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false, // 減少輸出大小
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public ResponseWrapperMiddleware(RequestDelegate next, ILogger<ResponseWrapperMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        catch
        {
            // 如果發生異常，恢復原始響應流並重新拋出
            context.Response.Body = originalBodyStream;
            throw;
        }

        responseBody.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(responseBody).ReadToEndAsync();

        // 檢查是否已經被包裝過
        if (IsAlreadyWrapped(responseContent))
        {
            await CopyResponseAsync(responseBody, originalBodyStream);
            return;
        }

        // 包裝響應 - 使用靜態 JsonSerializerOptions 提升效能
        var wrappedResponse = WrapResponse(responseContent, context.Response.StatusCode);
        var wrappedJson = JsonSerializer.Serialize(wrappedResponse, _jsonOptions);

        context.Response.Body = originalBodyStream;
        context.Response.ContentType = "application/json";
        context.Response.ContentLength = System.Text.Encoding.UTF8.GetByteCount(wrappedJson);

        await context.Response.WriteAsync(wrappedJson);
    }

    private bool IsAlreadyWrapped(string responseContent)
    {
        if (string.IsNullOrEmpty(responseContent))
            return false;

        try
        {
            var doc = JsonDocument.Parse(responseContent);
            return doc.RootElement.TryGetProperty("success", out _) ||
                   doc.RootElement.TryGetProperty("Success", out _);
        }
        catch
        {
            return false;
        }
    }

    private ApiResponse<object> WrapResponse(string responseContent, int statusCode)
    {
        if (string.IsNullOrEmpty(responseContent))
        {
            return statusCode >= 200 && statusCode < 300
                ? ApiResponse<object>.SuccessResult(new object(), "操作成功")
                : ApiResponse<object>.ErrorResult("請求失敗");
        }

        try
        {
            var data = JsonSerializer.Deserialize<object>(responseContent);
            
            if (statusCode >= 200 && statusCode < 300)
            {
                return ApiResponse<object>.SuccessResult(data ?? new object());
            }
            else
            {
                // 嘗試提取錯誤訊息
                var errorMessage = ExtractErrorMessage(responseContent);
                return ApiResponse<object>.ErrorResult(errorMessage ?? "請求失敗");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "包裝響應時發生錯誤: {ResponseContent}", responseContent);
            return ApiResponse<object>.ErrorResult("響應格式錯誤");
        }
    }

    private string? ExtractErrorMessage(string responseContent)
    {
        try
        {
            var doc = JsonDocument.Parse(responseContent);
            
            // 嘗試不同的錯誤訊息字段
            if (doc.RootElement.TryGetProperty("message", out var messageElement))
                return messageElement.GetString();
            
            if (doc.RootElement.TryGetProperty("Message", out var messageElement2))
                return messageElement2.GetString();

            if (doc.RootElement.TryGetProperty("error", out var errorElement))
                return errorElement.GetString();

            if (doc.RootElement.TryGetProperty("Error", out var errorElement2))
                return errorElement2.GetString();

            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task CopyResponseAsync(MemoryStream responseBody, Stream originalBodyStream)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }
}
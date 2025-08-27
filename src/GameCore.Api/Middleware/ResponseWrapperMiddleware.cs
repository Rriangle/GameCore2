using GameCore.Api.Models;
using GameCore.Shared.DTOs;
using System.Text.Json;

namespace GameCore.Api.Middleware;

/// <summary>
/// API響應包裝中間件，將所有響應統一為ApiResponse格式
/// </summary>
public class ResponseWrapperMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseWrapperMiddleware> _logger;

    public ResponseWrapperMiddleware(RequestDelegate next, ILogger<ResponseWrapperMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 跳過非API請求
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.Body = responseBody;
        responseBody.Seek(0, SeekOrigin.Begin);

        var responseContent = await new StreamReader(responseBody).ReadToEndAsync();

        // 如果響應已經是ApiResponse格式，直接返回
        if (IsAlreadyWrapped(responseContent))
        {
            await CopyResponseAsync(responseBody, originalBodyStream);
            return;
        }

        // 包裝響應
        var wrappedResponse = WrapResponse(responseContent, context.Response.StatusCode);
        var wrappedJson = JsonSerializer.Serialize(wrappedResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

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
                ? ApiResponse<object>.SuccessResponse(null)
                : ApiResponse<object>.ErrorResponse("請求失敗");
        }

        try
        {
            var data = JsonSerializer.Deserialize<object>(responseContent);
            
            if (statusCode >= 200 && statusCode < 300)
            {
                return ApiResponse<object>.SuccessResponse(data);
            }
            else
            {
                // 嘗試提取錯誤訊息
                var errorMessage = ExtractErrorMessage(responseContent);
                return ApiResponse<object>.ErrorResponse(errorMessage ?? "請求失敗");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "包裝響應時發生錯誤: {ResponseContent}", responseContent);
            return ApiResponse<object>.ErrorResponse("響應格式錯誤");
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
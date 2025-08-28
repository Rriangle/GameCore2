namespace GameCore.Api.Models;

/// <summary>
/// 統一的API響應格式
/// </summary>
/// <typeparam name="T">響應數據類型</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResponse(string error, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error,
            Message = message
        };
    }
}

/// <summary>
/// 無數據的API響應格式
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse SuccessResponse(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    public static new ApiResponse ErrorResponse(string error, string? message = null)
    {
        return new ApiResponse
        {
            Success = false,
            Error = error,
            Message = message
        };
    }
}
namespace GameCore.Shared.DTOs;

/// <summary>
/// 服務結果泛型類別
/// </summary>
/// <typeparam name="T">結果資料類型</typeparam>
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ServiceResult<T> CreateSuccess(T data) => new() { Success = true, Data = data };
    public static ServiceResult<T> CreateFailure(string message) => new() { Success = false, Message = message };
} 
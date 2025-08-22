using System.ComponentModel.DataAnnotations;

namespace GameCore.Api.DTOs;

/// <summary>
/// 統一 API 回應格式
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResult(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

/// <summary>
/// 分頁回應格式
/// </summary>
public class PagedResponse<T> : ApiResponse<List<T>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public static PagedResponse<T> Create(List<T> data, int page, int pageSize, int totalCount)
    {
        return new PagedResponse<T>
        {
            Success = true,
            Data = data,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }
}

/// <summary>
/// 基礎驗證屬性
/// </summary>
public class PasswordValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return new ValidationResult("密碼不能為空");

        var password = value.ToString()!;
        
        if (password.Length < 8)
            return new ValidationResult("密碼長度至少需要 8 個字元");
        
        if (!password.Any(char.IsUpper))
            return new ValidationResult("密碼需要包含至少一個大寫字母");
        
        if (!password.Any(char.IsLower))
            return new ValidationResult("密碼需要包含至少一個小寫字母");
        
        if (!password.Any(char.IsDigit))
            return new ValidationResult("密碼需要包含至少一個數字");
        
        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            return new ValidationResult("密碼需要包含至少一個特殊字元");

        return ValidationResult.Success;
    }
}

/// <summary>
/// 用戶名驗證屬性
/// </summary>
public class UsernameValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return new ValidationResult("用戶名不能為空");

        var username = value.ToString()!;
        
        if (username.Length < 3 || username.Length > 20)
            return new ValidationResult("用戶名長度必須在 3-20 個字元之間");
        
        if (!username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-'))
            return new ValidationResult("用戶名只能包含字母、數字、底線和連字號");
        
        if (username.StartsWith('-') || username.EndsWith('-') || 
            username.StartsWith('_') || username.EndsWith('_'))
            return new ValidationResult("用戶名不能以底線或連字號開頭或結尾");

        return ValidationResult.Success;
    }
}

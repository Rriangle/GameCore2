namespace GameCore.Shared.DTOs;

/// <summary>
/// 驗證結果
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// 是否驗證成功
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 創建成功結果
    /// </summary>
    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    /// <summary>
    /// 創建失敗結果
    /// </summary>
    public static ValidationResult Failure(string errorMessage)
    {
        return new ValidationResult { IsValid = false, ErrorMessage = errorMessage };
    }
} 
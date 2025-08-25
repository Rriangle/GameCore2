namespace GameCore.Shared.Models;

/// <summary>
/// 驗證結果模型，用於服務層的輸入驗證
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// 驗證是否通過
    /// </summary>
    public bool IsValid { get; private set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// 驗證錯誤列表
    /// </summary>
    public List<string> Errors { get; private set; } = new();

    /// <summary>
    /// 私有建構函數，強制使用靜態方法
    /// </summary>
    private ValidationResult() { }

    /// <summary>
    /// 建立成功的驗證結果
    /// </summary>
    /// <returns>成功的驗證結果</returns>
    public static ValidationResult Success()
    {
        return new ValidationResult
        {
            IsValid = true,
            ErrorMessage = null,
            Errors = new List<string>()
        };
    }

    /// <summary>
    /// 建立失敗的驗證結果
    /// </summary>
    /// <param name="errorMessage">錯誤訊息</param>
    /// <returns>失敗的驗證結果</returns>
    public static ValidationResult Failure(string errorMessage)
    {
        return new ValidationResult
        {
            IsValid = false,
            ErrorMessage = errorMessage,
            Errors = new List<string> { errorMessage }
        };
    }

    /// <summary>
    /// 建立失敗的驗證結果（多個錯誤）
    /// </summary>
    /// <param name="errors">錯誤列表</param>
    /// <returns>失敗的驗證結果</returns>
    public static ValidationResult Failure(List<string> errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            ErrorMessage = errors.FirstOrDefault(),
            Errors = errors
        };
    }

    /// <summary>
    /// 添加錯誤訊息
    /// </summary>
    /// <param name="error">錯誤訊息</param>
    /// <returns>當前驗證結果實例</returns>
    public ValidationResult AddError(string error)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            Errors.Add(error);
            if (string.IsNullOrWhiteSpace(ErrorMessage))
            {
                ErrorMessage = error;
            }
            IsValid = false;
        }
        return this;
    }

    /// <summary>
    /// 合併另一個驗證結果
    /// </summary>
    /// <param name="other">另一個驗證結果</param>
    /// <returns>合併後的驗證結果</returns>
    public ValidationResult Merge(ValidationResult other)
    {
        if (other != null)
        {
            Errors.AddRange(other.Errors);
            if (!other.IsValid)
            {
                IsValid = false;
                if (string.IsNullOrWhiteSpace(ErrorMessage))
                {
                    ErrorMessage = other.ErrorMessage;
                }
            }
        }
        return this;
    }

    /// <summary>
    /// 檢查是否有特定錯誤
    /// </summary>
    /// <param name="error">要檢查的錯誤</param>
    /// <returns>是否包含該錯誤</returns>
    public bool HasError(string error)
    {
        return Errors.Any(e => e.Equals(error, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 獲取所有錯誤的組合訊息
    /// </summary>
    /// <param name="separator">分隔符</param>
    /// <returns>組合後的錯誤訊息</returns>
    public string GetCombinedErrors(string separator = "; ")
    {
        return string.Join(separator, Errors);
    }
}
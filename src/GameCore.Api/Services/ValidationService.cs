using System.Net.Mail;
using System.Text.RegularExpressions;

namespace GameCore.Api.Services;

/// <summary>
/// 統一驗證服務，避免驗證邏輯重複
/// </summary>
public class ValidationService : IValidationService
{
    /// <summary>
    /// 驗證用戶名
    /// </summary>
    public ValidationResult ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return ValidationResult.Failure("用戶名不能為空");

        if (username.Length < 3 || username.Length > 20)
            return ValidationResult.Failure("用戶名長度必須在 3-20 個字符之間");

        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            return ValidationResult.Failure("用戶名只能包含字母、數字和下劃線");

        return ValidationResult.Success();
    }

    /// <summary>
    /// 驗證郵箱
    /// </summary>
    public ValidationResult ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return ValidationResult.Failure("郵箱不能為空");

        try
        {
            var addr = new MailAddress(email);
            if (addr.Address != email)
                return ValidationResult.Failure("郵箱格式不正確");
        }
        catch
        {
            return ValidationResult.Failure("郵箱格式不正確");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// 驗證密碼複雜度
    /// </summary>
    public ValidationResult ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return ValidationResult.Failure("密碼不能為空");

        if (password.Length < 8)
            return ValidationResult.Failure("密碼長度至少 8 個字符");

        var hasUpperCase = password.Any(char.IsUpper);
        var hasLowerCase = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

        if (!hasUpperCase || !hasLowerCase || !hasDigit || !hasSpecialChar)
        {
            if (hasUpperCase && hasLowerCase && hasDigit && !hasSpecialChar)
                return ValidationResult.Failure("密碼必須包含大小寫字母、數字和特殊字符");
            else
                return ValidationResult.Failure("密碼不符合安全要求");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// 驗證價格
    /// </summary>
    public ValidationResult ValidatePrice(decimal price, decimal minPrice = 0.01m, decimal maxPrice = 999999.99m)
    {
        if (price <= 0)
            return ValidationResult.Failure("價格必須大於 0");

        if (price < minPrice)
            return ValidationResult.Failure($"價格不能低於 {minPrice:C}");

        if (price > maxPrice)
            return ValidationResult.Failure($"價格不能高於 {maxPrice:C}");

        return ValidationResult.Success();
    }

    /// <summary>
    /// 驗證數量
    /// </summary>
    public ValidationResult ValidateQuantity(int quantity, int minQuantity = 1, int maxQuantity = 999)
    {
        if (quantity <= 0)
            return ValidationResult.Failure("數量必須大於 0");

        if (quantity < minQuantity)
            return ValidationResult.Failure($"數量不能少於 {minQuantity}");

        if (quantity > maxQuantity)
            return ValidationResult.Failure($"數量不能超過 {maxQuantity}");

        return ValidationResult.Success();
    }

    /// <summary>
    /// 驗證描述
    /// </summary>
    public ValidationResult ValidateDescription(string description, int maxLength = 500)
    {
        if (string.IsNullOrWhiteSpace(description))
            return ValidationResult.Failure("描述不能為空");

        if (description.Length > maxLength)
            return ValidationResult.Failure($"描述不能超過 {maxLength} 個字符");

        return ValidationResult.Success();
    }

    /// <summary>
    /// 檢查是否包含SQL注入攻擊
    /// </summary>
    public bool ContainsSqlInjection(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        var sqlKeywords = new[] { "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "CREATE", "ALTER", "EXEC", "EXECUTE", "UNION", "OR", "AND" };
        var upperInput = input.ToUpperInvariant();

        return sqlKeywords.Any(keyword => upperInput.Contains(keyword));
    }

    /// <summary>
    /// 檢查是否包含XSS攻擊
    /// </summary>
    public bool ContainsXss(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        var xssPatterns = new[]
        {
            @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>",
            @"javascript:",
            @"vbscript:",
            @"onload\s*=",
            @"onerror\s*=",
            @"onclick\s*="
        };

        return xssPatterns.Any(pattern => Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));
    }
}

/// <summary>
/// 驗證服務接口
/// </summary>
public interface IValidationService
{
    ValidationResult ValidateUsername(string username);
    ValidationResult ValidateEmail(string email);
    ValidationResult ValidatePassword(string password);
    ValidationResult ValidatePrice(decimal price, decimal minPrice = 0.01m, decimal maxPrice = 999999.99m);
    ValidationResult ValidateQuantity(int quantity, int minQuantity = 1, int maxQuantity = 999);
    ValidationResult ValidateDescription(string description, int maxLength = 500);
    bool ContainsSqlInjection(string input);
    bool ContainsXss(string input);
}

/// <summary>
/// 驗證結果
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public static ValidationResult Success() => new ValidationResult { IsValid = true };
    public static ValidationResult Failure(string errorMessage) => new ValidationResult { IsValid = false, ErrorMessage = errorMessage };
}
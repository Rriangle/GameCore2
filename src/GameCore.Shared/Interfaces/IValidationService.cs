using GameCore.Shared.DTOs;

namespace GameCore.Shared.Interfaces;

/// <summary>
/// 驗證服務介面
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// 驗證用戶名
    /// </summary>
    ValidationResult ValidateUsername(string username);

    /// <summary>
    /// 驗證郵箱
    /// </summary>
    ValidationResult ValidateEmail(string email);

    /// <summary>
    /// 驗證密碼複雜度
    /// </summary>
    ValidationResult ValidatePassword(string password);

    /// <summary>
    /// 驗證價格
    /// </summary>
    ValidationResult ValidatePrice(decimal price, decimal minPrice = 0.01m, decimal maxPrice = 999999.99m);

    /// <summary>
    /// 驗證數量
    /// </summary>
    ValidationResult ValidateQuantity(int quantity, int minQuantity = 1, int maxQuantity = 999);

    /// <summary>
    /// 驗證手機號碼
    /// </summary>
    ValidationResult ValidatePhone(string phone);

    /// <summary>
    /// 驗證身份證號
    /// </summary>
    ValidationResult ValidateIdNumber(string idNumber);
} 
using FluentAssertions;
using GameCore.Api.DTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace GameCore.Tests.Validation;

public class ValidationAttributeTests
{
    [Fact]
    public void PasswordValidationAttribute_WithValidPassword_ShouldPass()
    {
        // Arrange
        var attribute = new PasswordValidationAttribute();
        var validPassword = "ValidPass123!";

        // Act
        var result = attribute.GetValidationResult(validPassword, new ValidationContext(validPassword));

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void PasswordValidationAttribute_WithNullOrEmpty_ShouldFail(string? password)
    {
        // Arrange
        var attribute = new PasswordValidationAttribute();

        // Act
        var result = attribute.GetValidationResult(password, new ValidationContext(password));

        // Assert
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Be("密碼不能為空");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567")]
    public void PasswordValidationAttribute_WithTooShortPassword_ShouldFail(string password)
    {
        // Arrange
        var attribute = new PasswordValidationAttribute();

        // Act
        var result = attribute.GetValidationResult(password, new ValidationContext(password));

        // Assert
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Be("密碼長度至少需要 8 個字元");
    }

    [Theory]
    [InlineData("password123!")] // 沒有大寫
    [InlineData("PASSWORD123!")] // 沒有小寫
    [InlineData("Password!")]    // 沒有數字
    [InlineData("Password123")]  // 沒有特殊字元
    public void PasswordValidationAttribute_WithWeakPassword_ShouldFail(string password)
    {
        // Arrange
        var attribute = new PasswordValidationAttribute();

        // Act
        var result = attribute.GetValidationResult(password, new ValidationContext(password));

        // Assert
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().NotBe("密碼不能為空");
        result.ErrorMessage.Should().NotBe("密碼長度至少需要 8 個字元");
    }

    [Theory]
    [InlineData("ValidPass123!")]
    [InlineData("Strong@Password1")]
    [InlineData("MyP@ssw0rd")]
    [InlineData("C0mpl3x!Pass")]
    public void PasswordValidationAttribute_WithStrongPassword_ShouldPass(string password)
    {
        // Arrange
        var attribute = new PasswordValidationAttribute();

        // Act
        var result = attribute.GetValidationResult(password, new ValidationContext(password));

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void UsernameValidationAttribute_WithValidUsername_ShouldPass()
    {
        // Arrange
        var attribute = new UsernameValidationAttribute();
        var validUsername = "validuser123";

        // Act
        var result = attribute.GetValidationResult(validUsername, new ValidationContext(validUsername));

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UsernameValidationAttribute_WithNullOrEmpty_ShouldFail(string? username)
    {
        // Arrange
        var attribute = new UsernameValidationAttribute();

        // Act
        var result = attribute.GetValidationResult(username, new ValidationContext(username));

        // Assert
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Be("用戶名不能為空");
    }

    [Theory]
    [InlineData("ab")]           // 太短
    [InlineData("a")]            // 太短
    [InlineData("verylongusername123456789")] // 太長
    public void UsernameValidationAttribute_WithInvalidLength_ShouldFail(string username)
    {
        // Arrange
        var attribute = new UsernameValidationAttribute();

        // Act
        var result = attribute.GetValidationResult(username, new ValidationContext(username));

        // Assert
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Be("用戶名長度必須在 3-20 個字元之間");
    }

    [Theory]
    [InlineData("user@name")]    // 包含 @
    [InlineData("user name")]    // 包含空格
    [InlineData("user.name")]    // 包含點
    [InlineData("user#name")]    // 包含 #
    [InlineData("user$name")]    // 包含 $
    public void UsernameValidationAttribute_WithInvalidCharacters_ShouldFail(string username)
    {
        // Arrange
        var attribute = new UsernameValidationAttribute();

        // Act
        var result = attribute.GetValidationResult(username, new ValidationContext(username));

        // Assert
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Be("用戶名只能包含字母、數字、底線和連字號");
    }

    [Theory]
    [InlineData("_username")]
    [InlineData("-username")]
    [InlineData("username_")]
    [InlineData("username-")]
    public void UsernameValidationAttribute_WithInvalidStartOrEnd_ShouldFail(string username)
    {
        // Arrange
        var attribute = new UsernameValidationAttribute();

        // Act
        var result = attribute.GetValidationResult(username, new ValidationContext(username));

        // Assert
        result.Should().NotBe(ValidationResult.Success);
        result!.ErrorMessage.Should().Be("用戶名不能以底線或連字號開頭或結尾");
    }

    [Theory]
    [InlineData("validuser")]
    [InlineData("user123")]
    [InlineData("user_name")]
    [InlineData("user-name")]
    [InlineData("user123_name")]
    [InlineData("user-name_123")]
    public void UsernameValidationAttribute_WithValidCharacters_ShouldPass(string username)
    {
        // Arrange
        var attribute = new UsernameValidationAttribute();

        // Act
        var result = attribute.GetValidationResult(username, new ValidationContext(username));

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Theory]
    [InlineData("abc")]          // 最小長度
    [InlineData("validuser123")] // 正常長度
    [InlineData("verylonguser123456789")] // 最大長度
    public void UsernameValidationAttribute_WithValidLength_ShouldPass(string username)
    {
        // Arrange
        var attribute = new UsernameValidationAttribute();

        // Act
        var result = attribute.GetValidationResult(username, new ValidationContext(username));

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void RegisterRequestDto_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Username = "validuser123",
            Email = "test@example.com",
            Password = "ValidPass123!",
            ConfirmPassword = "ValidPass123!"
        };

        // Act
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void RegisterRequestDto_WithInvalidData_ShouldFailValidation()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Username = "a", // 太短
            Email = "invalid-email",
            Password = "weak",
            ConfirmPassword = "different"
        };

        // Act
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.ErrorMessage!.Contains("用戶名長度必須在 3-20 個字元之間"));
        results.Should().Contain(r => r.ErrorMessage!.Contains("請輸入有效的電子郵件地址"));
        results.Should().Contain(r => r.ErrorMessage!.Contains("密碼長度至少需要 8 個字元"));
        results.Should().Contain(r => r.ErrorMessage!.Contains("密碼與確認密碼不符"));
    }

    [Fact]
    public void LoginRequestDto_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "validuser",
            Password = "password123"
        };

        // Act
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void LoginRequestDto_WithInvalidData_ShouldFailValidation()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "",
            Password = ""
        };

        // Act
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.ErrorMessage!.Contains("用戶名為必填項目"));
        results.Should().Contain(r => r.ErrorMessage!.Contains("密碼為必填項目"));
    }
}

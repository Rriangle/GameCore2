using FluentAssertions;
using GameCore.Shared.DTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace GameCore.Tests.Validation;

public class ValidationAttributeTests
{
    [Fact]
    public void RegisterRequestDto_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Username = "validuser123",
            Email = "test@example.com",
            Password = "ValidPass123!"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [Theory]
    [InlineData("a", "test@example.com", "ValidPass123!")] // 用戶名太短
    [InlineData("validuser123", "invalid-email", "ValidPass123!")] // 無效郵箱
    [InlineData("validuser123", "test@example.com", "weak")] // 密碼太弱
    public void RegisterRequestDto_WithInvalidData_ShouldFailValidation(string username, string email, string password)
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Username = username,
            Email = email,
            Password = password
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().NotBeEmpty();
    }

    [Fact]
    public void LoginRequestDto_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "validuser123",
            Password = "ValidPass123!"
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "ValidPass123!")] // 空用戶名
    [InlineData("validuser123", "")] // 空密碼
    public void LoginRequestDto_WithInvalidData_ShouldFailValidation(string username, string password)
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = username,
            Password = password
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

        // Assert
        isValid.Should().BeFalse();
        validationResults.Should().NotBeEmpty();
    }

    [Fact]
    public void UserProfileDto_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new UserProfileDto
        {
            UserId = 1,
            Username = "validuser123",
            Email = "test@example.com",
            Balance = 100.00m,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void AuthResponseDto_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var request = new AuthResponseDto
        {
            Success = true,
            Message = "Success",
            Token = "valid-token",
            RefreshToken = "valid-refresh-token",
            User = new UserProfileDto
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                Balance = 100.00m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        // Act
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, new ValidationContext(request), validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }
}

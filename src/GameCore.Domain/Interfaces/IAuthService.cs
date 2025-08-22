namespace GameCore.Domain.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string username, string email, string password);
    Task<AuthResult> LoginAsync(string username, string password);
    Task<UserProfile?> GetUserProfileAsync(int userId);
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }
    public UserProfile? User { get; set; }
}

public class UserProfile
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

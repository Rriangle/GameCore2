using GameCore.Shared.DTOs;

namespace GameCore.Shared.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(string username, string email, string password);
        Task<AuthResult> LoginAsync(string username, string password);
        Task<AuthResult> GetUserProfileAsync(int userId);
    }
}

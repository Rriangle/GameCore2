using GameCore.Api.DTOs;

namespace GameCore.Domain.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<UserProfileDto?> GetUserProfileAsync(int userId);
}



namespace GameCore.Shared.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(object user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
    }
}

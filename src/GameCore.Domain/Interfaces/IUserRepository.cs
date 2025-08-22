using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
}

using GameCore.Application.Common;
using GameCore.Application.DTOs.Requests;
using GameCore.Application.DTOs.Responses;
using GameCore.Application.Interfaces;

namespace GameCore.Application.Services
{
    /// <summary>
    /// 使用者服務存根實作
    /// </summary>
    public class UserService : IUserService
    {
        public Task<Result<UserDto>> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("使用者服務尚未實作");
        }

        public Task<Result<PagedResult<UserDto>>> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("使用者服務尚未實作");
        }

        public Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("使用者服務尚未實作");
        }

        public Task<Result<UserDto>> UpdateUserAsync(int userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("使用者服務尚未實作");
        }

        public Task<Result<bool>> DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("使用者服務尚未實作");
        }

        public Task<Result<bool>> ToggleUserStatusAsync(int userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("使用者服務尚未實作");
        }

        public Task<Result<List<string>>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("使用者服務尚未實作");
        }
    }
} 
using GameCore.Application.Common;
using GameCore.Application.DTOs.Requests;
using GameCore.Application.DTOs.Responses;

namespace GameCore.Application.Interfaces
{
    /// <summary>
    /// 使用者服務介面
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 取得使用者資訊
        /// </summary>
        /// <param name="userId">使用者識別碼</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>使用者資料</returns>
        Task<Result<UserDto>> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得使用者列表
        /// </summary>
        /// <param name="pageNumber">頁碼</param>
        /// <param name="pageSize">頁面大小</param>
        /// <param name="searchTerm">搜尋關鍵字</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>分頁使用者列表</returns>
        Task<Result<PagedResult<UserDto>>> GetUsersAsync(
            int pageNumber, 
            int pageSize, 
            string? searchTerm = null, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 建立使用者
        /// </summary>
        /// <param name="request">建立使用者請求</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>建立的使用者資料</returns>
        Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新使用者資訊
        /// </summary>
        /// <param name="userId">使用者識別碼</param>
        /// <param name="request">更新使用者請求</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>更新後的使用者資料</returns>
        Task<Result<UserDto>> UpdateUserAsync(int userId, UpdateUserRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// 刪除使用者
        /// </summary>
        /// <param name="userId">使用者識別碼</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>刪除結果</returns>
        Task<Result<bool>> DeleteUserAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 啟用/停用使用者
        /// </summary>
        /// <param name="userId">使用者識別碼</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>操作結果</returns>
        Task<Result<bool>> ToggleUserStatusAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 取得使用者權限
        /// </summary>
        /// <param name="userId">使用者識別碼</param>
        /// <param name="cancellationToken">取消權杖</param>
        /// <returns>權限列表</returns>
        Task<Result<List<string>>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default);
    }
} 
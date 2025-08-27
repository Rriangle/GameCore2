using GameCore.Domain.Entities;

namespace GameCore.Domain.Interfaces
{
    /// <summary>
    /// 管理者服務接口
    /// </summary>
    public interface IManagerService
    {
        Task<ManagerData?> GetManagerAsync(int managerId);
        Task<ManagerData?> GetManagerByAccountAsync(string account);
        Task<ManagerData> CreateManagerAsync(ManagerData manager);
        Task<bool> UpdateManagerAsync(ManagerData manager);
        Task<bool> DeleteManagerAsync(int managerId);
        Task<IEnumerable<ManagerData>> GetAllManagersAsync();
        Task<bool> AssignRoleAsync(int managerId, int roleId);
        Task<bool> RemoveRoleAsync(int managerId, int roleId);
        Task<IEnumerable<ManagerRole>> GetManagerRolesAsync(int managerId);
        Task<bool> HasPermissionAsync(int managerId, string permissionName);
        Task<bool> UpdateLastLoginAsync(int managerId);
        Task<IEnumerable<ManagerRolePermission>> GetAllRolesAsync();
        Task<ManagerRolePermission> CreateRoleAsync(ManagerRolePermission role);
        Task<bool> UpdateRoleAsync(ManagerRolePermission role);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<IEnumerable<Mute>> GetMutesAsync();
        Task<Mute> CreateMuteAsync(Mute mute);
        Task<bool> UpdateMuteAsync(Mute mute);
        Task<bool> DeleteMuteAsync(int muteId);
        Task<IEnumerable<Style>> GetStylesAsync();
        Task<Style> CreateStyleAsync(Style style);
        Task<bool> UpdateStyleAsync(Style style);
        Task<bool> DeleteStyleAsync(int styleId);
    }
}
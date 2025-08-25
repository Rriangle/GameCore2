using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace GameCore.Infrastructure.Services
{
    /// <summary>
    /// 管理者服務實現
    /// </summary>
    public class ManagerService : IManagerService
    {
        private readonly GameCoreDbContext _context;
        private readonly ILogger<ManagerService> _logger;

        public ManagerService(GameCoreDbContext context, ILogger<ManagerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 獲取管理者資料
        /// </summary>
        public async Task<ManagerData?> GetManagerAsync(int managerId)
        {
            try
            {
                return await _context.ManagerData
                    .Include(m => m.Roles)
                        .ThenInclude(mr => mr.RolePermission)
                    .Include(m => m.AdminLogins)
                    .FirstOrDefaultAsync(m => m.Manager_Id == managerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取管理者 {ManagerId} 資料時發生錯誤", managerId);
                throw;
            }
        }

        /// <summary>
        /// 根據帳號獲取管理者資料
        /// </summary>
        public async Task<ManagerData?> GetManagerByAccountAsync(string account)
        {
            try
            {
                return await _context.ManagerData
                    .Include(m => m.Roles)
                        .ThenInclude(mr => mr.RolePermission)
                    .Include(m => m.AdminLogins)
                    .FirstOrDefaultAsync(m => m.Manager_Account == account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "根據帳號 {Account} 獲取管理者資料時發生錯誤", account);
                throw;
            }
        }

        /// <summary>
        /// 創建管理者
        /// </summary>
        public async Task<ManagerData> CreateManagerAsync(ManagerData manager)
        {
            try
            {
                // 加密密碼
                manager.Manager_Password = HashPassword(manager.Manager_Password);
                manager.Administrator_registration_date = DateTime.UtcNow;

                _context.ManagerData.Add(manager);
                await _context.SaveChangesAsync();

                return manager;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建管理者時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 更新管理者資料
        /// </summary>
        public async Task<bool> UpdateManagerAsync(ManagerData manager)
        {
            try
            {
                var existingManager = await _context.ManagerData
                    .FirstOrDefaultAsync(m => m.Manager_Id == manager.Manager_Id);

                if (existingManager == null)
                    return false;

                existingManager.Manager_Name = manager.Manager_Name;
                existingManager.Manager_Account = manager.Manager_Account;

                // 如果提供了新密碼，則加密
                if (!string.IsNullOrEmpty(manager.Manager_Password))
                {
                    existingManager.Manager_Password = HashPassword(manager.Manager_Password);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新管理者 {ManagerId} 資料時發生錯誤", manager.Manager_Id);
                throw;
            }
        }

        /// <summary>
        /// 刪除管理者
        /// </summary>
        public async Task<bool> DeleteManagerAsync(int managerId)
        {
            try
            {
                var manager = await _context.ManagerData
                    .Include(m => m.Roles)
                    .Include(m => m.AdminLogins)
                    .FirstOrDefaultAsync(m => m.Manager_Id == managerId);

                if (manager == null)
                    return false;

                _context.ManagerData.Remove(manager);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除管理者 {ManagerId} 時發生錯誤", managerId);
                throw;
            }
        }

        /// <summary>
        /// 獲取所有管理者
        /// </summary>
        public async Task<IEnumerable<ManagerData>> GetAllManagersAsync()
        {
            try
            {
                return await _context.ManagerData
                    .Include(m => m.Roles)
                        .ThenInclude(mr => mr.RolePermission)
                    .Include(m => m.AdminLogins)
                    .OrderBy(m => m.Manager_Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取所有管理者時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 指派角色
        /// </summary>
        public async Task<bool> AssignRoleAsync(int managerId, int roleId)
        {
            try
            {
                // 檢查是否已經指派
                var existingRole = await _context.ManagerRoles
                    .FirstOrDefaultAsync(mr => mr.Manager_Id == managerId && mr.ManagerRole_Id == roleId);

                if (existingRole != null)
                    return true;

                var managerRole = new ManagerRole
                {
                    Manager_Id = managerId,
                    ManagerRole_Id = roleId,
                    ManagerRole = "Assigned",
                    created_at = DateTime.UtcNow
                };

                _context.ManagerRoles.Add(managerRole);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "指派角色 {RoleId} 給管理者 {ManagerId} 時發生錯誤", roleId, managerId);
                throw;
            }
        }

        /// <summary>
        /// 移除角色
        /// </summary>
        public async Task<bool> RemoveRoleAsync(int managerId, int roleId)
        {
            try
            {
                var managerRole = await _context.ManagerRoles
                    .FirstOrDefaultAsync(mr => mr.Manager_Id == managerId && mr.ManagerRole_Id == roleId);

                if (managerRole == null)
                    return false;

                _context.ManagerRoles.Remove(managerRole);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "移除管理者 {ManagerId} 角色 {RoleId} 時發生錯誤", managerId, roleId);
                throw;
            }
        }

        /// <summary>
        /// 獲取管理者角色列表
        /// </summary>
        public async Task<IEnumerable<ManagerRole>> GetManagerRolesAsync(int managerId)
        {
            try
            {
                return await _context.ManagerRoles
                    .Include(mr => mr.RolePermission)
                    .Where(mr => mr.Manager_Id == managerId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取管理者 {ManagerId} 角色列表時發生錯誤", managerId);
                throw;
            }
        }

        /// <summary>
        /// 檢查管理者是否有特定權限
        /// </summary>
        public async Task<bool> HasPermissionAsync(int managerId, string permissionName)
        {
            try
            {
                var manager = await _context.ManagerData
                    .Include(m => m.Roles)
                        .ThenInclude(mr => mr.RolePermission)
                    .FirstOrDefaultAsync(m => m.Manager_Id == managerId);

                if (manager == null)
                    return false;

                return manager.Roles.Any(mr => permissionName switch
                {
                    "AdministratorPrivilegesManagement" => mr.RolePermission.AdministratorPrivilegesManagement,
                    "UserStatusManagement" => mr.RolePermission.UserStatusManagement,
                    "ShoppingPermissionManagement" => mr.RolePermission.ShoppingPermissionManagement,
                    "MessagePermissionManagement" => mr.RolePermission.MessagePermissionManagement,
                    "PetRightsManagement" => mr.RolePermission.Pet_Rights_Management,
                    "CustomerService" => mr.RolePermission.customer_service,
                    _ => false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "檢查管理者 {ManagerId} 權限 {PermissionName} 時發生錯誤", managerId, permissionName);
                throw;
            }
        }

        /// <summary>
        /// 更新最後登入時間
        /// </summary>
        public async Task<bool> UpdateLastLoginAsync(int managerId)
        {
            try
            {
                var admin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.manager_id == managerId);

                if (admin == null)
                {
                    // 創建新的登入記錄
                    admin = new Admin
                    {
                        manager_id = managerId,
                        last_login = DateTime.UtcNow,
                        created_at = DateTime.UtcNow
                    };
                    _context.Admins.Add(admin);
                }
                else
                {
                    admin.last_login = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新管理者 {ManagerId} 最後登入時間時發生錯誤", managerId);
                throw;
            }
        }

        /// <summary>
        /// 獲取所有角色
        /// </summary>
        public async Task<IEnumerable<ManagerRolePermission>> GetAllRolesAsync()
        {
            try
            {
                return await _context.ManagerRolePermissions
                    .OrderBy(r => r.role_name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取所有角色時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 創建角色
        /// </summary>
        public async Task<ManagerRolePermission> CreateRoleAsync(ManagerRolePermission role)
        {
            try
            {
                role.created_at = DateTime.UtcNow;
                _context.ManagerRolePermissions.Add(role);
                await _context.SaveChangesAsync();

                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建角色時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        public async Task<bool> UpdateRoleAsync(ManagerRolePermission role)
        {
            try
            {
                var existingRole = await _context.ManagerRolePermissions
                    .FirstOrDefaultAsync(r => r.ManagerRole_Id == role.ManagerRole_Id);

                if (existingRole == null)
                    return false;

                existingRole.role_name = role.role_name;
                existingRole.AdministratorPrivilegesManagement = role.AdministratorPrivilegesManagement;
                existingRole.UserStatusManagement = role.UserStatusManagement;
                existingRole.ShoppingPermissionManagement = role.ShoppingPermissionManagement;
                existingRole.MessagePermissionManagement = role.MessagePermissionManagement;
                existingRole.Pet_Rights_Management = role.Pet_Rights_Management;
                existingRole.customer_service = role.customer_service;
                existingRole.updated_at = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新角色 {RoleId} 時發生錯誤", role.ManagerRole_Id);
                throw;
            }
        }

        /// <summary>
        /// 刪除角色
        /// </summary>
        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            try
            {
                var role = await _context.ManagerRolePermissions
                    .Include(r => r.ManagerRoles)
                    .FirstOrDefaultAsync(r => r.ManagerRole_Id == roleId);

                if (role == null)
                    return false;

                // 檢查是否有管理者使用此角色
                if (role.ManagerRoles.Any())
                    return false;

                _context.ManagerRolePermissions.Remove(role);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除角色 {RoleId} 時發生錯誤", roleId);
                throw;
            }
        }

        /// <summary>
        /// 獲取禁言選項列表
        /// </summary>
        public async Task<IEnumerable<Mute>> GetMutesAsync()
        {
            try
            {
                return await _context.Mutes
                    .Include(m => m.Manager)
                    .OrderBy(m => m.mute_name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取禁言選項列表時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 創建禁言選項
        /// </summary>
        public async Task<Mute> CreateMuteAsync(Mute mute)
        {
            try
            {
                mute.created_at = DateTime.UtcNow;
                _context.Mutes.Add(mute);
                await _context.SaveChangesAsync();

                return mute;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建禁言選項時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 更新禁言選項
        /// </summary>
        public async Task<bool> UpdateMuteAsync(Mute mute)
        {
            try
            {
                var existingMute = await _context.Mutes
                    .FirstOrDefaultAsync(m => m.mute_id == mute.mute_id);

                if (existingMute == null)
                    return false;

                existingMute.mute_name = mute.mute_name;
                existingMute.is_active = mute.is_active;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新禁言選項 {MuteId} 時發生錯誤", mute.mute_id);
                throw;
            }
        }

        /// <summary>
        /// 刪除禁言選項
        /// </summary>
        public async Task<bool> DeleteMuteAsync(int muteId)
        {
            try
            {
                var mute = await _context.Mutes
                    .FirstOrDefaultAsync(m => m.mute_id == muteId);

                if (mute == null)
                    return false;

                _context.Mutes.Remove(mute);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除禁言選項 {MuteId} 時發生錯誤", muteId);
                throw;
            }
        }

        /// <summary>
        /// 獲取樣式列表
        /// </summary>
        public async Task<IEnumerable<Style>> GetStylesAsync()
        {
            try
            {
                return await _context.Styles
                    .Include(s => s.Manager)
                    .OrderBy(s => s.style_name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取樣式列表時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 創建樣式
        /// </summary>
        public async Task<Style> CreateStyleAsync(Style style)
        {
            try
            {
                style.created_at = DateTime.UtcNow;
                _context.Styles.Add(style);
                await _context.SaveChangesAsync();

                return style;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建樣式時發生錯誤");
                throw;
            }
        }

        /// <summary>
        /// 更新樣式
        /// </summary>
        public async Task<bool> UpdateStyleAsync(Style style)
        {
            try
            {
                var existingStyle = await _context.Styles
                    .FirstOrDefaultAsync(s => s.style_id == style.style_id);

                if (existingStyle == null)
                    return false;

                existingStyle.style_name = style.style_name;
                existingStyle.effect_desc = style.effect_desc;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新樣式 {StyleId} 時發生錯誤", style.style_id);
                throw;
            }
        }

        /// <summary>
        /// 刪除樣式
        /// </summary>
        public async Task<bool> DeleteStyleAsync(int styleId)
        {
            try
            {
                var style = await _context.Styles
                    .FirstOrDefaultAsync(s => s.style_id == styleId);

                if (style == null)
                    return false;

                _context.Styles.Remove(style);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除樣式 {StyleId} 時發生錯誤", styleId);
                throw;
            }
        }

        /// <summary>
        /// 加密密碼
        /// </summary>
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        /// <summary>
        /// 驗證密碼
        /// </summary>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
}
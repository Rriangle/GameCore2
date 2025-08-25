using GameCore.Domain.Interfaces;
using GameCore.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameCore.Api.Controllers
{
    /// <summary>
    /// 管理者控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(IManagerService managerService, ILogger<ManagerController> logger)
        {
            _managerService = managerService;
            _logger = logger;
        }

        /// <summary>
        /// 管理者登入
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<ManagerLoginResponseDto>> Login([FromBody] ManagerLoginRequestDto request)
        {
            try
            {
                var manager = await _managerService.GetManagerByAccountAsync(request.Account);
                if (manager == null)
                    return Unauthorized(new { error = "帳號或密碼錯誤" });

                // 驗證密碼
                if (!_managerService.VerifyPassword(request.Password, manager.Manager_Password))
                    return Unauthorized(new { error = "帳號或密碼錯誤" });

                // 更新最後登入時間
                await _managerService.UpdateLastLoginAsync(manager.Manager_Id);

                // 獲取權限列表
                var roles = await _managerService.GetManagerRolesAsync(manager.Manager_Id);
                var permissions = new List<string>();

                foreach (var role in roles)
                {
                    if (role.RolePermission.AdministratorPrivilegesManagement)
                        permissions.Add("AdministratorPrivilegesManagement");
                    if (role.RolePermission.UserStatusManagement)
                        permissions.Add("UserStatusManagement");
                    if (role.RolePermission.ShoppingPermissionManagement)
                        permissions.Add("ShoppingPermissionManagement");
                    if (role.RolePermission.MessagePermissionManagement)
                        permissions.Add("MessagePermissionManagement");
                    if (role.RolePermission.Pet_Rights_Management)
                        permissions.Add("PetRightsManagement");
                    if (role.RolePermission.customer_service)
                        permissions.Add("CustomerService");
                }

                var response = new ManagerLoginResponseDto
                {
                    ManagerId = manager.Manager_Id,
                    ManagerName = manager.Manager_Name,
                    Account = manager.Manager_Account,
                    Token = $"manager_token_{manager.Manager_Id}_{DateTime.UtcNow.Ticks}", // 簡化實現
                    Permissions = permissions,
                    LoginTime = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "管理者登入時發生錯誤");
                return StatusCode(500, new { error = "登入失敗" });
            }
        }

        /// <summary>
        /// 獲取管理者資料
        /// </summary>
        [HttpGet("{managerId}")]
        public async Task<ActionResult<ManagerResponseDto>> GetManager(int managerId)
        {
            try
            {
                var manager = await _managerService.GetManagerAsync(managerId);
                if (manager == null)
                    return NotFound(new { error = "管理者不存在" });

                var roles = await _managerService.GetManagerRolesAsync(managerId);
                var roleResponses = roles.Select(r => new ManagerRoleResponseDto
                {
                    RoleId = r.ManagerRole_Id,
                    RoleName = r.RolePermission.role_name,
                    AssignedRole = r.ManagerRole,
                    AssignedAt = r.created_at,
                    Permissions = new ManagerRolePermissionResponseDto
                    {
                        RoleId = r.RolePermission.ManagerRole_Id,
                        RoleName = r.RolePermission.role_name,
                        AdministratorPrivilegesManagement = r.RolePermission.AdministratorPrivilegesManagement,
                        UserStatusManagement = r.RolePermission.UserStatusManagement,
                        ShoppingPermissionManagement = r.RolePermission.ShoppingPermissionManagement,
                        MessagePermissionManagement = r.RolePermission.MessagePermissionManagement,
                        PetRightsManagement = r.RolePermission.Pet_Rights_Management,
                        CustomerService = r.RolePermission.customer_service
                    }
                }).ToList();

                var response = new ManagerResponseDto
                {
                    ManagerId = manager.Manager_Id,
                    ManagerName = manager.Manager_Name,
                    Account = manager.Manager_Account,
                    RegistrationDate = manager.Administrator_registration_date,
                    LastLogin = manager.AdminLogins?.OrderByDescending(a => a.last_login).FirstOrDefault()?.last_login,
                    Roles = roleResponses
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取管理者 {ManagerId} 資料時發生錯誤", managerId);
                return StatusCode(500, new { error = "獲取管理者資料失敗" });
            }
        }

        /// <summary>
        /// 創建管理者
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ManagerResponseDto>> CreateManager([FromBody] CreateManagerRequestDto request)
        {
            try
            {
                var manager = new GameCore.Domain.Entities.ManagerData
                {
                    Manager_Name = request.ManagerName,
                    Manager_Account = request.Account,
                    Manager_Password = request.Password,
                    Administrator_registration_date = DateTime.UtcNow
                };

                var createdManager = await _managerService.CreateManagerAsync(manager);

                // 指派角色
                foreach (var roleId in request.RoleIds)
                {
                    await _managerService.AssignRoleAsync(createdManager.Manager_Id, roleId);
                }

                var response = new ManagerResponseDto
                {
                    ManagerId = createdManager.Manager_Id,
                    ManagerName = createdManager.Manager_Name,
                    Account = createdManager.Manager_Account,
                    RegistrationDate = createdManager.Administrator_registration_date,
                    Roles = new List<ManagerRoleResponseDto>()
                };

                return CreatedAtAction(nameof(GetManager), new { managerId = createdManager.Manager_Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建管理者時發生錯誤");
                return StatusCode(500, new { error = "創建管理者失敗" });
            }
        }

        /// <summary>
        /// 更新管理者資料
        /// </summary>
        [HttpPut("{managerId}")]
        public async Task<ActionResult> UpdateManager(int managerId, [FromBody] UpdateManagerRequestDto request)
        {
            try
            {
                if (request.ManagerId != managerId)
                    return BadRequest(new { error = "管理者ID不匹配" });

                var manager = new GameCore.Domain.Entities.ManagerData
                {
                    Manager_Id = request.ManagerId,
                    Manager_Name = request.ManagerName,
                    Manager_Account = request.Account,
                    Manager_Password = request.Password ?? string.Empty
                };

                var success = await _managerService.UpdateManagerAsync(manager);
                if (!success)
                    return NotFound(new { error = "管理者不存在" });

                // 更新角色
                var currentRoles = await _managerService.GetManagerRolesAsync(managerId);
                var currentRoleIds = currentRoles.Select(r => r.ManagerRole_Id).ToList();

                // 移除不再需要的角色
                foreach (var roleId in currentRoleIds.Except(request.RoleIds))
                {
                    await _managerService.RemoveRoleAsync(managerId, roleId);
                }

                // 添加新角色
                foreach (var roleId in request.RoleIds.Except(currentRoleIds))
                {
                    await _managerService.AssignRoleAsync(managerId, roleId);
                }

                return Ok(new { message = "管理者資料更新成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新管理者 {ManagerId} 資料時發生錯誤", managerId);
                return StatusCode(500, new { error = "更新管理者資料失敗" });
            }
        }

        /// <summary>
        /// 刪除管理者
        /// </summary>
        [HttpDelete("{managerId}")]
        public async Task<ActionResult> DeleteManager(int managerId)
        {
            try
            {
                var success = await _managerService.DeleteManagerAsync(managerId);
                if (!success)
                    return NotFound(new { error = "管理者不存在" });

                return Ok(new { message = "管理者刪除成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除管理者 {ManagerId} 時發生錯誤", managerId);
                return StatusCode(500, new { error = "刪除管理者失敗" });
            }
        }

        /// <summary>
        /// 獲取所有管理者
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ManagerListResponseDto>> GetAllManagers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var managers = await _managerService.GetAllManagersAsync();
                var totalCount = managers.Count();
                var pagedManagers = managers.Skip((page - 1) * pageSize).Take(pageSize);

                var managerResponses = new List<ManagerResponseDto>();
                foreach (var manager in pagedManagers)
                {
                    var roles = await _managerService.GetManagerRolesAsync(manager.Manager_Id);
                    var roleResponses = roles.Select(r => new ManagerRoleResponseDto
                    {
                        RoleId = r.ManagerRole_Id,
                        RoleName = r.RolePermission.role_name,
                        AssignedRole = r.ManagerRole,
                        AssignedAt = r.created_at,
                        Permissions = new ManagerRolePermissionResponseDto
                        {
                            RoleId = r.RolePermission.ManagerRole_Id,
                            RoleName = r.RolePermission.role_name,
                            AdministratorPrivilegesManagement = r.RolePermission.AdministratorPrivilegesManagement,
                            UserStatusManagement = r.RolePermission.UserStatusManagement,
                            ShoppingPermissionManagement = r.RolePermission.ShoppingPermissionManagement,
                            MessagePermissionManagement = r.RolePermission.MessagePermissionManagement,
                            PetRightsManagement = r.RolePermission.Pet_Rights_Management,
                            CustomerService = r.RolePermission.customer_service
                        }
                    }).ToList();

                    managerResponses.Add(new ManagerResponseDto
                    {
                        ManagerId = manager.Manager_Id,
                        ManagerName = manager.Manager_Name,
                        Account = manager.Manager_Account,
                        RegistrationDate = manager.Administrator_registration_date,
                        LastLogin = manager.AdminLogins?.OrderByDescending(a => a.last_login).FirstOrDefault()?.last_login,
                        Roles = roleResponses
                    });
                }

                var response = new ManagerListResponseDto
                {
                    Managers = managerResponses,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取所有管理者時發生錯誤");
                return StatusCode(500, new { error = "獲取管理者列表失敗" });
            }
        }

        /// <summary>
        /// 獲取所有角色
        /// </summary>
        [HttpGet("roles")]
        public async Task<ActionResult<RoleListResponseDto>> GetAllRoles()
        {
            try
            {
                var roles = await _managerService.GetAllRolesAsync();

                var response = new RoleListResponseDto
                {
                    Roles = roles.Select(r => new ManagerRolePermissionResponseDto
                    {
                        RoleId = r.ManagerRole_Id,
                        RoleName = r.role_name,
                        AdministratorPrivilegesManagement = r.AdministratorPrivilegesManagement,
                        UserStatusManagement = r.UserStatusManagement,
                        ShoppingPermissionManagement = r.ShoppingPermissionManagement,
                        MessagePermissionManagement = r.MessagePermissionManagement,
                        PetRightsManagement = r.Pet_Rights_Management,
                        CustomerService = r.customer_service
                    }).ToList(),
                    TotalCount = roles.Count()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取所有角色時發生錯誤");
                return StatusCode(500, new { error = "獲取角色列表失敗" });
            }
        }

        /// <summary>
        /// 創建角色
        /// </summary>
        [HttpPost("roles")]
        public async Task<ActionResult<ManagerRolePermissionResponseDto>> CreateRole([FromBody] CreateRoleRequestDto request)
        {
            try
            {
                var role = new GameCore.Domain.Entities.ManagerRolePermission
                {
                    role_name = request.RoleName,
                    AdministratorPrivilegesManagement = request.AdministratorPrivilegesManagement,
                    UserStatusManagement = request.UserStatusManagement,
                    ShoppingPermissionManagement = request.ShoppingPermissionManagement,
                    MessagePermissionManagement = request.MessagePermissionManagement,
                    Pet_Rights_Management = request.PetRightsManagement,
                    customer_service = request.CustomerService
                };

                var createdRole = await _managerService.CreateRoleAsync(role);

                var response = new ManagerRolePermissionResponseDto
                {
                    RoleId = createdRole.ManagerRole_Id,
                    RoleName = createdRole.role_name,
                    AdministratorPrivilegesManagement = createdRole.AdministratorPrivilegesManagement,
                    UserStatusManagement = createdRole.UserStatusManagement,
                    ShoppingPermissionManagement = createdRole.ShoppingPermissionManagement,
                    MessagePermissionManagement = createdRole.MessagePermissionManagement,
                    PetRightsManagement = createdRole.Pet_Rights_Management,
                    CustomerService = createdRole.customer_service
                };

                return CreatedAtAction(nameof(GetAllRoles), response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建角色時發生錯誤");
                return StatusCode(500, new { error = "創建角色失敗" });
            }
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        [HttpPut("roles/{roleId}")]
        public async Task<ActionResult> UpdateRole(int roleId, [FromBody] UpdateRoleRequestDto request)
        {
            try
            {
                if (request.RoleId != roleId)
                    return BadRequest(new { error = "角色ID不匹配" });

                var role = new GameCore.Domain.Entities.ManagerRolePermission
                {
                    ManagerRole_Id = request.RoleId,
                    role_name = request.RoleName,
                    AdministratorPrivilegesManagement = request.AdministratorPrivilegesManagement,
                    UserStatusManagement = request.UserStatusManagement,
                    ShoppingPermissionManagement = request.ShoppingPermissionManagement,
                    MessagePermissionManagement = request.MessagePermissionManagement,
                    Pet_Rights_Management = request.PetRightsManagement,
                    customer_service = request.CustomerService
                };

                var success = await _managerService.UpdateRoleAsync(role);
                if (!success)
                    return NotFound(new { error = "角色不存在" });

                return Ok(new { message = "角色更新成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新角色 {RoleId} 時發生錯誤", roleId);
                return StatusCode(500, new { error = "更新角色失敗" });
            }
        }

        /// <summary>
        /// 刪除角色
        /// </summary>
        [HttpDelete("roles/{roleId}")]
        public async Task<ActionResult> DeleteRole(int roleId)
        {
            try
            {
                var success = await _managerService.DeleteRoleAsync(roleId);
                if (!success)
                    return BadRequest(new { error = "無法刪除角色，可能有管理者正在使用" });

                return Ok(new { message = "角色刪除成功" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除角色 {RoleId} 時發生錯誤", roleId);
                return StatusCode(500, new { error = "刪除角色失敗" });
            }
        }

        /// <summary>
        /// 獲取禁言選項列表
        /// </summary>
        [HttpGet("mutes")]
        public async Task<ActionResult<IEnumerable<MuteResponseDto>>> GetMutes()
        {
            try
            {
                var mutes = await _managerService.GetMutesAsync();

                var response = mutes.Select(m => new MuteResponseDto
                {
                    MuteId = m.mute_id,
                    MuteName = m.mute_name,
                    CreatedAt = m.created_at,
                    IsActive = m.is_active,
                    ManagerName = m.Manager?.Manager_Name ?? "未知"
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取禁言選項列表時發生錯誤");
                return StatusCode(500, new { error = "獲取禁言選項失敗" });
            }
        }

        /// <summary>
        /// 創建禁言選項
        /// </summary>
        [HttpPost("mutes")]
        public async Task<ActionResult<MuteResponseDto>> CreateMute([FromBody] MuteResponseDto request, [FromQuery] int managerId)
        {
            try
            {
                var mute = new GameCore.Domain.Entities.Mute
                {
                    mute_name = request.MuteName,
                    is_active = request.IsActive,
                    manager_id = managerId
                };

                var createdMute = await _managerService.CreateMuteAsync(mute);

                var response = new MuteResponseDto
                {
                    MuteId = createdMute.mute_id,
                    MuteName = createdMute.mute_name,
                    CreatedAt = createdMute.created_at,
                    IsActive = createdMute.is_active,
                    ManagerName = "創建者"
                };

                return CreatedAtAction(nameof(GetMutes), response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建禁言選項時發生錯誤");
                return StatusCode(500, new { error = "創建禁言選項失敗" });
            }
        }

        /// <summary>
        /// 獲取樣式列表
        /// </summary>
        [HttpGet("styles")]
        public async Task<ActionResult<IEnumerable<StyleResponseDto>>> GetStyles()
        {
            try
            {
                var styles = await _managerService.GetStylesAsync();

                var response = styles.Select(s => new StyleResponseDto
                {
                    StyleId = s.style_id,
                    StyleName = s.style_name,
                    EffectDesc = s.effect_desc,
                    CreatedAt = s.created_at,
                    ManagerName = s.Manager?.Manager_Name ?? "未知"
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取樣式列表時發生錯誤");
                return StatusCode(500, new { error = "獲取樣式失敗" });
            }
        }

        /// <summary>
        /// 創建樣式
        /// </summary>
        [HttpPost("styles")]
        public async Task<ActionResult<StyleResponseDto>> CreateStyle([FromBody] StyleResponseDto request, [FromQuery] int managerId)
        {
            try
            {
                var style = new GameCore.Domain.Entities.Style
                {
                    style_name = request.StyleName,
                    effect_desc = request.EffectDesc,
                    manager_id = managerId
                };

                var createdStyle = await _managerService.CreateStyleAsync(style);

                var response = new StyleResponseDto
                {
                    StyleId = createdStyle.style_id,
                    StyleName = createdStyle.style_name,
                    EffectDesc = createdStyle.effect_desc,
                    CreatedAt = createdStyle.created_at,
                    ManagerName = "創建者"
                };

                return CreatedAtAction(nameof(GetStyles), response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建樣式時發生錯誤");
                return StatusCode(500, new { error = "創建樣式失敗" });
            }
        }
    }
}
namespace GameCore.Shared.DTOs
{
    /// <summary>
    /// 管理者登入請求 DTO
    /// </summary>
    public class ManagerLoginRequestDto
    {
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// 管理者登入響應 DTO
    /// </summary>
    public class ManagerLoginResponseDto
    {
        public int ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
        public DateTime LoginTime { get; set; }
    }

    /// <summary>
    /// 管理者資料響應 DTO
    /// </summary>
    public class ManagerResponseDto
    {
        public int ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastLogin { get; set; }
        public List<ManagerRoleResponseDto> Roles { get; set; } = new();
    }

    /// <summary>
    /// 管理者角色響應 DTO
    /// </summary>
    public class ManagerRoleResponseDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string AssignedRole { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public ManagerRolePermissionResponseDto Permissions { get; set; } = new();
    }

    /// <summary>
    /// 管理者角色權限響應 DTO
    /// </summary>
    public class ManagerRolePermissionResponseDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool AdministratorPrivilegesManagement { get; set; }
        public bool UserStatusManagement { get; set; }
        public bool ShoppingPermissionManagement { get; set; }
        public bool MessagePermissionManagement { get; set; }
        public bool PetRightsManagement { get; set; }
        public bool CustomerService { get; set; }
    }

    /// <summary>
    /// 創建管理者請求 DTO
    /// </summary>
    public class CreateManagerRequestDto
    {
        public string ManagerName { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<int> RoleIds { get; set; } = new();
    }

    /// <summary>
    /// 更新管理者請求 DTO
    /// </summary>
    public class UpdateManagerRequestDto
    {
        public int ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
        public string? Password { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }

    /// <summary>
    /// 創建角色請求 DTO
    /// </summary>
    public class CreateRoleRequestDto
    {
        public string RoleName { get; set; } = string.Empty;
        public bool AdministratorPrivilegesManagement { get; set; }
        public bool UserStatusManagement { get; set; }
        public bool ShoppingPermissionManagement { get; set; }
        public bool MessagePermissionManagement { get; set; }
        public bool PetRightsManagement { get; set; }
        public bool CustomerService { get; set; }
    }

    /// <summary>
    /// 更新角色請求 DTO
    /// </summary>
    public class UpdateRoleRequestDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool AdministratorPrivilegesManagement { get; set; }
        public bool UserStatusManagement { get; set; }
        public bool ShoppingPermissionManagement { get; set; }
        public bool MessagePermissionManagement { get; set; }
        public bool PetRightsManagement { get; set; }
        public bool CustomerService { get; set; }
    }

    /// <summary>
    /// 禁言選項響應 DTO
    /// </summary>
    public class MuteResponseDto
    {
        public int MuteId { get; set; }
        public string MuteName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string ManagerName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 樣式響應 DTO
    /// </summary>
    public class StyleResponseDto
    {
        public int StyleId { get; set; }
        public string StyleName { get; set; } = string.Empty;
        public string EffectDesc { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string ManagerName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 管理者列表響應 DTO
    /// </summary>
    public class ManagerListResponseDto
    {
        public List<ManagerResponseDto> Managers { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    /// <summary>
    /// 角色列表響應 DTO
    /// </summary>
    public class RoleListResponseDto
    {
        public List<ManagerRolePermissionResponseDto> Roles { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
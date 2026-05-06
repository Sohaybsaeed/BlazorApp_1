using Microsoft.AspNetCore.Identity;

namespace FirstBlazorApp.Services.IService
{
    /// <summary>
    /// Wraps RoleManager and UserManager operations for all ASP.NET Identity tables:
    /// AspNetRoles, AspNetUserRoles, AspNetRoleClaims, AspNetUserClaims,
    /// AspNetUserLogins, AspNetUserTokens.
    /// </summary>
    public interface IIdentityService
    {
        // ── AspNetRoles ────────────────────────────────────────────────────
        Task<List<ApplicationRole>> GetAllRolesAsync();
        Task<(bool Success, string Error)> CreateRoleAsync(string roleName);
        Task<(bool Success, string Error)> UpdateRoleAsync(int roleId, string newName);
        Task<(bool Success, string Error)> DeleteRoleAsync(int roleId);

        // ── AspNetUserRoles ────────────────────────────────────────────────
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<List<string>> GetUserRolesAsync(int userId);
        Task<(bool Success, string Error)> AssignRoleAsync(int userId, string roleName);
        Task<(bool Success, string Error)> RemoveRoleAsync(int userId, string roleName);

        // ── AspNetRoleClaims (via ApplicationFunctionalities matrix) ───────
        Task<List<System.Security.Claims.Claim>> GetRoleClaimsAsync(int roleId);
        Task<List<FunctionalityGroupDto>> GetFunctionalitiesMatrixAsync(int roleId);
        Task<(bool Success, string Error)> SaveRolePermissionsAsync(int roleId, List<FunctionalityGroupDto> matrix);

        // ── AspNetUserClaims ───────────────────────────────────────────────
        Task<List<System.Security.Claims.Claim>> GetUserClaimsAsync(int userId);
        Task<(bool Success, string Error)> AddUserClaimAsync(int userId, string claimType, string claimValue);
        Task<(bool Success, string Error)> RemoveUserClaimAsync(int userId, string claimType, string claimValue);

        // ── AspNetUserLogins ───────────────────────────────────────────────
        Task<List<UserLoginInfo>> GetUserLoginsAsync(int userId);
        Task<(bool Success, string Error)> RemoveUserLoginAsync(int userId, string loginProvider, string providerKey);

        // ── AspNetUserTokens ───────────────────────────────────────────────
        Task<List<UserTokenDto>> GetUserTokensAsync(int userId);
        Task<(bool Success, string Error)> RemoveUserTokenAsync(int userId, string loginProvider, string tokenName);
    }

    /// <summary>
    /// One row in the permissions matrix — represents a Form/Module with its functionalities.
    /// ClaimType is always "permission"; ClaimValue = ActionMethodName.
    /// </summary>
    public class FunctionalityGroupDto
    {
        public int FormId { get; set; }
        public string FormDisplayName { get; set; } = "";
        public List<FunctionalityItemDto> Items { get; set; } = new();
    }

    /// <summary>One checkbox cell in the matrix.</summary>
    public class FunctionalityItemDto
    {
        public int FunctionalityId { get; set; }
        public string FunctionalityName { get; set; } = "";  // shown in UI
        public string ActionMethodName { get; set; } = "";   // saved as claim value
        public bool IsGranted { get; set; }                  // checkbox state
    }

    /// <summary>DTO for AspNetUserTokens rows (Value is masked in the UI).</summary>
    public class UserTokenDto
    {
        public string LoginProvider { get; set; } = "";
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }
}

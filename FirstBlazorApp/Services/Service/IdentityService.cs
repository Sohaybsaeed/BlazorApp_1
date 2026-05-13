using FirstBlazorApp.Services.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FirstBlazorApp.Services.Service
{
    /// <summary>
    /// Concrete implementation of IIdentityService.
    /// Uses RoleManager&lt;ApplicationRole&gt; and UserManager&lt;ApplicationUser&gt;
    /// to manage all standard ASP.NET Identity tables.
    /// </summary>
    public class IdentityService : IIdentityService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBRetailDbContext _ctx;

        public IdentityService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IBRetailDbContext ctx)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _ctx = ctx;
        }

        // ── AspNetRoles ────────────────────────────────────────────────────

        /// <summary>Returns all rows from AspNetRoles.</summary>
        public async Task<List<ApplicationRole>> GetAllRolesAsync()
            => await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();

        /// <summary>Inserts a new row into AspNetRoles via RoleManager.</summary>
        public async Task<(bool Success, string Error)> CreateRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return (false, "Role name cannot be empty.");

            if (await _roleManager.RoleExistsAsync(roleName.Trim()))
                return (false, $"Role '{roleName}' already exists.");

            var result = await _roleManager.CreateAsync(new ApplicationRole { Name = roleName.Trim() });
            return result.Succeeded
                ? (true, "")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        /// <summary>Updates the Name of an existing AspNetRoles row via RoleManager.</summary>
        public async Task<(bool Success, string Error)> UpdateRoleAsync(int roleId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return (false, "Role name cannot be empty.");

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role is null) return (false, "Role not found.");

            // Check duplicate (excluding self)
            var existing = await _roleManager.FindByNameAsync(newName.Trim());
            if (existing != null && existing.Id != roleId)
                return (false, $"Role '{newName}' already exists.");

            role.Name = newName.Trim();
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded
                ? (true, "")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        /// <summary>
        /// Deletes an AspNetRoles row via RoleManager.
        /// Refuses if any users are still assigned to the role (AspNetUserRoles).
        /// </summary>
        public async Task<(bool Success, string Error)> DeleteRoleAsync(int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role is null) return (false, "Role not found.");

            // Guard: do not delete if users are assigned
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Any())
                return (false, $"Cannot delete '{role.Name}' — {usersInRole.Count} user(s) are still assigned to it.");

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded
                ? (true, "")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // ── AspNetUserRoles ────────────────────────────────────────────────

        /// <summary>Returns all ApplicationUser rows from AspNetUsers.</summary>
        public async Task<List<ApplicationUser>> GetAllUsersAsync()
            => await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();

        /// <summary>Returns role names from AspNetUserRoles for the given user.</summary>
        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return new();
            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        /// <summary>Inserts a row into AspNetUserRoles via UserManager.</summary>
        public async Task<(bool Success, string Error)> AssignRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return (false, "User not found.");

            if (await _userManager.IsInRoleAsync(user, roleName))
                return (false, $"User is already in role '{roleName}'.");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded
                ? (true, "")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        /// <summary>Removes a row from AspNetUserRoles via UserManager.</summary>
        public async Task<(bool Success, string Error)> RemoveRoleAsync(int userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return (false, "User not found.");

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded
                ? (true, "")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // ── AspNetRoleClaims (via ApplicationFunctionalities matrix) ──────

        /// <summary>Returns all raw claims from AspNetRoleClaims for the given role.</summary>
        public async Task<List<Claim>> GetRoleClaimsAsync(int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role is null) return new();
            return (await _roleManager.GetClaimsAsync(role)).ToList();
        }

        /// <summary>
        /// Builds the full permissions matrix from ApplicationFunctionalities.
        /// Grouped by MenuGroupName (no FormDetail join needed).
        /// Each item's IsGranted = true if the role already has a "permission" claim with that ActionMethodName.
        /// </summary>
        public async Task<List<FunctionalityGroupDto>> GetFunctionalitiesMatrixAsync(int roleId)
        {
            // Get current granted ActionMethodNames for this role
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            var grantedActions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (role is not null)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                foreach (var c in claims.Where(c => c.Type == "permission"))
                    grantedActions.Add(c.Value);
            }

            // Load all active functionalities — no FormDetail join
            var rows = await _ctx.ApplicationFunctionalities
                .Where(f => f.IsActive == true)
                .OrderBy(f => f.MenuGroupOrder ?? 99)
                .ThenBy(f => f.MenuGroupName)
                .ThenBy(f => f.FunctionalityName)
                .Select(f => new
                {
                    GroupName  = f.MenuGroupName  ?? "General",
                    GroupOrder = f.MenuGroupOrder ?? 99,
                    FuncId     = f.Id,
                    FuncName   = f.FunctionalityName ?? "",
                    ActionName = f.ActionMethodName  ?? ""
                })
                .ToListAsync();

            // Group by MenuGroupName
            return rows
                .GroupBy(r => new { r.GroupName, r.GroupOrder })
                .OrderBy(g => g.Key.GroupOrder)
                .Select(g => new FunctionalityGroupDto
                {
                    FormId          = 0,
                    FormDisplayName = g.Key.GroupName,
                    Items = g.Select(r => new FunctionalityItemDto
                    {
                        FunctionalityId   = r.FuncId,
                        FunctionalityName = r.FuncName,
                        ActionMethodName  = r.ActionName,
                        IsGranted         = grantedActions.Contains(r.ActionName)
                    }).ToList()
                })
                .ToList();
        }

        /// <summary>
        /// Saves the full matrix for a role into AspNetRoleClaims.
        /// Claim Type is always "permission". Claim Value = ActionMethodName.
        /// Adds newly checked items, removes unchecked ones — diff-based, no duplicates.
        /// </summary>
        public async Task<(bool Success, string Error)> SaveRolePermissionsAsync(int roleId, List<FunctionalityGroupDto> matrix)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role is null) return (false, "Role not found.");

            const string claimType = "permission";

            // Current claims in DB
            var existing = (await _roleManager.GetClaimsAsync(role))
                .Where(c => c.Type == claimType)
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Desired state from matrix
            var desired = matrix
                .SelectMany(g => g.Items)
                .Where(i => i.IsGranted && !string.IsNullOrWhiteSpace(i.ActionMethodName))
                .Select(i => i.ActionMethodName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Add new
            foreach (var toAdd in desired.Except(existing))
            {
                var result = await _roleManager.AddClaimAsync(role, new Claim(claimType, toAdd));
                if (!result.Succeeded)
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            // Remove unchecked
            foreach (var toRemove in existing.Except(desired))
            {
                var result = await _roleManager.RemoveClaimAsync(role, new Claim(claimType, toRemove));
                if (!result.Succeeded)
                    return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return (true, "");
        }

        // ── AspNetUserClaims ───────────────────────────────────────────────

        /// <summary>Returns all claims from AspNetUserClaims for the given user.</summary>
        public async Task<List<Claim>> GetUserClaimsAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return new();
            return (await _userManager.GetClaimsAsync(user)).ToList();
        }

        /// <summary>Inserts a row into AspNetUserClaims via UserManager.</summary>
        public async Task<(bool Success, string Error)> AddUserClaimAsync(int userId, string claimType, string claimValue)
        {
            if (string.IsNullOrWhiteSpace(claimType) || string.IsNullOrWhiteSpace(claimValue))
                return (false, "Claim type and value cannot be empty.");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return (false, "User not found.");

            var result = await _userManager.AddClaimAsync(user, new Claim(claimType.Trim(), claimValue.Trim()));
            return result.Succeeded
                ? (true, "")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        /// <summary>Removes a matching row from AspNetUserClaims via UserManager.</summary>
        public async Task<(bool Success, string Error)> RemoveUserClaimAsync(int userId, string claimType, string claimValue)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return (false, "User not found.");

            var result = await _userManager.RemoveClaimAsync(user, new Claim(claimType, claimValue));
            return result.Succeeded
                ? (true, "")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // ── AspNetUserLogins ───────────────────────────────────────────────

        /// <summary>Returns all rows from AspNetUserLogins for the given user.</summary>
        public async Task<List<UserLoginInfo>> GetUserLoginsAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return new();
            return (await _userManager.GetLoginsAsync(user)).ToList();
        }

        /// <summary>Removes a row from AspNetUserLogins via UserManager.</summary>
        public async Task<(bool Success, string Error)> RemoveUserLoginAsync(int userId, string loginProvider, string providerKey)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return (false, "User not found.");

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            return result.Succeeded
                ? (true, "")
                : (false, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // ── AspNetUserTokens ───────────────────────────────────────────────

        /// <summary>
        /// Returns all rows from AspNetUserTokens for the given user.
        /// Reads directly from the DbContext because UserManager has no GetTokensAsync.
        /// </summary>
        public async Task<List<UserTokenDto>> GetUserTokensAsync(int userId)
        {
            return await _ctx.UserTokens
                .Where(t => t.UserId == userId)
                .Select(t => new UserTokenDto
                {
                    LoginProvider = t.LoginProvider,
                    Name = t.Name,
                    Value = t.Value ?? ""
                })
                .ToListAsync();
        }

        /// <summary>Removes a row from AspNetUserTokens via UserManager.</summary>
        public async Task<(bool Success, string Error)> RemoveUserTokenAsync(int userId, string loginProvider, string tokenName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return (false, "User not found.");

            await _userManager.RemoveAuthenticationTokenAsync(user, loginProvider, tokenName);
            return (true, "");
        }
    }
}

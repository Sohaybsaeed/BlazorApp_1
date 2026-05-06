using FirstBlazorApp.Models.Custom_Models;
using FirstBlazorApp.Services.IService;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FirstBlazorApp.Models;


public class UserService : IUserService
{
    private readonly IBRetailDbContext _ctx;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _um;
    private readonly RoleManager<ApplicationRole> _rm;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserAccessService _userAccess;

    public UserService(IBRetailDbContext ctx,
                       SignInManager<ApplicationUser> signInManager,
                       UserManager<ApplicationUser> um,
                       RoleManager<ApplicationRole> rm,
                       IConfiguration config,
                       IHttpContextAccessor httpContextAccessor,
                       IUserAccessService userAccess)
    {
        _ctx = ctx;
        _signInManager = signInManager;
        _um = um;
        _rm = rm;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        _userAccess = userAccess;
    }

    public async Task<UserLoginResponse> LoginAsync(string email, string password)
    {
        var response = new UserLoginResponse();

        try
        {
            var user = await _um.FindByEmailAsync(email);
            if (user is null)
            {
                response.IsUserExists = false;
                response.Succeeded = false;
                return response;
            }

            response.IsUserExists = true;

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                response.Succeeded = false;
                response.IsLockedOut = result.IsLockedOut;
                response.IsNotAllowed = result.IsNotAllowed;
                return response;
            }

            response.Succeeded = true;

            // ── Base claims ────────────────────────────────────────────────
            var claims = new List<Claim>
            {
                new Claim("UserId",         user.Id.ToString()),
                new Claim("UserName",       user.UserName       ?? string.Empty),
                new Claim("RoleTemplateId", user.RoleTemplateId.ToString()),
                new Claim("BranchCode",     user.BranchCode     ?? string.Empty),
                new Claim("FirstName",      user.FirstName      ?? string.Empty),
                new Claim("LastName",       user.LastName       ?? string.Empty),
                new Claim("UserEmail",      user.Email          ?? string.Empty)
            };

            // ── Identity Roles (AspNetUserRoles) ───────────────────────────
            var roles = await _um.GetRolesAsync(user);
            foreach (var roleName in roles)
                claims.Add(new Claim(ClaimTypes.Role, roleName));

            // ── Role Claims (AspNetRoleClaims) — permission claims ─────────
            var addedPermissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var roleName in roles)
            {
                var role = await _rm.FindByNameAsync(roleName);
                if (role is null) continue;

                var roleClaims = await _rm.GetClaimsAsync(role);
                foreach (var rc in roleClaims)
                {
                    // Deduplicate — same permission from multiple roles added only once
                    var key = $"{rc.Type}|{rc.Value}";
                    if (addedPermissions.Add(key))
                        claims.Add(new Claim(rc.Type, rc.Value));
                }
            }

            // ── User Claims (AspNetUserClaims) — override / supplement ─────
            var userClaims = await _um.GetClaimsAsync(user);
            foreach (var uc in userClaims)
            {
                var key = $"{uc.Type}|{uc.Value}";
                if (addedPermissions.Add(key))
                    claims.Add(new Claim(uc.Type, uc.Value));
            }

            response.Claims = claims;

            // ── Session data ───────────────────────────────────────────────
            response.SessionData = new List<KeyValuePair<string, string>>
            {
                new("UserEmail",  email),
                new("LoggedIn",   DateTime.UtcNow.ToString("O")),
                new("UserName",   user.UserName   ?? string.Empty),
                new("FirstName",  user.FirstName  ?? string.Empty),
                new("LastName",   user.LastName   ?? string.Empty),
                new("BranchCode", user.BranchCode ?? string.Empty),
                new("UserId",     user.Id.ToString())
            };

            return response;
        }
        catch
        {
            return response;
        }
    }

    public async Task<List<ApplicationUser>> GetUsersAsync()
    {
        return await _um.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedDate)
            .ToListAsync();
    }

    public async Task<List<PermissionTemplate>> GetActiveRoleTemplatesAsync()
    {
        return await _ctx.PermissionTemplate
            .Where(x => x.IsActive)
            .OrderBy(x => x.TemplateName)
            .ToListAsync();
    }
    public async Task<ActionResponse> CreateUserAsync(ApplicationUser request)
    {
        var response = new ActionResponse { Success = true, ErrorMessage = "" };
            var existing = await _um.FindByEmailAsync(request.Email);
            if (existing != null)
                return new ActionResponse { Success = false, ErrorMessage = "Email already exists." };
            request.CreatedDate = DateTime.Now;
        var passwordHasher = new PasswordHasher<string>();
        var hashedPassword = passwordHasher.HashPassword(null, request.PasswordHash);
        request.PasswordHash = hashedPassword;
        try
        {
            var identityResult = await _um.CreateAsync(request);

            if (!identityResult.Succeeded)
            {
                return new ActionResponse
                {
                    Success = false,
                    ErrorMessage = string.Join(", ",
                        identityResult.Errors.Select(e => e.Description))
                };
            }

            response.Id = request.Id;
            return response;
        }
        catch (Exception ex)
        {
            return new ActionResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<List<RoleTemplateDD>> GetRoleTemplates()
    {
        var templates = await _ctx.PermissionTemplate.Select(x => new RoleTemplateDD
        {
            Text = x.TemplateName,
            Value = x.Id,
        })
        .ToListAsync();

        return templates;
    }

    public async Task<ApplicationUser?> FindUserByUserIdAsync(string userId)
    {
        return await _um.FindByIdAsync(userId);
    }
    public async Task<ActionResponse> UpdateUserAsync(ApplicationUser request)
    {
        try
        {
            var user = await _um.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return new ActionResponse { Success = false, ErrorMessage = "User not found." };

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.IsActive = request.IsActive;
            user.RoleTemplateId = request.RoleTemplateId;
            user.BranchCode = request.BranchCode;
            user.DateOfBirth = request.DateOfBirth;

            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailExists = await _um.FindByEmailAsync(request.Email);
                if (emailExists != null && emailExists.Id != user.Id)
                    return new ActionResponse { Success = false, ErrorMessage = "Email already exists." };

                user.Email = request.Email;
            }

            //user.ModifiedDate = DateTime.UtcNow;

            var result = await _um.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new ActionResponse
                {
                    Success = false,
                    ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            return new ActionResponse { Success = true, Id = user.Id };
        }
        catch (Exception ex)
        {
            return new ActionResponse { Success = false, ErrorMessage = ex.Message };
        }
    }


}
using FirstBlazorApp.Services.IService;

/// <summary>
/// Checks permissions using ASP.NET Identity claims loaded at login.
/// ClaimType = "permission", ClaimValue = ActionMethodName (from AspNetRoleClaims / AspNetUserClaims).
/// No database hit per request — claims are already in the authenticated principal.
/// </summary>
public sealed class RolePermissionChecker : IRolePermissionChecker
{
    private readonly IHttpContextAccessor _http;

    public RolePermissionChecker(IHttpContextAccessor http)
    {
        _http = http;
    }

    public Task<bool> HasPermissionAsync(string actionMethodName)
    {
        var user = _http.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return Task.FromResult(false);

        // Check if the principal has a "permission" claim matching the actionMethodName.
        // These claims are loaded from AspNetRoleClaims + AspNetUserClaims at login.
        var hasClaim = user.Claims.Any(c =>
            string.Equals(c.Type, "permission", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(c.Value, actionMethodName, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(hasClaim);
    }

    /// <summary>No-op — claims are session-based, no cache to clear.</summary>
    public void ClearRoleCache(int roleId) { }
}

using FirstBlazorApp.Services.IService;
using Microsoft.AspNetCore.Authorization;

public sealed class DbPermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IRolePermissionChecker _checker;

    public DbPermissionHandler(IRolePermissionChecker checker)
    {
        _checker = checker;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var isActionAllowed = await _checker.HasPermissionAsync(requirement.ActionMethodName);
        
        if (isActionAllowed)
        {
            context.Succeed(requirement);
        }
    }
}
//ww
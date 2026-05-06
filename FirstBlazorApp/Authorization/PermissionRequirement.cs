using Microsoft.AspNetCore.Authorization;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string ActionMethodName { get; }

    public PermissionRequirement(string actionMethodName)
    {
        ActionMethodName = actionMethodName;
    }
}

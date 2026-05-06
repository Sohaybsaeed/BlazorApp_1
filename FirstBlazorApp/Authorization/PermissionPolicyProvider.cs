using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public sealed class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }

    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        const string prefix = "Permission:";
        if (policyName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            var actionMethodName = policyName.Substring(prefix.Length);

            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(actionMethodName))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return base.GetPolicyAsync(policyName);
    }
}

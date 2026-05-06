using Microsoft.AspNetCore.Authorization;

namespace FirstBlazorApp.Extensions
{
    public static class AuthorizationExtension
    {
        public static void AddAuthorizationConfiguration(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddAuthorization();
            services.AddCascadingAuthenticationState();
            services.AddMemoryCache();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

            // Handler
            services.AddScoped<IAuthorizationHandler, DbPermissionHandler>();

        }
    }
}
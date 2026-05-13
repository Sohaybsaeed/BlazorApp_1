using FirstBlazorApp.Service.IServices;
using FirstBlazorApp.Services.IService;
using FirstBlazorApp.Services.Service;
using LicenseIssuer.Console.Services;
using RetailChannel.Service.Services;

namespace FirstBlazorApp.Extensions
{
    public static class DependencyInjectionSetup
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddRazorComponents().AddInteractiveServerComponents();

            services.AddHttpContextAccessor();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserAccessService, UserAccessService>();   // kept — used by menu/other features
            services.AddScoped<ILicenseService, LicenseService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IApplicationFunctionalitiesService, ApplicationFunctionalitiesService>();
            services.AddScoped<IFormDetailService, FormDetailService>();
            services.AddScoped<IPermissionTemplateService, PermissionTemplateService>(); // kept — old pages still reference it
            services.AddScoped<IRolePermissionChecker, RolePermissionChecker>();         // now uses Identity claims
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IDynamicMenuService, DynamicMenuService>();               // dynamic sidebar menu
            services.AddScoped<LicenseGenerator>();
        }
    }
}
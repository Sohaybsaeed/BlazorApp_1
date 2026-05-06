using Microsoft.AspNetCore.Identity;
using static ApplicationUser;

namespace FirstBlazorApp.Extensions
{
    public static class IdentityConfiguration
    {
        public static void AddIdentityConfiguration(this IServiceCollection services)
        {
            // Identity Configuration code goes here
            // Identity (Core) + cookies
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.User.RequireUniqueEmail = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<IBRetailDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/not-authorized";
            });
        }
    }
}
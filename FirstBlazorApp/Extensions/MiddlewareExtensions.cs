using FirstBlazorApp.Components;
using FirstBlazorApp.Endpoints;

namespace FirstBlazorApp.Extensions
{
    public static class MiddlewareExtensions
    {
        public static WebApplication UseConfiguredMiddlewares(this WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();        // REQUIRED for HttpContext.Session
            app.UseAuthentication(); // REQUIRED for Identity cookie auth
            app.UseAuthorization();

            app.UseAntiforgery();

            app.MapAuthEndpoints();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            return app;
        }

        public static WebApplication UseDevelopmentTools(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            return app;
        }
    }
}
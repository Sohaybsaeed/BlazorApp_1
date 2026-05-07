using FirstBlazorApp.Components;
using FirstBlazorApp.Endpoints;

namespace FirstBlazorApp.Extensions
{
    public static class MiddlewareExtensions
    {
        public static WebApplication UseConfiguredMiddlewares(this WebApplication app)
        {
            app.UseHttpsRedirection();

            // Serve static files with correct headers for PWA
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Service worker must not be cached by browser (always fetch fresh)
                    if (ctx.File.Name == "service-worker.js")
                    {
                        ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                        ctx.Context.Response.Headers["Pragma"] = "no-cache";
                        ctx.Context.Response.Headers["Expires"] = "0";
                    }
                    // Manifest — short cache
                    else if (ctx.File.Name == "manifest.json")
                    {
                        ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=3600";
                    }
                }
            });

            app.UseRouting();

            app.UseSession();        // REQUIRED for HttpContext.Session
            app.UseAuthentication(); // REQUIRED for Identity cookie auth
            app.UseAuthorization();

            app.UseAntiforgery();

            app.MapAuthEndpoints();
            app.MapPwaIcons();

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
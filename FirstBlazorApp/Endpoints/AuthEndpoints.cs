using FirstBlazorApp.Services.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FirstBlazorApp.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/login", async (HttpContext http,IUserService _service,SignInManager<ApplicationUser> signInManager) =>
            {
                var form = await http.Request.ReadFormAsync();

                var email = form["email"].ToString();
                var password = form["password"].ToString();
                var remember = true;

                var result = await _service.LoginAsync(email, password);

                if (result.Succeeded)
                {
                    var identity = new ClaimsIdentity(result.Claims, "Identity.Application");
                    var principal = new ClaimsPrincipal(identity);

                    await http.SignInAsync("Identity.Application", principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = remember,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                        });

                    foreach (var kv in result.SessionData)
                    {
                        http.Session.SetString(kv.Key, kv.Value);
                    }

                    return Results.Redirect("/dashboard?success=login");
                }

                if (result.IsLockedOut)
                    return Results.Redirect("/login?error=locked");

                if (result.IsNotAllowed)
                    return Results.Redirect("/login?error=notallowed");

                return Results.Redirect("/login?error=invalid");
            });


            app.MapGet("/auth/logout", async (HttpContext http, SignInManager<ApplicationUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                http.Session.Clear();
                return Results.Redirect("/login");
            });




        }
    }
}

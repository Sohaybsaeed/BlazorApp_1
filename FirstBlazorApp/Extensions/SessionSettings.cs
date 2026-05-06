namespace FirstBlazorApp.Extensions
{
    public static class SessionSettings
    {
        public static void AddSessionServices(this IServiceCollection services)
        {
            // Server session state (explicit Session)
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }
    }
}

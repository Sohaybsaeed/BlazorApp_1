namespace FirstBlazorApp.Endpoints
{
    /// <summary>
    /// Serves PWA icons as SVG (rendered as PNG by browser).
    /// Route: /icons/icon-{size}x{size}.png
    /// No external libraries needed — pure SVG response.
    /// </summary>
    public static class PwaIconEndpoint
    {
        private static readonly int[] ValidSizes = { 72, 96, 128, 144, 152, 192, 384, 512 };

        public static void MapPwaIcons(this IEndpointRouteBuilder app)
        {
            // Serve SVG icons at the PNG paths (browsers accept SVG for PWA icons)
            app.MapGet("/icons/icon-{size}x{sizeY}.png", (int size, int sizeY, HttpContext ctx) =>
            {
                if (!ValidSizes.Contains(size) || size != sizeY)
                    return Results.NotFound();

                var svg = GenerateSvgIcon(size);
                ctx.Response.Headers["Cache-Control"] = "public, max-age=86400";
                // Return as SVG — works for PWA manifest icons in all modern browsers
                return Results.Content(svg, "image/svg+xml");
            });

            // Also serve a dedicated SVG endpoint
            app.MapGet("/icons/icon.svg", (HttpContext ctx) =>
            {
                var svg = GenerateSvgIcon(512);
                ctx.Response.Headers["Cache-Control"] = "public, max-age=86400";
                return Results.Content(svg, "image/svg+xml");
            });
        }

        private static string GenerateSvgIcon(int size)
        {
            double r = size * 0.18;
            return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{size}"" height=""{size}"" viewBox=""0 0 {size} {size}"">
  <defs>
    <linearGradient id=""g"" x1=""0%"" y1=""0%"" x2=""100%"" y2=""100%"">
      <stop offset=""0%"" stop-color=""#6366f1""/>
      <stop offset=""100%"" stop-color=""#8b5cf6""/>
    </linearGradient>
    <clipPath id=""clip"">
      <rect width=""{size}"" height=""{size}"" rx=""{r}"" ry=""{r}""/>
    </clipPath>
  </defs>
  <rect width=""{size}"" height=""{size}"" rx=""{r}"" ry=""{r}"" fill=""url(#g)""/>
  <text x=""{size / 2}"" y=""{size * 0.62}"" 
        font-family=""'Segoe UI', Arial, sans-serif"" 
        font-size=""{(int)(size * 0.52)}"" 
        font-weight=""bold"" 
        fill=""rgba(255,255,255,0.95)"" 
        text-anchor=""middle"">E</text>
  <circle cx=""{(int)(size * 0.72)}"" cy=""{(int)(size * 0.28)}"" r=""{(int)(size * 0.06)}"" fill=""rgba(255,255,255,0.4)""/>
</svg>";
        }
    }
}

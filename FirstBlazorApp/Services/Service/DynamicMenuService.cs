using FirstBlazorApp.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace FirstBlazorApp.Services.Service
{
    /// <summary>
    /// Builds the sidebar menu purely from ApplicationFunctionalities.
    /// No FormDetail join needed — grouping info (MenuGroupName, MenuGroupIcon, MenuGroupOrder)
    /// is stored directly on each ApplicationFunctionalities row.
    ///
    /// Visibility rules:
    ///   - Unauthenticated users  → no items
    ///   - User has NO "permission" claims at all → show every menu item (graceful fallback)
    ///   - User HAS permission claims → show only items whose ActionMethodName is in the claims
    /// </summary>
    public class DynamicMenuService : IDynamicMenuService
    {
        private readonly IBRetailDbContext _ctx;
        private readonly IHttpContextAccessor _http;

        public DynamicMenuService(IBRetailDbContext ctx, IHttpContextAccessor http)
        {
            _ctx = ctx;
            _http = http;
        }

        public async Task<List<MenuGroupDto>> GetMenuForCurrentUserAsync()
        {
            var user = _http.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return new List<MenuGroupDto>();

            // ── 1. Resolve permission claims ───────────────────────────────
            var permClaims = user.Claims
                .Where(c => string.Equals(c.Type, "permission", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            bool hasAnyClaims = permClaims.Count > 0;

            // ── 2. Load active menu items directly from ApplicationFunctionalities ──
            var rows = await _ctx.ApplicationFunctionalities
                .Where(f => f.IsActive == true
                         && f.IsMenuItem == true
                         && f.MenuReferenceName != null
                         && f.MenuReferenceName != "")
                .OrderBy(f => f.MenuGroupOrder ?? 99)
                .ThenBy(f => f.MenuGroupName)
                .ThenBy(f => f.FunctionalityName)
                .Select(f => new
                {
                    GroupName  = f.MenuGroupName  ?? "General",
                    GroupIcon  = f.MenuGroupIcon  ?? "📄",
                    GroupOrder = f.MenuGroupOrder ?? 99,
                    FuncId     = f.Id,
                    FuncName   = f.FunctionalityName ?? "",
                    NavUrl     = f.MenuReferenceName ?? "",
                    ActionName = f.ActionMethodName  ?? ""
                })
                .ToListAsync();

            // ── 3. Filter by permission ────────────────────────────────────
            var visible = hasAnyClaims
                ? rows.Where(r => string.IsNullOrEmpty(r.ActionName) || permClaims.Contains(r.ActionName))
                : rows;

            // ── 4. Normalise NavUrl — ensure it starts with "/" ────────────
            var normalised = visible.Select(r =>
            {
                var nav = r.NavUrl.Trim();
                if (!nav.StartsWith("/"))
                    nav = "/" + nav.ToLower();
                return new
                {
                    r.GroupName,
                    r.GroupIcon,
                    r.GroupOrder,
                    r.FuncId,
                    r.FuncName,
                    NavUrl     = nav,
                    r.ActionName
                };
            });

            // ── 5. Group by MenuGroupName ──────────────────────────────────
            var groups = normalised
                .GroupBy(r => new { r.GroupName, r.GroupIcon, r.GroupOrder })
                .OrderBy(g => g.Key.GroupOrder)
                .ThenBy(g => g.Key.GroupName)
                .Select(g => new MenuGroupDto
                {
                    FormId      = 0,   // no longer needed
                    DisplayName = g.Key.GroupName,
                    IconCode    = g.Key.GroupIcon,
                    DisplayOrder = g.Key.GroupOrder,
                    Items = g.Select(r => new MenuItemDto
                    {
                        FunctionalityId   = r.FuncId,
                        FunctionalityName = r.FuncName,
                        NavigationUrl     = r.NavUrl,
                        ActionMethodName  = r.ActionName
                    }).ToList()
                })
                .ToList();

            return groups;
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirstBlazorApp.Services.IService
{
    /// <summary>
    /// Builds the sidebar menu dynamically from ApplicationFunctionalities + FormDetail,
    /// filtered to only the items the current user has a "permission" claim for.
    /// </summary>
    public interface IDynamicMenuService
    {
        /// <summary>
        /// Returns menu groups visible to the current user.
        /// Each group maps to a FormDetail row; each item inside maps to an
        /// ApplicationFunctionality row where IsMenuItem = true.
        /// </summary>
        Task<List<MenuGroupDto>> GetMenuForCurrentUserAsync();
    }

    /// <summary>One top-level menu group (maps to a FormDetail / module).</summary>
    public class MenuGroupDto
    {
        public int FormId { get; set; }

        /// <summary>Display label shown in the sidebar (FormDetail.DisplayName).</summary>
        public string DisplayName { get; set; } = "";

        /// <summary>Emoji / icon code (FormDetail.IconCode).</summary>
        public string IconCode { get; set; } = "";

        public int DisplayOrder { get; set; }

        /// <summary>
        /// Child items. When there is exactly one item and its NavigationUrl matches
        /// the group, the group renders as a direct link instead of an expandable section.
        /// </summary>
        public List<MenuItemDto> Items { get; set; } = new();
    }

    /// <summary>One leaf menu item (maps to an ApplicationFunctionality row).</summary>
    public class MenuItemDto
    {
        public int FunctionalityId { get; set; }

        /// <summary>Label shown in the sidebar (ApplicationFunctionalities.FunctionalityName).</summary>
        public string FunctionalityName { get; set; } = "";

        /// <summary>
        /// The Blazor route / href (ApplicationFunctionalities.MenuReferenceName).
        /// e.g. "/license", "/users/create"
        /// </summary>
        public string NavigationUrl { get; set; } = "";

        /// <summary>
        /// The permission claim value (ApplicationFunctionalities.ActionMethodName).
        /// Used only for visibility logic — not exposed in the UI.
        /// </summary>
        public string ActionMethodName { get; set; } = "";
    }
}

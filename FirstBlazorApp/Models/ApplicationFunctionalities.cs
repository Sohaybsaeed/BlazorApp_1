using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstBlazorApp.Models;

public partial class ApplicationFunctionalities
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? FunctionalityName { get; set; }

    /// <summary>Legacy FK — kept for backward compatibility, no longer used for menu grouping.</summary>
    public int? FormId { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? ActionMethodName { get; set; }

    public bool? IsMenuItem { get; set; }

    /// <summary>Blazor route for this menu item, e.g. "/license", "/users/create"</summary>
    [StringLength(150)]
    public string? MenuReferenceName { get; set; }

    /// <summary>Menu group label shown in the sidebar, e.g. "License Management", "User Management"</summary>
    [StringLength(100)]
    public string? MenuGroupName { get; set; }

    /// <summary>Emoji icon for the menu group, e.g. "📋", "👤"</summary>
    [StringLength(50)]
    public string? MenuGroupIcon { get; set; }

    /// <summary>Sort order for the menu group.</summary>
    public int? MenuGroupOrder { get; set; }
}

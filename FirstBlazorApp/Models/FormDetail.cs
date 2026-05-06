using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstBlazorApp.Models;

[Table("FormDetail")]
public partial class FormDetail
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? ControllerName { get; set; }

    [StringLength(50)]
    public string? ActionName { get; set; }

    [StringLength(50)]
    public string? FormName { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? DisplayName { get; set; }

    [StringLength(40)]
    public string? IconCode { get; set; }

    public int? DisplayOrder { get; set; }
}

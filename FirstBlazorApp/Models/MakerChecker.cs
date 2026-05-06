using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstBlazorApp.Models;

[Table("MakerChecker")]
public partial class MakerChecker
{
    [Key]
    public long Id { get; set; }

    public int FormId { get; set; }

    [Unicode(false)]
    public string? NewValueJson { get; set; }

    [Unicode(false)]
    public string? OldValueJson { get; set; }

    public bool? Active { get; set; }

    public int? ReferenceId { get; set; }

    public int MakerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime MakerDate { get; set; }

    public int MakerStatusId { get; set; }

    public int? CheckerId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CheckerDate { get; set; }

    public int? CheckerStatusId { get; set; }

    [StringLength(200)]
    public string? Notes { get; set; }

    public int RoleId { get; set; }
}

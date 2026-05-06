using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FirstBlazorApp.Models;

[Table("PermissionTemplate")]
public partial class PermissionTemplate
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string TemplateName { get; set; } = null!;

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDate { get; set; }

    [InverseProperty("Template")]
    public virtual ICollection<PermissionTemplateDetail> PermissionTemplateDetails { get; set; } = new List<PermissionTemplateDetail>();
}

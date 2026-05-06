using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FirstBlazorApp.Models;

[Table("PermissionTemplateDetail")]
public partial class PermissionTemplateDetail
{
    [Key]
    public int Id { get; set; }

    public int TemplateId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string FormName { get; set; } = null!;

    public int FunctionalityId { get; set; }

    public bool IsAllow { get; set; }

    [ForeignKey("TemplateId")]
    [InverseProperty("PermissionTemplateDetails")]
    public virtual PermissionTemplate Template { get; set; } = null!;
}

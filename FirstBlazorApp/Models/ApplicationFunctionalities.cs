using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FirstBlazorApp.Models;

public partial class ApplicationFunctionalities
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? FunctionalityName { get; set; }

    public int? FormId { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? ActionMethodName { get; set; }

    public bool? IsMenuItem { get; set; }

    [StringLength(150)]
    public string? MenuReferenceName { get; set; }
}

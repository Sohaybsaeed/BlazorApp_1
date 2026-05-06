using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FirstBlazorApp.Models;

public partial class UserActivityLog
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Controller { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Action { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string Path { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string Method { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string QueryString { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? RequestBody { get; set; }

    [Column(TypeName = "text")]
    public string? ResponseBody { get; set; }

    public long? UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Datetime { get; set; }

    public bool IsException { get; set; }

    [Column(TypeName = "text")]
    public string? Exception { get; set; }

    public bool IsAjaxRequest { get; set; }

    [StringLength(450)]
    public string? LogId { get; set; }
}

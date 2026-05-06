using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FirstBlazorApp.Models;

[Table("UserActivityHistory")]
public partial class UserActivityHistory
{
    [Key]
    public long Id { get; set; }

    [StringLength(450)]
    public string? LogId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Action { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? ActionMethod { get; set; }

    [Unicode(false)]
    public string? NewValueJson { get; set; }

    [Unicode(false)]
    public string? OldValueJson { get; set; }

    public long? UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }
}

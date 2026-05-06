using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FirstBlazorApp.Models;

public partial class KillSwitchLog
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UserId { get; set; } = null!;

    public long CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    public bool ActiveStatus { get; set; }
}

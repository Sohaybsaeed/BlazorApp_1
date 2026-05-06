using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FirstBlazorApp.Models;

public partial class UserLoginLog
{
    [Key]
    public long Id { get; set; }

    public int UserId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime LoginTime { get; set; }

    public bool LoginStatus { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string StatusMessage { get; set; } = null!;
}

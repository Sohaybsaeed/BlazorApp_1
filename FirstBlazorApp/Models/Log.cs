using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FirstBlazorApp.Models;

public partial class Log
{
    [StringLength(50)]
    [Unicode(false)]
    public string? Application { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Logged { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Level { get; set; }

    [Unicode(false)]
    public string? Message { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Logger { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Callsite { get; set; }

    public string? Exception { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }

    [Column("DeviceID")]
    [StringLength(50)]
    [Unicode(false)]
    public string? DeviceId { get; set; }

    [Column("IP")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Ip { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? FunctionName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ControllerName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndTime { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Unicode(false)]
    public string? RequestParameters { get; set; }

    [Unicode(false)]
    public string? RequestResponse { get; set; }

    [Unicode(false)]
    public string? Channel { get; set; }

    [StringLength(450)]
    public string? LogId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RequestDateTime { get; set; }

    [StringLength(100)]
    public string? ErrorCode { get; set; }

    [Key]
    public long Id { get; set; }

    [StringLength(450)]
    public string? RequestId { get; set; }
}

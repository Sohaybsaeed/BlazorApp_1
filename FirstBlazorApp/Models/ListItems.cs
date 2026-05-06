using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FirstBlazorApp.Models;

public partial class ListItems
{
    [Key]
    public int Id { get; set; }

    public int? ListTypeId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Text { get; set; }

    public int? Value { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }
}

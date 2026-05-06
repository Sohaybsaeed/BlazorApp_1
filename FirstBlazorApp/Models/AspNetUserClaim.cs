using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FirstBlazorApp.Models;

public partial class AspNetUserClaim
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AspNetUserClaims")]
    public virtual AspNetUser User { get; set; } = null!;
}

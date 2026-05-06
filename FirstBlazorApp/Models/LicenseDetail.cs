using System;
using System.Collections.Generic;

namespace FirstBlazorApp.Models;

public partial class LicenseDetail
{
    public int Id { get; set; }

    public Guid LicenseGuid { get; set; }

    public int BankId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public string PassPhrase { get; set; } = null!;

    public string PrivateKey { get; set; } = null!;
    public string PublicKey { get; set; } = null!;

    public string? XmlFile { get; set; }

    public bool IsActive { get; set; }
}

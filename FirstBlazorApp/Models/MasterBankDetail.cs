using System;
using System.Collections.Generic;

namespace FirstBlazorApp.Models;

public partial class MasterBankDetail
{
    public int Id { get; set; }

    public string BankName { get; set; } = null!;

    public bool IsActive { get; set; }
}

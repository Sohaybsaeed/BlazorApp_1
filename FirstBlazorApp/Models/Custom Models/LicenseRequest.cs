using System.ComponentModel.DataAnnotations;

namespace FirstBlazorApp.Models.Custom_Models
{
    public class LicenseRequest
    {
        [Required(ErrorMessage = "Please select a bank")]
        public int SelectedBank { get; set; }

        [Required(ErrorMessage = "Expiry date is required")]
        [FutureDate(ErrorMessage = "Expiry date must be in the future")]
        public DateOnly ExpiryDate { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        public string PassPhrase { get; set; } = "";

        public bool GenerateKeyPair { get; set; } = false;

        public List<MasterBankDetail> BankName { get; set; } = new();
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateOnly dateOnly)
            {
                return dateOnly > DateOnly.FromDateTime(DateTime.Now);
            }
            return false;
        }
    }
}
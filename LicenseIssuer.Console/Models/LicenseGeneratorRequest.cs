namespace LicenseIssuer.Console.Models
{
    public class LicenseGeneratorRequest
    {
        public string BankName { get; set; } = default!;
        public DateOnly ExpiryDate { get; set; }
        public string PassPhrase { get; set; } = default!;
        public bool IsKeyPairRequired { get; set; }
        public string? PrivateKey { get; set; }
        public string? PublicKey { get; set; }
    }
}
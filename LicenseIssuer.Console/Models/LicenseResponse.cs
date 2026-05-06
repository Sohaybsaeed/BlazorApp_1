namespace LicenseIssuer.Console.Models
{
    public class LicenseResponse
    {
        public string LicenseXml { get; set; } = string.Empty;
        public string LicenseFilePath { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
        public Guid LicenseGuid { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
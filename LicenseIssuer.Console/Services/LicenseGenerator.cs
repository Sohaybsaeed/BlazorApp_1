using LicenseIssuer.Console.Models;
using Standard.Licensing;
using Standard.Licensing.Security.Cryptography;
using System.Text.RegularExpressions;

namespace LicenseIssuer.Console.Services
{
    public class LicenseGenerator
    {
        private static string SafeFileName(string key)
        {
            var invalid = new string(Path.GetInvalidFileNameChars());
            var pattern = $"[{Regex.Escape(invalid)}]";
            var safe = Regex.Replace(key, pattern, "_");
            safe = safe.Replace("..", "_").Replace("/", "_").Replace("\\", "_").Trim();
            return safe;
        }

        private static string GetSolutionRoot()
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        }

        public LicenseResponse Generate(LicenseGeneratorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.BankName))
                throw new ArgumentException("Bank name cannot be empty.", nameof(request.BankName));

            if (string.IsNullOrWhiteSpace(request.PassPhrase))
                throw new ArgumentException("Pass phrase cannot be empty.", nameof(request.PassPhrase));

            var expiryDateTime = request.ExpiryDate.ToDateTime(TimeOnly.MinValue);

            var solutionRoot = GetSolutionRoot();
            var licensesDir = Path.Combine(solutionRoot, "licenses");
            Directory.CreateDirectory(licensesDir);

            string effectivePrivateKey;
            string effectivePublicKey;

            if (request.IsKeyPairRequired)
            {
                var keyGenerator = KeyGenerator.Create();
                var keyPair = keyGenerator.GenerateKeyPair();

                effectivePrivateKey = keyPair.ToEncryptedPrivateKeyString(request.PassPhrase);
                effectivePublicKey = keyPair.ToPublicKeyString();
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.PrivateKey))
                    throw new ArgumentException(
                        "Private key is required when KeyPairRequired is false.",
                        nameof(request.PrivateKey));

                if (string.IsNullOrWhiteSpace(request.PublicKey))
                    throw new ArgumentException(
                        "Public key is required when KeyPairRequired is false.",
                        nameof(request.PublicKey));

                effectivePrivateKey = request.PrivateKey!;
                effectivePublicKey = request.PublicKey!;
            }

            var licenseGuid = Guid.NewGuid();

            var license = License.New()
                .WithUniqueIdentifier(licenseGuid)
                .As(LicenseType.Standard)
                .ExpiresAt(expiryDateTime)
                .LicensedTo("BankName", request.BankName)
                .CreateAndSignWithPrivateKey(effectivePrivateKey, request.PassPhrase);

            var safeBank = SafeFileName(request.BankName);
            var licensePath = Path.Combine(licensesDir, $"{safeBank}.lic.xml");

            File.WriteAllText(licensePath, license.ToString());

            return new LicenseResponse
            {
                LicenseXml = license.ToString(),
                LicenseFilePath = licensePath,
                PublicKey = effectivePublicKey,
                PrivateKey = effectivePrivateKey,
                LicenseGuid = licenseGuid,
                ExpiryDate = expiryDateTime
            };
        }
    }
}
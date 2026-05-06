using Standard.Licensing;
using Standard.Licensing.Security.Cryptography;
using System.Text.RegularExpressions;

static string SafeFileName(string key)
{
    var invalid = new string(Path.GetInvalidFileNameChars());
    var pattern = $"[{Regex.Escape(invalid)}]";
    var safe = Regex.Replace(key, pattern, "_");
    safe = safe.Replace("..", "_").Replace("/", "_").Replace("\\", "_").Trim();
    return safe;
}

static string GetSolutionRoot()
{
    return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
}

Console.Write("Enter License Key: ");

var licenseKey = (Console.ReadLine() ?? "").Trim();

if (string.IsNullOrWhiteSpace(licenseKey))
{
    Console.WriteLine("Invalid key.");
    return;
}

var solutionRoot = GetSolutionRoot();
var keysDir = Path.Combine(solutionRoot, "keys");
var licensesDir = Path.Combine(solutionRoot, "licenses");

Directory.CreateDirectory(keysDir);
Directory.CreateDirectory(licensesDir);

var passPhrase = "CBT2025LicenseKeyGen";

var privateKeyPath = Path.Combine(keysDir, "private.key");
var publicKeyPath = Path.Combine(keysDir, "public.key");

//Salman Bhai will confirm that each bank will be having a new key pair or else it will use the default one

string privateKey;
string publicKey;

if (File.Exists(privateKeyPath) && File.Exists(publicKeyPath))
{
    privateKey = File.ReadAllText(privateKeyPath);
    publicKey = File.ReadAllText(publicKeyPath);
}
else
{
    var keyGenerator = KeyGenerator.Create();
    var keyPair = keyGenerator.GenerateKeyPair();

    privateKey = keyPair.ToEncryptedPrivateKeyString(passPhrase);
    publicKey = keyPair.ToPublicKeyString();

    File.WriteAllText(privateKeyPath, privateKey);
    File.WriteAllText(publicKeyPath, publicKey);

    Console.WriteLine($"Keys generated in: {keysDir}");
}
Guid licenseGuid = Guid.NewGuid();

var license = License.New().WithUniqueIdentifier(licenseGuid)
            .As(LicenseType.Standard)
            .ExpiresAt(DateTime.Now.AddYears(1))
            .LicensedTo("Service", licenseKey)
            .CreateAndSignWithPrivateKey(privateKey, passPhrase);

var key = SafeFileName(licenseKey);
var licensePath = Path.Combine(licensesDir, $"{key}.lic.xml");

File.WriteAllText(licensePath, license.ToString());

Console.WriteLine("License generated!");
Console.WriteLine($"License file: {licensePath}");
Console.WriteLine($"Public key file: {publicKeyPath}");

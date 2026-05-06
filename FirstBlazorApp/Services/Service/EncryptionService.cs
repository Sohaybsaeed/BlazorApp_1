using FirstBlazorApp.Services.IService;
using System.Security.Cryptography;
using System.Text;

namespace FirstBlazorApp.Services.Service
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IConfiguration _configuration;
        private readonly string _encryptionKey;

        public EncryptionService(IConfiguration configuration)
        {
            _configuration = configuration;
            _encryptionKey = _configuration["EncryptionSettings:Key"] ?? "DefaultKey123456789012345678901234"; // 32 chars for AES-256
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.Substring(0, 32)); // AES-256 requires 32 bytes
                    aes.GenerateIV();

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var msEncrypt = new MemoryStream())
                    {
                        // Prepend IV to the encrypted data
                        msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Encryption failed", ex);
            }
        }

        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return string.Empty;

            try
            {
                byte[] fullCipher = Convert.FromBase64String(encryptedText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.Substring(0, 32));

                    // Extract IV from the beginning of the encrypted data
                    byte[] iv = new byte[aes.BlockSize / 8];
                    byte[] cipher = new byte[fullCipher.Length - iv.Length];

                    Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                    Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var msDecrypt = new MemoryStream(cipher))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Decryption failed", ex);
            }
        }

        public string GenerateKey()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] keyBytes = new byte[32]; // 256 bits for AES-256
                rng.GetBytes(keyBytes);
                return Convert.ToBase64String(keyBytes);
            }
        }

        public bool IsValidEncryptedText(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return false;

            try
            {
                Convert.FromBase64String(encryptedText);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
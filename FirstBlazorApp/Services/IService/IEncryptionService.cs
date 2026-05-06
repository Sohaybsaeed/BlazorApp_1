using System;

namespace FirstBlazorApp.Services.IService
{
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypts the provided text using AES-256 encryption
        /// </summary>
        /// <param name="plainText">The text to encrypt</param>
        /// <returns>Base64 encoded encrypted string</returns>
        string Encrypt(string plainText);

        /// <summary>
        /// Decrypts the provided encrypted text
        /// </summary>
        /// <param name="encryptedText">Base64 encoded encrypted string</param>
        /// <returns>Decrypted plain text</returns>
        string Decrypt(string encryptedText);

        /// <summary>
        /// Generates a new encryption key
        /// </summary>
        /// <returns>Base64 encoded encryption key</returns>
        string GenerateKey();

        /// <summary>
        /// Validates if the provided text can be decrypted
        /// </summary>
        /// <param name="encryptedText">Base64 encoded encrypted string</param>
        /// <returns>True if valid, false otherwise</returns>
        bool IsValidEncryptedText(string encryptedText);
    }
}
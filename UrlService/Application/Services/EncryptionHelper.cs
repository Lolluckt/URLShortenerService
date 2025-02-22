using System;
using System.Security.Cryptography;
using System.Text;

namespace UrlService.Application.Services
{
    public static class EncryptionHelper
    {
        private const string AesKey = "vG8mN2bX9qP5wS6zL1oR3jK4tY7eH0cI";
        private const string AesIV = "A1s2D3f4G5h6J7k8";

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(AesKey);
            aes.IV = Encoding.UTF8.GetBytes(AesIV);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(AesKey);
            aes.IV = Encoding.UTF8.GetBytes(AesIV);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var cipherBytes = Convert.FromBase64String(cipherText);
            var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}

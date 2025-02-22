using NUnit.Framework;
using UrlService.Application.Services;


namespace UrlService.Tests.Services
{
    [TestFixture]
    public class EncryptionHelperTests
    {
        [Test]
        public void Encrypt_Decrypt_ShouldReturnOriginalString()
        {
            // Arrange
            var originalText = "Hello, world!";

            // Act
            var encrypted = EncryptionHelper.Encrypt(originalText);
            var decrypted = EncryptionHelper.Decrypt(encrypted);

            // Assert
            Assert.That(encrypted, Is.Not.EqualTo(originalText), "Зашифрованный текст не должен совпадать с исходным");
            Assert.That(decrypted, Is.EqualTo(originalText), "Расшифрованный текст должен совпадать с исходным");
        }

        [Test]
        public void Encrypt_WhenPlainTextIsNullOrEmpty_ShouldReturnSameValue()
        {
            // Arrange
            string emptyText = "";
            string nullText = null;

            // Act
            var encryptedEmpty = EncryptionHelper.Encrypt(emptyText);
            var encryptedNull = EncryptionHelper.Encrypt(nullText);

            // Assert
            Assert.That(encryptedEmpty, Is.EqualTo(emptyText));
            Assert.That(encryptedNull, Is.EqualTo(nullText));
        }

        [Test]
        public void Decrypt_WhenCipherTextIsNullOrEmpty_ShouldReturnSameValue()
        {
            // Arrange
            string emptyText = "";
            string nullText = null;

            // Act
            var decryptedEmpty = EncryptionHelper.Decrypt(emptyText);
            var decryptedNull = EncryptionHelper.Decrypt(nullText);

            // Assert
            Assert.That(decryptedEmpty, Is.EqualTo(emptyText));
            Assert.That(decryptedNull, Is.EqualTo(nullText));
        }
    }
}

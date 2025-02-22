using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlService.Application.Services;
using UrlService.Application.DTOs;
using UrlService.Domain.Entities;
using UrlService.Infrastructure.Interfaces;

namespace UrlService.Tests.Services
{
    [TestFixture]
    public class UrlShortenerServiceTests
    {
        private Mock<IUrlRepository> _urlRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private UrlShortenerService _urlShortenerService;

        [SetUp]
        public void SetUp()
        {
            _urlRepositoryMock = new Mock<IUrlRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _urlShortenerService = new UrlShortenerService(_urlRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Test]
        public async Task CreateUrlAsync_WhenUrlDoesNotExist_ShouldCreateNewUrlAndReturnUrlDto()
        {
            // Arrange
            var createUrlDto = new CreateUrlDto { OriginalUrl = "https://example.com" };
            var creatorUserId = 123;

            _urlRepositoryMock.Setup(repo => repo.ExistsByOriginalUrlAsync(It.IsAny<string>())).ReturnsAsync(false);
            _urlRepositoryMock.Setup(repo => repo.GetByShortUrlAsync(It.IsAny<string>())).ReturnsAsync((Url)null);
            _urlRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Url>())).Returns(Task.FromResult((Url)null));
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(creatorUserId))
                .ReturnsAsync(new User { Id = creatorUserId, Email = "creator@example.com" });

            // Act
            var result = await _urlShortenerService.CreateUrlAsync(createUrlDto, creatorUserId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.OriginalUrl, Is.EqualTo("https://example.com"));
            Assert.That(result.ShortUrl, Is.Not.Empty, "Короткая ссылка должна быть сгенерирована");
            Assert.That(result.CreatedByEmail, Is.EqualTo("creator@example.com"));

            _urlRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Url>()), Times.Once);
        }

        [Test]
        public void CreateUrlAsync_WhenUrlAlreadyExists_ShouldThrowException()
        {
            // Arrange
            var createUrlDto = new CreateUrlDto { OriginalUrl = "https://example.com" };
            _urlRepositoryMock.Setup(repo => repo.ExistsByOriginalUrlAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act & Assert
            Assert.That(async () => await _urlShortenerService.CreateUrlAsync(createUrlDto, 123),
                Throws.Exception.With.Message.EqualTo("Such a URL already exists!"));
        }


        [Test]
        public void GetUrlDetailsAsync_WhenUrlNotFoundOrDeleted_ShouldThrowException()
        {
            // Arrange
            var urlId = 999;
            _urlRepositoryMock.Setup(repo => repo.GetByIdAsync(urlId)).ReturnsAsync((Url)null);

            // Act & Assert
            Assert.That(async () => await _urlShortenerService.GetUrlDetailsAsync(urlId),
                Throws.Exception.With.Message.EqualTo("URL not found or deleted."));
        }

        [Test]
        public async Task DeleteUrlAsync_WhenCalledByAdmin_ShouldSoftDeleteUrl()
        {
            // Arrange
            var urlId = 1;
            var currentUserId = 999;
            var isAdmin = true;
            var url = new Url { Id = urlId, CreatedByUserId = 100, IsDeleted = false };

            _urlRepositoryMock.Setup(repo => repo.GetByIdAsync(urlId)).ReturnsAsync(url);
            _urlRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Url>())).Returns(Task.CompletedTask);

            // Act
            await _urlShortenerService.DeleteUrlAsync(urlId, currentUserId, isAdmin);

            // Assert
            Assert.That(url.IsDeleted, Is.True);
            _urlRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Url>(u => u.Id == urlId && u.IsDeleted)), Times.Once);
        }

        [Test]
        public void DeleteUrlAsync_WhenCalledByNonOwnerNonAdmin_ShouldThrowException()
        {
            // Arrange
            var urlId = 1;
            var currentUserId = 200;
            var isAdmin = false;
            var url = new Url { Id = urlId, CreatedByUserId = 100, IsDeleted = false };

            _urlRepositoryMock.Setup(repo => repo.GetByIdAsync(urlId)).ReturnsAsync(url);

            // Act & Assert
            Assert.That(async () => await _urlShortenerService.DeleteUrlAsync(urlId, currentUserId, isAdmin),
                Throws.Exception.With.Message.EqualTo("You do not have permission to delete this URL."));
        }

        [Test]
        public async Task GetOriginalUrlByShortAsync_WhenUrlExistsAndNotDeleted_ShouldReturnDecryptedUrl()
        {
            // Arrange
            var shortUrl = "abc123";
            var url = new Url { ShortUrl = shortUrl, OriginalUrl = EncryptionHelper.Encrypt("https://example.com"), IsDeleted = false };

            _urlRepositoryMock.Setup(repo => repo.GetByShortUrlAsync(shortUrl)).ReturnsAsync(url);

            // Act
            var result = await _urlShortenerService.GetOriginalUrlByShortAsync(shortUrl);

            // Assert
            Assert.That(result, Is.EqualTo("https://example.com"));
        }

        [Test]
        public async Task GetOriginalUrlByShortAsync_WhenUrlNotFoundOrDeleted_ShouldReturnNull()
        {
            // Arrange
            var shortUrl = "notfound";
            _urlRepositoryMock.Setup(repo => repo.GetByShortUrlAsync(shortUrl)).ReturnsAsync((Url)null);

            // Act
            var result = await _urlShortenerService.GetOriginalUrlByShortAsync(shortUrl);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}

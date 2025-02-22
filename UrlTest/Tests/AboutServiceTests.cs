using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using UrlService.Application.Services;
using UrlService.Application.DTOs;
using UrlService.Domain.Entities;
using UrlService.Domain.Roles;
using UrlService.Infrastructure.Interfaces;

namespace UrlService.Tests.Services
{
    [TestFixture]
    public class AboutServiceTests
    {
        private Mock<IAboutRepository> _aboutRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private AboutService _aboutService;

        [SetUp]
        public void SetUp()
        {
            _aboutRepositoryMock = new Mock<IAboutRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _aboutService = new AboutService(_aboutRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Test]
        public async Task GetAboutAsync_WhenEntityExists_ShouldReturnAboutDto()
        {
            // Arrange
            var about = new About
            {
                Id = 1,
                Content = "Some info",
                ModifiedDate = DateTime.UtcNow
            };

            _aboutRepositoryMock
                .Setup(repo => repo.GetSingleAsync())
                .ReturnsAsync(about);

            // Act
            var result = await _aboutService.GetAboutAsync();

            // Assert
            NUnit.Framework.Assert.That(result, Is.Not.Null);
            NUnit.Framework.Assert.That(result.Id, Is.EqualTo(1));
            NUnit.Framework.Assert.That(result.Content, Is.EqualTo("Some info"));
            NUnit.Framework.Assert.That(result.ModifiedDate, Is.Not.Null);
        }

        [Test]
        public async Task GetAboutAsync_WhenEntityDoesNotExist_ShouldReturnDefaultAboutDto()
        {
            // Arrange
            _aboutRepositoryMock
                .Setup(repo => repo.GetSingleAsync())
                .ReturnsAsync((About)null);

            // Act
            var result = await _aboutService.GetAboutAsync();

            // Assert
            NUnit.Framework.Assert.That(result, Is.Not.Null);
            NUnit.Framework.Assert.That(result.Id, Is.EqualTo(0));
            NUnit.Framework.Assert.That(result.Content, Is.EqualTo("No About info yet."));
            NUnit.Framework.Assert.That(result.ModifiedDate, Is.Null);
        }

        [Test]
        public void UpdateAboutAsync_WhenUserIsNotAdmin_ShouldThrowException()
        {
            // Arrange
            var aboutDto = new AboutDto
            {
                Id = 1,
                Content = "New content"
            };
            var adminUserId = 10;

            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(adminUserId))
                .ReturnsAsync(new User { Id = adminUserId, Role = Role.User }); // не админ

            // Act & Assert
            NUnit.Framework.Assert.That(async () => await _aboutService.UpdateAboutAsync(aboutDto, adminUserId),
                Throws.Exception.With.Message.EqualTo("You do not have permissions to update About."));
        }

        [Test]
        public void UpdateAboutAsync_WhenAboutRecordDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var aboutDto = new AboutDto
            {
                Id = 1,
                Content = "New content"
            };
            var adminUserId = 10;

            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(adminUserId))
                .ReturnsAsync(new User { Id = adminUserId, Role = Role.Admin });

            _aboutRepositoryMock
                .Setup(repo => repo.GetSingleAsync())
                .ReturnsAsync((About)null);

            // Act & Assert
            NUnit.Framework.Assert.That(async () => await _aboutService.UpdateAboutAsync(aboutDto, adminUserId),
                Throws.Exception.With.Message.EqualTo("No About record found in the database. Update operation cannot be performed."));
        }

        [Test]
        public async Task UpdateAboutAsync_WhenUserIsAdminAndRecordExists_ShouldUpdateAboutRecord()
        {
            // Arrange
            var aboutDto = new AboutDto
            {
                Id = 1,
                Content = "Updated content"
            };
            var adminUserId = 10;

            _userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(adminUserId))
                .ReturnsAsync(new User { Id = adminUserId, Role = Role.Admin });

            var aboutEntity = new About
            {
                Id = 1,
                Content = "Old content",
                ModifiedDate = null
            };

            _aboutRepositoryMock
                .Setup(repo => repo.GetSingleAsync())
                .ReturnsAsync(aboutEntity);

            _aboutRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<About>()))
                .Returns(Task.CompletedTask);

            // Act
            await _aboutService.UpdateAboutAsync(aboutDto, adminUserId);

            // Assert
            _aboutRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<About>(a =>
                a.Content == "Updated content" && a.ModifiedDate.HasValue)), Times.Once);
        }
    }
}
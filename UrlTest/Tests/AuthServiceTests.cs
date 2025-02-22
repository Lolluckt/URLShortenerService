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
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private AuthService _authService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _authService = new AuthService(_userRepositoryMock.Object);
        }

        [Test]
        public async Task RegisterAsync_WhenUserDoesNotExist_ShouldCreateUserAndReturnUserDto()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "test@example.com",
                Password = "123456"
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User)null);

            _userRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .Returns(Task.FromResult((User)null));

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(registerDto.Email));
            Assert.That(result.FirstName, Is.EqualTo(registerDto.FirstName));
            Assert.That(result.LastName, Is.EqualTo(registerDto.LastName));
            Assert.That(result.Role, Is.EqualTo(Role.User));

            _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public void RegisterAsync_WhenUserWithEmailAlreadyExists_ShouldThrowException()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "existing@example.com",
                Password = "123456"
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync(new User { Email = registerDto.Email });

            // Act & Assert
            Assert.That(async () => await _authService.RegisterAsync(registerDto),
                Throws.Exception.With.Message.EqualTo("User with this email already exists."));
        }

        [Test]
        public async Task LoginAsync_WhenUserExistsAndPasswordIsValid_ShouldReturnUserDto()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "correct_password"
            };

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correct_password");

            var user = new User
            {
                Email = loginDto.Email,
                PasswordHash = hashedPassword,
                FirstName = "John",
                LastName = "Doe",
                Role = Role.User
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.FirstName, Is.EqualTo(user.FirstName));
            Assert.That(result.LastName, Is.EqualTo(user.LastName));
            Assert.That(result.Role, Is.EqualTo(user.Role));
        }

        [Test]
        public void LoginAsync_WhenUserDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "non_existent@example.com",
                Password = "some_password"
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync((User)null);

            // Act & Assert
            Assert.That(async () => await _authService.LoginAsync(loginDto),
                Throws.Exception.With.Message.EqualTo("User not found."));
        }

        [Test]
        public void LoginAsync_WhenPasswordIsInvalid_ShouldThrowException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "wrong_password"
            };
            var user = new User
            {
                Email = loginDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct_password")
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            // Act & Assert
            Assert.That(async () => await _authService.LoginAsync(loginDto),
                Throws.Exception.With.Message.EqualTo("Invalid password."));
        }
    }
}

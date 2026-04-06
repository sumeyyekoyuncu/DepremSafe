using AutoMapper;
using DepremSafe.Core.DTOs;
using DepremSafe.Core.Entities;
using DepremSafe.Core.Interfaces;
using DepremSafe.Data.Context;
using DepremSafe.Service.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DepremSafe.Tests
{
    public class UserServiceTests : IDisposable
    {
        private readonly DepremSafeDbContext _context;
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserService _sut;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<DepremSafeDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DepremSafeDbContext(options);
            _mockRepo = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();

            _sut = new UserService(_mockRepo.Object, _mockMapper.Object, _context);
        }

        public void Dispose() => _context.Dispose();

        #region AddAsync

        [Fact]
        public async Task AddAsync_CreatesUserWithDefaultLocation()
        {
            // Arrange
            var dto = new UserDTO { Id = Guid.NewGuid(), City = "İstanbul" };
            var user = new User { Id = dto.Id, City = dto.City };

            _mockMapper.Setup(m => m.Map<User>(dto)).Returns(user);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            await _sut.AddAsync(dto);

            // Assert — kullanıcıya otomatik bir lokasyon eklenmiş olmalı
            _mockRepo.Verify(r => r.AddAsync(It.Is<User>(u =>
                u.Locations != null &&
                u.Locations.Count == 1 &&
                u.Locations.First().City == "İstanbul" &&
                u.Locations.First().Source == "Default"
            )), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DefaultLocation_HasZeroCoordinates()
        {
            // Arrange
            var dto = new UserDTO { Id = Guid.NewGuid(), City = "Ankara" };
            var user = new User { Id = dto.Id, City = dto.City };

            _mockMapper.Setup(m => m.Map<User>(dto)).Returns(user);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            await _sut.AddAsync(dto);

            // Assert — başlangıç koordinatları 0,0 olmalı
            _mockRepo.Verify(r => r.AddAsync(It.Is<User>(u =>
                u.Locations.First().Latitude == 0.0 &&
                u.Locations.First().Longitude == 0.0
            )), Times.Once);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            // Act
            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_UserFound_ReturnsMappedDto()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), City = "İzmir" };
            var dto = new UserDTO { Id = user.Id, City = "İzmir" };

            _mockRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDTO>(user)).Returns(dto);

            // Act
            var result = await _sut.GetByIdAsync(user.Id);

            // Assert
            result.Should().NotBeNull();
            result!.City.Should().Be("İzmir");
        }

        #endregion

        #region GetByEmail

        [Fact]
        public async Task GetByEmail_UserExists_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Email = "test@test.com", City = "Bursa", Username = "testuser" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetByEmail("test@test.com");

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be("test@test.com");
        }

        [Fact]
        public async Task GetByEmail_UserNotFound_ReturnsNull()
        {
            // Act
            var result = await _sut.GetByEmail("yok@test.com");

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region UpdateCityAsync

        [Fact]
        public async Task UpdateCityAsync_UserExists_UpdatesCity()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), City = "İstanbul", Username = "testuser" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            await _sut.UpdateCityAsync(user.Id, "Ankara");

            // Assert
            var updated = await _context.Users.FindAsync(user.Id);
            updated!.City.Should().Be("Ankara");
        }

        [Fact]
        public async Task UpdateCityAsync_UserNotFound_ThrowsException()
        {
            // Act
            var act = async () => await _sut.UpdateCityAsync(Guid.NewGuid(), "Ankara");

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("User not found");
        }

        #endregion

        #region CreateGoogleUser

        [Fact]
        public async Task CreateGoogleUser_SavesUserToDatabase()
        {
            // Act
            var result = await _sut.CreateGoogleUser("google@test.com", "Test User", "googleid123");

            // Assert
            var saved = await _context.Users.FindAsync(result.Id);
            saved.Should().NotBeNull();
            saved!.Email.Should().Be("google@test.com");
            saved.LoginProvider.Should().Be("Google");
        }

        [Fact]
        public async Task CreateGoogleUser_SetsIsSafeTrue()
        {
            // Act
            var result = await _sut.CreateGoogleUser("google@test.com", "Test User", "googleid123");

            // Assert
            result.IsSafe.Should().BeTrue();
        }

        #endregion
    }
}
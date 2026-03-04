using Moq;
using Xunit;
using FluentAssertions;
using BusinessLayer.Concrete;
using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using EntityLayer.DTOs.Auth;
using EntityLayer.Exceptions;
using EntityLayer.Constants;
using BusinessLayer.Utilities;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace FarmApi.Tests.AuthTest
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IRoleRepository> _mockRoleRepo;
        private readonly Mock<IUserRoleRepository> _mockUserRoleRepo;
        private readonly IAuthService _authService;
        private readonly string _dummyJwtSecret = "BenimCokGizliVeUzunTestAnahtarim123456";

        public AuthServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockRoleRepo = new Mock<IRoleRepository>();
            _mockUserRoleRepo = new Mock<IUserRoleRepository>();

            _authService = new AuthService(
                _mockUserRepo.Object,
                _mockRoleRepo.Object,
                _mockUserRoleRepo.Object,
                _dummyJwtSecret
            );
        }

        [Fact]
        public async Task Register_WhenUsernameAlreadyExists()
        {
            var dto = new RegisterDto { Username = "testuser", Password = "123" };
            var existingUser = new User { Id = 1, Username = "testuser" };

            _mockUserRepo.Setup(x => x.GetByUsernameAsync(dto.Username)).ReturnsAsync(existingUser);

            Func<Task> action = async () => await _authService.RegisterAsync(dto);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.UsernameAlreadyExists);
        }

        [Fact]
        public async Task Register_WhenSuccessful()
        {
            var dto = new RegisterDto { Username = "newuser", Password = "123" };
            var defaultRole = new Role { Id = 5, Name = "User" };

            _mockUserRepo.Setup(x => x.GetByUsernameAsync(dto.Username)).ReturnsAsync((User)null);
            _mockRoleRepo.Setup(x => x.GetByNameAsync("User")).ReturnsAsync(defaultRole);

            var result = await _authService.RegisterAsync(dto);

            result.Should().NotBeNull();
            result.Username.Should().Be("newuser");
            result.Balance.Should().Be(1000);

            _mockUserRepo.Verify(x => x.AddAsync(It.Is<User>(u =>
                u.Username == "newuser" &&
                u.Balance == 1000
            )), Times.Once);

            _mockUserRoleRepo.Verify(x => x.AddUserRoleAsync(result.Id, defaultRole.Id), Times.Once);
        }

        [Fact]
        public async Task Login_WhenUserDoesNotExist()
        {
            var dto = new LoginDto { Username = "ghostuser", Password = "123" };
            _mockUserRepo.Setup(x => x.GetByUsernameAsync(dto.Username)).ReturnsAsync((User)null);

            Func<Task> action = async () => await _authService.LoginAsync(dto);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.InvalidCredentials);
        }

        [Fact]
        public async Task Login_WhenPasswordIsIncorrect()
        {
            var dto = new LoginDto { Username = "realuser", Password = "WrongPassword" };
            var dbUser = new User
            {
                Id = 1,
                Username = "realuser",
                PasswordHash = PasswordHelper.HashPassword("CorrectPassword")
            };

            _mockUserRepo.Setup(x => x.GetByUsernameAsync(dto.Username)).ReturnsAsync(dbUser);

            Func<Task> action = async () => await _authService.LoginAsync(dto);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.InvalidCredentials);
        }

        [Fact]
        public async Task Login_WhenCredentialsAreValid()
        {
            var dto = new LoginDto { Username = "adminuser", Password = "123" };
            var dbUser = new User
            {
                Id = 1,
                Username = "adminuser",
                PasswordHash = PasswordHelper.HashPassword("123")
            };
            var userRoles = new List<Role> { new Role { Id = 1, Name = "Admin" } };

            _mockUserRepo.Setup(x => x.GetByUsernameAsync(dto.Username)).ReturnsAsync(dbUser);
            _mockUserRoleRepo.Setup(x => x.GetRolesAsync(dbUser.Id)).ReturnsAsync(userRoles);

            var token = await _authService.LoginAsync(dto);

            token.Should().NotBeNullOrWhiteSpace();
            token.Should().StartWith("ey");
        }
    }
}
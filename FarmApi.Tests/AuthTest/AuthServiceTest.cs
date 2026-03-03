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
        public void Register_WhenUsernameAlreadyExists()
        {
            var dto = new RegisterDto { Username = "testuser", Password = "123" };
            var existingUser = new User { Id = 1, Username = "testuser" };

            _mockUserRepo.Setup(x => x.GetByUsername(dto.Username)).Returns(existingUser);

            Action action = () => _authService.Register(dto);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.UsernameAlreadyExists);
        }

        [Fact]
        public void Register_WhenSuccessful()
        {
            var dto = new RegisterDto { Username = "newuser", Password = "123" };
            var defaultRole = new Role { Id = 5, Name = "User" };

            _mockUserRepo.Setup(x => x.GetByUsername(dto.Username)).Returns((User)null); 
            _mockRoleRepo.Setup(x => x.GetByName("User")).Returns(defaultRole); 

            var result = _authService.Register(dto);

            result.Should().NotBeNull();
            result.Username.Should().Be("newuser");
            result.Balance.Should().Be(1000);

            _mockUserRepo.Verify(x => x.Add(It.Is<User>(u =>
                u.Username == "newuser" &&
                u.Balance == 1000 &&
                !string.IsNullOrEmpty(u.PasswordHash) 
            )), Times.Once);

            _mockUserRoleRepo.Verify(x => x.AddUserRole(result.Id, defaultRole.Id), Times.Once);
        }

        [Fact]
        public void Login_WhenUserDoesNotExist()
        {
            var dto = new LoginDto { Username = "ghostuser", Password = "123" };
            _mockUserRepo.Setup(x => x.GetByUsername(dto.Username)).Returns((User)null);

            Action action = () => _authService.Login(dto);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.InvalidCredentials);
        }

        [Fact]
        public void Login_WhenPasswordIsIncorrect()
        {
            var dto = new LoginDto { Username = "realuser", Password = "WrongPassword" };

            var dbUser = new User
            {
                Id = 1,
                Username = "realuser",
                PasswordHash = PasswordHelper.HashPassword("CorrectPassword")
            };

            _mockUserRepo.Setup(x => x.GetByUsername(dto.Username)).Returns(dbUser);

            Action action = () => _authService.Login(dto);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.InvalidCredentials);
        }

        [Fact]
        public void Login_WhenCredentialsAreValid()
        {
            var dto = new LoginDto { Username = "adminuser", Password = "123" };

            var dbUser = new User
            {
                Id = 1,
                Username = "adminuser",
                PasswordHash = PasswordHelper.HashPassword("123")
            };

            var userRoles = new List<Role> { new Role { Id = 1, Name = "Admin" } };

            _mockUserRepo.Setup(x => x.GetByUsername(dto.Username)).Returns(dbUser);
            _mockUserRoleRepo.Setup(x => x.GetRoles(dbUser.Id)).Returns(userRoles);

            var token = _authService.Login(dto);

            token.Should().NotBeNullOrWhiteSpace();
            token.Should().StartWith("ey");
        }
    }
}
using Moq;
using Xunit;
using FluentAssertions;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using EntityLayer.DTOs.Role;
using EntityLayer.Exceptions;
using EntityLayer.Constants;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace FarmApi.Test
{
    public class RoleServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IRoleRepository> _mockRoleRepo;
        private readonly Mock<IUserRoleRepository> _mockUserRoleRepo;
        private readonly RoleService _roleService;

        public RoleServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockRoleRepo = new Mock<IRoleRepository>();
            _mockUserRoleRepo = new Mock<IUserRoleRepository>();

            _roleService = new RoleService(
                _mockUserRepo.Object,
                _mockRoleRepo.Object,
                _mockUserRoleRepo.Object);
        }

        [Fact]
        public async Task AssignRole_WhenUserAlreadyHasRole()
        {
            var dto = new AssignRoleDto { UserId = 1, RoleName = "Admin" };
            var fakeUser = new User { Id = 1 };
            var fakeRole = new Role { Id = 2, Name = "Admin" };

            _mockUserRepo.Setup(x => x.GetByIdAsync(dto.UserId)).ReturnsAsync(fakeUser);
            _mockRoleRepo.Setup(x => x.GetByNameAsync(dto.RoleName)).ReturnsAsync(fakeRole);
            _mockUserRoleRepo.Setup(x => x.HasRoleAsync(dto.UserId, dto.RoleName)).ReturnsAsync(true);

            Func<Task> action = async () => await _roleService.AssignRoleAsync(dto);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.UserAlreadyHasRole);

            _mockUserRoleRepo.Verify(x => x.AddUserRoleAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task AssignRole_Successful()
        {
            var dto = new AssignRoleDto { UserId = 1, RoleName = "Admin" };
            var fakeUser = new User { Id = 1 };
            var fakeRole = new Role { Id = 5, Name = "Admin" };

            _mockUserRepo.Setup(x => x.GetByIdAsync(dto.UserId)).ReturnsAsync(fakeUser);
            _mockRoleRepo.Setup(x => x.GetByNameAsync(dto.RoleName)).ReturnsAsync(fakeRole);
            _mockUserRoleRepo.Setup(x => x.HasRoleAsync(dto.UserId, dto.RoleName)).ReturnsAsync(false);

            await _roleService.AssignRoleAsync(dto);

            _mockUserRoleRepo.Verify(x => x.AddUserRoleAsync(dto.UserId, fakeRole.Id), Times.Once);
        }

        [Fact]
        public async Task RemoveRole_UserDoesNotHaveRole()
        {
            var dto = new RemoveRoleDto { UserId = 1, RoleName = "Admin" };
            var fakeUser = new User { Id = 1 };
            var fakeRole = new Role { Id = 2, Name = "Admin" };

            _mockUserRepo.Setup(x => x.GetByIdAsync(dto.UserId)).ReturnsAsync(fakeUser);
            _mockRoleRepo.Setup(x => x.GetByNameAsync(dto.RoleName)).ReturnsAsync(fakeRole);
            _mockUserRoleRepo.Setup(x => x.HasRoleAsync(dto.UserId, dto.RoleName)).ReturnsAsync(false);

            Func<Task> action = async () => await _roleService.RemoveRoleAsync(dto);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.UserDoesNotHaveRole);

            _mockUserRoleRepo.Verify(x => x.RemoveUserRoleAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RemoveRole_Successful()
        {
            var dto = new RemoveRoleDto { UserId = 1, RoleName = "Admin" };
            var fakeUser = new User { Id = 1 };
            var fakeRole = new Role { Id = 2, Name = "Admin" };

            _mockUserRepo.Setup(x => x.GetByIdAsync(dto.UserId)).ReturnsAsync(fakeUser);
            _mockRoleRepo.Setup(x => x.GetByNameAsync(dto.RoleName)).ReturnsAsync(fakeRole);
            _mockUserRoleRepo.Setup(x => x.HasRoleAsync(dto.UserId, dto.RoleName)).ReturnsAsync(true);

            await _roleService.RemoveRoleAsync(dto);

            _mockUserRoleRepo.Verify(x => x.RemoveUserRoleAsync(dto.UserId, fakeRole.Id), Times.Once);
        }

        [Fact]
        public async Task GetRoles_ReturnListOfRoleNames()
        {
            int userId = 1;
            var fakeRoles = new List<Role>
            {
                new Role { Name = "Admin" },
                new Role { Name = "Mod" }
            };

            _mockUserRoleRepo.Setup(x => x.GetRolesAsync(userId)).ReturnsAsync(fakeRoles);

            var result = await _roleService.GetRolesAsync(userId);

            result.Should().HaveCount(2);
            result.Should().Contain("Admin");
            result.Should().Contain("Mod");
        }
    }
}
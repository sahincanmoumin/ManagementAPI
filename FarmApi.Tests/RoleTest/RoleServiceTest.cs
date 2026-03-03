using Moq;
using Xunit;
using FluentAssertions;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using EntityLayer.DTOs.Role;
using EntityLayer.DTOs.User;
using EntityLayer.Exceptions;
using EntityLayer.Constants;
using System.Collections.Generic;
using System.Linq;
using System;

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
        public void AssignRole_WhenUserAlreadyHasRole()
        {
            var dto = new AssignRoleDto { UserId = 1, RoleName = "Admin" };
            var fakeUser = new User { Id = 1 };
            var fakeRole = new Role { Id = 2, Name = "Admin" };

            _mockUserRepo.Setup(x => x.GetById(dto.UserId)).Returns(fakeUser);
            _mockRoleRepo.Setup(x => x.GetByName(dto.RoleName)).Returns(fakeRole);

            _mockUserRoleRepo.Setup(x => x.HasRole(dto.UserId, dto.RoleName)).Returns(true);

            Action action = () => _roleService.AssignRole(dto);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.UserAlreadyHasRole);

            _mockUserRoleRepo.Verify(x => x.AddUserRole(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void AssignRole_Successful()
        {
            var dto = new AssignRoleDto { UserId = 1, RoleName = "Admin" };
            var fakeUser = new User { Id = 1 };
            var fakeRole = new Role { Id = 5, Name = "Admin" };

            _mockUserRepo.Setup(x => x.GetById(dto.UserId)).Returns(fakeUser);
            _mockRoleRepo.Setup(x => x.GetByName(dto.RoleName)).Returns(fakeRole);
            _mockUserRoleRepo.Setup(x => x.HasRole(dto.UserId, dto.RoleName)).Returns(false);

            _roleService.AssignRole(dto);

            _mockUserRoleRepo.Verify(x => x.AddUserRole(dto.UserId, fakeRole.Id), Times.Once);
        }

        [Fact]
        public void RemoveRole_UserDoesNotHaveRole()
        {
            var dto = new RemoveRoleDto { UserId = 1, RoleName = "Admin" };
            var fakeUser = new User { Id = 1 };
            var fakeRole = new Role { Id = 2, Name = "Admin" };

            _mockUserRepo.Setup(x => x.GetById(dto.UserId)).Returns(fakeUser);
            _mockRoleRepo.Setup(x => x.GetByName(dto.RoleName)).Returns(fakeRole);
            _mockUserRoleRepo.Setup(x => x.HasRole(dto.UserId, dto.RoleName)).Returns(false);

            Action action = () => _roleService.RemoveRole(dto);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.UserDoesNotHaveRole);

            _mockUserRoleRepo.Verify(x => x.RemoveUserRole(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void RemoveRole_Successful()
        {
            var dto = new RemoveRoleDto { UserId = 1, RoleName = "Admin" };
            var fakeUser = new User { Id = 1 };
            var fakeRole = new Role { Id = 2, Name = "Admin" };

            _mockUserRepo.Setup(x => x.GetById(dto.UserId)).Returns(fakeUser);
            _mockRoleRepo.Setup(x => x.GetByName(dto.RoleName)).Returns(fakeRole);
            _mockUserRoleRepo.Setup(x => x.HasRole(dto.UserId, dto.RoleName)).Returns(true);

            _roleService.RemoveRole(dto);

            _mockUserRoleRepo.Verify(x => x.RemoveUserRole(dto.UserId, fakeRole.Id), Times.Once);
        }

        [Fact]
        public void GetRoles_ReturnListOfRoleNames()
        {
            int userId = 1;
            var fakeRoles = new List<Role>
            {
                new Role { Name = "Admin" },
                new Role { Name = "Mod" }
            };

            _mockUserRoleRepo.Setup(x => x.GetRoles(userId)).Returns(fakeRoles);

            var result = _roleService.GetRoles(userId);

            result.Should().HaveCount(2);
            result.Should().Contain("Admin");
            result.Should().Contain("Mod");
        }
    }
}
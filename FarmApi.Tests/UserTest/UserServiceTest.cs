using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using BusinessLayer.Utilities;
using DataAccessLayer.Abstract;
using Entity.Enums;
using EntityLayer.Constants;
using EntityLayer.DTOs.Animal;
using EntityLayer.DTOs.User;
using EntityLayer.Entities;
using EntityLayer.Exceptions;
using FluentAssertions;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FarmApi.Tests.UserTest
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly IUserService _userService;

        public UserServiceTest()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepo.Object);
        }

        [Fact]
        public async Task GetUserById_WhenUserNotFound()
        {
            int testid = 1;
            _mockUserRepo.Setup(r => r.GetByIdAsync(testid)).ReturnsAsync((User)null);

            Func<Task> action = async () => await _userService.GetByIdAsync(testid);

            await action.Should().ThrowAsync<BusinessException>()
                .Where(e => e.ErrorKey == ErrorKeys.UserNotFound);
        }

        [Fact]
        public async Task GetUserById_Succesful()
        {
            int testid = 1;
            var user = new User { Id = testid, Username = "Can", Balance = 500 };

            _mockUserRepo.Setup(r => r.GetByIdAsync(testid)).ReturnsAsync(user);

            var result = await _userService.GetByIdAsync(testid);

            result.Should().NotBeNull();
            result.Id.Should().Be(testid);
            result.Username.Should().Be("Can");
            result.Balance.Should().Be(500);
        }

        [Fact]
        public async Task UpdateUser_Succesful()
        {
            int testid = 1;
            var user = new User { Id = testid, Username = "Can", PasswordHash = "oldhash", Balance = 500 };
            var dto = new UpdateUserDto { Username = "CanUpdated", Password = "testUpdate" };

            _mockUserRepo.Setup(r => r.GetByIdAsync(testid)).ReturnsAsync(user);

            await _userService.UpdateUserAsync(testid, dto);

            _mockUserRepo.Verify(x => x.UpdateAsync(It.Is<User>(u => u.Id == testid)), Times.Once);
            user.Username.Should().Be("CanUpdated");
            user.PasswordHash.Should().NotBe("oldhash");
        }

        [Fact]
        public async Task GetBalance_WhenUserExists()
        {
            int userId = 1;
            _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId, Balance = 500.50m });

            var result = await _userService.GetBalanceAsync(userId);

            result.Should().Be(500.50m);
        }

        [Fact]
        public async Task GetAllUsers_WithUsernameFilter()
        {
            var filter = new UserFilterDto { UserName = "admin", PageNumber = 1, PageSize = 10 };
            var fakeUsersList = new List<User>
            {
                new User { Id = 1, Username = "admin_test", Balance = 100 },
                new User { Id = 2, Username = "superuser", Balance = 200 }, 
                new User { Id = 3, Username = "administrator", Balance = 300 }
            };

            var mockQueryable = fakeUsersList.BuildMock();

            _mockUserRepo.Setup(x => x.GetQueryable()).Returns(mockQueryable);

            var result = await _userService.GetAllUsersAsync(filter);

            result.TotalRecords.Should().Be(2); 
            result.Data.Should().HaveCount(2);
        }
    }
}
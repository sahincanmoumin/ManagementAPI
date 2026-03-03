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
using Moq;
using System.Collections.Generic;
using System.Linq;
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
        public void GetUserById_WhenUserNotFound()
        {
            int testid = 1;
            var user = new User { Id = testid };

            _mockUserRepo.Setup(r => r.GetById(testid)).Returns((User)null);

            Action action = () => _userService.GetById(testid);

            action.Should().Throw<BusinessException>()
                .Where(e => e.Message == ErrorKeys.UserNotFound);

        }
        [Fact]
        public void GetUserById_Succesful() 
        {
            int testid = 1;
            var user = new User { Id = testid, Username ="Can",Balance=500};

            _mockUserRepo.Setup(r => r.GetById(testid)).Returns(user);

           var result = _userService.GetById(testid);

            result.Should().NotBeNull();
            result.Id.Should().Be(testid);
            result.Username.Should().Be("Can");
            result.Balance.Should().Be(500);

        }
        [Fact]
        public void UpdateUser_Succesful()
        {
            int testid = 1;
            var user = new User { Id = testid, Username = "Can",PasswordHash= "oldhash", Balance = 500 };
            var dto = new UpdateUserDto { Username = "CanUpdated" ,Password= "testUpdate" };
           

            _mockUserRepo.Setup(r => r.GetById(testid)).Returns(user);
            
            _userService.UpdateUser(testid, dto);

  
            _mockUserRepo.Verify(x => x.Update(It.Is<User>(u => u.Id == testid)), Times.Once);
            user.Username.Should().Be("CanUpdated");
            user.PasswordHash.Should().NotBe("oldhash");
        }
        [Fact]
        public void GetBalance_WhenUserExists()
        {
            int userId = 1;
            _mockUserRepo.Setup(x => x.GetById(userId)).Returns(new User { Id = userId, Balance = 500.50m });

            var result = _userService.GetBalance(userId);

            result.Should().Be(500.50m);
        }
        [Fact]
        public void GetAllUsers_WithUsernameFilter()
        {
            var filter = new UserFilterDto { UserName = "admin", PageNumber = 1, PageSize = 10 };
            var fakeUsers = new List<User>
            {
                new User { Id = 1, Username = "admin_test", Balance = 100 },
                new User { Id = 2, Username = "superuser", Balance = 200 }, 
                new User { Id = 3, Username = "administrator", Balance = 300 }
            }.AsQueryable();

            _mockUserRepo.Setup(x => x.GetQueryable()).Returns(fakeUsers);

            var result = _userService.GetAllUsers(filter);

            result.TotalRecords.Should().Be(2); 
            result.Data.Should().HaveCount(2);
            result.Data.All(u => u.Username.Contains("admin")).Should().BeTrue();
        }

    }
}

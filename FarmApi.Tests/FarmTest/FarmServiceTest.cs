using Moq;
using Xunit;
using FluentAssertions;
using BusinessLayer.Concrete;
using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using EntityLayer.DTOs.Farm;
using EntityLayer.Exceptions;
using EntityLayer.Constants;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FarmApi.Tests.FarmTest
{
    public class FarmServiceTests
    {
        private readonly Mock<IFarmRepository> _mockFarmRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly IFarmService _farmService;

        public FarmServiceTests()
        {
            _mockFarmRepo = new Mock<IFarmRepository>();
            _mockUserRepo = new Mock<IUserRepository>();

            _farmService = new FarmService(_mockFarmRepo.Object, _mockUserRepo.Object);
        }


        [Fact]
        public void CreateFarm_WhenUserNotFound()
        {
            int invalidUserId = 99;
            var dto = new CreateFarmDto { Name = "Benim Çiftliğim" };

            _mockUserRepo.Setup(x => x.GetById(invalidUserId)).Returns((User)null);

            Action action = () => _farmService.CreateFarm(invalidUserId, dto);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.UserNotFound);

            _mockFarmRepo.Verify(x => x.Add(It.IsAny<Farm>()), Times.Never);
        }

        [Fact]
        public void CreateFarm_WhenSuccessful_ShouldAddFarm()
        {
            int userId = 1;
            var dto = new CreateFarmDto { Name = "test farm" };
            var fakeUser = new User { Id = userId, Username = "testuser" };

            _mockUserRepo.Setup(x => x.GetById(userId)).Returns(fakeUser);

            var result = _farmService.CreateFarm(userId, dto);

            result.Should().NotBeNull();
            result.Name.Should().Be(dto.Name);
            result.UserId.Should().Be(userId);

            _mockFarmRepo.Verify(x => x.Add(It.Is<Farm>(f =>
                f.Name == dto.Name &&
                f.UserId == userId
            )), Times.Once);
        }


        [Fact]
        public void GetById_WhenFarmNotFound()
        {
            int invalidFarmId = 999;
            _mockFarmRepo.Setup(x => x.GetById(invalidFarmId)).Returns((Farm)null);

            Action action = () => _farmService.GetById(invalidFarmId);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.FarmNotFound);
        }

        [Fact]
        public void GetById_WhenSuccessful()
        {
            int farmId = 10;
            var fakeFarm = new Farm { Id = farmId, Name = "Süper Çiftlik" };

            _mockFarmRepo.Setup(x => x.GetById(farmId)).Returns(fakeFarm);

            var result = _farmService.GetById(farmId);

            result.Should().NotBeNull();
            result.Id.Should().Be(farmId);
            result.Name.Should().Be("Süper Çiftlik");
        }


        [Fact]
        public void GetUserFarms()
        {
            int userId = 1;
            var filter = new FarmFilterDto { Name = "Gunes", PageNumber = 1, PageSize = 2 };

            var fakeUser = new User { Id = userId, Username = "ciftci_ali" };
            var otherUser = new User { Id = 2, Username = "veli" };

            var fakeFarms = new List<Farm>
            {
                new Farm { Id = 1, UserId = userId, Name = "Gunesli Vadi", User = fakeUser },
                new Farm { Id = 2, UserId = userId, Name = "Karanlık Vadi", User = fakeUser },
                new Farm { Id = 3, UserId = userId, Name = "Gunes Tepesi", User = fakeUser },
                new Farm { Id = 4, UserId = 2, Name = "Başkasına Ait Gunes", User = otherUser } 
            }.AsQueryable();

            _mockFarmRepo.Setup(x => x.GetQueryable()).Returns(fakeFarms);

            var result = _farmService.GetUserFarms(userId, filter);

            result.Should().NotBeNull();
            result.TotalRecords.Should().Be(2);
            result.Data.All(f => f.Name.Contains("Gunes")).Should().BeTrue();
            result.Data.First().UserName.Should().Be("ciftci_ali");
        }

        [Fact]
        public void UpdateFarm_WhenFarmNotFound()
        {
            int invalidFarmId = 999;
            var dto = new UpdateFarmDto { Name = "Yeni İsim" };

            _mockFarmRepo.Setup(x => x.GetById(invalidFarmId)).Returns((Farm)null);

            Action action = () => _farmService.UpdateFarm(invalidFarmId, dto);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.FarmNotFound);

            _mockFarmRepo.Verify(x => x.Update(It.IsAny<Farm>()), Times.Never);
        }

        [Fact]
        public void UpdateFarm_WhenSuccessful()
        {
            int farmId = 5;
            var dto = new UpdateFarmDto { Name = "Değişen Çiftlik" };
            var existingFarm = new Farm { Id = farmId, Name = "Eski Çiftlik" };

            _mockFarmRepo.Setup(x => x.GetById(farmId)).Returns(existingFarm);

            _farmService.UpdateFarm(farmId, dto);

            existingFarm.Name.Should().Be(dto.Name);

            _mockFarmRepo.Verify(x => x.Update(It.Is<Farm>(f => f.Id == farmId && f.Name == dto.Name)), Times.Once);
        }


        [Fact]
        public void DeleteFarm_WhenFarmNotFound()
        {
            int invalidFarmId = 999;

            _mockFarmRepo.Setup(x => x.GetById(invalidFarmId)).Returns((Farm)null);

            Action action = () => _farmService.DeleteFarm(invalidFarmId);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.FarmNotFound);

            _mockFarmRepo.Verify(x => x.Delete(It.IsAny<Farm>()), Times.Never);
        }

        [Fact]
        public void DeleteFarm_WhenSuccessful()
        {
            int farmId = 5;
            var existingFarm = new Farm { Id = farmId, Name = "Silinecek Çiftlik" };

            _mockFarmRepo.Setup(x => x.GetById(farmId)).Returns(existingFarm);

            _farmService.DeleteFarm(farmId);

            _mockFarmRepo.Verify(x => x.Delete(It.Is<Farm>(f => f.Id == farmId)), Times.Once);
        }

        
    }
}
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using EntityLayer.Constants;
using EntityLayer.DTOs.Farm;
using EntityLayer.Entities;
using EntityLayer.Exceptions;
using FluentAssertions;
using MockQueryable;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

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
        public async Task CreateFarm_WhenUserNotFound()
        {
            int invalidUserId = 99;
            var dto = new CreateFarmDto { Name = "Benim Çiftliğim" };
            _mockUserRepo.Setup(x => x.GetByIdAsync(invalidUserId)).ReturnsAsync((User)null);

            Func<Task> action = async () => await _farmService.CreateFarmAsync(invalidUserId, dto);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.UserNotFound);

            _mockFarmRepo.Verify(x => x.AddAsync(It.IsAny<Farm>()), Times.Never);
        }

        [Fact]
        public async Task CreateFarm_WhenSuccessful_ShouldAddFarm()
        {
            int userId = 1;
            var dto = new CreateFarmDto { Name = "test farm" };
            var fakeUser = new User { Id = userId, Username = "testuser" };
            _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(fakeUser);

            var result = await _farmService.CreateFarmAsync(userId, dto);

            result.Should().NotBeNull();
            result.Name.Should().Be(dto.Name);
            _mockFarmRepo.Verify(x => x.AddAsync(It.Is<Farm>(f =>
                f.Name == dto.Name && f.UserId == userId
            )), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenFarmNotFound()
        {
            int invalidFarmId = 999;
            _mockFarmRepo.Setup(x => x.GetByIdAsync(invalidFarmId)).ReturnsAsync((Farm)null);

            Func<Task> action = async () => await _farmService.GetByIdAsync(invalidFarmId);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.FarmNotFound);
        }

        [Fact]
        public async Task GetUserFarms_Async()
        {
            int userId = 1;
            var filter = new FarmFilterDto { Name = "Gunes", PageNumber = 1, PageSize = 2 };
            var fakeUser = new User { Id = userId, Username = "ciftci_ali" };

            var fakeFarmsList = new List<Farm>
            {
                new Farm { Id = 1, UserId = userId, Name = "Gunesli Vadi", User = fakeUser },
                new Farm { Id = 2, UserId = userId, Name = "Karanlık Vadi", User = fakeUser },
                new Farm { Id = 3, UserId = userId, Name = "Gunes Tepesi", User = fakeUser }
            };

            var mockQueryable = fakeFarmsList.BuildMock();

            _mockFarmRepo.Setup(x => x.GetQueryable()).Returns(mockQueryable);

            var result = await _farmService.GetUserFarmsAsync(userId, filter);

            result.Should().NotBeNull();
            result.Data.All(f => f.Name.Contains("Gunes")).Should().BeTrue();
        }
        [Fact]
        public async Task UpdateFarm_WhenSuccessful()
        {
            int farmId = 5;
            var dto = new UpdateFarmDto { Name = "Değişen Çiftlik" };
            var existingFarm = new Farm { Id = farmId, Name = "Eski Çiftlik" };
            _mockFarmRepo.Setup(x => x.GetByIdAsync(farmId)).ReturnsAsync(existingFarm);

            await _farmService.UpdateFarmAsync(farmId, dto);

            existingFarm.Name.Should().Be(dto.Name);
            _mockFarmRepo.Verify(x => x.UpdateAsync(It.Is<Farm>(f => f.Id == farmId)), Times.Once);
        }

        [Fact]
        public async Task DeleteFarm_WhenSuccessful()
        {
            int farmId = 5;
            var existingFarm = new Farm { Id = farmId, Name = "Silinecek Çiftlik" };
            _mockFarmRepo.Setup(x => x.GetByIdAsync(farmId)).ReturnsAsync(existingFarm);

            await _farmService.DeleteFarmAsync(farmId);

            _mockFarmRepo.Verify(x => x.DeleteAsync(It.Is<Farm>(f => f.Id == farmId)), Times.Once);
        }
    }
}
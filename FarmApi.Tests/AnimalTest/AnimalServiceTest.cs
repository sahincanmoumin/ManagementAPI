using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using Entity.Enums;
using EntityLayer.Constants;
using EntityLayer.DTOs.Animal;
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

namespace FarmApi.Tests.AnimalTest
{
    public class AnimalServiceTests
    {
        private readonly Mock<IAnimalRepository> _mockAnimalRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IFarmRepository> _mockFarmRepo;
        private readonly IAnimalService _animalService;

        public AnimalServiceTests()
        {
            _mockAnimalRepo = new Mock<IAnimalRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockFarmRepo = new Mock<IFarmRepository>();

            _animalService = new AnimalService(
                _mockAnimalRepo.Object,
                _mockUserRepo.Object,
                _mockFarmRepo.Object
            );
        }

        [Fact]
        public async Task GetFarmAnimalsTest()
        {
            int userId = 1;
            var filter = new AnimalFilterDto { PageNumber = 1, PageSize = 4 };

            var fakeFarmsList = new List<Farm>
            {
                new Farm { UserId = 1, Animals = new List<Animal> { new Animal { Name = "A" }, new Animal { Name = "B" }, new Animal { Name = "C" }}},
                new Farm { UserId = 2, Animals = new List<Animal> { new Animal { Name = "D" } }}
            };

            var mockQueryable = fakeFarmsList.BuildMock();

            _mockFarmRepo.Setup(x => x.GetQueryable()).Returns(mockQueryable);

            var result = await _animalService.GetFarmAnimalsAsync(userId, filter);

            result.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
            result.Data.Should().AllBeOfType<AnimalListDto>();
        }

        [Fact]
        public async Task BuyAnimal_InsufficientBalance()
        {
            int userId = 1;
            var buyDto = new BuyAnimalDto { FarmId = 10, Type = AnimalType.Cow, Name = "Inek" };
            var fakeUser = new User { Id = userId, Balance = 100 };
            var fakeFarm = new Farm { Id = 10 };

            _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(fakeUser);
            _mockFarmRepo.Setup(x => x.GetByIdAsync(buyDto.FarmId)).ReturnsAsync(fakeFarm);

            Func<Task> action = async () => await _animalService.BuyAnimalAsync(userId, buyDto);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.InsufficientBalance);

            _mockAnimalRepo.Verify(x => x.AddAsync(It.IsAny<Animal>()), Times.Never);
        }

        [Fact]
        public async Task BuyAnimal_Success()
        {
            int userId = 1;
            var buyDto = new BuyAnimalDto { FarmId = 10, Type = AnimalType.Chicken, Name = "tes Tavuk" };
            var fakeUser = new User { Id = userId, Balance = 100 };
            var fakeFarm = new Farm { Id = 10 };

            _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(fakeUser);
            _mockFarmRepo.Setup(x => x.GetByIdAsync(buyDto.FarmId)).ReturnsAsync(fakeFarm);

            var result = await _animalService.BuyAnimalAsync(userId, buyDto);

            fakeUser.Balance.Should().Be(50);
            _mockUserRepo.Verify(x => x.UpdateAsync(fakeUser), Times.Once);
            _mockAnimalRepo.Verify(x => x.AddAsync(It.IsAny<Animal>()), Times.Once);

            result.Should().NotBeNull();
            result.Price.Should().Be(50);
        }

        

        [Fact]
        public async Task GetById_WhenAnimalDoesNotExist()
        {

            int invalidAnimalId = 999;
            _mockAnimalRepo.Setup(x => x.GetByIdAsync(invalidAnimalId)).ReturnsAsync((Animal)null);

            Func<Task> action = async () => await _animalService.GetByIdAsync(invalidAnimalId);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.AnimalNotFound);
        }
    }
}
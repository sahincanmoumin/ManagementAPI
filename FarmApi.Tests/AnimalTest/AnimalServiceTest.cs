using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using Entity.Enums;
using EntityLayer.Constants;
using EntityLayer.DTOs.Animal;
using EntityLayer.Entities;
using EntityLayer.Exceptions;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
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
        public void GetFarmAnimalsTest()
        { 
            int userId = 1;

            var filter = new AnimalFilterDto { PageNumber = 1, PageSize = 4 };

            var fakeFarms = new List<Farm>
            {
                new Farm { UserId = 1, Animals = new List<Animal> {new Animal { Name = "A" }, new Animal { Name = "B" }, new Animal { Name = "C" }}},

                new Farm { UserId = 2, Animals = new List<Animal> { new Animal { Name = "D" } }}
            }.AsQueryable();
        
            _mockFarmRepo.Setup(x => x.GetQueryable()).Returns(fakeFarms);
            var result = _animalService.GetFarmAnimals(userId, filter);
            result.Data.Should().HaveCount(3);
            result.Should().NotBeNull();
            result.Data.Should().AllBeOfType<AnimalListDto>();
        }
        [Fact]
        public void BuyAnimal_InsufficientBalance()
        {
            int userId = 1;
            var buyDto = new BuyAnimalDto { FarmId = 10, Type = AnimalType.Cow, Name = "Inek" }; 
            var fakeUser = new User { Id = userId, Balance = 100 };
            var fakeFarm = new Farm { Id = 10 };

            _mockUserRepo.Setup(x => x.GetById(userId)).Returns(fakeUser);
            _mockFarmRepo.Setup(x => x.GetById(buyDto.FarmId)).Returns(fakeFarm);

            Action action = () => _animalService.BuyAnimal(userId, buyDto);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.InsufficientBalance);

            _mockAnimalRepo.Verify(x => x.Add(It.IsAny<Animal>()), Times.Never);
        }

        [Fact]
        public void BuyAnimal_Success()
        {
            int userId = 1;
            var buyDto = new BuyAnimalDto { FarmId = 10, Type = AnimalType.Chicken, Name = "tes Tavuk" }; 

            var fakeUser = new User { Id = userId, Balance = 100 }; 
            var fakeFarm = new Farm { Id = 10 };

            _mockUserRepo.Setup(x => x.GetById(userId)).Returns(fakeUser);
            _mockFarmRepo.Setup(x => x.GetById(buyDto.FarmId)).Returns(fakeFarm);

            var result = _animalService.BuyAnimal(userId, buyDto);

            fakeUser.Balance.Should().Be(50);
            _mockUserRepo.Verify(x => x.Update(fakeUser), Times.Once); 

            _mockAnimalRepo.Verify(x => x.Add(It.IsAny<Animal>()), Times.Once);

            result.Should().NotBeNull();
            result.Price.Should().Be(50);
            result.LifeSpanDays.Should().Be(180); 
        }
        [Fact]
        public void BuyAnimal_Correct_Changes()
        {
            int userId = 1;
            decimal initialBalance = 1000m;
            var buyDto = new BuyAnimalDto { Name = "test inek", Type = AnimalType.Cow, FarmId = 10 };
            decimal cowPrice = 500m; 
            var fakeUser = new User { Id = userId, Balance = initialBalance };
            var fakeFarm = new Farm { Id = 10 };

            _mockUserRepo.Setup(x => x.GetById(userId)).Returns(fakeUser);
            _mockFarmRepo.Setup(x => x.GetById(buyDto.FarmId)).Returns(fakeFarm);

            _animalService.BuyAnimal(userId, buyDto);

            _mockUserRepo.Verify(x => x.Update(It.Is<User>(u =>
                u.Id == userId &&
                u.Balance == (initialBalance - cowPrice) 
            )), Times.Once);

            _mockAnimalRepo.Verify(x => x.Add(It.Is<Animal>(a =>
                a.Name == buyDto.Name &&
                a.Type == buyDto.Type &&
                a.Price == cowPrice &&
                a.FarmId == buyDto.FarmId
            )), Times.Once);
        }

        [Fact]
        public void SellAnimal_WhenUserIsNotOwner()
        {
            int fakeUserId = 99; 
            int animalId = 5;
            
            var otheruser = new User { Id = 1};
            var fakeUser = new User { Id = fakeUserId };

            var fakeAnimal = new Animal { Id = animalId, FarmId = 10, Price = 100 };
            var fakeFarm = new Farm { Id = 10, UserId = 1 };

            _mockUserRepo.Setup(x => x.GetById(fakeUserId)).Returns(fakeUser);
            _mockAnimalRepo.Setup(x => x.GetById(animalId)).Returns(fakeAnimal);
            _mockFarmRepo.Setup(x => x.GetById(fakeAnimal.FarmId)).Returns(fakeFarm);

            Action action = () => _animalService.SellAnimal(fakeUserId, animalId);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.FarmNotFound);
        }
        
        [Fact]
        public void GetById_WhenAnimalDoesNotExist()
        {
            int invalidAnimalId = 999;
            _mockAnimalRepo.Setup(x => x.GetById(invalidAnimalId)).Returns((Animal)null);
            Action action = () => _animalService.GetById(invalidAnimalId);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.AnimalNotFound);
        }
    }
}
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using Entity.Enums;
using EntityLayer.Constants;
using EntityLayer.DTOs.Product;
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

namespace FarmApi.Test
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IAnimalRepository> _mockAnimalRepo;
        private readonly IProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepo = new Mock<IProductRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockAnimalRepo = new Mock<IAnimalRepository>();

            _productService = new ProductService(
                _mockProductRepo.Object,
                _mockUserRepo.Object,
                _mockAnimalRepo.Object);
        }

        [Fact]
        public async Task GetProductWithOwnership_WhenProductNotFound()
        {
            int invalidProductId = 999;
            _mockProductRepo.Setup(x => x.GetByIdWithDetailsAsync(invalidProductId)).ReturnsAsync((Product)null);

            Func<Task> action = async () => await _productService.GetProductWithOwnershipAsync(invalidProductId);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.ProductNotFound);
        }

        [Fact]
        public async Task GetProductWithOwnership_WhenSuccessful()
        {
            int productId = 1;
            var fakeProduct = new Product { Id = productId, Name = ProductType.Milk, Price = 50 };
            _mockProductRepo.Setup(x => x.GetByIdWithDetailsAsync(productId)).ReturnsAsync(fakeProduct);

            var result = await _productService.GetProductWithOwnershipAsync(productId);

            result.Should().NotBeNull();
            result.Name.Should().Be(ProductType.Milk);
            result.Id.Should().Be(productId);
        }

        [Fact]
        public async Task SellProduct_WhenUserNotFound()
        {
            int userId = 99;
            _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User)null);

            Func<Task> action = async () => await _productService.SellProductAsync(userId, 1);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.UserNotFound);

            _mockProductRepo.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task SellProduct_WhenProductAlreadySold()
        {
            int userId = 1;
            int productId = 5;
            var fakeUser = new User { Id = userId };
            var fakeProduct = new Product { Id = productId, IsSold = true };

            _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(fakeUser);
            _mockProductRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(fakeProduct);

            Func<Task> action = async () => await _productService.SellProductAsync(userId, productId);

            await action.Should().ThrowAsync<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.ProductAlreadySold);

            _mockUserRepo.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task SellProduct_WhenSuccessful()
        {
            int userId = 1;
            int productId = 5;
            decimal initialBalance = 100m;
            decimal productPrice = 25m;

            var fakeUser = new User { Id = userId, Balance = initialBalance };
            var fakeProduct = new Product { Id = productId, Price = productPrice, IsSold = false, AnimalId = 10 };
            var fakeAnimal = new Animal { Id = 10 };

            _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(fakeUser);
            _mockProductRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(fakeProduct);
            _mockAnimalRepo.Setup(x => x.GetByIdAsync(fakeProduct.AnimalId)).ReturnsAsync(fakeAnimal);

            await _productService.SellProductAsync(userId, productId);

            _mockUserRepo.Verify(x => x.UpdateAsync(It.Is<User>(u =>
                u.Id == userId && u.Balance == 125m
            )), Times.Once);

            _mockProductRepo.Verify(x => x.UpdateAsync(It.Is<Product>(p =>
                p.Id == productId && p.IsSold == true
            )), Times.Once);
        }

        [Fact]
        public async Task GetUnsoldProducts_OnlyUnsoldProduct()
        {
            int targetUserId = 1;
            var filter = new ProductFilterDto { PageNumber = 1, PageSize = 11 };

            var targetFarm = new Farm { UserId = targetUserId };
            var targetAnimal = new Animal { Farm = targetFarm };
            var otherAnimal = new Animal { Farm = new Farm { UserId = 2 } };

            var fakeProductsList = new List<Product>
            {
                new Product { Id = 1, IsSold = false, Animal = targetAnimal, Price = 10 },
                new Product { Id = 2, IsSold = true, Animal = targetAnimal, Price = 20 },
                new Product { Id = 3, IsSold = false, Animal = otherAnimal, Price = 30 }
            };

            var mockQueryable = fakeProductsList.BuildMock();

            _mockProductRepo.Setup(x => x.GetQueryable()).Returns(mockQueryable);

            var result = await _productService.GetUnsoldProductsAsync(targetUserId, filter);

            result.Should().NotBeNull();
            result.TotalRecords.Should().Be(1);
            result.Data.First().Id.Should().Be(1);
        }
    }
}
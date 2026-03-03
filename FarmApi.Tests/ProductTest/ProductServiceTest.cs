using Moq;
using Xunit;
using FluentAssertions;
using BusinessLayer.Concrete;
using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using EntityLayer.DTOs.Product;
using EntityLayer.Exceptions;
using EntityLayer.Constants;
using Entity.Enums;
using System.Collections.Generic;
using System.Linq;
using System;

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
        public void GetProductWithOwnership_WhenProductNotFound()
        {
            int invalidProductId = 999;
            _mockProductRepo.Setup(x => x.GetByIdWithDetails(invalidProductId)).Returns((Product)null);

            Action action = () => _productService.GetProductWithOwnership(invalidProductId);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.ProductNotFound);
        }

        [Fact]
        public void GetProductWithOwnership_WhenSuccessful()
        {
            int productId = 1;
            var fakeProduct = new Product { Id = productId, Name = ProductType.Milk, Price = 50 };
            _mockProductRepo.Setup(x => x.GetByIdWithDetails(productId)).Returns(fakeProduct);

            var result = _productService.GetProductWithOwnership(productId);

            result.Should().NotBeNull();
            result.Name.Should().Be(ProductType.Milk);
            result.Price.Should().Be(50);
            result.Id.Should().Be(productId);
        }

        [Fact]
        public void SellProduct_WhenUserNotFound()
        {
            int userId = 99;
            _mockUserRepo.Setup(x => x.GetById(userId)).Returns((User)null);

            Action action = () => _productService.SellProduct(userId, 1);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.UserNotFound);

            _mockProductRepo.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public void SellProduct_WhenProductAlreadySold()
        {
            int userId = 1;
            int productId = 5;
            var fakeUser = new User { Id = userId };
            var fakeProduct = new Product { Id = productId, IsSold = true }; 

            _mockUserRepo.Setup(x => x.GetById(userId)).Returns(fakeUser);
            _mockProductRepo.Setup(x => x.GetById(productId)).Returns(fakeProduct);

            Action action = () => _productService.SellProduct(userId, productId);

            action.Should().Throw<BusinessException>()
                  .Where(ex => ex.ErrorKey == ErrorKeys.ProductAlreadySold);

            _mockUserRepo.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void SellProduct_WhenSuccessful()
        {
            int userId = 1;
            int productId = 5;
            decimal initialBalance = 100m;
            decimal productPrice = 25m;

            var fakeUser = new User { Id = userId, Balance = initialBalance };
            var fakeProduct = new Product { Id = productId, Price = productPrice, IsSold = false, AnimalId = 10 };
            var fakeAnimal = new Animal { Id = 10 };

            _mockUserRepo.Setup(x => x.GetById(userId)).Returns(fakeUser);
            _mockProductRepo.Setup(x => x.GetById(productId)).Returns(fakeProduct);
            _mockAnimalRepo.Setup(x => x.GetById(fakeProduct.AnimalId)).Returns(fakeAnimal);

            _productService.SellProduct(userId, productId);

            _mockUserRepo.Verify(x => x.Update(It.Is<User>(u =>
                u.Id == userId &&
                u.Balance == 125m
            )), Times.Once);

            _mockProductRepo.Verify(x => x.Update(It.Is<Product>(p =>
                p.Id == productId &&
                p.IsSold == true &&
                p.SoldAt != null
            )), Times.Once);
        }

      
        [Fact]
        public void GetUnsoldProducts_OnlyUnsoldProduct()
        {
            int targetUserId = 1;
            var filter = new ProductFilterDto { PageNumber = 1, PageSize = 11 };

            var targetFarm = new Farm { UserId = targetUserId };
            var targetAnimal = new Animal { Farm = targetFarm };
            var otherAnimal = new Animal { Farm = new Farm { UserId = 2 } };

            var fakeProducts = new List<Product>
            {
                new Product { Id = 1, IsSold = false, Animal = targetAnimal, Price = 10 }, 
                new Product { Id = 2, IsSold = true, Animal = targetAnimal, Price = 20 },  
                new Product { Id = 3, IsSold = false, Animal = otherAnimal, Price = 30 }  
            }.AsQueryable();

            _mockProductRepo.Setup(x => x.GetQueryable()).Returns(fakeProducts);

            var result = _productService.GetUnsoldProducts(targetUserId, filter);

            result.Should().NotBeNull();
            result.TotalRecords.Should().Be(1); 
            result.Data.First().Id.Should().Be(1);
            result.Data.First().IsSold.Should().BeFalse();
        }
    }
}
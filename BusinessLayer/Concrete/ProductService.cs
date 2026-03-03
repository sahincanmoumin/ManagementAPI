using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using EntityLayer.Constants;
using EntityLayer.DTOs.Pagination;
using EntityLayer.DTOs.Product;
using EntityLayer.Entities;
using EntityLayer.Exceptions;
using Microsoft.EntityFrameworkCore;
using Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAnimalRepository _animalRepository;
        

        public ProductService(IProductRepository productRepository, IUserRepository userRepository, IAnimalRepository animalRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _animalRepository = animalRepository;
            
        }
        public Product GetProductWithOwnership(int productId)
        {
            var product = _productRepository.GetByIdWithDetails(productId) ?? throw new BusinessException(ErrorKeys.ProductNotFound);
            return (product);
        }
        public void SellProduct(int userId, int productId)
        {
            var user = _userRepository.GetById(userId) ?? throw new BusinessException(ErrorKeys.UserNotFound);      
            var product = _productRepository.GetById(productId) ?? throw new BusinessException(ErrorKeys.ProductNotFound);

            if (product.IsSold)
                throw new BusinessException(ErrorKeys.ProductAlreadySold);

            var animal = _animalRepository.GetById(product.AnimalId);
            
            user.Balance += product.Price;
            _userRepository.Update(user);

            product.IsSold = true;
            product.SoldAt = DateTime.Now;
            _productRepository.Update(product);
        }

        public PagedResponse<ProductListDto> GetAnimalProducts(int userId, ProductFilterDto filter, int? animalId = null)
        {
            
            var query = _productRepository.GetQueryable()
                .Include(p => p.Animal)
                .ThenInclude(a => a.Farm)
                .AsQueryable();

            
            query = query.Where(p => p.Animal.Farm.UserId == userId);

            
            if (animalId.HasValue && animalId > 0)
            {
                query = query.Where(p => p.AnimalId == animalId.Value);
            }
            if (filter.ProductName.HasValue)
            {
                query = query.Where(p => p.Name == filter.ProductName.Value);
            }
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }
            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }


            int totalRecords = query.Count();

            var products = query
                .OrderByDescending(p => p.ProducedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    AnimalId = p.AnimalId,
                    Price = p.Price,
                    IsSold = p.IsSold,
                    ProducedAt = p.ProducedAt,
                    SoldAt = p.SoldAt
                }).ToList();

            return new PagedResponse<ProductListDto>(products, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public PagedResponse<ProductListDto> GetUnsoldProducts(int userId, ProductFilterDto filter)
        {
            var query = _productRepository.GetQueryable()
                .Include(p => p.Animal)
                    .ThenInclude(a => a.Farm)
                .AsQueryable();

            query = query.Where(p =>
                p.Animal.Farm.UserId == userId && 
                !p.IsSold                  
            );

            if (filter.ProductName.HasValue)
            {
                query = query.Where(p => p.Name == filter.ProductName.Value);
            }
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }
            if (filter.MaxPrice.HasValue) {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            int totalRecords = query.Count();

            var products = query
                .OrderByDescending(p => p.ProducedAt)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    AnimalId = p.AnimalId,
                    Price = p.Price,
                    IsSold = p.IsSold,
                    ProducedAt = p.ProducedAt,
                    SoldAt = p.SoldAt
                }).ToList();

            return new PagedResponse<ProductListDto>(products, totalRecords, filter.PageNumber, filter.PageSize);
        }
    }
}
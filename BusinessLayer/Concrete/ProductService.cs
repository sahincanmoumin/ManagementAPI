using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Constants;
using EntityLayer.DTOs.Pagination;
using EntityLayer.DTOs.Product;
using EntityLayer.Entities;
using EntityLayer.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Product> GetProductWithOwnershipAsync(int productId)
        {
            var product = await _productRepository.GetByIdWithDetailsAsync(productId) ?? throw new BusinessException(ErrorKeys.ProductNotFound);
            return product;
        }

        public async Task SellProductAsync(int userId, int productId)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new BusinessException(ErrorKeys.UserNotFound);
            var product = await _productRepository.GetByIdAsync(productId) ?? throw new BusinessException(ErrorKeys.ProductNotFound);

            if (product.IsSold)
                throw new BusinessException(ErrorKeys.ProductAlreadySold);

            // Bakiye güncelleme
            user.Balance += product.Price;
            await _userRepository.UpdateAsync(user);

            // Ürün durum güncelleme
            product.IsSold = true;
            product.SoldAt = DateTime.Now;
            await _productRepository.UpdateAsync(product);
        }

        public async Task<PagedResponse<ProductListDto>> GetAnimalProductsAsync(int userId, ProductFilterDto filter, int? animalId = null)
        {
            var query = _productRepository.GetQueryable()
                .Include(p => p.Animal)
                .ThenInclude(a => a.Farm)
                .Where(p => p.Animal.Farm.UserId == userId);

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

            int totalRecords = await query.CountAsync();

            var products = await query
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
                }).ToListAsync();

            return new PagedResponse<ProductListDto>(products, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<PagedResponse<ProductListDto>> GetUnsoldProductsAsync(int userId, ProductFilterDto filter)
        {
            var query = _productRepository.GetQueryable()
                .Include(p => p.Animal)
                    .ThenInclude(a => a.Farm)
                .Where(p => p.Animal.Farm.UserId == userId && !p.IsSold);

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

            int totalRecords = await query.CountAsync();

            var products = await query
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
                }).ToListAsync();

            return new PagedResponse<ProductListDto>(products, totalRecords, filter.PageNumber, filter.PageSize);
        }
    }
}
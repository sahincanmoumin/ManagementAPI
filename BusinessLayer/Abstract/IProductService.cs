using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.DTOs.Pagination;
using EntityLayer.DTOs.Product;
using EntityLayer.Entities;

namespace BusinessLayer.Abstract
{
    public interface IProductService
    {
        Task SellProductAsync(int userId, int productId);
        Task<PagedResponse<ProductListDto>> GetAnimalProductsAsync(int userId, ProductFilterDto filter, int? animalId = null);
        Task<PagedResponse<ProductListDto>> GetUnsoldProductsAsync(int userId, ProductFilterDto filter);
        Task<Product> GetProductWithOwnershipAsync(int productId);
    }
}
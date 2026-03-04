using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<List<Product>> GetByAnimalIdAsync(int animalId);
        Task<List<Product>> GetUnsoldProductsAsync();
        Task<Product> GetByIdWithDetailsAsync(int id);
    }
}
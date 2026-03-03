using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        List<Product> GetByAnimalId(int animalId);
        List<Product> GetUnsoldProducts();
        Product GetByIdWithDetails(int id);
        
    }
}
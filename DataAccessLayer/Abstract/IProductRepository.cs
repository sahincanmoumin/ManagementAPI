using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IProductRepository
    {
        Product GetById(int id);
        List<Product> GetByAnimalId(int animalId);
        List<Product> GetUnsoldProducts();
        List<Product> GetAll();
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
    }
}
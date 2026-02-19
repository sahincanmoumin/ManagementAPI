using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityLayer.Entities;

namespace BusinessLayer.Abstract
{
    public interface IProductService
    {
        void SellProduct(int userId, int productId);
        List<Product> GetAnimalProducts(int animalId);
        List<Product> GetUnsoldProducts();
    }
}
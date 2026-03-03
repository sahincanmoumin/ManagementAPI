using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer.DTOs.Pagination;
using System.Collections.Generic;
using EntityLayer.DTOs.Product;

namespace BusinessLayer.Abstract
{
    public interface IProductService
    {
        void SellProduct(int userId, int productId);
        PagedResponse<ProductListDto> GetAnimalProducts(int userId, ProductFilterDto filter, int? animalId = null);
        PagedResponse<ProductListDto> GetUnsoldProducts(int userId, ProductFilterDto filter);
        Product GetProductWithOwnership(int productId);

    }
}
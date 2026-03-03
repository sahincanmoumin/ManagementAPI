using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Enums;
using EntityLayer.DTOs.Pagination;

namespace EntityLayer.DTOs.Product
{
    public class ProductFilterDto : PaginationFilter
    {
        public ProductType? ProductName { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}

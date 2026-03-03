using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Enums;
using EntityLayer.DTOs.Pagination;

namespace EntityLayer.DTOs.Animal
{
    public class AnimalFilterDto : PaginationFilter
    {
        public string Name { get; set; }
        public AnimalType? Type { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
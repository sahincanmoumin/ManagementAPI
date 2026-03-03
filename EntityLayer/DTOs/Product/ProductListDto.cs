using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entity.Enums;

namespace EntityLayer.DTOs.Product
{
    public class ProductListDto
    {
        public int Id { get; set; }
        public ProductType? Name { get; set; }
        public int AnimalId { get; set; }
        public decimal Price { get; set; }
        public DateTime ProducedAt { get; set; } = DateTime.Now;
        public bool IsSold { get; set; } = false;
        public DateTime? SoldAt { get; set; }
    }
}

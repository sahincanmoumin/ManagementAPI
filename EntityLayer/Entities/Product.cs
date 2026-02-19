using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace EntityLayer.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } 

        public decimal Price { get; set; }

        public int AnimalId { get; set; }

        public DateTime ProducedAt { get; set; } = DateTime.Now;

        public bool IsSold { get; set; } = false;

        public DateTime? SoldAt { get; set; }

        
        public Animal Animal { get; set; }
    }
}
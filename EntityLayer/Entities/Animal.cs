using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Enums;

namespace EntityLayer.Entities
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public AnimalType Type { get; set; }

        public decimal Price { get; set; }

        public int ProductionIntervalHours { get; set; } 

        public int LifeSpanDays { get; set; } 

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        public DateTime LastProductionDate { get; set; } = DateTime.Now;

        public int FarmId { get; set; }

        
        public Farm Farm { get; set; }
        public List<Product> Products { get; set; }

       
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entity.Enums;

namespace EntityLayer.DTOs.Animal
{
    public class AnimalListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AnimalType Type { get; set; }
        public decimal Price { get; set; }
        public int FarmId { get; set; }
    }
}
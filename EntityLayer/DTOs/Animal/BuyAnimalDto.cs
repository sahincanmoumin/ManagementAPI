using Entity.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EntityLayer.DTOs.Animal
{
    public class BuyAnimalDto
    {
        public string Name { get; set; }
        public AnimalType Type { get; set; }
        public int FarmId { get; set; }
    }
}
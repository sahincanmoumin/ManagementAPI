using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.DTOs.Animal
{
    public class BuyAnimalDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int FarmId { get; set; }
    }
}
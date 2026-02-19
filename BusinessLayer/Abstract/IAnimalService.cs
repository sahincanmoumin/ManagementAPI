using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityLayer.DTOs.Animal;
using EntityLayer.Entities;

namespace BusinessLayer.Abstract
{
    public interface IAnimalService
    {
        Animal BuyAnimal(int userId, BuyAnimalDto dto);
        void SellAnimal(int userId, int animalId);
        List<Animal> GetFarmAnimals(int farmId);
        Animal GetById(int id);
    }
}
using EntityLayer.DTOs.Animal;
using EntityLayer.DTOs.Pagination;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IAnimalService
    {
        Animal BuyAnimal(int userId, BuyAnimalDto dto);
        void SellAnimal(int userId, int animalId);
        public PagedResponse<AnimalListDto> GetFarmAnimals(int userId, AnimalFilterDto filter);
        Animal GetById(int id);

    }
}
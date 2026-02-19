using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IAnimalRepository
    {
        Animal GetById(int id);
        List<Animal> GetByFarmId(int farmId);
        List<Animal> GetAll();
        void Add(Animal animal);
        void Update(Animal animal);
        void Delete(Animal animal);
    }
}
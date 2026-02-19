using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Entities;

namespace DataAccessLayer.Concrete
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly AppDbContext _context;

        public AnimalRepository(AppDbContext context)
        {
            _context = context;
        }

        public Animal GetById(int id)
        {
            return _context.Animals.Find(id);
        }

        public List<Animal> GetByFarmId(int farmId)
        {
            return _context.Animals.Where(a => a.FarmId == farmId).ToList();
        }

        public List<Animal> GetAll()
        {
            return _context.Animals.ToList();
        }

        public void Add(Animal animal)
        {
            _context.Animals.Add(animal);
            _context.SaveChanges();
        }

        public void Update(Animal animal)
        {
            _context.Animals.Update(animal);
            _context.SaveChanges();
        }

        public void Delete(Animal animal)
        {
            _context.Animals.Remove(animal);
            _context.SaveChanges();
        }
    }
}
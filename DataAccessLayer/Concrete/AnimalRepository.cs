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
    public class AnimalRepository : GenericRepository<Animal> , IAnimalRepository
    {
        private readonly AppDbContext _context;

        public AnimalRepository(AppDbContext context) : base(context)
        {
            _context = context; 
        }
        public List<Animal> GetByFarmId(int farmId)
        {
            return _context.Animals.Where(a => a.FarmId == farmId).ToList();
        }

        public List<Animal> GetAll()
        {
            return _context.Animals.ToList();
        }
    }
}
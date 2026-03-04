using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class AnimalRepository : GenericRepository<Animal>, IAnimalRepository
    {
        public AnimalRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Animal>> GetByFarmIdAsync(int farmId)
        {
            return await _context.Animals.Where(x => x.FarmId == farmId).ToListAsync();
        }

        public async Task<List<Animal>> GetAllAsync()
        {
            return await _context.Animals.ToListAsync();
        }
    }
}
using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class FarmRepository : GenericRepository<Farm>, IFarmRepository
    {
        public FarmRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Farm>> GetByUserIdAsync(int userId)
        {
            return await _context.Farms.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<List<Farm>> GetAllAsync()
        {
            return await _context.Farms.ToListAsync();
        }
    }
}
using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Animal)
                    .ThenInclude(a => a.Farm)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetByAnimalIdAsync(int animalId)
        {
            return await _context.Products
                .Where(p => p.AnimalId == animalId)
                .ToListAsync();
        }

        public async Task<List<Product>> GetUnsoldProductsAsync()
        {
            return await _context.Products
                .Where(p => !p.IsSold)
                .ToListAsync();
        }
    }
}
using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
    public class ProductRepository : GenericRepository<Product> , IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }
        public Product GetByIdWithDetails(int id)
        {
            
            return _context.Products
                .Include(p => p.Animal)          
                    .ThenInclude(a => a.Farm)    
                .FirstOrDefault(p => p.Id == id);
        }
        public List<Product> GetByAnimalId(int animalId)
        {
            return _context.Products.Where(p => p.AnimalId == animalId).ToList();
        }

        public List<Product> GetUnsoldProducts()
        {
            return _context.Products.Where(p => !p.IsSold).ToList();
        }
    }
}
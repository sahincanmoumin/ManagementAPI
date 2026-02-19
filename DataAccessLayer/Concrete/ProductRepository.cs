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
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public Product GetById(int id)
        {
            return _context.Products.Find(id);
        }

        public List<Product> GetByAnimalId(int animalId)
        {
            return _context.Products.Where(p => p.AnimalId == animalId).ToList();
        }

        public List<Product> GetUnsoldProducts()
        {
            return _context.Products.Where(p => !p.IsSold).ToList();
        }

        public List<Product> GetAll()
        {
            return _context.Products.ToList();
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void Delete(Product product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}
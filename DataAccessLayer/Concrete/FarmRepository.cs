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
    public class FarmRepository : IFarmRepository
    {
        private readonly AppDbContext _context;

        public FarmRepository(AppDbContext context)
        {
            _context = context;
        }

        public Farm GetById(int id)
        {
            return _context.Farms.Find(id);
        }

        public List<Farm> GetByUserId(int userId)
        {
            return _context.Farms.Where(f => f.UserId == userId).ToList();
        }

        public List<Farm> GetAll()
        {
            return _context.Farms.ToList();
        }

        public void Add(Farm farm)
        {
            _context.Farms.Add(farm);
            _context.SaveChanges();
        }

        public void Update(Farm farm)
        {
            _context.Farms.Update(farm);
            _context.SaveChanges();
        }

        public void Delete(Farm farm)
        {
            _context.Farms.Remove(farm);
            _context.SaveChanges();
        }
    }
}
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
    public class FarmRepository : GenericRepository<Farm>,IFarmRepository 
    {
        private readonly AppDbContext _context;

        public FarmRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public List<Farm> GetByUserId(int userId)
        {
            return _context.Farms.Where(f => f.UserId == userId).ToList();
        }

        public List<Farm> GetAll()
        {
            return _context.Farms.ToList();
        }
    }
}
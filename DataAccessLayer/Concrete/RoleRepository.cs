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
    public class RoleRepository : GenericRepository<Role> , IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Role GetById(int id)
        {
            return _context.Roles.Find(id);
        }

        public Role GetByName(string name)
        {
            return _context.Roles.FirstOrDefault(rol => rol.Name == name);
        }   

        public List<Role> GetAll()
        {
            return _context.Roles.ToList();
        }
        

    }
}
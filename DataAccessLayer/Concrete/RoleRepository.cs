using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Concrete
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
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
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Role GetById(int id);
        Role GetByName(string name);
        List<Role> GetAll();
    }
}
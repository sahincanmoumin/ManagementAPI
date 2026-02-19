using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IUserRepository
    {
        User GetById(int id);
        User GetByUsername(string username);
        List<User> GetAll();
        void Add(User user);
        void Update(User user);
        void Delete(User user);
    }
}
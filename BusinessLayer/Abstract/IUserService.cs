using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntityLayer.DTOs.User;
using EntityLayer.Entities;

namespace BusinessLayer.Abstract
{
    public interface IUserService
    {
        User GetById(int id);
        void UpdateUser(int id, UpdateUserDto dto);
        decimal GetBalance(int id);
        List<User> GetAllUsers();

    }
}
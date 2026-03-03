using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        void AddUserRole(int userId, int roleId);
        void RemoveUserRole(int userId, int roleId);
        bool HasRole(int userId, string roleName);
        List<Role> GetRoles(int userId);


    }
}
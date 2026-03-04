using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IUserRoleRepository : IGenericRepository<UserRole>
    {
        Task AddUserRoleAsync(int userId, int roleId);
        Task RemoveUserRoleAsync(int userId, int roleId);
        Task<bool> HasRoleAsync(int userId, string roleName);
        Task<List<Role>> GetRolesAsync(int userId);
    }
}
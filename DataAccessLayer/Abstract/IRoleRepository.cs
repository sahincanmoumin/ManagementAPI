using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetByIdAsync(int id);
        Task<Role> GetByNameAsync(string name);
        Task<List<Role>> GetAllAsync();
    }
}
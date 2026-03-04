using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IFarmRepository : IGenericRepository<Farm>
    {
        Task<List<Farm>> GetByUserIdAsync(int userId);
        Task<List<Farm>> GetAllAsync();
    }
}
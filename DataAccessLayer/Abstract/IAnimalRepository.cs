using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IAnimalRepository : IGenericRepository<Animal>
    {
        Task<List<Animal>> GetByFarmIdAsync(int farmId);
        Task<List<Animal>> GetAllAsync();
    }
}
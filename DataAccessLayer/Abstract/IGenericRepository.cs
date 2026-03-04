using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQueryable();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity); 
        Task DeleteAsync(T entity); 
        Task<T> GetByIdAsync(int id);
        Task SaveChangesAsync();
    }
}
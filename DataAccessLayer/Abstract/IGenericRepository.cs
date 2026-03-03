using System.Linq;
using System.Linq.Expressions;

namespace DataAccessLayer.Abstract
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQueryable();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        T GetById(int id);
    }
}
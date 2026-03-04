using System.Collections.Generic;
using System.Threading.Tasks;
using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<List<User>> GetAllAsync();
    }
}
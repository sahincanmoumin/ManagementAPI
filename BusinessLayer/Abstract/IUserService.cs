using EntityLayer.DTOs.Pagination;
using EntityLayer.DTOs.User;
using EntityLayer.Entities;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(int id);
        Task UpdateUserAsync(int id, UpdateUserDto dto);
        Task<decimal> GetBalanceAsync(int id);
        Task<PagedResponse<UserListDto>> GetAllUsersAsync(UserFilterDto filter);
    }
}
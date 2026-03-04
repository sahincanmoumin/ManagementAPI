using EntityLayer.DTOs.Farm;
using EntityLayer.DTOs.Pagination;
using EntityLayer.Entities;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IFarmService
    {
        Task<Farm> CreateFarmAsync(int userId, CreateFarmDto dto);
        Task<Farm> GetByIdAsync(int id);
        Task<PagedResponse<FarmListDto>> GetUserFarmsAsync(int userId, FarmFilterDto filter);
        Task UpdateFarmAsync(int id, UpdateFarmDto dto);
        Task DeleteFarmAsync(int id);
    }
}
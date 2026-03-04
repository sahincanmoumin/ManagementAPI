using EntityLayer.DTOs.Animal;
using EntityLayer.DTOs.Pagination;
using EntityLayer.Entities;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IAnimalService
    {
        Task<Animal> BuyAnimalAsync(int userId, BuyAnimalDto dto);
        Task SellAnimalAsync(int userId, int animalId);
        Task<PagedResponse<AnimalListDto>> GetFarmAnimalsAsync(int userId, AnimalFilterDto filter);
        Task<Animal> GetByIdAsync(int id);
    }
}
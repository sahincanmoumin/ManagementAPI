using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.Farm;
using EntityLayer.Entities;
using EntityLayer.Exceptions;
using EntityLayer.Exceptions;
using EntityLayer.Constants;
using EntityLayer.DTOs.Pagination;
                
namespace BusinessLayer.Concrete
{
    public class FarmService :IFarmService
    {
        private readonly IFarmRepository _farmRepository;
        private readonly IUserRepository _userRepository;

        public FarmService(IFarmRepository farmRepository, IUserRepository userRepository)
        {
            _farmRepository = farmRepository;
            _userRepository = userRepository;
        }

        public Farm CreateFarm(int userId, CreateFarmDto dto)
        {
            var user = _userRepository.GetById(userId) ?? throw new BusinessException(ErrorKeys.UserNotFound);

            var farm = new Farm
            {
                Name = dto.Name,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            _farmRepository.Add(farm);
            return farm;
        }

        public Farm GetById(int id)
        {
            var farm = _farmRepository.GetById(id)?? throw new BusinessException(ErrorKeys.FarmNotFound);
            return farm;
        }       


        public PagedResponse<FarmListDto> GetUserFarms(int userId, FarmFilterDto filter)
        {
            var query = _farmRepository.GetQueryable()
                                       .Where(f => f.UserId == userId); 
            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(f => f.Name.Contains(filter.Name));
            }
            var totalRecords = query.Count();

            var farms = query.Skip((filter.PageNumber - 1) * filter.PageSize)
                             .Take(filter.PageSize)
                             .Select(f => new FarmListDto
                             {
                                 Id = f.Id,
                                 Name = f.Name,
                                 UserId = f.UserId,
                                 UserName = f.User.Username
                             })
                             .ToList();
            return new PagedResponse<FarmListDto>(farms, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public void UpdateFarm(int id, UpdateFarmDto dto)
        {
            var farm = _farmRepository.GetById(id) ?? throw new BusinessException(ErrorKeys.FarmNotFound);

            farm.Name = dto.Name;
            _farmRepository.Update(farm);
        }

        public void DeleteFarm(int id)
        {
            var farm = _farmRepository.GetById(id) ?? throw new BusinessException(ErrorKeys.FarmNotFound);
            _farmRepository.Delete(farm);
        }
    }
}
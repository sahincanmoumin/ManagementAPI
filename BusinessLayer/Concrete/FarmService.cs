using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.Farm;
using EntityLayer.Entities;
using EntityLayer.Exceptions;
using EntityLayer.Constants;
using EntityLayer.DTOs.Pagination;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Concrete
{
    public class FarmService : IFarmService
    {
        private readonly IFarmRepository _farmRepository;
        private readonly IUserRepository _userRepository;

        public FarmService(IFarmRepository farmRepository, IUserRepository userRepository)
        {
            _farmRepository = farmRepository;
            _userRepository = userRepository;
        }

        public async Task<Farm> CreateFarmAsync(int userId, CreateFarmDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new BusinessException(ErrorKeys.UserNotFound);

            var farm = new Farm
            {
                Name = dto.Name,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            await _farmRepository.AddAsync(farm);
            return farm;
        }

        public async Task<Farm> GetByIdAsync(int id)
        {
            var farm = await _farmRepository.GetByIdAsync(id) ?? throw new BusinessException(ErrorKeys.FarmNotFound);
            return farm;
        }

        public async Task<PagedResponse<FarmListDto>> GetUserFarmsAsync(int userId, FarmFilterDto filter)
        {
            var query = _farmRepository.GetQueryable()
                                       .Where(f => f.UserId == userId);

            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(f => f.Name.Contains(filter.Name));
            }

            var totalRecords = await query.CountAsync();

            var farms = await query.Skip((filter.PageNumber - 1) * filter.PageSize)
                                 .Take(filter.PageSize)
                                 .Select(f => new FarmListDto
                                 {
                                     Id = f.Id,
                                     Name = f.Name,
                                     UserId = f.UserId,
                                     UserName = f.User.Username
                                 })
                                 .ToListAsync();

            return new PagedResponse<FarmListDto>(farms, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task UpdateFarmAsync(int id, UpdateFarmDto dto)
        {
            var farm = await _farmRepository.GetByIdAsync(id) ?? throw new BusinessException(ErrorKeys.FarmNotFound);

            farm.Name = dto.Name;
            await _farmRepository.UpdateAsync(farm);
        }

        public async Task DeleteFarmAsync(int id)
        {
            var farm = await _farmRepository.GetByIdAsync(id) ?? throw new BusinessException(ErrorKeys.FarmNotFound);
            await _farmRepository.DeleteAsync(farm);
        }
    }
}
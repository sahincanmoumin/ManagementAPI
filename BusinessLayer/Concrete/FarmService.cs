using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.Farm;
using EntityLayer.Entities;

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

        public Farm CreateFarm(int userId, CreateFarmDto dto)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new Exception("User not found");

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
            var farm = _farmRepository.GetById(id);
            if (farm == null)
                throw new Exception("Farm not found");
            return farm;
        }

        public List<Farm> GetUserFarms(int userId)
        {
            return _farmRepository.GetByUserId(userId);
        }

        public void UpdateFarm(int id, UpdateFarmDto dto)
        {
            var farm = _farmRepository.GetById(id);
            if (farm == null)
                throw new Exception("Farm not found");

            farm.Name = dto.Name;
            _farmRepository.Update(farm);
        }

        public void DeleteFarm(int id)
        {
            var farm = _farmRepository.GetById(id);
            if (farm == null)
                throw new Exception("Farm not found");

            _farmRepository.Delete(farm);
        }
    }
}
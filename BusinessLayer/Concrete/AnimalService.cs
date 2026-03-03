using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using Entity.Enums;
using EntityLayer.Constants;
using EntityLayer.DTOs.Animal;
using EntityLayer.DTOs.Pagination;
using EntityLayer.Entities;
using EntityLayer.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace BusinessLayer.Concrete
{
    public class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFarmRepository _farmRepository;

        public AnimalService(IAnimalRepository animalRepository, IUserRepository userRepository, IFarmRepository farmRepository)
        {
            _animalRepository = animalRepository;
            _userRepository = userRepository;
            _farmRepository = farmRepository;
        }

        public Animal BuyAnimal(int userId, BuyAnimalDto dto)
        {   
            var user = _userRepository.GetById(userId) ?? throw new BusinessException(ErrorKeys.UserNotFound);
            var farm = _farmRepository.GetById(dto.FarmId) ?? throw new BusinessException(ErrorKeys.FarmNotFound);

            decimal animalPrice = GetAnimalPrice(dto.Type);

            if (user.Balance < animalPrice)
                throw new BusinessException(ErrorKeys.InsufficientBalance);

            user.Balance -= animalPrice;
            _userRepository.Update(user);

            
            var animal = new Animal
            {
                Name = dto.Name,
                Type = dto.Type,
                Price = animalPrice,
                ProductionIntervalHours = GetProductionInterval(dto.Type),
                LifeSpanDays = GetLifeSpan(dto.Type),
                PurchaseDate = DateTime.Now,
                LastProductionDate = DateTime.Now,
                FarmId = dto.FarmId
            };

            _animalRepository.Add(animal);
            return animal;
        }

        public void SellAnimal(int userId, int animalId)
        {
            var user = _userRepository.GetById(userId) ?? throw new BusinessException(ErrorKeys.UserNotFound);

            var animal = _animalRepository.GetById(animalId) ?? throw new BusinessException(ErrorKeys.AnimalNotFound);

            var farm = _farmRepository.GetById(animal.FarmId) ?? throw new BusinessException(ErrorKeys.FarmNotFound);

            if (farm.UserId != userId)
                throw new BusinessException(ErrorKeys.FarmNotFound);

            decimal sellPrice = animal.Price * 0.7m;
            user.Balance += sellPrice;
            _userRepository.Update(user);

            _animalRepository.Delete(animal);
        }

        public PagedResponse<AnimalListDto> GetFarmAnimals(int userId, AnimalFilterDto filter)
        {
            var query = _farmRepository.GetQueryable()
                .Where(f => f.UserId == userId)
                .SelectMany(f => f.Animals);

            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                query = query.Where(a => a.Name.Contains(filter.Name));
            }

            if (filter.Type.HasValue)
            {
                query = query.Where(a => a.Type == filter.Type.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(a => a.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(a => a.Price <= filter.MaxPrice.Value);
            }

            var totalRecords = query.Count();

            var pagedData = query
                .OrderBy(a => a.Id)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(a => new AnimalListDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Type = a.Type,
                    Price = a.Price,
                    FarmId = a.FarmId
                })
                .ToList();

            return new PagedResponse<AnimalListDto>(pagedData, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public Animal GetById(int id)
        {
            var animal = _animalRepository.GetById(id) ?? throw new BusinessException(ErrorKeys.AnimalNotFound);
            return animal;
        }

        
        private decimal GetAnimalPrice(AnimalType type)
        {
            return type switch
            {
                AnimalType.Cow => 500,
                AnimalType.Chicken => 50,
                AnimalType.Sheep => 200,
                _ => 100
            };
        }

        private int GetProductionInterval(AnimalType type)
        {
            return type switch
            {
                AnimalType.Cow => 1,      
                AnimalType.Chicken => 1,  // 12 saatte bir yumurta
                AnimalType.Sheep => 1,    // 48 saatte bir yün
                _ => 24
            };
        }

        private int GetLifeSpan(AnimalType type)
        {
            return type switch
            {
                AnimalType.Cow => 365,     // 1 yıl
                AnimalType.Chicken => 180, // 6 ay
                AnimalType.Sheep => 270,   // 9 ay
                _ => 365
            };
        }
    }
}
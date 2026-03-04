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
using System.Threading.Tasks;

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

        public async Task<Animal> BuyAnimalAsync(int userId, BuyAnimalDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new BusinessException(ErrorKeys.UserNotFound);
            var farm = await _farmRepository.GetByIdAsync(dto.FarmId) ?? throw new BusinessException(ErrorKeys.FarmNotFound);

            decimal animalPrice = GetAnimalPrice(dto.Type);

            if (user.Balance < animalPrice)
                throw new BusinessException(ErrorKeys.InsufficientBalance);

            user.Balance -= animalPrice;
            await _userRepository.UpdateAsync(user);

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

            await _animalRepository.AddAsync(animal);
            return animal;
        }

        public async Task SellAnimalAsync(int userId, int animalId)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new BusinessException(ErrorKeys.UserNotFound);
            var animal = await _animalRepository.GetByIdAsync(animalId) ?? throw new BusinessException(ErrorKeys.AnimalNotFound);
            var farm = await _farmRepository.GetByIdAsync(animal.FarmId) ?? throw new BusinessException(ErrorKeys.FarmNotFound);

            if (farm.UserId != userId)
                throw new BusinessException(ErrorKeys.FarmNotFound);

            decimal sellPrice = animal.Price * 0.7m;
            user.Balance += sellPrice;
            await _userRepository.UpdateAsync(user);

            await _animalRepository.DeleteAsync(animal);
        }

        public async Task<PagedResponse<AnimalListDto>> GetFarmAnimalsAsync(int userId, AnimalFilterDto filter)
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

            var totalRecords = await query.CountAsync();

            var pagedData = await query
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
                .ToListAsync();

            return new PagedResponse<AnimalListDto>(pagedData, totalRecords, filter.PageNumber, filter.PageSize);
        }

        public async Task<Animal> GetByIdAsync(int id)
        {
            var animal = await _animalRepository.GetByIdAsync(id) ?? throw new BusinessException(ErrorKeys.AnimalNotFound);
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
                AnimalType.Chicken => 1,
                AnimalType.Sheep => 1,
                _ => 24
            };
        }

        private int GetLifeSpan(AnimalType type)
        {
            return type switch
            {
                AnimalType.Cow => 365,
                AnimalType.Chicken => 180,
                AnimalType.Sheep => 270,
                _ => 365
            };
        }
    }
}
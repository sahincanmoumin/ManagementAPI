using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.DTOs.Animal;
using EntityLayer.Entities;

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
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new Exception("User not found");

            var farm = _farmRepository.GetById(dto.FarmId);
            if (farm == null || farm.UserId != userId)
                throw new Exception("Farm not found or not owned by user");

            // Hayvan fiyatını belirle (tip'e göre)
            decimal animalPrice = GetAnimalPrice(dto.Type);

            if (user.Balance < animalPrice)
                throw new Exception("Insufficient balance");

            // Bakiyeden düş
            user.Balance -= animalPrice;
            _userRepository.Update(user);

            // Hayvanı ekle
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
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new Exception("User not found");

            var animal = _animalRepository.GetById(animalId);
            if (animal == null)
                throw new Exception("Animal not found");

            var farm = _farmRepository.GetById(animal.FarmId);
            if (farm.UserId != userId)
                throw new Exception("Animal not owned by user");

            // Satış fiyatı alış fiyatının %70'i
            decimal sellPrice = animal.Price * 0.7m;
            user.Balance += sellPrice;
            _userRepository.Update(user);

            _animalRepository.Delete(animal);
        }

        public List<Animal> GetFarmAnimals(int farmId)
        {
            return _animalRepository.GetByFarmId(farmId);
        }

        public Animal GetById(int id)
        {
            var animal = _animalRepository.GetById(id);
            if (animal == null)
                throw new Exception("Animal not found");
            return animal;
        }

        // Yardımcı metodlar
        private decimal GetAnimalPrice(string type)
        {
            return type.ToLower() switch
            {
                "cow" => 500,
                "chicken" => 50,
                "sheep" => 200,
                _ => 100
            };
        }

        private int GetProductionInterval(string type)
        {
            return type.ToLower() switch
            {
                "cow" => 24,      // 24 saatte bir süt
                "chicken" => 12,  // 12 saatte bir yumurta
                "sheep" => 48,    // 48 saatte bir yün
                _ => 24
            };
        }

        private int GetLifeSpan(string type)
        {
            return type.ToLower() switch
            {
                "cow" => 365,     // 1 yıl
                "chicken" => 180, // 6 ay
                "sheep" => 270,   // 9 ay
                _ => 365
            };
        }
    }
}
using BusinessLayer.Abstract;
using EntityLayer.Constants; 
using EntityLayer.DTOs.Animal;
using EntityLayer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EntityLayer.Exceptions;

namespace ApiLayer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnimalController : BaseController
    {
        private readonly IAnimalService _animalService;
        private readonly IFarmService _farmservice;
        private readonly ILogger<AnimalController> _logger;

        public AnimalController(IAnimalService animalService, IFarmService farmService, ILogger<AnimalController> logger)
        {
            _animalService = animalService;
            _farmservice = farmService;
            _logger = logger;
        }

        [HttpPost("buy")]
        public IActionResult BuyAnimal([FromBody] BuyAnimalDto dto)
        {

            var userId = CurrentUserId;
            var animal = _animalService.BuyAnimal(userId, dto) ?? throw new BusinessException(ErrorKeys.AnimalBuyingFailed);

            _logger.LogInformation($"User {userId} bought animal {animal.Name}");
            return Ok(new { message = "Animal purchased successfully", animalId = animal.Id });
        }

        [HttpDelete("{id}/sell")]
        public IActionResult SellAnimal(int id)
        {

            var animal = _animalService.GetById(id);
            var farm = _farmservice.GetById(animal.FarmId);

            if (!IsAdmin && CurrentUserId != farm.UserId)
            {
                throw new BusinessException(ErrorKeys.UnauthorizedAction);
            }

            _animalService.SellAnimal(CurrentUserId, id);
            _logger.LogInformation($"User {CurrentUserId} sold animal {id}");
            return Ok(new { message = "Animal sold successfully" });


        }

        [HttpGet("{id}")]
        public IActionResult GetAnimal(int id)
        {
            var animal = _animalService.GetById(id);
            return Ok(animal);

        }

        [HttpGet("my-animals")]
        public IActionResult GetMyAnimals([FromQuery] AnimalFilterDto filter)
        {
           
            filter ??= new AnimalFilterDto();

            
            var result = _animalService.GetFarmAnimals(CurrentUserId, filter);

            return Ok(result);
        }
    }
}
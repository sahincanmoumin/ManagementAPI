using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using EntityLayer.DTOs.Animal;
using System.Security.Claims;

namespace ApiLayer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnimalController : ControllerBase
    {
        private readonly IAnimalService _animalService;
        private readonly ILogger<AnimalController> _logger;

        public AnimalController(IAnimalService animalService, ILogger<AnimalController> logger)
        {
            _animalService = animalService;
            _logger = logger;
        }

        [HttpPost("buy")]
        public IActionResult BuyAnimal([FromBody] BuyAnimalDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var animal = _animalService.BuyAnimal(userId, dto);
                _logger.LogInformation($"User {userId} bought animal {animal.Name}");
                return Ok(new { message = "Animal purchased successfully", animalId = animal.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Buy animal failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/sell")]
        public IActionResult SellAnimal(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _animalService.SellAnimal(userId, id);
                _logger.LogInformation($"User {userId} sold animal {id}");
                return Ok(new { message = "Animal sold successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sell animal failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("farm/{farmId}")]
        public IActionResult GetFarmAnimals(int farmId)
        {
            try
            {
                var animals = _animalService.GetFarmAnimals(farmId);
                return Ok(animals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get farm animals failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetAnimal(int id)
        {
            try
            {
                var animal = _animalService.GetById(id);
                return Ok(animal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get animal failed");
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
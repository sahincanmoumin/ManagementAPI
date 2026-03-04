using ApiLayer.Controller;
using BusinessLayer.Abstract;
using EntityLayer.Constants;
using EntityLayer.DTOs.Animal;
using EntityLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AnimalController : BaseController
{
    private readonly IAnimalService _animalService;
    private readonly IFarmService _farmService;
    private readonly ILogger<AnimalController> _logger;

    public AnimalController(IAnimalService animalService, IFarmService farmService, ILogger<AnimalController> logger)
    {
        _animalService = animalService;
        _farmService = farmService;
        _logger = logger;
    }

    [HttpPost("buy")]
    public async Task<IActionResult> BuyAnimal([FromBody] BuyAnimalDto dto)
    {
        var animal = await _animalService.BuyAnimalAsync(CurrentUserId, dto)
                     ?? throw new BusinessException(ErrorKeys.AnimalBuyingFailed);

        _logger.LogInformation($"User {CurrentUserId} bought animal {animal.Name}");
        return Ok(new { message = "Animal purchased successfully", animalId = animal.Id });
    }

    [HttpDelete("{id}/sell")]
    public async Task<IActionResult> SellAnimal(int id)
    {
        var animal = await _animalService.GetByIdAsync(id);
        var farm = await _farmService.GetByIdAsync(animal.FarmId);

        if (!IsAdmin && CurrentUserId != farm.UserId)
            throw new BusinessException(ErrorKeys.UnauthorizedAction);

        await _animalService.SellAnimalAsync(CurrentUserId, id);
        _logger.LogInformation($"User {CurrentUserId} sold animal {id}");
        return Ok(new { message = "Animal sold successfully" });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAnimal(int id)
    {
        var animal = await _animalService.GetByIdAsync(id);
        return Ok(animal);
    }

    [HttpGet("my-animals")]
    public async Task<IActionResult> GetMyAnimals([FromQuery] AnimalFilterDto filter)
    {
        filter ??= new AnimalFilterDto();
        var result = await _animalService.GetFarmAnimalsAsync(CurrentUserId, filter);
        return Ok(result);
    }
}
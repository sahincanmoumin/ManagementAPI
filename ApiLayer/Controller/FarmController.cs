using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using EntityLayer.DTOs.Farm;
using EntityLayer.Extensions;
using EntityLayer.Exceptions;
using EntityLayer.Constants;
using System.Threading.Tasks;

namespace ApiLayer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FarmController : BaseController
    {
        private readonly IFarmService _farmService;
        private readonly ILogger<FarmController> _logger;

        public FarmController(IFarmService farmService, ILogger<FarmController> logger)
        {
            _farmService = farmService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFarm([FromBody] CreateFarmDto dto)
        {
            var farm = await _farmService.CreateFarmAsync(CurrentUserId, dto);
            _logger.LogInformation($"Farm {farm.Name} created by user {CurrentUserId}");

            return Ok(new { message = "Farm created successfully", farmId = farm.Id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFarm(int id)
        {
            var farm = await _farmService.GetByIdAsync(id);

            if (!IsAdmin && farm.UserId != CurrentUserId)
            {
                throw new BusinessException(ErrorKeys.UnauthorizedAction);
            }

            return Ok(farm);
        }

        [HttpGet("my-farms")]
        public async Task<IActionResult> GetMyFarms([FromQuery] FarmFilterDto filter)
        {
            filter ??= new FarmFilterDto();

            var result = await _farmService.GetUserFarmsAsync(CurrentUserId, filter);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFarm(int id, [FromBody] UpdateFarmDto dto)
        {
            var farm = await _farmService.GetByIdAsync(id);

            if (!IsAdmin && farm.UserId != CurrentUserId)
            {
                throw new BusinessException(ErrorKeys.UnauthorizedAction);
            }

            await _farmService.UpdateFarmAsync(id, dto);
            _logger.LogInformation($"Farm {id} updated by user {CurrentUserId}");

            return Ok(new { message = "Farm updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarm(int id)
        {
            var farm = await _farmService.GetByIdAsync(id);

            if (!IsAdmin && farm.UserId != CurrentUserId)
            {
                throw new BusinessException(ErrorKeys.UnauthorizedAction);
            }

            await _farmService.DeleteFarmAsync(id);
            _logger.LogInformation($"Farm {id} deleted by user {CurrentUserId}");

            return Ok(new { message = "Farm deleted successfully" });
        }
    }
}
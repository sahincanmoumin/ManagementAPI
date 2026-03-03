using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using EntityLayer.DTOs.Farm;
using System.Security.Claims;
using EntityLayer.Extensions;
using EntityLayer.Exceptions;
using EntityLayer.Constants;

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
        public IActionResult CreateFarm([FromBody] CreateFarmDto dto)
        {
           
            var farm = _farmService.CreateFarm(CurrentUserId, dto);
            _logger.LogInformation($"Farm {farm.Name} created by user {CurrentUserId}");
            return Ok(new { message = "Farm created successfully", farmId = farm.Id });
            
        }

        [HttpGet("{id}")]
        public IActionResult GetFarm(int id)
        {
            var farm = _farmService.GetById(id);
 
            if (!User.IsAdmin() && farm.UserId != CurrentUserId)
            {
                throw new BusinessException(ErrorKeys.UnauthorizedAction);
            }

            return Ok(farm);
            
        }

        [HttpGet("my-farms")]
        public IActionResult GetMyFarms([FromQuery] FarmFilterDto filter)
        {
            
            filter ??= new FarmFilterDto();

            
            var result = _farmService.GetUserFarms(CurrentUserId, filter);

            
            return Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateFarm(int id, [FromBody] UpdateFarmDto dto)
        {
            
            var farm = _farmService.GetById(id);

            if (!IsAdmin && farm.UserId != CurrentUserId)
            {
                throw new BusinessException(ErrorKeys.UnauthorizedAction);
            }

            _farmService.UpdateFarm(id, dto);
            _logger.LogInformation($"Farm {id} updated");
            return Ok(new { message = "Farm updated successfully" });
           
        }
            
        [HttpDelete("{id}")]
        public IActionResult DeleteFarm(int id)
        {
            
            var farm = _farmService.GetById(id);
                
            if (!IsAdmin && farm.UserId != CurrentUserId)
            {
                throw new BusinessException(ErrorKeys.UnauthorizedAction);
            }

            _farmService.DeleteFarm(id);
            _logger.LogInformation($"Farm {id} deleted");
            return Ok(new { message = "Farm deleted successfully" });
            
        }
    }
}
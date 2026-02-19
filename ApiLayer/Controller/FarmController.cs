using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using EntityLayer.DTOs.Farm;
using System.Security.Claims;

namespace ApiLayer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FarmController : ControllerBase
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
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var farm = _farmService.CreateFarm(userId, dto);
                _logger.LogInformation($"Farm {farm.Name} created by user {userId}");
                return Ok(new { message = "Farm created successfully", farmId = farm.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create farm failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetFarm(int id)
        {
            try
            {
                var farm = _farmService.GetById(id);
                return Ok(farm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get farm failed");
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("my-farms")]
        public IActionResult GetMyFarms()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var farms = _farmService.GetUserFarms(userId);
                return Ok(farms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get user farms failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateFarm(int id, [FromBody] UpdateFarmDto dto)
        {
            try
            {
                _farmService.UpdateFarm(id, dto);
                _logger.LogInformation($"Farm {id} updated");
                return Ok(new { message = "Farm updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update farm failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFarm(int id)
        {
            try
            {
                _farmService.DeleteFarm(id);
                _logger.LogInformation($"Farm {id} deleted");
                return Ok(new { message = "Farm deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete farm failed");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
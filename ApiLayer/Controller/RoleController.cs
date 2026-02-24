using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using EntityLayer.DTOs.Role;

namespace ApiLayer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpPost("assign")]
        public IActionResult AssignRole([FromBody] AssignRoleDto dto)
        {
            try
            {
                _roleService.AssignRole(dto);
                _logger.LogInformation($"Role '{dto.RoleName}' assigned to user {dto.UserId}");
                return Ok(new { message = $"Role '{dto.RoleName}' assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Assign role failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("remove")]
        public IActionResult RemoveRole([FromBody] RemoveRoleDto dto)
        {
            try
            {
                _roleService.RemoveRole(dto);
                _logger.LogInformation($"Role '{dto.RoleName}' removed from user {dto.UserId}");
                return Ok(new { message = $"Role '{dto.RoleName}' removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Remove role failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetUserRoles(int userId)
        {
            try
            {
                var roles = _roleService.GetUserRoles(userId);
                return Ok(new { userId, roles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get user roles failed");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
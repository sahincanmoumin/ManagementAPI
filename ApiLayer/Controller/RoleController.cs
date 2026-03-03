using BusinessLayer.Abstract;
using EntityLayer.DTOs.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

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

            _roleService.AssignRole(dto);
            _logger.LogInformation($"Role '{dto.RoleName}' assigned to user {dto.UserId}");
            return Ok(new { message = $"Role '{dto.RoleName}' assigned successfully" });

        }

        [HttpPost("remove")]
        public IActionResult RemoveRole([FromBody] RemoveRoleDto dto)
        {

            _roleService.RemoveRole(dto);
            _logger.LogInformation($"Role '{dto.RoleName}' removed from user {dto.UserId}");
            return Ok(new { message = $"Role '{dto.RoleName}' removed successfully" });

        }

        [HttpGet("user")]
        public IActionResult GetUserRoles([FromQuery] RoleFilterDto filter)
        {
            var roles = _roleService.GetUserRoles(filter);
            return Ok(new { roles });
        }
    }
}
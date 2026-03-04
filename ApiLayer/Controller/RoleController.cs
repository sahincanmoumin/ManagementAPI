using ApiLayer.Controller;
using BusinessLayer.Abstract;
using EntityLayer.DTOs.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        await _roleService.AssignRoleAsync(dto);
        _logger.LogInformation($"Role '{dto.RoleName}' assigned to user {dto.UserId}");
        return Ok(new { message = $"Role '{dto.RoleName}' assigned successfully" });
    }

    [HttpPost("remove")]
    public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleDto dto)
    {
        await _roleService.RemoveRoleAsync(dto);
        _logger.LogInformation($"Role '{dto.RoleName}' removed from user {dto.UserId}");
        return Ok(new { message = $"Role '{dto.RoleName}' removed successfully" });
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetUserRoles([FromQuery] RoleFilterDto filter)
    {
        var result = await _roleService.GetUserRolesAsync(filter);
        return Ok(result);
    }
}
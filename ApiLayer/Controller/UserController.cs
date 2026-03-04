using ApiLayer.Controller;
using BusinessLayer.Abstract;
using EntityLayer.Constants;
using EntityLayer.DTOs.User;
using EntityLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, IRoleService roleService, ILogger<UserController> logger)
    {
        _userService = userService;
        _roleService = roleService;
        _logger = logger;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = CurrentUserId;
        var user = await _userService.GetByIdAsync(userId);
        var roles = await _roleService.GetRolesAsync(userId);

        return Ok(new { user.Id, user.Username, user.Balance, user.CreatedAt, roles });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
    {
        await _userService.UpdateUserAsync(CurrentUserId, dto);
        _logger.LogInformation($"User {CurrentUserId} updated profile");
        return Ok(new { message = "Profile updated successfully" });
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var balance = await _userService.GetBalanceAsync(CurrentUserId);
        return Ok(new { balance });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        if (!IsAdmin && CurrentUserId != id)
            throw new BusinessException(ErrorKeys.UnauthorizedAction);

        var user = await _userService.GetByIdAsync(id);
        var roles = await _roleService.GetRolesAsync(id);

        return Ok(new { user.Id, user.Username, user.Balance, user.CreatedAt, roles });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterDto filter)
    {
        var users = await _userService.GetAllUsersAsync(filter);
        return Ok(users);
    }
}
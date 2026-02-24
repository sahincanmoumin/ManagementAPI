using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using EntityLayer.DTOs.User;
using EntityLayer.Extensions;
using System.Security.Claims;

namespace ApiLayer.Controller
{
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
        public IActionResult GetProfile()
        {
            try
            {
                var userId = CurrentUserId;
                var user = _userService.GetById(userId);
                var roles = _roleService.GetUserRoles(userId);

                return Ok(new
                {
                    user.Id,
                    user.Username,
                    user.Balance,
                    user.CreatedAt,
                    roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get profile failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("profile")]
        public IActionResult UpdateProfile([FromBody] UpdateUserDto dto)
        {
            try
            {
                var userId = CurrentUserId;
                _userService.UpdateUser(userId, dto);
                _logger.LogInformation($"User {userId} updated profile");
                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update profile failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("balance")]
        public IActionResult GetBalance()
        {
            try
            {
                var userId = CurrentUserId;
                var balance = _userService.GetBalance(userId);
                return Ok(new { balance });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get balance failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
       
                if (!IsAdmin && CurrentUserId != id)
                {
                    return Forbid();
                }

                var user = _userService.GetById(id);
                var roles = _roleService.GetUserRoles(id);

                return Ok(new
                {
                    user.Id,
                    user.Username,
                    user.Balance,
                    user.CreatedAt,
                    roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get user failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userService.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get all users failed");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
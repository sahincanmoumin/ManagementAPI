using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using EntityLayer.DTOs.User;
using System.Security.Claims;

namespace ApiLayer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var user = _userService.GetById(userId);
                return Ok(new { user.Id, user.Username, user.Balance, user.CreatedAt });
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
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
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
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var balance = _userService.GetBalance(userId);
                return Ok(new { balance });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get balance failed");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
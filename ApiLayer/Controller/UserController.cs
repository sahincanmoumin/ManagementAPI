using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using EntityLayer.DTOs.User;
using EntityLayer.Extensions;
using System.Security.Claims;
using EntityLayer.Exceptions;
using EntityLayer.Constants;

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

            var userId = CurrentUserId;
            var user = _userService.GetById(userId);
            var roles = _roleService.GetRoles(userId);

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Balance,
                user.CreatedAt,
                roles
            });


        }

        [HttpPut("profile")]
        public IActionResult UpdateProfile([FromBody] UpdateUserDto dto)
        {
            var userId = CurrentUserId;
            _userService.UpdateUser(userId, dto);
            _logger.LogInformation($"User {userId} updated profile");
            return Ok(new { message = "Profile updated successfully" });
            
        }

        [HttpGet("balance")]
        public IActionResult GetBalance()
        {

            var userId = CurrentUserId;
            var balance = _userService.GetBalance(userId);
            return Ok(new { balance });

        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {

            if (!IsAdmin && CurrentUserId != id)
            {
                throw new BusinessException(ErrorKeys.UnauthorizedAction);
            }

            var user = _userService.GetById(id);
            var roles = _roleService.GetRoles(id);

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Balance,
                user.CreatedAt,
                roles
            });


        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllUsers([FromQuery] UserFilterDto filter)
        {
            var users = _userService.GetAllUsers(filter);
            return Ok(users);
        }
    }
}
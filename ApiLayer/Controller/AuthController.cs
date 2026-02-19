using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using EntityLayer.DTOs.Auth;

namespace ApiLayer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            try
            {
                var user = _authService.Register(dto);
                _logger.LogInformation($"User {user.Username} registered successfully");
                return Ok(new { message = "User registered successfully", userId = user.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = _authService.Login(dto);
                _logger.LogInformation($"User {dto.Username} logged in successfully");
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
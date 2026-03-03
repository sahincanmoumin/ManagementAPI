using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Abstract;
using EntityLayer.DTOs.Auth;

namespace ApiLayer.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
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
            
            var user = _authService.Register(dto);
            _logger.LogInformation($"User {user.Username} registered successfully");
            return Ok(new { message = "User registered successfully", userId = user.Id });
            
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            
            var token = _authService.Login(dto);
            _logger.LogInformation($"User {dto.Username} logged in successfully");
            return Ok(new { token });
 
        }
    }
}
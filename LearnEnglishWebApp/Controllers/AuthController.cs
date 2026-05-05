using Microsoft.AspNetCore.Mvc;
using LearnEnglishWebApp.Services.Interfaces;
using LearnEnglishWebApp.DTOs.Request;
using Microsoft.Extensions.Logging; 

namespace LearnEnglishWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger; 

        public AuthController(IUserService userService, ILogger<AuthController> logger) 
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto) 
        {
            try
            {
                _logger.LogInformation($"Register attempt for email: {registerDto?.Email}");

                if (registerDto == null)
                {
                    _logger.LogWarning("RegisterDto is null");
                    return BadRequest(new { message = "Данные не получены" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    _logger.LogWarning($"ModelState errors: {string.Join(", ", errors)}");
                    return BadRequest(new { message = "Неверный формат данных", errors });
                }

                var user = await _userService.RegisterAsync(registerDto);
                _logger.LogInformation($"User registered successfully: {user.Email}");
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Registration error for {registerDto?.Email}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto) 
        {
            try
            {
                _logger.LogInformation($"Login attempt for email: {loginDto?.Email}");

                if (loginDto == null)
                {
                    _logger.LogWarning("LoginDto is null");
                    return BadRequest(new { message = "Данные не получены. Убедитесь, что отправляете JSON с полями email и password" });
                }

                if (string.IsNullOrEmpty(loginDto.Email))
                {
                    _logger.LogWarning("Email is empty");
                    return BadRequest(new { message = "Email не может быть пустым" });
                }

                if (string.IsNullOrEmpty(loginDto.Password))
                {
                    _logger.LogWarning("Password is empty");
                    return BadRequest(new { message = "Пароль не может быть пустым" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    _logger.LogWarning($"ModelState errors: {string.Join(", ", errors)}");
                    return BadRequest(new { message = "Неверный формат данных", errors });
                }

                var response = await _userService.LoginAsync(loginDto);
                Response.Cookies.Append("auth_token", response.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(24)
                });

                _logger.LogInformation($"Login successful for: {loginDto.Email}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Login error for {loginDto?.Email}");
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            try
            {
                _logger.LogInformation($"Get user by id: {id}");
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning($"User not found: {id}");
                    return NotFound(new { message = "Пользователь не найден" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by id: {id}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/by-email")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            try
            {
                _logger.LogInformation($"Get user by email: {email}");

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Email не указан" });
                }

                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning($"User not found: {email}");
                    return NotFound(new { message = "Пользователь не найден" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by email: {email}");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
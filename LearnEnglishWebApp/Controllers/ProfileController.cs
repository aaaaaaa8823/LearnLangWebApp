using LearnEnglishWebApp.DTOs.Request;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LearnEnglishWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;

        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userId);
            return Ok(user);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if(userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized(new { message = "Пользователь не авторизован" });

                var updatedUser = await _userService.UpdateProfileAsync(userId, updateDto);
                return Ok(updatedUser);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetUserStats()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var stats = await _userService.GetUserStatsAsync(userId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
    
}



// Controllers/UserWordsController.cs
using LearnEnglishWebApp.DTOs.Request;
using LearnEnglishWebApp.Services.Implementations;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnEnglishWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserWordsController : ControllerBase
    {
        private readonly IUserWordsService _userWordService;

        public UserWordsController(IUserWordsService userWordService)
        {
            _userWordService = userWordService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddWordToUser([FromBody] AddUserWordDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized(new { message = "Пользователь не авторизован" });

                if (userId != dto.UserId)
                    return Unauthorized(new { message = "Нельзя добавить слово другому пользователю" });

                var result = await _userWordService.AddWordToUserAsync(dto.UserId, dto.WordId, dto.Status);
                return Ok(result);
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

        [HttpGet("my-words")]
        public async Task<IActionResult> GetMyWords([FromQuery] string status = null)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var words = await _userWordService.GetUserWordsAsync(userId, status);
                return Ok(words);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateWordsStatus([FromBody] UpdateWordStatusDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                if (userId != dto.UserId)
                    return Unauthorized();

                var result = await _userWordService.UpdateWordStatusAsync(dto.UserId, dto.WordId, dto.Status);
                return Ok(result);
            }
            catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveWord([FromBody] RemoveUserWordDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                if (userId != dto.UserId)
                    return Unauthorized();

                var result = await _userWordService.RemoveWordFromUserAsync(dto.UserId, dto.WordId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}
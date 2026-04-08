using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnEnglishWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GrammarController : ControllerBase
    {
        private readonly IGrammarTopicService _grammarTopicService;

        public GrammarController(IGrammarTopicService grammarTopicService)
        {
            _grammarTopicService = grammarTopicService;
        }

        [HttpGet("topics")]
        public async Task<IActionResult> GetAllTopics()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"userIdClaim: {userIdClaim}");

                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                {
                    Console.WriteLine("Unauthorized - userIdClaim is null or invalid");
                    return Unauthorized();
                }

                Console.WriteLine($"UserId: {userId}");

                var topics = await _grammarTopicService.GetUserTopicsWithProgressAsync(userId);

                Console.WriteLine($"Topics count: {topics?.Count() ?? 0}");

                return Ok(topics);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllTopics: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { message = ex.Message, details = ex.ToString() });
            }
        }

        [HttpGet("topic/{id}")]
        public async Task<IActionResult> GetTopicById(long id)
        {
            try
            {
                var topic = await _grammarTopicService.GetTopicByIdAsync(id);
                if (topic == null)
                    return NotFound(new { message = "Тема не найдена" });

                return Ok(topic);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("topic/{id}/complete")]
        public async Task<IActionResult> MarkTopicAsCompleted(long id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var result = await _grammarTopicService.MarkTopicAsCompletedAsync(userId, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
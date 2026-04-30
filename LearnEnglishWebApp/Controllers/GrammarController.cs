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
    public class GrammarController : ControllerBase
    {
        private readonly IGrammarTopicService _grammarTopicService;
        private readonly IUserLessonService _userLessonService;
        private readonly IGrammarTestService _grammarTestService;

        public GrammarController(IGrammarTopicService grammarTopicService,
                                 IUserLessonService userLessonService,
                                 IGrammarTestService grammarTestService)  
        {
            _grammarTopicService = grammarTopicService;
            _userLessonService = userLessonService;
            _grammarTestService = grammarTestService;
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

        [HttpPost("topic/{id}/save")]
        public async Task<IActionResult> SaveLesson(long id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized(new { message = "Пользователь не авторизован" });

                var result = await _userLessonService.SaveLessonAsync(userId, id, "grammar");
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

        [HttpGet("saved")]
        public async Task<IActionResult> GetSavedLessons()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var lessons = await _userLessonService.GetSavedLessonsAsync(userId);
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("topic/{id}/unsave")]
        public async Task<IActionResult> UnsaveLesson(long id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var result = await _userLessonService.RemoveSavedLessonAsync(userId, id, "grammar");
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("tests")]
        public async Task<IActionResult> GetAllTests()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var tests = await _grammarTestService.GetAllTestAsync(userId);
                return Ok(tests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("test/{id}")]
        public async Task<IActionResult> GetTestById(long id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var test = await _grammarTestService.GetTestByIdAsync(userId, id);
                if (test == null)
                    return NotFound(new { message = "Тест не найден" });

                return Ok(test);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("tests/level/{level}")]
        public async Task<IActionResult> GetTestsByLevel(string level)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var tests = await _grammarTestService.GetTestsByLevelAsync(userId, level);
                return Ok(tests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
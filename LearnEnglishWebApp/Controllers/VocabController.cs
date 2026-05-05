using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearnEnglishWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VocabController : ControllerBase
    {
        private readonly IVocabTopicService _vocabTopicService;
        private readonly IUserLessonService _userLessonService;

        public VocabController(
    IVocabTopicService vocabTopicService,
    IUserLessonService userLessonService)  
        {
            _vocabTopicService = vocabTopicService;
            _userLessonService = userLessonService;
        }

        [HttpGet("topics")]
        public async Task<IActionResult> GetAllTopics()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var topics = await _vocabTopicService.GetUserTopicsWithProgressAsync(userId);
                return Ok(topics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("topic/{id}")]
        public async Task<IActionResult> GetTopicById(long id)
        {
            try
            {
                var topic = await _vocabTopicService.GetTopicByIdAsync(id);
                if (topic == null)
                    return NotFound(new { message = "Урок не найден" });

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

                var result = await _vocabTopicService.MarkTopicAsCompletedAsync(userId, id);
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

                var result = await _userLessonService.SaveLessonAsync(userId, id, "vocab");
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

        [HttpDelete("topic/{id}/unsave")]
        public async Task<IActionResult> UnsaveLesson(long id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !long.TryParse(userIdClaim, out long userId))
                    return Unauthorized();

                var result = await _userLessonService.RemoveSavedLessonAsync(userId, id, "vocab");
                return Ok(new { success = result });
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
                // Фильтруем только vocab уроки для этого контроллера
                var vocabLessons = lessons.Where(l => l.LessonType == "vocab");
                return Ok(vocabLessons);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
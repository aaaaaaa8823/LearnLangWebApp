using LearnEnglishWebApp.DTOs.Request;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnEnglishWebApp.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminApiController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminApiController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        //слова
        [HttpGet("words")]
        public async Task<IActionResult> GetAllWords()
        {
            try
            {
                var words = await _adminService.GetAllWordsAsync();
                return Ok(words);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("words/{id}")]
        public async Task<IActionResult> GetWordById(long id)
        {
            try
            {
                var word = await _adminService.GetWordByIdAsync(id);
                if (word == null)
                    return NotFound(new { message = "Слово не найдено" });
                return Ok(word);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("words")]
        public async Task<IActionResult> AddWord([FromBody] AddWordDto dto)
        {
            try
            {
                var result = await _adminService.AddWordAsync(dto);
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

        [HttpPut("words/{id}")]
        public async Task<IActionResult> UpdateWord(long id, [FromBody] UpdateWordDto dto)
        {
            try
            {
                var result = await _adminService.UpdateWordAsync(id, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("words/{id}")]
        public async Task<IActionResult> DeleteWord(long id)
        {
            try
            {
                var result = await _adminService.DeleteWordAsync(id);
                if (!result)
                    return NotFound(new { message = "Слово не найдено" });
                return Ok(new { success = true, message = "Слово удалено" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //граммар уроки
        [HttpGet("grammar/topics")]
        public async Task<IActionResult> GetAllGrammarTopics()
        {
            try
            {
                var topics = await _adminService.GetAllGrammarTopicsAsync();
                return Ok(topics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("grammar/topics/{id}")]
        public async Task<IActionResult> GetGrammarTopicById(long id)
        {
            try
            {
                var topic = await _adminService.GetGrammarTopicByIdAsync(id);
                if (topic == null)
                    return NotFound(new { message = "Урок не найден" });
                return Ok(topic);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("grammar/topics")]
        public async Task<IActionResult> AddGrammarTopic([FromBody] AddGrammarTopicDto dto)
        {
            try
            {
                var result = await _adminService.AddGrammarTopicAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("grammar/topics/{id}")]
        public async Task<IActionResult> UpdateGrammarTopic(long id, [FromBody] UpdateGrammarTopicDto dto)
        {
            try
            {
                var result = await _adminService.UpdateGrammarTopicAsync(id, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("grammar/topics/{id}")]
        public async Task<IActionResult> DeleteGrammarTopic(long id)
        {
            try
            {
                var result = await _adminService.DeleteGrammarTopicAsync(id);
                if (!result)
                    return NotFound(new { message = "Урок не найден" });
                return Ok(new { success = true, message = "Урок удалён" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //граммар тесты
        [HttpGet("grammar/tests")]
        public async Task<IActionResult> GetAllGrammarTests()
        {
            try
            {
                var tests = await _adminService.GetAllGrammarTestsAsync();
                return Ok(tests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("grammar/tests/{id}")]
        public async Task<IActionResult> GetGrammarTestById(long id)
        {
            try
            {
                var test = await _adminService.GetGrammarTestByIdAsync(id);
                if (test == null)
                    return NotFound(new { message = "Тест не найден" });
                return Ok(test);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("grammar/tests/by-topic/{topicId}")]
        public async Task<IActionResult> GetGrammarTestsByTopicId(long topicId)
        {
            try
            {
                var tests = await _adminService.GetGrammarTestsByTopicIdAsync(topicId);
                return Ok(tests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("grammar/tests")]
        public async Task<IActionResult> AddGrammarTest([FromBody] AddGrammarTestDto dto)
        {
            try
            {
                var result = await _adminService.AddGrammarTestAsync(dto);
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

        [HttpPut("grammar/tests/{id}")]
        public async Task<IActionResult> UpdateGrammarTest(long id, [FromBody] UpdateGrammarTestDto dto)
        {
            try
            {
                var result = await _adminService.UpdateGrammarTestAsync(id, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("grammar/tests/{id}")]
        public async Task<IActionResult> DeleteGrammarTest(long id)
        {
            try
            {
                var result = await _adminService.DeleteGrammarTestAsync(id);
                if (!result)
                    return NotFound(new { message = "Тест не найден" });
                return Ok(new { success = true, message = "Тест удалён" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        //вокаб уроки
        [HttpGet("vocab/lessons")]
        public async Task<IActionResult> GetAllVocabLessons()
        {
            try
            {
                var lessons = await _adminService.GetAllVocabLessonsAsync();
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("vocab/lessons/{id}")]
        public async Task<IActionResult> GetVocabLessonById(long id)
        {
            try
            {
                var lesson = await _adminService.GetVocabLessonByIdAsync(id);
                if (lesson == null)
                    return NotFound(new { message = "Урок не найден" });
                return Ok(lesson);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("vocab/lessons")]
        public async Task<IActionResult> AddVocabLesson([FromBody] AddVocabLessonDto dto)
        {
            try
            {
                var result = await _adminService.AddVocabLessonAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("vocab/lessons/{id}")]
        public async Task<IActionResult> UpdateVocabLesson(long id, [FromBody] UpdateVocabLessonDto dto)
        {
            try
            {
                var result = await _adminService.UpdateVocabLessonAsync(id, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("vocab/lessons/{id}")]
        public async Task<IActionResult> DeleteVocabLesson(long id)
        {
            try
            {
                var result = await _adminService.DeleteVocabLessonAsync(id);
                if (!result)
                    return NotFound(new { message = "Урок не найден" });
                return Ok(new { success = true, message = "Урок удалён" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        //вокаб тесты
        [HttpGet("vocab/tests")]
        public async Task<IActionResult> GetAllVocabTests()
        {
            try
            {
                var tests = await _adminService.GetAllVocabTestsAsync();
                return Ok(tests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("vocab/tests/{id}")]
        public async Task<IActionResult> GetVocabTestById(long id)
        {
            try
            {
                var test = await _adminService.GetVocabTestByIdAsync(id);
                if (test == null)
                    return NotFound(new { message = "Тест не найден" });
                return Ok(test);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("vocab/tests/by-lesson/{lessonId}")]
        public async Task<IActionResult> GetVocabTestsByLessonId(long lessonId)
        {
            try
            {
                var tests = await _adminService.GetVocabTestsByLessonIdAsync(lessonId);
                return Ok(tests);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("vocab/tests")]
        public async Task<IActionResult> AddVocabTest([FromBody] AddVocabTestDto dto)
        {
            try
            {
                var result = await _adminService.AddVocabTestAsync(dto);
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

        [HttpPut("vocab/tests/{id}")]
        public async Task<IActionResult> UpdateVocabTest(long id, [FromBody] UpdateVocabTestDto dto)
        {
            try
            {
                var result = await _adminService.UpdateVocabTestAsync(id, dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("vocab/tests/{id}")]
        public async Task<IActionResult> DeleteVocabTest(long id)
        {
            try
            {
                var result = await _adminService.DeleteVocabTestAsync(id);
                if (!result)
                    return NotFound(new { message = "Тест не найден" });
                return Ok(new { success = true, message = "Тест удалён" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
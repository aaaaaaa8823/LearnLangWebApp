using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LearnEnglishWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DictionaryController: ControllerBase
    {
        private readonly IDictionaryService _dictionary;

        public DictionaryController(IDictionaryService dictionary)
        {
            _dictionary = dictionary;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllWords()
        {
            try
            {
                var words = await _dictionary.GetAllWordAsync();
                return Ok(words);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchWords([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    var allWords = await _dictionary.GetAllWordAsync();
                    return Ok(allWords);
                }

                var words = await _dictionary.SearchWordAsync(q);
                return Ok(words);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("level/{level}")]
        public async Task<IActionResult> GetWordsByLevel(string level)
        {
            try
            {
                var words = await _dictionary.GetWordByLevelAsync(level);
                return Ok(words);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

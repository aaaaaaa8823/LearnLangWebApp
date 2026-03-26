using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.DTOs.Response;
using LearnEnglishWebApp.Services.Interfaces;
using System.Reflection.Emit;

namespace LearnEnglishWebApp.Services.Implementations
{
    public class DictionaryService: IDictionaryService
    {
        private readonly IDictionaryRepository _repository;
        private readonly ILogger<DictionaryService> _logger;

        public DictionaryService(IDictionaryRepository repository, ILogger<DictionaryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<WordDto>> GetAllWordAsync()
        {
            try
            {
                var words = await _repository.GetAllAsync();
                return words.Select(w => new WordDto
                {
                    Id = w.Id,
                    Word = w.Word,
                    Translation = w.Translation,
                    Definition = w.Definition,
                    PartOfSpeech = w.PartOfSpeech,
                    DifficultyLevel = w.DifficultyLevel,
                    Examples = w.Examples ?? new List<string>()
                });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Ошибка при получении всех слов");
                throw;
            }
        }

        public async Task<WordDto> GetWordById(long id)
        {
            try
            {
                var word = await _repository.GetByIdAsync(id);
                if (word == null) return null;

                return new WordDto
                {
                    Id = word.Id,
                    Word = word.Word,
                    Translation = word.Translation,
                    Definition = word.Definition,
                    PartOfSpeech = word.PartOfSpeech,
                    DifficultyLevel = word.DifficultyLevel,
                    Examples = word.Examples ?? new List<string>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении слова по ID: {WordId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<WordDto>> GetWordByLevelAsync(string level)
        {
            try
            {
                var words = await _repository.GetByLevelAsync(level);
                return words.Select(w => new WordDto
                {
                    Id = w.Id,
                    Word = w.Word,
                    Translation = w.Translation,
                    Definition = w.Definition,
                    PartOfSpeech = w.PartOfSpeech,
                    DifficultyLevel = w.DifficultyLevel,
                    Examples = w.Examples ?? new List<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении слов по уровню: {Level}", level);
                throw;
            }
        }

        public async Task<IEnumerable<WordDto>> SearchWordAsync(string searchTerm)
        {
            try
            {
                var words = await _repository.SearchAsync(searchTerm);
                return words.Select(w => new WordDto
                {
                    Id = w.Id,
                    Word = w.Word,
                    Translation = w.Translation,
                    Definition = w.Definition,
                    PartOfSpeech = w.PartOfSpeech,
                    DifficultyLevel = w.DifficultyLevel,
                    Examples = w.Examples ?? new List<string>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске слов: {SearchTerm}", searchTerm);
                throw;
            }
        }
    }
}

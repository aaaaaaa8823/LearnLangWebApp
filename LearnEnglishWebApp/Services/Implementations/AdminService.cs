using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.DTOs.Request;
using LearnEnglishWebApp.DTOs.Response;
using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Services.Interfaces;
using System.Text.Json;

namespace LearnEnglishWebApp.Services.Implementations
{
    public class AdminService: IAdminService
    {
        public readonly IAdminRepository _repository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IAdminRepository repository, ILogger<AdminService> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        //cлова
        public async Task<IEnumerable<WordDto>> GetAllWordsAsync()
        {
            var words = await _repository.GetAllWordsAsync();
            return words.Select(MapToWordDto);
        }

        public async Task<WordDto> GetWordByIdAsync(long id)
        {
            var word = await _repository.GetWordByIdAsync(id);
            return word != null ? MapToWordDto(word) : null;
        }

        public async Task<WordDto> AddWordAsync(AddWordDto dto)
        {
            var existing = await _repository.GetWordByTextAsync(dto.Word);
            if (existing != null)
                throw new InvalidOperationException($"Слово '{dto.Word}' уже существует");

            var word = new Dictionary
            {
                Word = dto.Word.ToLower(),
                Translation = dto.Translation,
                Definition = dto.Definition,
                PartOfSpeech = dto.PartOfSpeech,
                DifficultyLevel = dto.DifficultyLevel,
                Examples = dto.Examples ?? new List<string>(),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddWordAsync(word);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Добавлено слово: {Word}", word.Word);
            return MapToWordDto(word);
        }

        public async Task<WordDto> UpdateWordAsync(long id, UpdateWordDto dto)
        {
            var word = await _repository.GetWordByIdAsync(id);
            if (word == null)
                throw new InvalidOperationException("Слово не найдено");

            if (!word.Word.Equals(dto.Word, StringComparison.OrdinalIgnoreCase))
            {
                var existing = await _repository.GetWordByTextAsync(dto.Word);
                if (existing != null)
                    throw new InvalidOperationException($"Слово '{dto.Word}' уже существует");
            }

            word.Word = dto.Word.ToLower();
            word.Translation = dto.Translation;
            word.Definition = dto.Definition;
            word.PartOfSpeech = dto.PartOfSpeech;
            word.DifficultyLevel = dto.DifficultyLevel;
            word.Examples = dto.Examples ?? new List<string>();

            _repository.UpdateWord(word);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Обновлено слово: {Word}", word.Word);
            return MapToWordDto(word);
        }

        public async Task<bool> DeleteWordAsync(long id)
        {
            var word = await _repository.GetWordByIdAsync(id);
            if (word == null)
                return false;

            _repository.DeleteWord(word);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Удалено слово: {Word}", word.Word);
            return true;
        }

        //граммар уроки
        public async Task<IEnumerable<GrammarTopicDto>> GetAllGrammarTopicsAsync()
        {
            var topics = await _repository.GetAllGrammarTopicsAsync();
            return topics.Select(MapToGrammarTopicDto);
        }

        public async Task<GrammarTopicDto> GetGrammarTopicByIdAsync(long id)
        {
            var topic = await _repository.GetGrammarTopicByIdAsync(id);
            return topic != null ? MapToGrammarTopicDto(topic) : null;
        }

        public async Task<GrammarTopicDto> AddGrammarTopicAsync(AddGrammarTopicDto dto)
        {
            var topic = new GrammarTopic
            {
                Level = dto.Level,
                Title = dto.Title,
                Description = dto.Description,
                TheoryContent = dto.TheoryContent,
                ExamplesContent = dto.ExamplesContent,
                OrderIndex = dto.OrderIndex,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddGrammarTopicAsync(topic);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Добавлен грамматический урок: {Title}", topic.Title);
            return MapToGrammarTopicDto(topic);
        }

        public async Task<GrammarTopicDto> UpdateGrammarTopicAsync(long id, UpdateGrammarTopicDto dto)
        {
            var topic = await _repository.GetGrammarTopicByIdAsync(id);
            if (topic == null)
                throw new InvalidOperationException("Урок не найден");

            topic.Level = dto.Level;
            topic.Title = dto.Title;
            topic.Description = dto.Description;
            topic.OrderIndex = dto.OrderIndex;
            topic.TheoryContent = dto.TheoryContent;
            topic.ExamplesContent = dto.ExamplesContent;

            _repository.UpdateGrammarTopic(topic);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Обновлён грамматический урок: {Title}", topic.Title);
            return MapToGrammarTopicDto(topic);
        }

        public async Task<bool> DeleteGrammarTopicAsync(long id)
        {
            var topic = await _repository.GetGrammarTopicByIdAsync(id);
            if (topic == null)
                return false;

            _repository.DeleteGrammarTopic(topic);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Удалён грамматический урок: {Title}", topic.Title);
            return true;
        }

        //граммар тесты
        public async Task<IEnumerable<GrammarTestAdminDto>> GetAllGrammarTestsAsync()
        {
            var tests = await _repository.GetAllGrammarTestsAsync();
            return tests.Select(MapToGrammarTestDto);
        }

        public async Task<GrammarTestAdminDto> GetGrammarTestByIdAsync(long id)
        {
            var test = await _repository.GetGrammarTestByIdAsync(id);
            return test != null ? MapToGrammarTestDto(test) : null;
        }

        public async Task<IEnumerable<GrammarTestAdminDto>> GetGrammarTestsByTopicIdAsync(long topicId)
        {
            var tests = await _repository.GetGrammarTestsByTopicIdAsync(topicId);
            return tests.Select(MapToGrammarTestDto);
        }

        public async Task<GrammarTestAdminDto> AddGrammarTestAsync(AddGrammarTestDto dto)
        {
            var topic = await _repository.GetGrammarTopicByIdAsync(dto.GrammarTopicId);
            if (topic == null)
                throw new InvalidOperationException("Грамматический урок не найден");

            var test = new GrammarTest
            {
                GrammarTopicId = dto.GrammarTopicId,
                Title = dto.Title,
                Level = dto.Level,
                QuestionCount = dto.QuestionCount,
                PassingScore = dto.PassingScore,
                TimeLimitMinutes = dto.TimeLimitMinutes,
                OrderIndex = dto.OrderIndex,
                QuestionsText = dto.QuestionsText,
                AnswersText = dto.AnswersText,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddGrammarTestAsync(test);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Добавлен грамматический тест: {Title}", test.Title);
            return MapToGrammarTestDto(test);
        }

        public async Task<GrammarTestAdminDto> UpdateGrammarTestAsync(long id, UpdateGrammarTestDto dto)
        {
            var test = await _repository.GetGrammarTestByIdAsync(id);
            if (test == null)
                throw new InvalidOperationException("Тест не найден");

            test.GrammarTopicId = dto.GrammarTopicId;
            test.Title = dto.Title;
            test.Level = dto.Level;
            test.QuestionCount = dto.QuestionCount;
            test.PassingScore = dto.PassingScore;
            test.TimeLimitMinutes = dto.TimeLimitMinutes;
            test.OrderIndex = dto.OrderIndex;
            test.QuestionsText = dto.QuestionsText;
            test.AnswersText = dto.AnswersText;

            _repository.UpdateGrammarTest(test);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Обновлён грамматический тест: {Title}", test.Title);
            return MapToGrammarTestDto(test);
        }

        public async Task<bool> DeleteGrammarTestAsync(long id)
        {
            var test = await _repository.GetGrammarTestByIdAsync(id);
            if (test == null)
                return false;

            _repository.DeleteGrammarTest(test);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Удалён грамматический тест: {Title}", test.Title);
            return true;
        }

        //вокаб уроки
        public async Task<IEnumerable<VocabTopicDto>> GetAllVocabLessonsAsync()
        {
            var lessons = await _repository.GetAllVocabLessonsAsync();
            return lessons.Select(MapToVocabTopicDto);
        }

        public async Task<VocabTopicDto> GetVocabLessonByIdAsync(long id)
        {
            var lesson = await _repository.GetVocabLessonByIdAsync(id);
            return lesson != null ? MapToVocabTopicDto(lesson) : null;
        }

        public async Task<VocabTopicDto> AddVocabLessonAsync(AddVocabLessonDto dto)
        {
            var lesson = new VocabLesson
            {
                Level = dto.Level,
                Title = dto.Title,
                Description = dto.Description,
                OrderIndex = dto.OrderIndex,
                TheoryContent = dto.TheoryContent,
                ExamplesContent = dto.ExamplesContent,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddVocabLessonAsync(lesson);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Добавлен вокабулярный урок: {Title}", lesson.Title);
            return MapToVocabTopicDto(lesson);
        }

        public async Task<VocabTopicDto> UpdateVocabLessonAsync(long id, UpdateVocabLessonDto dto)
        {
            var lesson = await _repository.GetVocabLessonByIdAsync(id);
            if (lesson == null)
                throw new InvalidOperationException("Урок не найден");

            lesson.Level = dto.Level;
            lesson.Title = dto.Title;
            lesson.Description = dto.Description;
            lesson.OrderIndex = dto.OrderIndex;
            lesson.TheoryContent = dto.TheoryContent;
            lesson.ExamplesContent = dto.ExamplesContent;

            _repository.UpdateVocabLesson(lesson);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Обновлён вокабулярный урок: {Title}", lesson.Title);
            return MapToVocabTopicDto(lesson);
        }

        public async Task<bool> DeleteVocabLessonAsync(long id)
        {
            var lesson = await _repository.GetVocabLessonByIdAsync(id);
            if (lesson == null)
                return false;

            _repository.DeleteVocabLesson(lesson);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Удалён вокабулярный урок: {Title}", lesson.Title);
            return true;
        }

        //вокаб тесты
        public async Task<IEnumerable<VocabTestDto>> GetAllVocabTestsAsync()
        {
            var tests = await _repository.GetAllVocabTestsAsync();
            return tests.Select(MapToVocabTestDto);
        }

        public async Task<VocabTestDto> GetVocabTestByIdAsync(long id)
        {
            var test = await _repository.GetVocabTestByIdAsync(id);
            return test != null ? MapToVocabTestDto(test) : null;
        }

        public async Task<IEnumerable<VocabTestDto>> GetVocabTestsByLessonIdAsync(long lessonId)
        {
            var tests = await _repository.GetVocabTestsByLessonIdAsync(lessonId);
            return tests.Select(MapToVocabTestDto);
        }

        public async Task<VocabTestDto> AddVocabTestAsync(AddVocabTestDto dto)
        {
            var lesson = await _repository.GetVocabLessonByIdAsync(dto.VocabLessonId);
            if (lesson == null)
                throw new InvalidOperationException("Вокабулярный урок не найден");

            var test = new VocabTest
            {
                VocabLessonId = dto.VocabLessonId,
                Title = dto.Title,
                Level = dto.Level,
                Description = dto.Description,
                TestType = dto.TestType,
                QuestionCount = dto.QuestionCount,
                PassingScore = dto.PassingScore,
                TimeLimitMinutes = dto.TimeLimitMinutes,
                OrderIndex = dto.OrderIndex,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddVocabTestAsync(test);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Добавлен вокабулярный тест: {Title}", test.Title);
            return MapToVocabTestDto(test);
        }

        public async Task<VocabTestDto> UpdateVocabTestAsync(long id, UpdateVocabTestDto dto)
        {
            var test = await _repository.GetVocabTestByIdAsync(id);
            if (test == null)
                throw new InvalidOperationException("Тест не найден");

            test.VocabLessonId = dto.VocabLessonId;
            test.Title = dto.Title;
            test.Level = dto.Level;
            test.Description = dto.Description;
            test.TestType = dto.TestType;
            test.QuestionCount = dto.QuestionCount;
            test.PassingScore = dto.PassingScore;
            test.TimeLimitMinutes = dto.TimeLimitMinutes;
            test.OrderIndex = dto.OrderIndex;

            _repository.UpdateVocabTest(test);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Обновлён вокабулярный тест: {Title}", test.Title);
            return MapToVocabTestDto(test);
        }

        public async Task<bool> DeleteVocabTestAsync(long id)
        {
            var test = await _repository.GetVocabTestByIdAsync(id);
            if (test == null)
                return false;

            _repository.DeleteVocabTest(test);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Удалён вокабулярный тест: {Title}", test.Title);
            return true;
        }

        //маппинг
        private WordDto MapToWordDto(Dictionary word)
        {
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

        private GrammarTopicDto MapToGrammarTopicDto(GrammarTopic topic)
        {
            return new GrammarTopicDto
            {
                Id = topic.Id,
                Level = topic.Level,
                Title = topic.Title,
                Description = topic.Description,
                OrderIndex = topic.OrderIndex,
                TheoryContent = topic.TheoryContent,
                ExamplesContent = topic.ExamplesContent,
                CreatedAt = topic.CreatedAt,
                IsCompleted = false,
                Tests = topic.GrammarTests?.Select(t => new GrammarTestDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Level = t.Level,
                    QuestionCount = t.QuestionCount,
                    PassingScore = t.PassingScore,
                    TimeLimitMinutes = t.TimeLimitMinutes,
                    IsActive = t.IsActive
                }).ToList() ?? new List<GrammarTestDto>()
            };
        }

        private GrammarTestAdminDto MapToGrammarTestDto(GrammarTest test)
        {
            return new GrammarTestAdminDto
            {
                Id = test.Id,
                GrammarTopicId = test.GrammarTopicId,
                Title = test.Title,
                Level = test.Level,
                QuestionCount = test.QuestionCount,
                PassingScore = test.PassingScore,
                TimeLimitMinutes = test.TimeLimitMinutes,
                OrderIndex = test.OrderIndex,
                IsActive = test.IsActive,
                CreatedAt = test.CreatedAt,
                QuestionsText = test.QuestionsText,
                AnswersText = test.AnswersText,
                GrammarTopic = test.GrammarTopic != null ? new GrammarTopicSimpleDto
                {
                    Id = test.GrammarTopic.Id,
                    Title = test.GrammarTopic.Title,
                    Level = test.GrammarTopic.Level
                } : null
            };
        }

        private VocabTopicDto MapToVocabTopicDto(VocabLesson lesson)
        {
            return new VocabTopicDto
            {
                Id = lesson.Id,
                Level = lesson.Level,
                Title = lesson.Title,
                Description = lesson.Description ?? "",
                OrderIndex = lesson.OrderIndex,
                TheoryContent = lesson.TheoryContent,
                ExamplesContent = lesson.ExamplesContent,
                CreatedAt = lesson.CreatedAt,
                IsCompleted = false,
                Tests = lesson.VocabTests?.Select(t => new VocabTestDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Level = t.Level,
                    TestType = t.TestType,
                    QuestionCount = t.QuestionCount,
                    PassingScore = t.PassingScore,
                    TimeLimitMinutes = t.TimeLimitMinutes,
                    IsActive = t.IsActive
                }).ToList() ?? new List<VocabTestDto>()
            };
        }

        private VocabTestDto MapToVocabTestDto(VocabTest test)
        {
            return new VocabTestDto
            {
                Id = test.Id,
                Title = test.Title,
                Level = test.Level,
                TestType = test.TestType,
                QuestionCount = test.QuestionCount,
                PassingScore = test.PassingScore,
                TimeLimitMinutes = test.TimeLimitMinutes,
                IsActive = test.IsActive
            };
        }
    }
}

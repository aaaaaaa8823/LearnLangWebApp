using LearnEnglishWebApp.Data;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.DTOs.Response;
using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnEnglishWebApp.Services.Implementations
{
    public class VocabTopicService : IVocabTopicService
    {
        private readonly IVocabTopicRepository _vocabRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<VocabTopicService> _logger;

        public VocabTopicService(
            IVocabTopicRepository vocabRepository,
            AppDbContext context,
            ILogger<VocabTopicService> logger)
        {
            _vocabRepository = vocabRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<VocabTopicDto>> GetAllTopicsAsync()
        {
            var topics = await _vocabRepository.GetAllAsync();
            return topics.Select(t => MapToDto(t, false, null));
        }

        public async Task<VocabTopicDto> GetTopicByIdAsync(long id)
        {
            var topic = await _vocabRepository.GetByIdAsync(id);
            if (topic == null) return null;
            return MapToDto(topic, false, null);
        }

        public async Task<IEnumerable<VocabTopicDto>> GetTopicsByLevelAsync(string level)
        {
            var topics = await _vocabRepository.GetByLevelAsync(level);
            return topics.Select(t => MapToDto(t, false, null));
        }

        public async Task<IEnumerable<VocabTopicDto>> GetUserTopicsWithProgressAsync(long userId)
        {
            var topics = await _vocabRepository.GetAllAsync();
            var userProgress = await _context.UserVocabProgresses
                .Where(uvp => uvp.UserId == userId)
                .ToDictionaryAsync(uvp => uvp.LessonId, uvp => uvp);

            return topics.Select(topic => MapToDto(topic,
                userProgress.ContainsKey(topic.Id) && userProgress[topic.Id].Completed,
                userProgress.ContainsKey(topic.Id) ? userProgress[topic.Id].CompletedAt : null));
        }

        public async Task<UserVocabProgressDto> MarkTopicAsCompletedAsync(long userId, long lessonId)
        {
            try
            {
                var lesson = await _vocabRepository.GetByIdAsync(lessonId);
                if (lesson == null)
                    throw new InvalidOperationException("Урок не найден");

                var existingProgress = await _context.UserVocabProgresses
                    .FirstOrDefaultAsync(uvp => uvp.UserId == userId && uvp.LessonId == lessonId);

                if (existingProgress != null)
                {
                    existingProgress.Completed = true;
                    existingProgress.CompletedAt = DateTime.UtcNow;
                    _context.UserVocabProgresses.Update(existingProgress);
                }
                else
                {
                    var progress = new UserVocabProgress
                    {
                        UserId = userId,
                        LessonId = lessonId,
                        Completed = true,
                        CompletedAt = DateTime.UtcNow
                    };
                    await _context.UserVocabProgresses.AddAsync(progress);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Пользователь {UserId} завершил Vocab урок {LessonId}", userId, lessonId);

                return new UserVocabProgressDto
                {
                    UserId = userId,
                    LessonId = lessonId,
                    Completed = true,
                    CompletedAt = DateTime.UtcNow,
                    LessonTitle = lesson.Title
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отметке Vocab урока как завершенного");
                throw;
            }
        }

        public async Task<UserVocabProgressDto> GetUserProgressAsync(long userId, long lessonId)
        {
            var progress = await _context.UserVocabProgresses
                .FirstOrDefaultAsync(uvp => uvp.UserId == userId && uvp.LessonId == lessonId);

            var lesson = await _vocabRepository.GetByIdAsync(lessonId);

            return new UserVocabProgressDto
            {
                UserId = userId,
                LessonId = lessonId,
                Completed = progress?.Completed ?? false,
                CompletedAt = progress?.CompletedAt,
                LessonTitle = lesson?.Title ?? ""
            };
        }

        private VocabTopicDto MapToDto(VocabLesson lesson, bool isCompleted, DateTime? completedAt)
        {
            return new VocabTopicDto
            {
                Id = lesson.Id,
                Level = lesson.Level,
                Title = lesson.Title,
                Description = lesson.Description ?? "",
                OrderIndex = lesson.OrderIndex,
                CreatedAt = lesson.CreatedAt,
                IsCompleted = isCompleted,
                CompletedAt = completedAt,
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
    }
}
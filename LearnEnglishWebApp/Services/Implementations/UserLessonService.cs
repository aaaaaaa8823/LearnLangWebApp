using LearnEnglishWebApp.Data;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.DTOs.Response;
using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Services.Interfaces;

namespace LearnEnglishWebApp.Services.Implementations
{
    public class UserLessonService : IUserLessonService
    {
        private readonly IUserLessonRepository _userLessonRepository;
        private readonly IGrammarTopicRepository _grammarTopicRepository;
        //private readonly IVocabLessonRepository _vocabLessonRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<UserLessonService> _logger;

        public UserLessonService(
            IUserLessonRepository userLessonRepository,
            IGrammarTopicRepository grammarTopicRepository,
            AppDbContext context,
            ILogger<UserLessonService> logger)
        {
            _userLessonRepository = userLessonRepository;
            _grammarTopicRepository = grammarTopicRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<SavedLessonDto>> GetSavedLessonsAsync(long userId)
        {
           var savedLessons = await _userLessonRepository.GetByUserIdAsync(userId);

            var result = new List<SavedLessonDto>();

            foreach (var saved in savedLessons)
            {
                if (saved.LessonType == "grammar")
                {
                    var lesson = await _grammarTopicRepository.GetTopicByIdAsync(saved.LessonId);
                    if (lesson != null)
                    {
                        result.Add(new SavedLessonDto
                        {
                            Id = saved.Id,
                            LessonId = saved.LessonId,
                            Title = lesson.Title,
                            Level = lesson.Level,
                            Description = lesson.Description,
                            LessonType = saved.LessonType,
                            SavedAt = saved.SavedAt
                        });
                    }
                }
                //else if (saved.LessonType == "vocab")
                //{
                //    var lesson = await _vocabLessonRepository.GetByIdAsync(saved.LessonId);
                //    if (lesson != null)
                //    {
                //        result.Add(new SavedLessonDto
                //        {
                //            Id = saved.Id,
                //            LessonId = saved.LessonId,
                //            Title = lesson.Title,
                //            Level = lesson.Level,
                //            Description = lesson.Description,
                //            LessonType = saved.LessonType,
                //            SavedAt = saved.SavedAt
                //        });
                //    }
                //}
            }

            return result.OrderByDescending(r => r.SavedAt);
        }

        public async Task<bool> IsLessonSavedAsync(long userId, long lessonId, string lessonType)
        {
            return await _userLessonRepository.ExistsAsync(userId, lessonId, lessonType);
        }

        public async Task<bool> RemoveSavedLessonAsync(long userId, long lessonId, string lessonType)
        {
            try
            {
                var userLesson = await _userLessonRepository.GetByUserAndLessonAsync(userId, lessonId, lessonType);
                if (userLesson == null)
                    return false;

                _userLessonRepository.Delete(userLesson);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении сохраненного урока");
                throw;
            }
        }

        public async Task<SavedLessonDto> SaveLessonAsync(long userId, long lessonId, string lessonType)
        {
            try
            {
                var exists = await _userLessonRepository.ExistsAsync(userId, lessonId, lessonType);
                if (exists)
                    throw new InvalidOperationException("Урок уже сохранен");

                string title = "";
                string level = "";
                string description = "";

                if (lessonType == "grammar")
                {
                    var lesson = await _grammarTopicRepository.GetTopicByIdAsync(lessonId);
                    if (lesson == null)
                        throw new InvalidOperationException("Грамматический урок не найден");
                    title = lesson.Title;
                    level = lesson.Level;
                    description = lesson.Description;
                }
                //else if (lessonType == "vocab")
                //{
                //    var lesson = await _vocabLessonRepository.GetByIdAsync(lessonId);
                //    if (lesson == null)
                //        throw new InvalidOperationException("Лексический урок не найден");
                //    title = lesson.Title;
                //    level = lesson.Level;
                //    description = lesson.Description;
                //}
                else
                {
                    throw new InvalidOperationException($"Неизвестный тип урока: {lessonType}");
                }

                var userLesson = new UserLesson
                {
                    UserId = userId,
                    LessonId = lessonId,
                    LessonType = lessonType,
                    SavedAt = DateTime.UtcNow
                };

                await _userLessonRepository.AddAsync(userLesson);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Урок сохранен: UserId={UserId}, LessonId={LessonId}, Type={LessonType}",
                    userId, lessonId, lessonType);

                return new SavedLessonDto
                {
                    Id = userLesson.Id,
                    LessonId = lessonId,
                    Title = title,
                    Level = level,
                    Description = description,
                    LessonType = lessonType,
                    SavedAt = userLesson.SavedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении урока");
                throw;
            }
        }
    }
}

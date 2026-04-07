using LearnEnglishWebApp.Data;
using LearnEnglishWebApp.Data.Repositories.Implementations;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.DTOs.Response;
using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnEnglishWebApp.Services.Implementations
{
    public class GrammarTopicService : IGrammarTopicService
    {
        private readonly IGrammarTopicRepository _grammar;

        private readonly AppDbContext _context;
        private readonly ILogger<GrammarTopicService> _logger;
        
        public GrammarTopicService(
            IGrammarTopicRepository grammar,
            AppDbContext context,
            ILogger<GrammarTopicService> logger
            )
        {
            _grammar = grammar;
            _context = context;
            _logger = logger;
        }


        public async Task<IEnumerable<GrammarTopicDto>> GetAllTopicsAsync()
        {
            var topics = await _grammar.GetAllTopicAsync();
            return topics.Select(t => MapToDto(t, false, null));
        }

        public async Task<IEnumerable<GrammarTopicDto>> GetAvailableTopicsForUserAsync(long userId)
        {
            var allTopics = await _grammar.GetAllTopicAsync();
            var userProgress = await _context.UserGrammarProgress
                .Where(ugp => ugp.UserId == userId)
                .ToDictionaryAsync(ugp => ugp.GrammarTopicId, ugp => ugp);

            return allTopics.Select(topic => MapToDto(topic,
                userProgress.ContainsKey(topic.Id) && userProgress[topic.Id].Completed,
                userProgress.ContainsKey(topic.Id) ? userProgress[topic.Id].CompletedAt : null));
        }

        public async Task<GrammarTopicDto> GetTopicByIdAsync(long id)
        {
            var topic = await _grammar.GetTopicByIdAsync(id);
            if (topic == null) return null;
            return MapToDto(topic, false, null);
        }

        public async Task<IEnumerable<GrammarTopicDto>> GetTopicsByLevelAsync(string level)
        {
            var topics = await _grammar.GetByLevelAsync(level);
            return topics.Select(t => MapToDto(t, false, null));
        }

        public async Task<UserGrammarProgressDto> GetUserProgressAsync(long userId, long topicId)
        {
            var progress = await _context.UserGrammarProgress
                .FirstOrDefaultAsync(ugp => ugp.UserId == userId && ugp.GrammarTopicId == topicId);

            var topic = await _grammar.GetTopicByIdAsync(topicId);

            return new UserGrammarProgressDto
            {
                UserId = userId,
                TopicId = topicId,
                Completed = progress?.Completed ?? false,
                CompletedAt = progress?.CompletedAt,
                TopicTitle = topic?.Title ?? ""
            };
        }

        public async Task<IEnumerable<GrammarTopicDto>> GetUserTopicsWithProgressAsync(long userId)
        {
            var topics = await _grammar.GetAllTopicAsync();
            var userProgress = await _context.UserGrammarProgress
                .Where(ugp => ugp.UserId == userId)
                .ToDictionaryAsync(ugp => ugp.GrammarTopicId, ugp => ugp);

            return topics.Select(topic => MapToDto(topic,
                userProgress.ContainsKey(topic.Id) && userProgress[topic.Id].Completed,
                userProgress.ContainsKey(topic.Id) ? userProgress[topic.Id].CompletedAt : null));
        }

        public async Task<UserGrammarProgressDto> MarkTopicAsCompletedAsync(long userId, long topicId)
        {
            try
            {
                var topic = await _grammar.GetTopicByIdAsync(topicId);
                if (topic == null)
                    throw new InvalidOperationException("Тема не найдена");

                var existingProgress = await _context.UserGrammarProgress
                    .FirstOrDefaultAsync(ugp => ugp.UserId == userId && ugp.GrammarTopicId == topicId);

                if (existingProgress != null)
                {
                    existingProgress.Completed = true;
                    existingProgress.CompletedAt = DateTime.UtcNow;
                    _context.UserGrammarProgress.Update(existingProgress);
                }
                else
                {
                    var progress = new UserGrammarProgress
                    {
                        UserId = userId,
                        GrammarTopicId = topicId,
                        Completed = true,
                        CompletedAt = DateTime.UtcNow
                    };
                    await _context.UserGrammarProgress.AddAsync(progress);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Пользователь {UserId} завершил тему {TopicId}", userId, topicId);

                return new UserGrammarProgressDto
                {
                    UserId = userId,
                    TopicId = topicId,
                    Completed = true,
                    CompletedAt = DateTime.UtcNow,
                    TopicTitle = topic.Title
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отметке темы как завершенной");
                throw;
            }
        }

        private GrammarTopicDto MapToDto(GrammarTopic topic, bool isCompleted, DateTime? completedAt)
        {
            return new GrammarTopicDto
            {
                Id = topic.Id,
                Level = topic.Level,
                Title = topic.Title,
                Description = topic.Description ?? "",
                OrderIndex = topic.OrderIndex,
                CreatedAt = topic.CreatedAt,
                IsCompleted = isCompleted,
                CompletedAt = completedAt,
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
    }
}

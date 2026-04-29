using LearnEnglishWebApp.Data;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.DTOs.Response;
using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LearnEnglishWebApp.Services.Implementations
{
    public class GrammarTestService : IGrammarTestService
    {
        private readonly IGrammarTestRepository _grammarTestRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<GrammarTestService> _logger;

        public GrammarTestService(
            IGrammarTestRepository grammarTestRepository,
            AppDbContext context,
            ILogger<GrammarTestService> logger)
        {
            _grammarTestRepository = grammarTestRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<GrammarTestDetailDto>> GetAllTestAsync(long userId)
        {
            var tests = await _grammarTestRepository.GetAllTestAsync();
            var results = new List<GrammarTestDetailDto>();
            foreach (var test in tests)
            {
                var userResults = await _context.TestResults
                    .Where(tr => tr.UserId == userId && tr.TestId == test.Id && tr.TestType == "grammar")
                    .ToListAsync();

                results.Add(MapToDto(test, userResults));
            }

            return results.OrderBy(t => t.Level).ThenBy(t => t.Id);
        }

        public async Task<TestResultDto> GetBestResultAsync(long userId, long testId)
        {
            throw new NotImplementedException();
        }

        public async Task<GrammarTestDetailDto> GetTestByIdAsync(long userId, long testId)
        {
            var test = await _grammarTestRepository.GetTestByIdAsync(testId);
            if (test == null) return null;

            var userResults = await _context.TestResults
                .Where(tr => tr.UserId == userId && tr.TestId == testId && tr.TestType == "grammar")
                .ToListAsync();

            return MapToDto(test, userResults);
        }

        public async Task<IEnumerable<GrammarTestDetailDto>> GetTestByTopicAsync(long userId, long topicId)
        {
            var tests = await _grammarTestRepository.GetByTopicIdAsync(topicId);
            var results = new List<GrammarTestDetailDto>();

            foreach (var test in tests)
            {
                var userResults = await _context.TestResults
                    .Where(tr => tr.UserId == userId && tr.TestId == test.Id && tr.TestType == "grammar")
                    .ToListAsync();

                results.Add(MapToDto(test, userResults));
            }

            return results.OrderBy(t => t.OrderIndex);
        }

        public async Task<IEnumerable<GrammarTestDetailDto>> GetTestsByLevelAsync(long userId, string level)
        {
            var tests = await _grammarTestRepository.GetByLevelAsync(level);
            var results = new List<GrammarTestDetailDto>();

            foreach (var test in tests)
            {
                var userResults = await _context.TestResults
                    .Where(tr => tr.UserId == userId && tr.TestId == test.Id && tr.TestType == "grammar")
                    .ToListAsync();

                results.Add(MapToDto(test, userResults));
            }

            return results.OrderBy(t => t.OrderIndex);
        }

        public async Task<IEnumerable<TestResultDto>> GetUserTestResultsAsync(long userId, long? testId = null)
        {
            var query = _context.TestResults
               .Where(tr => tr.UserId == userId && tr.TestType == "grammar");

            if (testId.HasValue)
            {
                query = query.Where(tr => tr.TestId == testId.Value);
            }

            var results = await query
                .OrderByDescending(tr => tr.CompletedAt)
                .ToListAsync();

            return results.Select(r => new TestResultDto
            {
                Id = r.Id,
                TestId = r.TestId,
                Score = r.Score,
                MaxScore = r.MaxScore,
                Percentage = r.Percentage,
                IsPassed = r.IsPassed,
                AttemptNumber = r.AttemptNumber,
                TimeSpentSeconds = r.TimeSpentSeconds,
                CompletedAt = r.CompletedAt
            });
        }

        public async Task<TestResultDto> SubmitTestResultAsync(long userId, SubmitTestResultDto result)
        {
            try
            {
                var test = await _grammarTestRepository.GetTestByIdAsync(result.TestId);
                if (test == null)
                    throw new InvalidOperationException("Тест не найден");

                var attemptNumber = await _context.TestResults
                    .CountAsync(tr => tr.UserId == userId && tr.TestId == result.TestId && tr.TestType == "grammar") + 1;

                var testResult = new TestResult
                {
                    UserId = userId,
                    TestType = "grammar",
                    TestId = result.TestId,
                    Score = result.Score,
                    MaxScore = result.MaxScore,
                    Percentage = result.Percentage,
                    IsPassed = result.Percentage >= test.PassingScore,
                    AttemptNumber = attemptNumber,
                    TimeSpentSeconds = result.TimeSpentSeconds,
                    CorrectAnswers = result.CorrectAnswers,
                    WrongAnswers = result.WrongAnswers,
                    CompletedAt = DateTime.UtcNow
                };

                if (result.Answers != null && result.Answers.Any())
                {
                    var answersJson = JsonSerializer.Serialize(result.Answers);
                }

                _context.TestResults.Add(testResult);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Результат теста сохранен: UserId={UserId}, TestId={TestId}, Score={Score}%, Passed={IsPassed}",
                    userId, result.TestId, result.Percentage, testResult.IsPassed);

                return new TestResultDto
                {
                    Id = testResult.Id,
                    TestId = testResult.TestId,
                    Score = testResult.Score,
                    MaxScore = testResult.MaxScore,
                    Percentage = testResult.Percentage,
                    IsPassed = testResult.IsPassed,
                    AttemptNumber = testResult.AttemptNumber,
                    TimeSpentSeconds = testResult.TimeSpentSeconds,
                    CompletedAt = testResult.CompletedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении результата теста");
                throw;
            }
        }

        private GrammarTestDetailDto MapToDto(GrammarTest test, List<TestResult> results)
        {
            var bestResult = results.OrderByDescending(r => r.Percentage).FirstOrDefault();

            return new GrammarTestDetailDto
            {
                Id = test.Id,
                GrammarTopicId = test.GrammarTopicId,
                TopicTitle = test.GrammarTopic?.Title ?? "",
                Level = test.Level,
                QuestionCount = test.QuestionCount,
                PassingScore = test.PassingScore,
                TimeLimitMinutes = test.TimeLimitMinutes,
                IsActive = test.IsActive,
                IsCompleted = results.Any(r => r.IsPassed),
                BestScore = bestResult?.Score,
                BestPercentage = bestResult?.Percentage,
                AttemptCount = results.Count
            };
        }
    }
}

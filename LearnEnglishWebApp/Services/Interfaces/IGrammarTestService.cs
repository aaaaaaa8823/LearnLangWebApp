using LearnEnglishWebApp.DTOs.Response;

namespace LearnEnglishWebApp.Services.Interfaces
{
    public interface IGrammarTestService
    {
        Task<IEnumerable<GrammarTestDetailDto>> GetAllTestAsync(long userId);
        Task<GrammarTestDetailDto> GetTestByIdAsync(long userId, long testId);
        Task<IEnumerable<GrammarTestDetailDto>> GetTestByTopicAsync(long userId, long topicId);
        Task<IEnumerable<GrammarTestDetailDto>> GetTestsByLevelAsync(long userId, string level);
        Task<TestResultDto> SubmitTestResultAsync(long userId, SubmitTestResultDto result);
        Task<IEnumerable<TestResultDto>> GetUserTestResultsAsync(long userId, long? testId = null);
    }
}
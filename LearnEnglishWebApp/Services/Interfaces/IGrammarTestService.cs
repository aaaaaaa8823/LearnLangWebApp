using LearnEnglishWebApp.DTOs.Response;

namespace LearnEnglishWebApp.Services.Interfaces
{
    public interface IGrammarTestService
    {
        Task<IEnumerable<GrammarTestDetailDto>> GetAllTestAsync(long userId);
        Task<IEnumerable<GrammarTestDetailDto>> GetTestsByLevelAsync(long userId, string level);
        Task<IEnumerable<GrammarTestDetailDto>> GetTestByTopicAsync(long userId, long topicId);
        Task<GrammarTestDetailDto> GetTestByIdAsync(long userId, long testId);
        Task<TestResultDto> SubmitTestResultAsync(long userId, SubmitTestResultDto result);
        Task<IEnumerable<TestResultDto>> GetUserTestResultsAsync(long userId, long? testId = null);
        Task<TestResultDto> GetBestResultAsync(long userId, long testId);

    }
}

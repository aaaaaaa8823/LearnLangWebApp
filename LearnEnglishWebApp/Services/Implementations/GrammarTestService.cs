using LearnEnglishWebApp.DTOs.Response;
using LearnEnglishWebApp.Services.Interfaces;

namespace LearnEnglishWebApp.Services.Implementations
{
    public class GrammarTestService : IGrammarTestService
    {
        public async Task<IEnumerable<GrammarTestDetailDto>> GetAllTestAsync(long userId)
        {
            throw new NotImplementedException();
        }

        public async Task<TestResultDto> GetBestResultAsync(long userId, long testId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GrammarTestDetailDto>> GetTestById(long userId, long testId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GrammarTestDetailDto>> GetTestByTopicAsync(long userId, long topicId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GrammarTestDetailDto>> GetTestsByLevelAsync(long userId, string level)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TestResultDto>> GetUserTestResultsAsync(long userId, long? testId = null)
        {
            throw new NotImplementedException();
        }

        public async Task<TestResultDto> SubmitTestResultAsync(long userId, SubmitTestResultDto result)
        {
            throw new NotImplementedException();
        }
    }
}

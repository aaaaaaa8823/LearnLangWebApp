using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data.Repositories.Interfaces
{
    public interface IGrammarTestRepository
    {
        Task<IEnumerable<GrammarTest>> GetAllTestAsync();
        Task<GrammarTest> GetTestByIdAsync(long id);
        Task<IEnumerable<GrammarTest>> GetByLevelAsync(string level);
        Task<IEnumerable<GrammarTest>> GetByUserProgressAsync(long userId, bool? passed = null);
        Task<IEnumerable<GrammarTest>> GetByTopicIdAsync(long topicId);
        Task<bool> ExistsAsync(long id);
    }
}

using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data.Repositories.Interfaces
{
    public interface IGrammarTopicRepository
    {
        Task<IEnumerable<GrammarTopic>> GetAllTopicAsync();
        Task<GrammarTopic> GetTopicByIdAsync(long id);
        Task<IEnumerable<GrammarTopic>> GetByLevelAsync(string level);
        Task<IEnumerable<GrammarTopic>> GetByUserProgressAsync(long userId, bool? completed = null);
        Task<bool> ExistsAsync(long id);
    }
}

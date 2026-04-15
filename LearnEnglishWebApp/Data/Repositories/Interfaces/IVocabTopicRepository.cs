using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data.Repositories.Interfaces
{
    public interface IVocabTopicRepository
    {
        Task<IEnumerable<GrammarTopic>> GetAllTopicAsync();
        Task<VocabLesson> GetTopicByIdAsync(long id);
        Task<IEnumerable<VocabLesson>> GetByLevelAsync(string level);
        Task<IEnumerable<VocabLesson>> GetByUserProgressAsync(long userId, bool? completed = null);
        Task<bool> ExistsAsync(long id);
    }
}

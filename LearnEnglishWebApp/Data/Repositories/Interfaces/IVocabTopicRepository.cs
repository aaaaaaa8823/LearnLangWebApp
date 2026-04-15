using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data.Repositories.Interfaces
{
    public interface IVocabTopicRepository
    {
        Task<IEnumerable<VocabLesson>> GetAllAsync();
        Task<VocabLesson> GetByIdAsync(long id);
        Task<IEnumerable<VocabLesson>> GetByLevelAsync(string level);
        Task<IEnumerable<VocabLesson>> GetByUserProgressAsync(long userId, bool? completed = null);
        Task<bool> ExistsAsync(long id);
    }
}

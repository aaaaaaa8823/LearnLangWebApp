using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data.Repositories.Interfaces
{
    public interface IDictionaryRepository
    {
        Task<IEnumerable<Dictionary>> GetAllAsync();
        Task<Dictionary> GetByIdAsync(long id);
        Task<Dictionary> GetByWordAsync(string word);
        Task<IEnumerable<Dictionary>> GetByLevelAsync(string level);
        Task<IEnumerable<Dictionary>> GetByPartOfSpeechAsync(string partOfSpeech);
        Task<IEnumerable<Dictionary>> SearchAsync(string searchTerm);
        Task AddAsync(Dictionary word);
        void Update(Dictionary word);
        void Delete(Dictionary word);

    }
}

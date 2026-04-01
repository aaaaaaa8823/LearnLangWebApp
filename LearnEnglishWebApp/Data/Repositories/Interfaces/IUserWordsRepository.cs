using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data.Repositories.Interfaces
{
    public interface IUserWordsRepository
    {
        Task<UserWord> GetByIdAsync(int id);
        Task<UserWord> GetByUserAndWordAsync(long userId, long wordId);

        Task<IEnumerable<UserWord>> GetByUserIdAsync(long userId, string status = null);

        Task AddAsync(UserWord userWord);

        void Update(UserWord userWord);

        void Delete(UserWord userWord);

        Task<bool> ExistsAsync(long userId, long wordId);
    }
}

using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data.Repositories.Interfaces
{
    public interface IUserTestRepository
    {
        Task<UserTest> GetByIdAsync(long id);
        Task<IEnumerable<UserTest>> GetByUserIdAsync(long userId);
        Task<UserTest> GetByUserAndTestAsync(long userId, long testId, string testType);
        Task<bool> ExistsAsync(long userId, long testId, string testType);
        Task AddAsync(UserTest userTest);
        void Delete(UserTest userTest);
    }
}

using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(long id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);

        Task AddAsync(User user);
        void Update(User user);

        Task<User> GetUserWithWordsAsync(long userId);
        Task<User> GetUserWithCollectionsAsync(long userId);
    }
}

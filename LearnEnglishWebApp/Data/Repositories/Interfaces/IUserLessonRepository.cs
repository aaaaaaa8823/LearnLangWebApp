using LearnEnglishWebApp.Models;

namespace LearnEnglishWebApp.Data.Repositories.Interfaces
{
    public interface IUserLessonRepository
    {
        Task<UserLesson> GetByIdAsync(long id);
        Task<IEnumerable<UserLesson>> GetByUserIdAsync(long userId);
        Task<UserLesson> GetByUserAndLessonAsync(long userId, long lessonId, string lessonType);
        Task<bool> ExistsAsync(long userId, long lessonId, string lessonType);
        Task AddAsync(UserLesson userLesson);
        void Delete(UserLesson userLesson);
    }
}

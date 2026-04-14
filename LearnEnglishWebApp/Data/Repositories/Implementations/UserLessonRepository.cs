using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnEnglishWebApp.Data.Repositories.Implementations
{
    public class UserLessonRepository: IUserLessonRepository
    {
        private readonly AppDbContext _context;

        public UserLessonRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserLesson> GetByIdAsync(long id)
        {
            return await _context.UserLessons.FindAsync(id);
        }

        public async Task<IEnumerable<UserLesson>> GetByUserIdAsync(long userId)
        {
            return await _context.UserLessons
                .Where(ul => ul.UserId == userId)
                .OrderByDescending(ul => ul.SavedAt)
                .ToListAsync();
        }

        public async Task<UserLesson> GetByUserAndLessonAsync(long userId, long lessonId, string lessonType)
        {
            return await _context.UserLessons
                .FirstOrDefaultAsync(ul => ul.UserId == userId && ul.LessonId == lessonId && ul.LessonType == lessonType);
        }

        public async Task<bool> ExistsAsync(long userId, long lessonId, string lessonType)
        {
            return await _context.UserLessons
                .AnyAsync(ul => ul.UserId == userId && ul.LessonId == lessonId && ul.LessonType == lessonType);
        }

        public async Task AddAsync(UserLesson userLesson)
        {
            await _context.UserLessons.AddAsync(userLesson);
        }

        public void Delete(UserLesson userLesson)
        {
            _context.UserLessons.Remove(userLesson);
        }
    }
}

using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnEnglishWebApp.Data.Repositories.Implementations
{
    public class VocabTopicRepository: IVocabTopicRepository
    {
        private readonly AppDbContext _context;

        public VocabTopicRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VocabLesson>> GetAllAsync()
        {
            return await _context.VocabLessons
                .Include(vl => vl.VocabTests)
                .Include(vl => vl.UserProgress)
                .OrderBy(vl => vl.OrderIndex)
                .ToListAsync();
        }

        public async Task<VocabLesson> GetByIdAsync(long id)
        {
            return await _context.VocabLessons
                .Include(vl => vl.VocabTests)
                .Include(vl => vl.UserProgress)
                .FirstOrDefaultAsync(vl => vl.Id == id);
        }

        public async Task<IEnumerable<VocabLesson>> GetByLevelAsync(string level)
        {
            return await _context.VocabLessons
                .Include(vl => vl.VocabTests)
                .Include(vl => vl.UserProgress)
                .Where(vl => vl.Level == level)
                .OrderBy(vl => vl.OrderIndex)
                .ToListAsync();
        }

        public async Task<IEnumerable<VocabLesson>> GetByUserProgressAsync(long userId, bool? completed = null)
        {
            var query = _context.VocabLessons
                .Include(vl => vl.VocabTests)
                .Include(vl => vl.UserProgress)
                .Where(vl => vl.UserProgress.Any(up => up.UserId == userId));

            if (completed.HasValue)
            {
                query = query.Where(vl => vl.UserProgress.Any(up => up.UserId == userId && up.Completed == completed.Value));
            }

            return await query.OrderBy(vl => vl.OrderIndex).ToListAsync();
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.VocabLessons.AnyAsync(vl => vl.Id == id);
        }
    }
}

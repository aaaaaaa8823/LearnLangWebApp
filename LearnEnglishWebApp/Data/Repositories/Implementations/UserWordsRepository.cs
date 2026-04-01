using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnEnglishWebApp.Data.Repositories.Implementations
{
    public class UserWordsRepository : IUserWordsRepository
    {
        private readonly AppDbContext _context;

        public UserWordsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserWord userWord)
        {
            await _context.UsersWords.AddAsync(userWord);
        }

        public async void Delete(UserWord userWord)
        {
            _context.UsersWords.Remove(userWord);
        }

        public async Task<bool> ExistsAsync(long userId, long wordId)
        {
            return await _context.UsersWords.AnyAsync(uw => uw.UserId == userId  && uw.WordId == wordId);
        }

        public async Task<UserWord> GetByIdAsync(int id)
        {
            return await _context.UsersWords.Include(uw => uw.Word).FirstOrDefaultAsync(uw => uw.Id == id);
        }

        public async Task<UserWord> GetByUserAndWordAsync(long userId, long wordId)
        {
            return await _context.UsersWords.Include(uw => uw.Word).FirstOrDefaultAsync(uw => uw.UserId == userId && uw.WordId == wordId);
        }

        public async Task<IEnumerable<UserWord>> GetByUserIdAsync(long userId, string status = null)
        {
            var query = _context.UsersWords.Include(uw => uw.Word).Where(uw => uw.UserId == userId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(uw => uw.Status == status);
            }

            return await query.OrderByDescending(uw => uw.AddedAt).ToListAsync();
        }

        public async void Update(UserWord userWord)
        {
            _context.UsersWords.Update(userWord);
        }
    }
}

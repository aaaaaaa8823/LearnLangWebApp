using LearnEnglishWebApp.Data.Repositories.Interfaces;
using LearnEnglishWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnEnglishWebApp.Data.Repositories.Implementations
{
    public class UserTestRepository: IUserTestRepository
    {
        private readonly AppDbContext _context;

        public UserTestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserTest> GetByIdAsync(long id)
        {
            return await _context.UserTests.FindAsync(id);
        }

        public async Task<IEnumerable<UserTest>> GetByUserIdAsync(long userId)
        {
            return await _context.UserTests
                .Where(ut => ut.UserId == userId)
                .OrderByDescending(ut => ut.SavedAt)
                .ToListAsync();
        }

        public async Task<UserTest> GetByUserAndTestAsync(long userId, long testId, string testType)
        {
            return await _context.UserTests
                .FirstOrDefaultAsync(ut => ut.UserId == userId && ut.TestId == testId && ut.TestType == testType);
        }

        public async Task<bool> ExistsAsync(long userId, long testId, string testType)
        {
            return await _context.UserTests
                .AnyAsync(ut => ut.UserId == userId && ut.TestId == testId && ut.TestType == testType);
        }

        public async Task AddAsync(UserTest userTest)
        {
            await _context.UserTests.AddAsync(userTest);
        }

        public void Delete(UserTest userTest)
        {
            _context.UserTests.Remove(userTest);
        }
    }
}

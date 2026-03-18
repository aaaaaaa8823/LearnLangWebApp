using Microsoft.EntityFrameworkCore;
using LearnEnglishWebApp.Models;
using LearnEnglishWebApp.Data.Repositories.Interfaces;

namespace LearnEnglishWebApp.Data.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(long id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<User> GetUserWithWordsAsync(long userId)
        {
            return await _context.Users
                .Include(u => u.UserWords)
                    .ThenInclude(uw => uw.Word)
                .Include(u => u.UserWords)
                    .ThenInclude(uw => uw.CollectionWords)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserWithCollectionsAsync(long userId)
        {
            return await _context.Users
                .Include(u => u.Collections)
                    .ThenInclude(c => c.CollectionWords)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}